// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Internal.Inspectors")]
namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// Used to define a controller or other input device's physical buttons, and other attributes.
    /// </summary>
    [Serializable]
    public struct MixedRealityControllerMapping
    {
        public MixedRealityControllerMapping(uint id, string description, Type controllerType, Handedness handedness = Handedness.None, bool useCustomInteractionMappings = false, GameObject overrideModel = null) : this()
        {
            this.id = id;
            this.description = description;
            this.controllerType = new SystemType(controllerType);
            this.handedness = handedness;
            this.overrideModel = overrideModel;
            this.useCustomInteractionMappings = useCustomInteractionMappings;
            interactions = null;
            useDefaultModel = false;
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
        public SystemType ControllerType => controllerType;

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType;

        /// <summary>
        /// The designated hand that the device is managing.
        /// </summary>
        public Handedness Handedness => handedness;

        [SerializeField]
        [Tooltip("The designated hand that the device is managing.")]
        private Handedness handedness;

        [SerializeField]
        [Tooltip("Use the platform SDK to load the default controller model for this controller.")]
        private bool useDefaultModel;

        /// <summary>
        /// User the controller model loader provided by the SDK, or provide override models.
        /// </summary>
        public bool UseDefaultModel => useDefaultModel;

        /// <summary>
        /// The controller model prefab to be rendered.
        /// </summary>
        public GameObject OverrideControllerModel => overrideModel;

        [SerializeField]
        [Tooltip("An override model to display for this specific controller.")]
        private GameObject overrideModel;

        [SerializeField]
        [Tooltip("Override the default interaction mappings.")]
        private bool useCustomInteractionMappings;

        /// <summary>
        /// Is this controller mapping using custom interactions?
        /// </summary>
        public bool HasCustomInteractionMappings => useCustomInteractionMappings;

        /// <summary>
        /// Details the list of available buttons / interactions available from the device.
        /// </summary>
        public MixedRealityInteractionMapping[] Interactions => interactions;

        [SerializeField]
        [Tooltip("Details the list of available buttons / interactions available from the device.")]
        private MixedRealityInteractionMapping[] interactions;

        /// <summary>
        /// Sets the default interaction mapping based on the current controller type.
        /// </summary>
        internal void SetDefaultInteractionMapping(bool overwrite = false)
        {
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, handedness, null, null) as BaseController;

            if (detectedController != null && (interactions == null || interactions.Length == 0 || overwrite))
            {
                switch (handedness)
                {
                    case Handedness.Left:
                        interactions = detectedController.DefaultLeftHandedInteractions;
                        break;
                    case Handedness.Right:
                        interactions = detectedController.DefaultRightHandedInteractions;
                        break;
                    default:
                        interactions = detectedController.DefaultInteractions;
                        break;
                }
            }
        }

        /// <summary>
        /// Synchronizes the Input Actions of the same physical controller of a different concrete type.
        /// </summary>
        /// <param name="otherControllerMapping"></param>
        internal void SynchronizeInputActions(MixedRealityInteractionMapping[] otherControllerMapping)
        {
            if (otherControllerMapping.Length != interactions.Length)
            {
                Debug.LogError("Controller Input Actions must be the same length!");
                return;
            }

            for (int i = 0; i < interactions.Length; i++)
            {
                interactions[i].MixedRealityInputAction = otherControllerMapping[i].MixedRealityInputAction;
            }
        }
    }
}