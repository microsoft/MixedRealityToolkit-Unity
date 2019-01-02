using UnityEngine;
using Pixie.Initialization;
using Photon.Pun;
using Pixie.Core;

namespace Pixie.AppSystems.TimeSync.Photon
{
    /// <summary>
    /// This class tracks latency and provides a synchronized timer for all connected devices
    /// </summary>
    public class DeviceTime : MonoBehaviourPun, IDeviceTime, IPunObservable
    {
        public bool Synchronized { get { return synchronized; } }
        public float Latency { get { return latency; } set { latency = value; } }
        public ITimeHandler TimeHandler { set { timeHandler = value; } }
        public AppRoleEnum AppRole { get; set; }
        public DeviceTypeEnum DeviceType { get; set; }
        public short DeviceID { get { return deviceID; } }

        [SerializeField]
        private bool synchronized = false;

        [SerializeField]
        private float latency = 0f;
        private ITimeHandler timeHandler;
        private short deviceID = -1;
        private object[] requestRpcArgs = new object[1];
        private object[] respondRpcArgs = new object[2];
        private object[] latencyRpcArgs = new object[2];

        public void AssignDeviceID(short deviceID)
        {
            this.deviceID = deviceID;

            requestRpcArgs[0] = deviceID;
            photonView.RPC("ReceiveAssignedDeviceID", RpcTarget.AllBufferedViaServer, requestRpcArgs);
        }

        #region Server calls

        public void UpdateServerTime(float targetTime)
        {
            requestRpcArgs[0] = targetTime;
            photonView.RPC("ReceiveUpdateServerTime", RpcTarget.Others, requestRpcArgs);
        }

        public void RequestLatencyCheck(float localTime)
        {
            requestRpcArgs[0] = localTime;
            photonView.RPC("ReceiveRequestLatencyCheck", RpcTarget.Others, requestRpcArgs);
        }

        public void UpdateLatency(float latency, bool synchronized)
        {
            latencyRpcArgs[0] = latency;
            latencyRpcArgs[1] = synchronized;
            // Everyone wants to know our latency, including this object
            photonView.RPC("ReceiveUpdateLatency", RpcTarget.All, latencyRpcArgs);
        }

        #endregion

        #region Server -> Client RPCs

        /// <summary>
        /// Asks the target device to send a timestamp back to check for latency
        /// </summary>
        [PunRPC]
        void ReceiveRequestLatencyCheck(float timeRequestSent)
        {
            // Send the time stamp back to the server
            respondRpcArgs[0] = deviceID;
            respondRpcArgs[1] = timeRequestSent;
            photonView.RPC("RespondToLatencyCheck", RpcTarget.MasterClient, respondRpcArgs);
        }

        /// <summary>
        /// Updates the server target time on the target device
        /// </summary>
        [PunRPC]
        void ReceiveUpdateServerTime(float targetTime)
        {
            // Add latency to synced time so it's accurate
            timeHandler.UpdateServerTime(targetTime + Latency);
        }

        /// <summary>
        /// Assigns a device ID to the target device
        /// </summary>
        /// <param name="deviceID"></param>
        [PunRPC]
        void ReceiveAssignedDeviceID(short deviceID)
        {
            this.deviceID = deviceID;
        }

        /// <summary>
        /// Sets latency value determined by server
        /// </summary>
        [PunRPC]
        void ReceiveUpdateLatency(float latency, bool synchronized)
        {
            this.latency = latency;
            this.synchronized = synchronized;
        }

        #endregion

        #region Client-> Server RPCs

        /// <summary>
        /// Contacts the server-side time handler, which then determines our latency
        /// </summary>
        [PunRPC]
        public void RespondToLatencyCheck(short deviceID, float timeRequestSent)
        {
            if (timeHandler == null)
                ComponentFinder.FindInScenes<ITimeHandler>(out timeHandler);

            timeHandler.RespondToLatencyCheck(deviceID, timeRequestSent);
        }

        #endregion

        public void OnAppInitialize() { }

        public void OnAppConnect() { }

        public void OnAppSynchronize() { }

        public void OnAppShutDown() { }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        private void OnEnable()
        {
            if (timeHandler == null)
                ComponentFinder.FindInScenes<ITimeHandler>(out timeHandler);

            timeHandler.AddDevice(this);
        }

        private void OnDisable()
        {
            if (timeHandler != null)
                timeHandler.RemoveDevice(this);
        }
    }
}