// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    public abstract class BaseCameraSettingsProvider : BaseDataProvider<IMixedRealityCameraSystem>, IMixedRealityCameraSettingsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        protected BaseCameraSettingsProvider(
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(cameraSystem, name, priority, profile)
        { }

        /// <inheritdoc/>
        public virtual bool IsOpaque { get; } = false;

        /// <inheritdoc/>
        public virtual void ApplyConfiguration()
        {
            // It is the responsibility of the camera settings provider to set the display settings (this allows overriding the
            // default values with per-camera provider values).
            MixedRealityCameraProfile cameraProfile = Service?.CameraProfile;
            if (cameraProfile == null) { return; }

            if (IsOpaque)
            {
                CameraCache.Main.clearFlags = cameraProfile.CameraClearFlagsOpaqueDisplay;
                CameraCache.Main.nearClipPlane = cameraProfile.NearClipPlaneOpaqueDisplay;
                CameraCache.Main.farClipPlane = cameraProfile.FarClipPlaneOpaqueDisplay;
                CameraCache.Main.backgroundColor = cameraProfile.BackgroundColorOpaqueDisplay;
                QualitySettings.SetQualityLevel(cameraProfile.OpaqueQualityLevel, false);
            }
            else
            {
                CameraCache.Main.clearFlags = cameraProfile.CameraClearFlagsTransparentDisplay;
                CameraCache.Main.backgroundColor = cameraProfile.BackgroundColorTransparentDisplay;
                CameraCache.Main.nearClipPlane = cameraProfile.NearClipPlaneTransparentDisplay;
                CameraCache.Main.farClipPlane = cameraProfile.FarClipPlaneTransparentDisplay;
                QualitySettings.SetQualityLevel(cameraProfile.TransparentQualityLevel, false);
            }

            if(Application.isPlaying)
            {
                // Move the camera upwards by FloorHeight units if the experience settings explicitly have MRTK initialize the camera to floor height
                if (!MixedRealityToolkit.Instance.ActiveProfile.ExperienceSettingsProfile.IsNull())
                {
                    float floorHeight = MixedRealityToolkit.Instance.ActiveProfile.ExperienceSettingsProfile.FloorHeight;
                    bool cameraAdjustedByXRDevice = XRSubsystemHelpers.InputSubsystem != null && !XRSubsystemHelpers.InputSubsystem.GetTrackingOriginMode().HasFlag(TrackingOriginModeFlags.Unknown) ||
                        XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale;

                    if (cameraAdjustedByXRDevice)
                    {
                        CameraCache.Main.transform.position = Vector3.up * floorHeight;
                    }
                    else
                    {
                        // Ensure the camera is parented to the playspace which starts, unrotated, at FloorHeight units below the origin if there is no tracking mode
                        MixedRealityPlayspace.Rotation = Quaternion.identity;
                        MixedRealityPlayspace.Position = Vector3.down * floorHeight;
                        CameraCache.Main.transform.position = Vector3.zero;
                    }
                }
            }
        }
    }
}