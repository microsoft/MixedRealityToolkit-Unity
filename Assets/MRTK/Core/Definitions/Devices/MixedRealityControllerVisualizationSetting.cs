// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. 

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
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
            defaultModelMaterial = null;
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
        [Tooltip("Check to obtain controller models from the platform SDK. If left unchecked, the global models will be used.")]
        private bool useDefaultModel;

        /// <summary>
        /// Check to obtain controller models from the platform SDK. If left unchecked, the global models will be used.
        /// </summary>
        public bool UseDefaultModel => useDefaultModel;

        [SerializeField]
        [Tooltip("The default controller model material when loading platform SDK controller models.")]
        private Material defaultModelMaterial;

        /// <summary>
        /// The default controller model material when loading platform SDK controller models. This value is used as a fallback if no controller definition exists with a custom material type.
        /// </summary>
        public Material DefaultModelMaterial => defaultModelMaterial;

        [SerializeField]
        [Tooltip("An override model to display for this specific controller.")]
        private GameObject overrideModel;

        /// <summary>
        /// The controller model prefab to be rendered.
        /// </summary>
        public GameObject OverrideControllerModel => overrideModel;

        [SerializeField]
        [Tooltip("The concrete Controller Visualizer component to use on the rendered controller model.")]
        [Implements(typeof(IMixedRealityControllerVisualizer), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerVisualizationType;

        /// <summary>
        /// The concrete Controller Visualizer component to use on the rendered controller model
        /// </summary>
        public SystemType ControllerVisualizationType
        {
            get => controllerVisualizationType;
            private set => controllerVisualizationType = value;
        }
    }
}