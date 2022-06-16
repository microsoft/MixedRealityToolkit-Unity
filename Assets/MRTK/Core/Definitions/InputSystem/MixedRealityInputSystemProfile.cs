// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Profiles/Mixed Reality Input System Profile", fileName = "MixedRealityInputSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    [MixedRealityServiceProfile(typeof(IMixedRealityInputSystem))]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/input/overview")]
    public class MixedRealityInputSystemProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private MixedRealityInputDataProviderConfiguration[] dataProviderConfigurations = System.Array.Empty<MixedRealityInputDataProviderConfiguration>();

        /// <summary>
        /// List of input data provider configurations to initialize and manage by the Input System registrar
        /// </summary>
        public MixedRealityInputDataProviderConfiguration[] DataProviderConfigurations
        {
            get { return dataProviderConfigurations; }
            internal set { dataProviderConfigurations = value; }
        }

        [SerializeField]
        [Tooltip("The focus provider service concrete type to use when raycasting.")]
        [Implements(typeof(IMixedRealityFocusProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType focusProviderType;

        /// <summary>
        /// The focus provider service concrete type to use when raycasting.
        /// </summary>
        public SystemType FocusProviderType
        {
            get { return focusProviderType; }
            internal set { focusProviderType = value; }
        }

        [SerializeField]
        [Tooltip("The raycast provider service concrete type to use when raycasting.")]
        [Implements(typeof(IMixedRealityRaycastProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType raycastProviderType;

        /// <summary>
        /// The raycast provider service concrete type to use when raycasting.
        /// </summary>
        public SystemType RaycastProviderType
        {
            get { return raycastProviderType; }
            internal set { raycastProviderType = value; }
        }

        [SerializeField]
        [Range(1, 2048)]
        [Tooltip("Maximum number of colliders that can be detected in a SphereOverlap scene query.")]
        private int focusQueryBufferSize = 128;

        /// <summary>
        /// Maximum number of colliders that can be detected in a SphereOverlap scene query.
        /// </summary>
        public int FocusQueryBufferSize => focusQueryBufferSize;

        [SerializeField]
        [Tooltip("Whether or not MRTK should try to raycast against Unity UI.")]
        private bool shouldUseGraphicsRaycast = true;

        /// <summary>
        /// Whether or not MRTK should try to raycast against Unity UI.
        /// </summary>
        public bool ShouldUseGraphicsRaycast => shouldUseGraphicsRaycast;

        [SerializeField]
        [Tooltip("In case of a compound collider, does the individual collider receive focus")]
        private bool focusIndividualCompoundCollider = false;

        /// <summary>
        /// In case of a compound collider, does the individual collider receive focus
        /// </summary>
        public bool FocusIndividualCompoundCollider
        {
            get { return focusIndividualCompoundCollider; }
            set { focusIndividualCompoundCollider = value; }
        }

        [SerializeField]
        [Tooltip("Input System Action Mapping profile for wiring up Controller input to Actions.")]
        private MixedRealityInputActionsProfile inputActionsProfile;

        /// <summary>
        /// Input System Action Mapping profile for wiring up Controller input to Actions.
        /// </summary>
        public MixedRealityInputActionsProfile InputActionsProfile
        {
            get { return inputActionsProfile; }
            internal set { inputActionsProfile = value; }
        }

        [SerializeField]
        [Tooltip("Input Action Rules Profile for raising actions based on specific criteria.")]
        private MixedRealityInputActionRulesProfile inputActionRulesProfile;

        /// <summary>
        /// Input Action Rules Profile for raising actions based on specific criteria.
        /// </summary>
        public MixedRealityInputActionRulesProfile InputActionRulesProfile
        {
            get { return inputActionRulesProfile; }
            internal set { inputActionRulesProfile = value; }
        }

        [SerializeField]
        [Tooltip("Pointer Configuration options")]
        private MixedRealityPointerProfile pointerProfile;

        /// <summary>
        /// Pointer configuration options
        /// </summary>
        public MixedRealityPointerProfile PointerProfile
        {
            get { return pointerProfile; }
            internal set { pointerProfile = value; }
        }

        [SerializeField]
        [Tooltip("Gesture Mapping Profile for recognizing gestures across all platforms.")]
        private MixedRealityGesturesProfile gesturesProfile;

        /// <summary>
        /// Gesture Mapping Profile for recognizing gestures across all platforms.
        /// </summary>
        public MixedRealityGesturesProfile GesturesProfile
        {
            get { return gesturesProfile; }
            internal set { gesturesProfile = value; }
        }

        /// <summary>
        /// The list of cultures where speech recognition is supported
        /// </summary>
        private List<CultureInfo> supportedVoiceCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("en-CA"),
            new CultureInfo("fr-CA"),
            new CultureInfo("en-GB"),
            new CultureInfo("en-AU"),
            new CultureInfo("de-DE"),
            new CultureInfo("fr-FR"),
            new CultureInfo("zh-CN"),
            new CultureInfo("ja-JP"),
            new CultureInfo("es-ES"),
            new CultureInfo("it-IT")
        };

        /// <summary>
        /// Returns whether speech is supported for the current language or not
        /// </summary>
        public bool IsSpeechSupported => supportedVoiceCultures.Contains(CultureInfo.CurrentUICulture);

        [SerializeField]
        [Tooltip("Speech Command profile for wiring up Voice Input to Actions.")]
        private MixedRealitySpeechCommandsProfile speechCommandsProfile;

        /// <summary>
        /// Speech commands profile for configured speech commands, for use by the speech recognition system
        /// </summary>
        public MixedRealitySpeechCommandsProfile SpeechCommandsProfile
        {
            get { return speechCommandsProfile; }
            internal set { speechCommandsProfile = value; }
        }

        [SerializeField]
        [Tooltip("Enable and configure the devices for your application.")]
        private bool enableControllerMapping = false;

        /// <summary>
        /// Enable and configure the devices for your application.
        /// </summary>
        public bool IsControllerMappingEnabled
        {
            get { return controllerMappingProfile != null && enableControllerMapping; }
            internal set { enableControllerMapping = value; }
        }

        [SerializeField]
        [Tooltip("Device profile for wiring up physical inputs to Actions.")]
        private MixedRealityControllerMappingProfile controllerMappingProfile;

        /// <summary>
        /// Active profile for controller mapping configuration
        /// </summary>
        public MixedRealityControllerMappingProfile ControllerMappingProfile
        {
            get { return controllerMappingProfile; }
            internal set { controllerMappingProfile = value; }
        }

        [SerializeField]
        [Tooltip("Device profile for rendering spatial controllers.")]
        private MixedRealityControllerVisualizationProfile controllerVisualizationProfile;

        /// <summary>
        /// Device profile for rendering spatial controllers.
        /// </summary>
        public MixedRealityControllerVisualizationProfile ControllerVisualizationProfile
        {
            get { return controllerVisualizationProfile; }
            internal set { controllerVisualizationProfile = value; }
        }

        [SerializeField]
        [Tooltip("Profile for configuring Hands tracking.")]
        private MixedRealityHandTrackingProfile handTrackingProfile;

        /// <summary>
        /// Active profile for hands tracking
        /// </summary>
        public MixedRealityHandTrackingProfile HandTrackingProfile
        {
            get { return handTrackingProfile; }
            private set { handTrackingProfile = value; }
        }
    }
}
