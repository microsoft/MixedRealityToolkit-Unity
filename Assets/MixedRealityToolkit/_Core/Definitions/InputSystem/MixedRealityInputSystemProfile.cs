// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up controller pointers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input System Profile", fileName = "MixedRealityInputSystemProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    public class MixedRealityInputSystemProfile : ScriptableObject
    {
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

        private IMixedRealitySpeechSystem speechSystem;

        /// <summary>
        /// Current Registered Speech System.
        /// </summary>
        public IMixedRealitySpeechSystem SpeechSystem => speechSystem ?? (speechSystem = MixedRealityManager.Instance.GetManager<IMixedRealitySpeechSystem>());

        /// <summary>
        /// Is the speech Commands Enabled?
        /// </summary>
        public bool IsSpeechCommandsEnabled => speechCommandsProfile != null && MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled && SpeechSystem != null;

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

        private IMixedRealityDictationManager dictationManager;

        /// <summary>
        /// Current Registered Dictation Manager.
        /// </summary>
        public IMixedRealityDictationManager DictationManager => dictationManager ?? (dictationManager = MixedRealityManager.Instance.GetManager<IMixedRealityDictationManager>());

        /// <summary>
        /// Is Dictation Enabled?
        /// </summary>
        public bool IsDictationEnabled => MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled && DictationManager != null;

        [SerializeField]
        [Tooltip("Enable Touch Screen Input for your application.")]
        private bool enableTouchScreenInput = false;

        /// <summary>
        /// Enable Touch Screen Input for your application.
        /// </summary>
        public bool IsTouchScreenInputEnabled
        {
            get { return touchScreenInputProfile != null && enableTouchScreenInput && MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled; }
            private set { enableTouchScreenInput = value; }
        }

        [SerializeField]
        [Tooltip("Touch Screen Input Source profile for wiring up Actions.")]
        private MixedRealityTouchInputProfile touchScreenInputProfile;

        /// <summary>
        /// Touch Screen Input Source profile for wiring up Actions.
        /// </summary>
        public MixedRealityTouchInputProfile TouchScreenInputProfile
        {
            get { return touchScreenInputProfile; }
            private set { touchScreenInputProfile = value; }
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
            private set { enableControllerMapping = value; }
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
            private set { controllerMappingProfile = value; }
        }

    }
}
