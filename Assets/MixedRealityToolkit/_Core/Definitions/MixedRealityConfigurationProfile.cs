// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Configuration Profile", fileName = "MixedRealityConfigurationProfile", order = 0)]
    public class MixedRealityConfigurationProfile : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Manager Registry properties

        /// <summary>
        /// Serialized list of managers for the Mixed Reality manager
        /// </summary>
        [SerializeField]
        private IMixedRealityManager[] initialManagers = null;

        /// <summary>
        /// Serialized list of the Interface types for the Mixed Reality manager
        /// </summary>
        [SerializeField]
        private Type[] initialManagerTypes = null;

        /// <summary>
        /// Dictionary list of active managers used by the Mixed Reality Manager at runtime
        /// </summary>
        public Dictionary<Type, IMixedRealityManager> ActiveManagers { get; } = new Dictionary<Type, IMixedRealityManager>();

        #endregion Manager Registry properties

        #region Mixed Reality Manager configurable properties

        [SerializeField]
        [Tooltip("Enable the Input System on Startup")]
        private bool enableInputSystem = false;

        /// <summary>
        /// Enable and configure the Input System component for the Mixed Reality Toolkit
        /// </summary>
        public bool EnableInputSystem
        {
            get { return enableInputSystem; }
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
        [Tooltip("Enable Speech Commands on startup.")]
        private bool enableSpeechCommands = false;

        /// <summary>
        /// Enable and configure the speech commands for your application.
        /// </summary>
        public bool EnableSpeechCommands
        {
            get { return enableSpeechCommands; }
            private set { enableSpeechCommands = value; }
        }

        [SerializeField]
        [Tooltip("Speech Command profile for wiring up Voice Input to Actions.")]
        private MixedRealitySpeechCommandsProfile speechCommandsProfile;
        public MixedRealitySpeechCommandsProfile SpeechCommandsProfile
        {
            get { return speechCommandsProfile; }
            private set { speechCommandsProfile = value; }
        }

        [SerializeField]
        [Tooltip("Enable and configure the devices for your application.")]
        private bool enableControllerProfiles = false;

        /// <summary>
        /// Enable and configure the devices for your application.
        /// </summary>
        public bool EnableControllerProfiles
        {
            get { return enableControllerProfiles; }
            private set { enableControllerProfiles = value; }
        }

        [SerializeField]
        [Tooltip("Device profile for wiring up physical inputs to Actions.")]
        private MixedRealityControllerMappingProfile controllersProfile;
        public MixedRealityControllerMappingProfile ControllersProfile
        {
            get { return controllersProfile; }
            private set { controllersProfile = value; }
        }


        [SerializeField]
        [Tooltip("Enable and configure the controller rendering of the Motion Controllers on Startup.")]
        private bool renderMotionControllers = false;

        /// <summary>
        /// Enable and configure the controller rendering of the Motion Controllers on Startup.
        /// </summary>
        public bool RenderMotionControllers
        {
            get { return renderMotionControllers; }
            private set { renderMotionControllers = value; }
        }

        [SerializeField]
        [Tooltip("Enable the Boundary on Startup")]
        private bool enableBoundarySystem = false;

        /// <summary>
        /// Enable and configure the Boundary component on the Mixed Reality Camera
        /// </summary>
        public bool EnableBoundarySystem
        {
            get { return enableBoundarySystem; }
            private set { enableBoundarySystem = value; }
        }

        #endregion Mixed Reality Manager configurable properties

        #region ISerializationCallbackReceiver Implementation

        /// <summary>
        /// Unity function to prepare data for serialization.
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            var count = ActiveManagers.Count;
            initialManagers = new IMixedRealityManager[count];
            initialManagerTypes = new Type[count];

            foreach (var manager in ActiveManagers)
            {
                --count;
                initialManagers[count] = manager.Value;
                initialManagerTypes[count] = manager.Key;
            }
        }

        /// <summary>
        /// Unity function to resolve data from serialization when a project is loaded
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // From the serialized fields for the MixedRealityConfigurationProfile, populate the Active managers list
            // *Note This will only take effect once the Mixed Reality Toolkit has a custom editor for the MixedRealityConfigurationProfile

            ActiveManagers.Clear();
            for (int i = 0; i < initialManagers?.Length; i++)
            {
                Managers.MixedRealityManager.Instance.AddManager(initialManagerTypes[i], initialManagers[i]);
            }
        }

        #endregion  ISerializationCallbackReceiver Implementation
    }
}