// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
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
        public MixedRealityControllerMapping(uint id, string description, SystemType controller, Handedness handedness, GameObject controllerModel, InteractionMapping[] interactions) : this()
        {
            this.id = id;
            this.description = description;
            this.controller = controller;
            this.handedness = handedness;
            this.model = controllerModel;
            this.interactions = interactions;
        }

        /// <summary>
        /// The ID assigned to the Device.
        /// </summary>
        public uint Id => id;

        [SerializeField]
        private uint id;

        /// <summary>
        /// Description of the Device.
        /// </summary>
        public string Description => description;

        [SerializeField]
        private string description;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        public SystemType Controller => controller;

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controller;

        /// <summary>
        /// The designated hand that the device is managing.
        /// </summary>
        public Handedness Handedness => handedness;

        [SerializeField]
        [Tooltip("The designated hand that the device is managing.")]
        private Handedness handedness;

        /// <summary>
        /// The controller model prefab to be rendered.
        /// </summary>
        public GameObject ControllerModel => model;

        [SerializeField]
        [Tooltip("The designated hand that the device is managing.")]
        private GameObject model;

        /// <summary>
        /// Details the list of available buttons / interactions available from the device.
        /// </summary>
        public InteractionMapping[] Interactions => interactions;

        [SerializeField]
        [Tooltip("Details the list of available buttons / interactions available from the device.")]
        private InteractionMapping[] interactions;

        public InteractionMapping GetInteractionMapping(DeviceInputType inputType)
        {
            return default(InteractionMapping);
        }
    }
}