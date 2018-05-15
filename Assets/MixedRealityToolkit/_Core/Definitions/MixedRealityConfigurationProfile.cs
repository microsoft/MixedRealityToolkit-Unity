// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Configuration Profile")]
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
        [Header("Input Settings")]
        [Tooltip("Enable the Input System on Startup")]
        private bool enableInputSystem = true;

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
        [Implements(typeof(IMixedRealityInputSystem), ClassGrouping.None)]
        private SystemType inputSystem;

        /// <summary>
        /// Input System Script File to instantiate at runtime.
        /// </summary>
        public SystemType InputSystem
        {
            get { return inputSystem; }
            private set { inputSystem = value; }
        }

        [SerializeField]
        [Tooltip("Enable the Motion Controllers on Startup")]
        private bool enableControllers = true;

        /// <summary>
        /// Enable and configure the controller rendering for the Mixed Reality Toolkit
        /// </summary>
        public bool EnableControllers
        {
            get { return enableControllers; }
            private set { enableControllers = value; }
        }

        [SerializeField]
        [Tooltip("Default Left Controller Model")]
        private GameObject leftControllerModel;

        /// <summary>
        /// Enable and configure the controller rendering for the Mixed Reality Toolkit
        /// </summary>
        public GameObject LeftControllerModel
        {
            get { return leftControllerModel; }
            private set { leftControllerModel = value; }
        }

        [SerializeField]
        [Tooltip("Default Right Controller Model")]
        private GameObject rightControllerModel;

        /// <summary>
        /// Enable and configure the controller rendering for the Mixed Reality Toolkit
        /// </summary>
        public GameObject RightControllerModel
        {
            get { return rightControllerModel; }
            private set { rightControllerModel = value; }
        }

        [SerializeField]
        [Header("Boundary Settings")]
        [Tooltip("Enable the Boundary on Startup")]
        private bool enableBoundarySystem = true;

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