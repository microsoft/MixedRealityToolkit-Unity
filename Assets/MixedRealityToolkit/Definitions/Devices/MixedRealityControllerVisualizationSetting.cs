// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Devices
{
    /// <summary>
    /// Used to define a controller's visualization settings.
    /// </summary>
    [Serializable]
    public struct MixedRealityControllerVisualizationSetting
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="description">Description of the Device.</param>
        /// <param name="controllerType">Controller Type to instantiate at runtime.</param>
        /// <param name="handedness">The designated hand that the device is managing.</param>
        /// <param name="overrideModel">The controller model prefab to be rendered.</param>
        public MixedRealityControllerVisualizationSetting(string description, Type controllerType, Handedness handedness = Handedness.None, GameObject overrideModel = null) : this()
        {
            this.description = description;
            this.controllerType = new SystemType(controllerType);
            this.handedness = handedness;
            this.overrideModel = overrideModel;
            useDefaultModel = false;
        }

        [SerializeField]
        private string description;

        /// <summary>
        /// Description of the Device.
        /// </summary>
        public string Description => description;

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        public SystemType ControllerType => controllerType;

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
        public bool UseDefaultModel => useDefaultModel;

        [SerializeField]
        [Tooltip("An override model to display for this specific controller.")]
        private GameObject overrideModel;

        /// <summary>
        /// The controller model prefab to be rendered.
        /// </summary>
        public GameObject OverrideControllerModel => overrideModel;
    }
}