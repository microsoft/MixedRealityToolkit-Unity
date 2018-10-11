using UnityEngine;
using UnityEngine.Networking;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    /// <summary>
    /// This class tracks latency and provides a synchronized timer for all player objects
    /// Because it makes command calls it must be attacked to the local player object
    /// The static ServerTime value is ONLY updated by either the server, or by the local player object
    /// Accessing NetworkTime.ServerTime will return a synchronized time regardless of how many player objects exist
    /// </summary>
    public class UserTime : NetworkBehaviour, IUserTime
    {
        public bool Synchronized { get { return synchronized; } }
        public float Latency { get { return latency; } set { latency = value; } }
        public ITimeHandler TimeHandler { set { timeHandler = value; } }
        
        [SerializeField]
        [SyncVar]
        private bool synchronized = false;

        [SerializeField]
        [SyncVar]
        private float latency = 0f;
        
        private IUserObject user;
        private ITimeHandler timeHandler;

        private void OnEnable()
        {
            user = (IUserObject)gameObject.GetComponent(typeof(IUserObject));
            SceneScraper.FindInScenes<ITimeHandler>(out timeHandler);
        }

        public override float GetNetworkSendInterval()
        {
            return Globals.UNet.SendIntervalAllCosts;
        }

        [Server]
        public void UpdateServerTime(float targetTime)
        {
            Target_UpdateServerTime(connectionToClient, targetTime);
        }

        [Server]
        public void RequestLatencyCheck(float localTime)
        {
            Target_RequestLatencyCheck(connectionToClient, localTime);
        }

        #region Server -> Client actions (specific clients)

        /// <summary>
        /// Asks the target player to send a timestamp back to check for latency
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="connectionID"></param>
        /// <param name="timeRequestSent"></param>
        [TargetRpc(channel = Globals.UNet.ChannelAllCosts)]
        void Target_RequestLatencyCheck(NetworkConnection conn, float timeRequestSent)
        {
            //Debug.Log("Target_RequestLatencyCheck in " + name);

            // Send the time stamp back to the server
            Cmd_RespondToLatencyCheck(user.UserNum, timeRequestSent);
        }

        /// <summary>
        /// Updates the server time on the target player
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="latestSyncedTime"></param>
        [TargetRpc(channel = Globals.UNet.ChannelAllCosts)]
        void Target_UpdateServerTime(NetworkConnection conn, float latestSyncedTime)
        {
            timeHandler.UpdateServerTime(latestSyncedTime + Latency);
        }

        #endregion

        #region Client-> Server actions (called internally on this object)

        [Command(channel = Globals.UNet.ChannelAllCosts)]
        public void Cmd_RespondToLatencyCheck(sbyte userNum, float timeRequestSent)
        {
            //Debug.Log("Cmd_RespondToLatencyCheck in " + name);

            latency = timeHandler.RespondToLatencyCheck(userNum, timeRequestSent);
        }

        #endregion
    }
}