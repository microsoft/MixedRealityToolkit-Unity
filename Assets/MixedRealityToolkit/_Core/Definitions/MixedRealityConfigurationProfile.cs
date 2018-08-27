// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Configuration Profile", fileName = "MixedRealityConfigurationProfile", order = (int)CreateProfileMenuItemIndices.Configuration)]
    public class MixedRealityConfigurationProfile : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Manager Registry properties

        [SerializeField]
        private SystemType[] initialManagerTypes = null;

        /// <summary>
        /// Dictionary list of active managers used by the Mixed Reality Manager at runtime
        /// </summary>
        public Dictionary<Type, IMixedRealityManager> ActiveManagers { get; } = new Dictionary<Type, IMixedRealityManager>();

        #endregion Manager Registry properties

        #region Mixed Reality Manager configurable properties

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
        [Tooltip("Enable the Camera Profile on Startup.")]
        private bool enableCameraProfile = false;

        /// <summary>
        /// Enable and configure the Camera Profile for the Mixed Reality Toolkit
        /// </summary>
        public bool IsCameraProfileEnabled
        {
            get
            {
                return CameraProfile != null && enableCameraProfile;
            }

            private set { enableCameraProfile = value; }
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
            private set { cameraProfile = value; }
        }

        [SerializeField]
        [Tooltip("Enable the Input System on Startup.")]
        private bool enableInputSystem = false;

        /// <summary>
        /// Enable and configure the Input System component for the Mixed Reality Toolkit
        /// </summary>
        public bool IsInputSystemEnabled
        {
            get
            {
                return inputActionsProfile != null && inputSystemType != null && inputSystemType.Type != null && enableInputSystem;
            }
            private set { enableInputSystem = value; }
        }

        [SerializeField]
        [Tooltip("Input System Class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityInputSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType inputSystemType;

        /// <summary>
        /// Input System Script File to instantiate at runtime.
        /// </summary>
        public SystemType InputSystemType
        {
            get { return inputSystemType; }
            private set { inputSystemType = value; }
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
            private set { inputActionsProfile = value; }
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
        [Tooltip("Enable Speech Commands on startup.")]
        private bool enableSpeechCommands = false;

        /// <summary>
        /// Enable and configure the speech commands for your application.
        /// </summary>
        public bool IsSpeechCommandsEnabled
        {
            get { return speechCommandsProfile != null && enableSpeechCommands && enableInputSystem; }
            private set { enableSpeechCommands = value; }
        }

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

        [SerializeField]
        [Tooltip("Enable dictation input for your application.")]
        private bool enableDictation = false;

        /// <summary>
        /// Enable dictation input for your application.
        /// </summary>
        public bool IsDictationEnabled
        {
            get { return enableDictation && enableInputSystem; }
            private set { enableDictation = value; }
        }

        [SerializeField]
        private int recognitionConfidenceLevel = 1;

        /// <summary>
        /// The speech recognizer's minimum confidence level setting that will raise the action.<para/>
        /// 0 == High, 1 == Medium, 2 == Low, 3 == Unknown
        /// </summary>
        public int SpeechRecognitionConfidenceLevel => recognitionConfidenceLevel;

        [SerializeField]
        [Tooltip("Enable Touch Screen Input for your application.")]
        private bool enableTouchScreenInput = false;

        /// <summary>
        /// Enable Touch Screen Input for your application.
        /// </summary>
        public bool IsTouchScreenInputEnabled
        {
            get { return touchScreenInputProfile != null && enableTouchScreenInput && enableInputSystem; }
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
            get { return controllerMappingProfile != null && enableControllerMapping && enableInputSystem; }
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

        [SerializeField]
        [Tooltip("Enable the Boundary on Startup")]
        private bool enableBoundarySystem = false;

        /// <summary>
        /// Enable and configure the boundary system.
        /// </summary>
        public bool IsBoundarySystemEnabled
        {
            get { return boundarySystemType != null && boundarySystemType.Type != null && enableBoundarySystem; }
            private set { enableInputSystem = value; }
        }

        [SerializeField]
        [Tooltip("Boundary System Class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityBoundarySystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType boundarySystemType;

        /// <summary>
        /// Boundary System Script File to instantiate at runtime.
        /// </summary>
        public SystemType BoundarySystemSystemType
        {
            get { return boundarySystemType; }
            private set { boundarySystemType = value; }
        }

        [SerializeField]
        [Tooltip("The approximate height of the play space, in meters.")]
        private float boundaryHeight = 3.0f;

        /// <summary>
        /// The developer defined height of the boundary, in meters.
        /// </summary>
        /// <remarks>
        /// The BoundaryHeight property is used to create a three dimensional volume for the play space.
        /// </remarks>
        public float BoundaryHeight => boundaryHeight;

        [SerializeField]
        [Tooltip("Profile for wiring up boundary visualization assets.")]
        private MixedRealityBoundaryVisualizationProfile boundaryVisualizationProfile;

        /// <summary>
        /// Active profile for controller mapping configuration
        /// </summary>
        public MixedRealityBoundaryVisualizationProfile BoundaryVisualizationProfile
        {
            get { return boundaryVisualizationProfile; }
            private set { boundaryVisualizationProfile = value; }
        }

        [SerializeField]
        [Tooltip("Enable the Teleport System on Startup")]
        private bool enableTeleportSystem = false;

        /// <summary>
        /// Enable and configure the boundary system.
        /// </summary>
        public bool IsTeleportSystemEnabled
        {
            get { return teleportSystemType != null && teleportSystemType.Type != null && enableTeleportSystem; }
            private set { enableTeleportSystem = value; }
        }

        [SerializeField]
        [Tooltip("Boundary System Class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityTeleportSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType teleportSystemType;

        /// <summary>
        /// Boundary System Script File to instantiate at runtime.
        /// </summary>
        public SystemType TeleportSystemSystemType
        {
            get { return teleportSystemType; }
            private set { teleportSystemType = value; }
        }

        [SerializeField]
        [Tooltip("The duration of the teleport in seconds.")]
        private float teleportDuration = 0.25f;

        /// <summary>
        /// The duration of the teleport in seconds.
        /// </summary>
        public float TeleportDuration
        {
            get { return teleportDuration; }
            set { teleportDuration = value; }
        }

        #endregion Mixed Reality Manager configurable properties

        #region ISerializationCallbackReceiver Implementation

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            var count = ActiveManagers.Count;
            initialManagerTypes = new SystemType[count];

            foreach (var manager in ActiveManagers)
            {
                --count;
                initialManagerTypes[count] = new SystemType(manager.Value.GetType());
            }
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (ActiveManagers.Count == 0)
            {
                for (int i = 0; i < initialManagerTypes?.Length; i++)
                {
                    ActiveManagers.Add(initialManagerTypes[i], Activator.CreateInstance(initialManagerTypes[i]) as IMixedRealityManager);
                }
            }
        }
    }
    #endregion  ISerializationCallbackReceiver Implementation
}
