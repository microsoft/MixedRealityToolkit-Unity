// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests")]
namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input System Profile", fileName = "MixedRealityInputSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityInputSystemProfile : BaseMixedRealityProfile
    {
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


        private IMixedRealitySpeechSystem speechSystem;

        /// <summary>
        /// Current Registered Speech System.
        /// </summary>
        public IMixedRealitySpeechSystem SpeechSystem => speechSystem ?? (speechSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySpeechSystem>());

        /// <summary>
        /// Is the speech Commands Enabled?
        /// </summary>
        public bool IsSpeechCommandsEnabled => speechCommandsProfile != null && SpeechSystem != null && MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled;

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

        private IMixedRealityDictationSystem dictationSystem;

        /// <summary>
        /// Current Registered Dictation System.
        /// </summary>
        public IMixedRealityDictationSystem DictationSystem => dictationSystem ?? (dictationSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityDictationSystem>());

        /// <summary>
        /// Is Dictation Enabled?
        /// </summary>
        public bool IsDictationEnabled => MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled && DictationSystem != null;

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
    }
}