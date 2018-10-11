using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public class ServerTimeSync : MonoBehaviour, ITimeHandler, INetworkTimeController
    {
        public AppRoleEnum AppRole { get; set; }

        public bool Started { get { return started; } }

        public float TargetTime { get { return targetTime; } }

        private float targetTime;
        private bool started;
        private float lastLatencyCheckTime;
        private float lastSyncTime;
        private HashSet<sbyte> outstandingRequests = new HashSet<sbyte>();
        private Dictionary<sbyte, Queue<float>> latencyValues = new Dictionary<sbyte, Queue<float>>();
        private IUserView users;

        private void Update()
        {
            if (!started)
                return;

            targetTime = Time.unscaledTime;

            if (targetTime > lastSyncTime + Globals.UNet.SendIntervalTimeSync)
            {
                lastSyncTime = targetTime;

                foreach (IUserObject player in users.UserObjects)
                {
                    if (!player.HasRole)
                        continue;

                    //Debug.Log("Update server time in player " + player.PlayerNum);
                    // Add latency to the target time so it's accurate by the time it gets there
                    player.UserTime.UpdateServerTime(targetTime);
                }
            }

            if (targetTime > lastLatencyCheckTime + Globals.UNet.SendIntervalTimeSync)
            {
                lastLatencyCheckTime = targetTime;

                foreach (IUserObject player in users.UserObjects)
                {
                    if (!player.HasRole)
                        continue;

                    if (player.Simulated)
                        continue;

                    if (outstandingRequests.Contains(player.UserNum))
                    {
                        //Debug.Log("Skipping network ID " + player.UserNum + " - has outstanding request");
                        continue;
                    }
                    // Store an outstanding request so we don't accidentally double up on latency checks
                    outstandingRequests.Add(player.UserNum);
                    //Debug.Log("Requesting latency check from player " + player.PlayerNum);
                    player.UserTime.RequestLatencyCheck(targetTime);
                }
            }
        }

        public float RespondToLatencyCheck(sbyte userNum, float timeRequestSent)
        {
            // The difference between the time stamp and the current unscaled time is latency * 2
            float latencyResult = (targetTime - timeRequestSent) / 2;

            // Get our queue of latency values
            // Add the latest value
            Queue<float> latencyVals = null;
            if (!latencyValues.TryGetValue(userNum, out latencyVals))
            {
                latencyVals = new Queue<float>();
                latencyValues.Add(userNum, latencyVals);
            }
            while (latencyVals.Count > Globals.UNet.MaxAverageLatencyValues)
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

            // Update our sync vars - these will propogate to all clients
            float latency = averagedLatency;
            bool synchronized = latencyVals.Count >= Globals.UNet.MinLatencyChecks;

            // Remove this connection id from our outstanding requests
            outstandingRequests.Remove(userNum);

            return latency;
        }

        public void UpdateServerTime(float latestSyncedTime)
        {
            Debug.LogError("This should not be called on the server.");
        }

        public void OnSharingStart()
        {
            SceneScraper.FindInScenes<IUserView>(out users);
        }

        public void OnStateInitialized()
        {
            // Set the network time controller
            // It will pull its target time from us
            NetworkTime.Controller = this;
            // Start time
            started = true;
        }

        public void OnSharingStop() { }
    }
}