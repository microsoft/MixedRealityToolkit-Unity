// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// Used to define a controller or other input device's physical buttons, and other attributes.
    /// </summary>
    [Serializable]
    public struct MixedRealityControllerMapping
    {
        public MixedRealityControllerMapping(uint id, string description, SystemType controller, Handedness handedness, GameObject overrideModel, MixedRealityPose poseOffset) : this()
        {
            this.id = id;
            this.description = description;
            this.controller = controller;
            this.handedness = handedness;
            this.overrideModel = overrideModel;
            this.poseOffset = poseOffset == new MixedRealityPose() ? MixedRealityPose.ZeroIdentity : PoseOffset;
            useCustomInteractionMappings = false;
            interactions = null;
            useDefaultModel = false;
        }

        [SerializeField]
        private uint id;

        /// <summary>
        /// The ID assigned to the Device.
        /// </summary>
        public uint Id => id;

        [SerializeField]
        private string description;

        /// <summary>
        /// Description of the Device.
        /// </summary>
        public string Description => description;

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controller;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        public SystemType Controller => controller;

        [SerializeField]
        [Tooltip("The designated hand that the device is managing.")]
        private Handedness handedness;

        /// <summary>
        /// The designated hand that the device is managing.
        /// </summary>
        public Handedness Handedness => handedness;

        [SerializeField]
        [Tooltip("Use the platform SDK to load the default controller model for this controller.")]
        private bool useDefaultModel;

        /// <summary>
        /// User the controller model loader provided by the SDK, or provide override models.
        /// </summary>
        public bool UseDefaultModel
        {
            get { return useDefaultModel; }
            private set { useDefaultModel = value; }
        }

        [SerializeField]
        [Tooltip("An override model to display for this specific controller.")]
        private GameObject overrideModel;

        /// <summary>
        /// The controller model prefab to be rendered.
        /// </summary>
        public GameObject OverrideControllerModel => overrideModel;

        [SerializeField]
        [Tooltip("An override model to display for this specific controller.")]
        private MixedRealityPose poseOffset;

        /// <summary>
        /// The controller model prefab to be rendered.
        /// </summary>
        public MixedRealityPose PoseOffset => poseOffset;

        [SerializeField]
        [Tooltip("Override the default interaction mappings.")]
        private bool useCustomInteractionMappings;

        /// <summary>
        /// Is this controller mapping using custom interactions?. 
        /// </summary>
        public bool UseCustomInteractionMappings => useCustomInteractionMappings;

        [SerializeField]
        [Tooltip("Details the list of available buttons / interactions available from the device.")]
        private MixedRealityInteractionMapping[] interactions;

        /// <summary>
        /// Details the list of available buttons / interactions available from the device.
        /// </summary>
        public MixedRealityInteractionMapping[] Interactions => interactions;

        /// <summary>
        /// Sets the default interaction mapping based on the current controller type.
        /// </summary>
        public void SetDefaultInteractionMapping()
        {
            interactions = ControllerMappingLibrary.GetMappingsForControllerType(controller.Type, handedness);
        }
    }
}