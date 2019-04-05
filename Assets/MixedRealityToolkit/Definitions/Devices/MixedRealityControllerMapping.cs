// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Core.Inspectors")]
namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Used to define a controller or other input device's physical buttons, and other attributes.
    /// </summary>
    [Serializable]
    public struct MixedRealityControllerMapping
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controllerType">Controller Type to instantiate at runtime.</param>
        /// <param name="handedness">The designated hand that the device is managing.</param>
        public MixedRealityControllerMapping(Type controllerType, Handedness handedness = Handedness.None) : this()
        {
            this.controllerType = new SystemType(controllerType);
            this.handedness = handedness;
            interactions = null;
        }

        /// <summary>
        /// Description of the Device.
        /// </summary>
        public string Description
        {
            get
            {
                string controllerName = "Unknown";
                if (controllerType.Type != null)
                {
                    var attr = MixedRealityControllerAttribute.Find(controllerType);
                    if (attr != null)
                    {
                        controllerName = attr.SupportedControllerType.ToString().ToProperCase();
                    }
                }

                string handednessText = string.Empty;
                switch (handedness)
                {
                    case Handedness.Left:
                    case Handedness.Right:
                        handednessText = $"{handedness} Hand ";
                        // Avoid multiple occurrences of "Hand":
                        controllerName = controllerName.Replace("Hand", "").Trim();
                        break;
                }

                return $"{controllerName} {handednessText}Controller";
            }
        }

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        public SystemType ControllerType => controllerType;

        public SupportedControllerType SupportedControllerType
        {
            get
            {
                if (controllerType.Type != null)
                {
                    var attr = MixedRealityControllerAttribute.Find(controllerType);
                    if (attr != null)
                    {
                        return attr.SupportedControllerType;
                    }
                }
                return 0;
            }
        }

        [SerializeField]
        [Tooltip("The designated hand that the device is managing.")]
        private Handedness handedness;

        /// <summary>
        /// The designated hand that the device is managing.
        /// </summary>
        public Handedness Handedness => handedness;

        /// <summary>
        /// Is this controller mapping using custom interactions?
        /// </summary>
        public bool HasCustomInteractionMappings
        {
            get
            {
                if (controllerType.Type != null)
                {
                    var attr = MixedRealityControllerAttribute.Find(controllerType);
                    if (attr != null)
                    {
                        return attr.Flags.HasFlag(MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings);
                    }
                }
                return false;
            }
        }

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
                throw new ArgumentException($"otherControllerMapping length {otherControllerMapping.Length} does not match this length {interactions.Length}.");
            }

            for (int i = 0; i < interactions.Length; i++)
            {
                interactions[i].MixedRealityInputAction = otherControllerMapping[i].MixedRealityInputAction;
            }
        }
    }
}