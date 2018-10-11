using UnityEngine;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    public class ClientTimeSync : MonoBehaviour, ITimeHandler, INetworkTimeController
    {
        public AppRoleEnum AppRole { get; set; }

        #region INetworkTimeController implementation

        public bool Started { get { return started; } }

        public float TargetTime { get { return targetTime; } }

        #endregion

        [SerializeField]
        private float targetTime;
        [SerializeField]
        private bool started;

        public float RespondToLatencyCheck(sbyte userNum, float timeRequestSent)
        {
            Debug.LogError("This should not be called on the client.");
            return 0f;
        }

        public void UpdateServerTime(float latestSyncedTime)
        {
            targetTime = latestSyncedTime;
        }

        private void Update()
        {
            targetTime += Time.unscaledDeltaTime;
        }

        public void OnSharingStop() { }

        public void OnSharingStart() { }

        public void OnStateInitialized()
        {
            NetworkTime.Controller = this;
            started = true;
        }
    }
}