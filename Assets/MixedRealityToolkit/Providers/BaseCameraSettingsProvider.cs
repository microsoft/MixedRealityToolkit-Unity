// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    public abstract class BaseCameraSettingsProvider : BaseDataProvider, IMixedRealityCameraSettingsProvider
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
            MixedRealityCameraProfile cameraProfile = (Service as IMixedRealityCameraSystem)?.CameraProfile;
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
        }
    }
}