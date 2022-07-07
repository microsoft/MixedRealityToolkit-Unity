// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

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
        {
            CameraProfile = cameraSystem?.CameraProfile;
        }

        protected MixedRealityCameraProfile CameraProfile { get; }

        /// <inheritdoc/>
        public virtual bool IsOpaque =>
            Application.isEditor
            && !Application.isPlaying
            && CameraProfile != null
            && CameraProfile.EditTimeDisplayType == DisplayType.Opaque;

        /// <inheritdoc/>
        public virtual void ApplyConfiguration()
        {
            // It is the responsibility of the camera settings provider to set the display settings (this allows overriding the
            // default values with per-camera provider values).
            if (CameraProfile == null || CameraCache.Main == null) { return; }

            if (IsOpaque)
            {
                CameraCache.Main.clearFlags = CameraProfile.CameraClearFlagsOpaqueDisplay;
                CameraCache.Main.nearClipPlane = CameraProfile.NearClipPlaneOpaqueDisplay;
                CameraCache.Main.farClipPlane = CameraProfile.FarClipPlaneOpaqueDisplay;
                CameraCache.Main.backgroundColor = CameraProfile.BackgroundColorOpaqueDisplay;
                QualitySettings.SetQualityLevel(CameraProfile.OpaqueQualityLevel, false);
            }
            else
            {
                CameraCache.Main.clearFlags = CameraProfile.CameraClearFlagsTransparentDisplay;
                CameraCache.Main.backgroundColor = CameraProfile.BackgroundColorTransparentDisplay;
                CameraCache.Main.nearClipPlane = CameraProfile.NearClipPlaneTransparentDisplay;
                CameraCache.Main.farClipPlane = CameraProfile.FarClipPlaneTransparentDisplay;
                QualitySettings.SetQualityLevel(CameraProfile.TransparentQualityLevel, false);
            }
        }
    }
}
