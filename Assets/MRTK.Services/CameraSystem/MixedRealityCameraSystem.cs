// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// The Camera system controls the settings of the main camera.
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/MixedRealityConfigurationGuide.html#camera")]
    public class MixedRealityCameraSystem : BaseDataProviderAccessCoreSystem, IMixedRealityCameraSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public MixedRealityCameraSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : this(profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        public MixedRealityCameraSystem(
            BaseMixedRealityProfile profile = null) : base(profile)
        { }

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Camera System";

        /// <inheritdoc/>
        public bool IsOpaque
        {
            get
            {
                currentDisplayType = DisplayType.Opaque;

                IReadOnlyList<IMixedRealityCameraSettingsProvider> dataProviders = GetDataProviders<IMixedRealityCameraSettingsProvider>();
                if (dataProviders.Count > 0)
                {
                    // Takes the first settings provider's setting.
                    if (!dataProviders[0].IsOpaque)
                    {
                        currentDisplayType = DisplayType.Transparent;
                    }
                }
#if UNITY_WSA
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                else if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
#pragma warning restore 0618
                {
                    currentDisplayType = DisplayType.Transparent;
                }

                Debug.LogWarning("Windows Mixed Reality specific camera code has been moved into Windows Mixed Reality Camera Settings. Please ensure you have this added under your Camera System's Settings Providers, as this deprecated code path may be removed in a future update.");
#endif
                return currentDisplayType == DisplayType.Opaque;
            }
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Camera System";

        /// <inheritdoc/>
        public MixedRealityCameraProfile CameraProfile => ConfigurationProfile as MixedRealityCameraProfile;

        private DisplayType currentDisplayType;
        private bool cameraOpaqueLastFrame = false;

        /// <summary>
        /// Specifies whether or not the camera system was able to register a camera settings provider.
        /// If so, camera management is not to be performed by the camera system itself. If not, the camera
        /// system is to behave as it did in MRTK versions 2.0.0 and 2.1.0.
        /// </summary>
        private bool useFallbackBehavior = true;

        /// <inheritdoc />
        public override void Initialize()
        {
            MixedRealityCameraProfile profile = ConfigurationProfile as MixedRealityCameraProfile;

            if ((GetDataProviders<IMixedRealityCameraSettingsProvider>().Count == 0) && (profile != null))
            {
                // Register the settings providers.
                for (int i = 0; i < profile.SettingsConfigurations.Length; i++)
                {
                    MixedRealityCameraSettingsConfiguration configuration = profile.SettingsConfigurations[i];

                    if (configuration.ComponentType?.Type == null) 
                    { 
                        // Incomplete configuration, do not try to register until a type is set in the profile.
                        continue; 
                    }

                    object[] args = { this, configuration.ComponentName, configuration.Priority, configuration.SettingsProfile };

                    if (RegisterDataProvider<IMixedRealityCameraSettingsProvider>(
                        configuration.ComponentType.Type,
                        configuration.RuntimePlatform,
                        args))
                    {
                        // Apply the display settings
                        IMixedRealityCameraSettingsProvider provider = GetDataProvider<IMixedRealityCameraSettingsProvider>(configuration.ComponentName);
                        provider?.ApplyConfiguration();
                    }
                }
            }

            // Check to see if any providers were loaded.
            useFallbackBehavior = (GetDataProviders<IMixedRealityCameraSettingsProvider>().Count == 0);

            if (useFallbackBehavior)
            {
                cameraOpaqueLastFrame = IsOpaque;

                if (IsOpaque)
                {
                    ApplySettingsForOpaqueDisplay();
                }
                else
                {
                    ApplySettingsForTransparentDisplay();
                }

                // Ensure the camera is parented to the playspace which starts, unrotated, at the origin.
                MixedRealityPlayspace.Position = Vector3.zero;
                MixedRealityPlayspace.Rotation = Quaternion.identity;
                if (CameraCache.Main.transform.position != Vector3.zero)
                {
                    Debug.LogWarning($"The main camera is not positioned at the origin ({Vector3.zero}), experiences may not behave as expected.");
                }
                if (CameraCache.Main.transform.rotation != Quaternion.identity)
                {
                    Debug.LogWarning($"The main camera is configured with a non-zero rotation, experiences may not behave as expected.");
                }
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            IReadOnlyList<IMixedRealityCameraSettingsProvider> providers = GetDataProviders<IMixedRealityCameraSettingsProvider>();
            for (int i = 0; i < providers.Count; i++)
            {
                providers[i].Enable();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            IReadOnlyList<IMixedRealityCameraSettingsProvider> providers = GetDataProviders<IMixedRealityCameraSettingsProvider>();
            for (int i = 0; i < providers.Count; i++)
            {
                providers[i].Disable();
            }

            base.Disable();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            foreach (var provider in GetDataProviders<IMixedRealityCameraSettingsProvider>())
            {
                UnregisterDataProvider(provider);
            }

            useFallbackBehavior = true;

            base.Destroy();
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (useFallbackBehavior)
            {
                if (IsOpaque != cameraOpaqueLastFrame)
                {
                    cameraOpaqueLastFrame = IsOpaque;

                    if (IsOpaque)
                    {
                        ApplySettingsForOpaqueDisplay();
                    }
                    else
                    {
                        ApplySettingsForTransparentDisplay();
                    }
                }
            }
        }

        /// <summary>
        /// Applies opaque settings from camera profile.
        /// </summary>
        private void ApplySettingsForOpaqueDisplay()
        {
            CameraCache.Main.clearFlags = CameraProfile.CameraClearFlagsOpaqueDisplay;
            CameraCache.Main.nearClipPlane = CameraProfile.NearClipPlaneOpaqueDisplay;
            CameraCache.Main.farClipPlane = CameraProfile.FarClipPlaneOpaqueDisplay;
            CameraCache.Main.backgroundColor = CameraProfile.BackgroundColorOpaqueDisplay;
            QualitySettings.SetQualityLevel(CameraProfile.OpaqueQualityLevel, false);
        }

        /// <summary>
        /// Applies transparent settings from camera profile.
        /// </summary>
        private void ApplySettingsForTransparentDisplay()
        {
            CameraCache.Main.clearFlags = CameraProfile.CameraClearFlagsTransparentDisplay;
            CameraCache.Main.backgroundColor = CameraProfile.BackgroundColorTransparentDisplay;
            CameraCache.Main.nearClipPlane = CameraProfile.NearClipPlaneTransparentDisplay;
            CameraCache.Main.farClipPlane = CameraProfile.FarClipPlaneTransparentDisplay;
            QualitySettings.SetQualityLevel(CameraProfile.TransparentQualityLevel, false);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Camera Systems to compare to.
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }
    }
}