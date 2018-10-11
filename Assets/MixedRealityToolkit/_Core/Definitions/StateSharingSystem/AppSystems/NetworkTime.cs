using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems
{
    public class NetworkTime : MonoBehaviour
    {
        #region static values

        public static INetworkTimeController Controller { set { controller = value; } }
        
        public static float Time
        {
            get
            {
                if (controller != null && controller.Started)
                {
                    return time;
                }
                return UnityEngine.Time.unscaledTime;
            }
        }

        public static float TargetTime
        {
            get
            {
                if (controller != null && controller.Started)
                {
                    return controller.TargetTime;
                }
                return 0;
            }
        }

        public static float DeltaTime
        {
            get
            {
                if (controller != null && controller.Started)
                {
                    return time - prevTime;
                }
                return UnityEngine.Time.unscaledDeltaTime;
            }
        }

        public static float SyncDelta
        {
            get
            {
                return syncDelta;
            }
        }

        /// <summary>
        /// How far out of sync the server and local time have drifted
        /// </summary>
        private static float syncDelta;

        /// <summary>
        /// Synchronized time
        /// </summary>
        private static float time;

        /// <summary>
        /// Prev time (set locally)
        /// </summary>
        private static float prevTime;

        /// <summary>
        /// The controller that drives time synchronization
        /// </summary>
        private static INetworkTimeController controller;

        #endregion

        private void Update()
        {
            prevTime = time;

            if (controller == null || !controller.Started)
                return;
            
            // Add delta time to latest server time
            // This will keep it updating at a steady rate between sync calls
            time += UnityEngine.Time.unscaledDeltaTime;
            // If time has drifted out of sync with our target time
            // this will ensure that the correction is relatively painless
            time = Mathf.Lerp(time, controller.TargetTime, UnityEngine.Time.deltaTime * 2);
            syncDelta = time - controller.TargetTime;
            // If we've drifted WAY out of sync, just snap to position
            if (syncDelta > 1f)
            {
                time = controller.TargetTime;
            }
        }
    }
}