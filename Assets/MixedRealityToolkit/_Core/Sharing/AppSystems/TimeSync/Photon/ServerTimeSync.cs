using Photon.Pun;
using Photon.Realtime;
using Pixie.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AppSystems.TimeSync
{
    public class ServerTimeSync : MonoBehaviourPunCallbacks, ITimeHandler, INetworkTimeController, IPunObservable
    {
        public const float SendIntervalTimeSync = 3f;
        public const float SendIntervalLatencyCheck = 0.5f;
        public const int MaxAverageLatencyValues = 10;
        public const int MinLatencyChecks = 5;

        public AppRoleEnum AppRole { get; set; }
        public DeviceTypeEnum DeviceType { get; set; }
        public bool Started { get { return started; } }
        public float TargetTime { get { return targetTime; } }
        public IEnumerable<IDeviceTime> DeviceTimeSources { get { return deviceTimeSources; } }

        private float targetTime;
        private bool started;
        private float lastLatencyCheckTime;
        private float lastSyncTime;
        private HashSet<short> outstandingRequests = new HashSet<short>();
        private Dictionary<short, Queue<float>> latencyValues = new Dictionary<short, Queue<float>>();
        private List<IDeviceTime> deviceTimeSources = new List<IDeviceTime>();
        private bool deviceAdded;

        private void Update()
        {
            if (!started)
                return;

            // If a device was removed, take it out of list
            for (int i = deviceTimeSources.Count - 1; i >= 0; i--)
            {
                IDeviceTime deviceTime = deviceTimeSources[i];
                // The device disconnected
                if (deviceTime == null)
                    deviceTimeSources.RemoveAt(i);
            }

            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    // Client accumulates delta time for its target time
                    targetTime += Time.unscaledDeltaTime;
                    return;

                default:
                    // Server just uses unscaled time directly
                    targetTime = Time.unscaledTime;
                    UpdateServerDevices();
                    break;
            }
        }

        public void AddDevice(IDeviceTime deviceTime)
        {
            if (!deviceTimeSources.Contains(deviceTime))
                deviceTimeSources.Add(deviceTime);
        }

        public void RemoveDevice(IDeviceTime deviceTime)
        {
            deviceTimeSources.Remove(deviceTime);
        }

        [PunRPC]
        public void RespondToLatencyCheck(short deviceID, float timeRequestSent)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    throw new System.Exception("This should not be called on the client.");

                default:
                    break;
            }

            IDeviceTime device = null;
            foreach (IDeviceTime d in deviceTimeSources)
            {
                if (deviceID == d.DeviceID)
                {
                    device = d;
                    break;
                }
            }

            if (device == null)
            {
                Debug.LogWarning("Device " + deviceID + " not found in latency response.");
                return;
            }

            // The difference between the time stamp and the current unscaled time is latency * 2
            float latencyResult = (targetTime - timeRequestSent) / 2;

            // Get our queue of latency values
            // Add the latest value
            Queue<float> latencyVals = null;
            if (!latencyValues.TryGetValue(deviceID, out latencyVals))
            {
                latencyVals = new Queue<float>();
                latencyValues.Add(deviceID, latencyVals);
            }
            while (latencyVals.Count > MaxAverageLatencyValues)
            {
                latencyVals.Dequeue();
            }
            latencyVals.Enqueue(latencyResult);

            // Get the average of all the stored values
            float averagedLatency = 0f;
            foreach (float latencyVal in latencyVals)
            {
                averagedLatency += latencyVal;
            }
            averagedLatency /= latencyVals.Count;

            float latency = averagedLatency;
            bool synchronized = latencyVals.Count >= MinLatencyChecks;

            // Update the device's latency and sync settings
            device.UpdateLatency(latency, synchronized);

            // Remove this connection id from our outstanding requests
            outstandingRequests.Remove(deviceID);
        }

        public void OnAppInitialize() { }

        public void OnAppConnect()
        {
            // Set the network time controller
            // It will pull its target time from us
            NetworkTime.Controller = this;
            // Start time
            started = true;
        }

        public void OnAppSynchronize() { }

        public void OnAppShutDown() { }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            deviceAdded = true;
        }

        private void UpdateServerDevices()
        {
            if (targetTime > lastSyncTime + SendIntervalTimeSync)
            {
                lastSyncTime = targetTime;

                foreach (IDeviceTime deviceTime in deviceTimeSources)
                {
                    deviceTime.UpdateServerTime(targetTime);
                }
            }

            if (targetTime > lastLatencyCheckTime + SendIntervalTimeSync)
            {
                lastLatencyCheckTime = targetTime;

                foreach (IDeviceTime deviceTime in deviceTimeSources)
                {
                    if (outstandingRequests.Contains(deviceTime.DeviceID))
                        continue;

                    // Store an outstanding request so we don't accidentally double up on latency checks
                    outstandingRequests.Add(deviceTime.DeviceID);
                    deviceTime.RequestLatencyCheck(targetTime);
                }
            }
        }

        public void UpdateServerTime(float newTargetTime)
        {
            switch (AppRole)
            {
                case AppRoleEnum.Client:
                    break;

                default:
                    throw new System.Exception("This method should only be called on the client.");
            }
            
            targetTime = newTargetTime;
        }
    }
}