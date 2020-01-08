// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Teleport;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.EditModeTests")]
[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests")]
namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Toolkit Configuration Profile", fileName = "MixedRealityToolkitConfigurationProfile", order = (int)CreateProfileMenuItemIndices.Configuration)]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/MixedRealityConfigurationGuide.html")]
    public class MixedRealityToolkitConfigurationProfile : BaseMixedRealityProfile
    {
        #region Mixed Reality Toolkit configurable properties

        [SerializeField]
        [Tooltip("The scale of the Mixed Reality experience.")]
        private ExperienceScale targetExperienceScale = ExperienceScale.Room;

        /// <summary>
        /// The desired the scale of the experience.
        /// </summary>
        public ExperienceScale TargetExperienceScale
        {
            get { return targetExperienceScale; }
            set { targetExperienceScale = value; }
        }

        [SerializeField]
        [FormerlySerializedAs("enableCameraProfile")]
        [Tooltip("Enable the Camera System on Startup.")]
        private bool enableCameraSystem = false;

        /// <summary>
        /// Enable and configure the Camera Profile for the Mixed Reality Toolkit
        /// </summary>
        public bool IsCameraSystemEnabled
        {
            get { return CameraProfile != null && cameraSystemType != null && cameraSystemType.Type != null && enableCameraSystem; }
            internal set { enableCameraSystem = value; }
        }

        [SerializeField]
        [Tooltip("Camera profile.")]
        private MixedRealityCameraProfile cameraProfile;

        /// <summary>
        /// Profile for customizing your camera and quality settings based on if your 
        /// head mounted display (HMD) is a transparent device or an occluded device.
        /// </summary>
        public MixedRealityCameraProfile CameraProfile
        {
            get { return cameraProfile; }
            internal set { cameraProfile = value; }
        }

        /// <summary>
        /// Camera System class to instantiate at runtime.
        /// </summary>
        public SystemType CameraSystemType
        {
            get { return cameraSystemType; }
            internal set { cameraSystemType = value; }
        }

        [SerializeField]
        [Tooltip("Camera System Class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityCameraSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType cameraSystemType;

        [SerializeField]
        [Tooltip("Enable the Input System on Startup.")]
        private bool enableInputSystem = false;

        /// <summary>
        /// Enable and configure the Input System component for the Mixed Reality Toolkit
        /// </summary>
        public bool IsInputSystemEnabled
        {
            get { return inputSystemProfile != null && inputSystemType != null && inputSystemType.Type != null && enableInputSystem; }
            internal set { enableInputSystem = value; }
        }

        [SerializeField]
        [Tooltip("Input System profile for setting wiring up events and actions to input devices.")]
        private MixedRealityInputSystemProfile inputSystemProfile;

        /// <summary>
        /// Input System profile for configuring events and actions to input devices.
        /// </summary>
        public MixedRealityInputSystemProfile InputSystemProfile
        {
            get { return inputSystemProfile; }
            internal set { inputSystemProfile = value; }
        }

        [SerializeField]
        [Tooltip("Input System class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityInputSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType inputSystemType;

        /// <summary>
        /// Input System class to instantiate at runtime.
        /// </summary>
        public SystemType InputSystemType
        {
            get { return inputSystemType; }
            internal set { inputSystemType = value; }
        }

        [SerializeField]
        [Tooltip("Enable the Boundary on Startup")]
        private bool enableBoundarySystem = false;

        /// <summary>
        /// Enable and configure the boundary system.
        /// </summary>
        public bool IsBoundarySystemEnabled
        {
            get { return boundarySystemType != null && boundarySystemType.Type != null && enableBoundarySystem && boundaryVisualizationProfile != null; }
            internal set { enableBoundarySystem = value; }
        }

        [SerializeField]
        [Tooltip("Boundary System class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityBoundarySystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType boundarySystemType;

        /// <summary>
        /// Boundary System class to instantiate at runtime.
        /// </summary>
        public SystemType BoundarySystemSystemType
        {
            get { return boundarySystemType; }
            internal set { boundarySystemType = value; }
        }

        [SerializeField]
        [Tooltip("Profile for wiring up boundary visualization assets.")]
        private MixedRealityBoundaryVisualizationProfile boundaryVisualizationProfile;

        /// <summary>
        /// Active profile for boundary visualization
        /// </summary>
        public MixedRealityBoundaryVisualizationProfile BoundaryVisualizationProfile
        {
            get { return boundaryVisualizationProfile; }
            internal set { boundaryVisualizationProfile = value; }
        }

        [SerializeField]
        [Tooltip("Enable the Teleport System on Startup")]
        private bool enableTeleportSystem = false;

        /// <summary>
        /// Enable and configure the teleport system.
        /// </summary>
        public bool IsTeleportSystemEnabled
        {
            get { return teleportSystemType != null && teleportSystemType.Type != null && enableTeleportSystem; }
            internal set { enableTeleportSystem = value; }
        }

        [SerializeField]
        [Tooltip("Boundary System Class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityTeleportSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType teleportSystemType;

        /// <summary>
        /// Teleport System class to instantiate at runtime.
        /// </summary>
        public SystemType TeleportSystemSystemType
        {
            get { return teleportSystemType; }
            internal set { teleportSystemType = value; }
        }

        [SerializeField]
        [Tooltip("Enable the Spatial Awareness system on startup")]
        private bool enableSpatialAwarenessSystem = false;

        /// <summary>
        /// Enable and configure the spatial awareness system.
        /// </summary>
        public bool IsSpatialAwarenessSystemEnabled
        {
            get { return spatialAwarenessSystemType != null && spatialAwarenessSystemType.Type != null && enableSpatialAwarenessSystem; }
            internal set { enableSpatialAwarenessSystem = value; }
        }

        [SerializeField]
        [Tooltip("Spatial Awareness System Class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealitySpatialAwarenessSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType spatialAwarenessSystemType;

        /// <summary>
        /// Spatial Awareness System class to instantiate at runtime.
        /// </summary>
        public SystemType SpatialAwarenessSystemSystemType
        {
            get { return spatialAwarenessSystemType; }
            internal set { spatialAwarenessSystemType = value; }
        }

        [SerializeField]
        [Tooltip("Profile for configuring the spatial awareness system.")]
        private MixedRealitySpatialAwarenessSystemProfile spatialAwarenessSystemProfile;

        /// <summary>
        /// Active profile for spatial awareness system
        /// </summary>
        public MixedRealitySpatialAwarenessSystemProfile SpatialAwarenessSystemProfile
        {
            get { return spatialAwarenessSystemProfile; }
            internal set { spatialAwarenessSystemProfile = value; }
        }

        [SerializeField]
        [Tooltip("Profile for configuring diagnostic components.")]
        private MixedRealityDiagnosticsProfile diagnosticsSystemProfile;

        /// <summary>
        /// Active profile for diagnostic configuration
        /// </summary>
        public MixedRealityDiagnosticsProfile DiagnosticsSystemProfile
        {
            get { return diagnosticsSystemProfile; }
            internal set { diagnosticsSystemProfile = value; }
        }

        [SerializeField]
        [Tooltip("Enable diagnostic system")]
        private bool enableDiagnosticsSystem = false;

        /// <summary>
        /// Is the Diagnostics System enabled?
        /// </summary>
        public bool IsDiagnosticsSystemEnabled
        {
            get { return enableDiagnosticsSystem && DiagnosticsSystemSystemType?.Type != null && diagnosticsSystemProfile != null; }
            internal set { enableDiagnosticsSystem = value; }
        }

        [SerializeField]
        [Tooltip("Diagnostics System class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityDiagnosticsSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType diagnosticsSystemType;

        /// <summary>
        /// Diagnostics System Script File to instantiate at runtime
        /// </summary>
        public SystemType DiagnosticsSystemSystemType
        {
            get { return diagnosticsSystemType; }
            internal set { diagnosticsSystemType = value; }
        }

        [SerializeField]
        [Tooltip("Profile for configuring scene system components.")]
        private MixedRealitySceneSystemProfile sceneSystemProfile;

        /// <summary>
        /// Active profile for scene configuration
        /// </summary>
        public MixedRealitySceneSystemProfile SceneSystemProfile
        {
            get { return sceneSystemProfile; }
            internal set { sceneSystemProfile = value; }
        }

        [SerializeField]
        [Tooltip("Enable scene system")]
        private bool enableSceneSystem = false;

        /// <summary>
        /// Is the Scene System enabled?
        /// </summary>
        public bool IsSceneSystemEnabled
        {
            get { return enableSceneSystem && SceneSystemSystemType?.Type != null && sceneSystemProfile != null; }
            internal set { enableSceneSystem = value; }
        }

        [SerializeField]
        [Tooltip("Scene System class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealitySceneSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType sceneSystemType;

        /// <summary>
        /// Scene System Script File to instantiate at runtime
        /// </summary>
        public SystemType SceneSystemSystemType
        {
            get { return sceneSystemType; }
            internal set { sceneSystemType = value; }
        }

        [SerializeField]
        [Tooltip("All the additional non-required services registered with the Mixed Reality Toolkit.")]
        private MixedRealityRegisteredServiceProvidersProfile registeredServiceProvidersProfile = null;

        /// <summary>
        /// All the additional non-required systems, features, and managers registered with the Mixed Reality Toolkit.
        /// </summary>
        public MixedRealityRegisteredServiceProvidersProfile RegisteredServiceProvidersProfile => registeredServiceProvidersProfile;

        /// <summary>
        /// If true, MRTK will generate components that let you to view the state of running services. These objects will not be generated at runtime.
        /// </summary>
        public bool UseServiceInspectors
        {
            get { return useServiceInspectors; }
        }

        [SerializeField]
        [Tooltip("If true, MRTK will generate components that let you to view the state of running services. These objects will not be generated at runtime.")]
        private bool useServiceInspectors = false;

        /// <summary>
        /// If true, MRTK will render the depth buffer as color. Only valid in editor.
        /// </summary>
        public bool RenderDepthBuffer
        {
            get { return renderDepthBuffer; }
        }

        [SerializeField]
        [Tooltip("If true, MRTK will render the depth buffer as color. Only valid in editor.")]
        private bool renderDepthBuffer = false;

        #endregion Mixed Reality Toolkit configurable properties
    }
}
