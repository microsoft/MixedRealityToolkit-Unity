// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.Controllers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System;
using UnityEngine;

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
        public SystemType FocusProviderType => focusProviderType;

        [SerializeField]
        [Tooltip("Input System Action Mapping profile for wiring up Controller input to Actions.")]
        private MixedRealityInputActionsProfile inputActionsProfile;

        /// <summary>
        /// Input System Action Mapping profile for wiring up Controller input to Actions.
        /// </summary>
        public MixedRealityInputActionsProfile InputActionsProfile
        {
            get { return inputActionsProfile; }
            private set { inputActionsProfile = value; }
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
            private set { inputActionRulesProfile = value; }
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
            private set { pointerProfile = value; }
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
            private set { gesturesProfile = value; }
        }

        /// <summary>
        /// Is the speech Commands Enabled?
        /// </summary>
        public bool IsSpeechCommandsEnabled => speechCommandsProfile != null && SpeechDataProvider != null && MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled;

        [SerializeField]
        [Tooltip("Speech Command profile for wiring up Voice Input to Actions.")]
        private MixedRealitySpeechCommandsProfile speechCommandsProfile;

        /// <summary>
        /// Speech commands profile for configured speech commands, for use by the speech recognition system
        /// </summary>
        public MixedRealitySpeechCommandsProfile SpeechCommandsProfile
        {
            get { return speechCommandsProfile; }
            private set { speechCommandsProfile = value; }
        }

        /// <summary>
        /// Is Dictation Enabled?
        /// </summary>
        public bool IsDictationEnabled => MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled && DictationDataProvider != null;

        /// <summary>
        /// Enable and configure the devices for your application.
        /// </summary>
        [Obsolete("Removed. Controller mapping is now enabled by default if a controller data provider is registered in the MixedRealityControllerDataProvidersProfile.")]
        public bool IsControllerMappingEnabled => false;

        [SerializeField]
        [Tooltip("Device profile for registering platform specific input data sources.")]
        private MixedRealityControllerDataProvidersProfile controllerDataProvidersProfile;

        /// <summary>
        /// Active profile for controller mapping configuration
        /// </summary>
        public MixedRealityControllerDataProvidersProfile ControllerDataProvidersProfile
        {
            get { return controllerDataProvidersProfile; }
            private set { controllerDataProvidersProfile = value; }
        }

        [Obsolete("Property renamed to ControllerMappingProfiles")]
        public MixedRealityControllerMappingProfile ControllerMappingProfile = null;

        [SerializeField]
        private MixedRealityControllerMappingProfiles controllerMappingProfiles;

        public MixedRealityControllerMappingProfiles ControllerMappingProfiles => controllerMappingProfiles;

        [SerializeField]
        [Tooltip("Device profile for rendering spatial controllers.")]
        private MixedRealityControllerVisualizationProfile controllerVisualizationProfile;

        /// <summary>
        /// Device profile for rendering spatial controllers.
        /// </summary>
        public MixedRealityControllerVisualizationProfile ControllerVisualizationProfile
        {
            get { return controllerVisualizationProfile; }
            private set { controllerVisualizationProfile = value; }
        }

        private IMixedRealityFocusProvider focusProvider;

        /// <summary>
        /// Current Registered <see cref="IMixedRealityFocusProvider"/>.
        /// </summary>
        public IMixedRealityFocusProvider FocusProvider => focusProvider ?? (focusProvider = MixedRealityToolkit.Instance.GetService<IMixedRealityFocusProvider>());

        private IMixedRealitySpeechDataProvider speechDataProvider;

        /// <summary>
        /// Current Registered <see cref="IMixedRealitySpeechDataProvider"/>
        /// </summary>
        public IMixedRealitySpeechDataProvider SpeechDataProvider => speechDataProvider ?? (speechDataProvider = MixedRealityToolkit.Instance.GetService<IMixedRealitySpeechDataProvider>());

        private IMixedRealityDictationDataProvider dictationDataProvider;

        /// <summary>
        /// Current Registered <see cref="IMixedRealityDictationDataProvider"/>.
        /// </summary>
        public IMixedRealityDictationDataProvider DictationDataProvider => dictationDataProvider ?? (dictationDataProvider = MixedRealityToolkit.Instance.GetService<IMixedRealityDictationDataProvider>());
    }
}