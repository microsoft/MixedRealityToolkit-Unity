// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// Camera settings provider for use with XR SDK.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        (SupportedPlatforms)(-1),
        "XR SDK Camera Settings")]
    public class GenericXRSDKCameraSettings : BaseCameraSettingsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        public GenericXRSDKCameraSettings(
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(cameraSystem, name, priority, profile)
        { }

        private TrackedPoseDriver trackedPoseDriver = null;

        #region IMixedRealityCameraSettings

        /// <inheritdoc/>
        public override bool IsOpaque => XRSDKSubsystemHelpers.DisplaySubsystem?.displayOpaque ?? true;

        /// <inheritdoc/>
        public override void Enable()
        {
            base.Enable();

            // Only track the TrackedPoseDriver if we added it ourselves.
            // There may be a pre-configured TrackedPoseDriver on the camera.
            if (!CameraCache.Main.GetComponent<TrackedPoseDriver>())
            {
                trackedPoseDriver = CameraCache.Main.gameObject.AddComponent<TrackedPoseDriver>();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (trackedPoseDriver != null)
            {
                Object.Destroy(trackedPoseDriver);
                trackedPoseDriver = null;
            }

            base.Disable();
        }

        #endregion IMixedRealityCameraSettings
    }
}
