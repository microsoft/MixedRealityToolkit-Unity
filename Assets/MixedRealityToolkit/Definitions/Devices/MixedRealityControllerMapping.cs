// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Editor.Inspectors")]
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
            if (interactions == null || interactions.Length == 0 || overwrite)
            {
                MixedRealityInteractionMapping[] defaultMappings = GetDefaultInteractionMappings();

                if (defaultMappings != null)
                {
                    interactions = defaultMappings;
                }
            }
        }

        internal bool UpdateInteractionSettingsFromDefault()
        {
            if (interactions == null || interactions.Length == 0) { return false; }

            MixedRealityInteractionMapping[] newDefaultInteractions = GetDefaultInteractionMappings();

            if (newDefaultInteractions == null)
            {
                return false;
            }

            if (interactions.Length != newDefaultInteractions.Length)
            {
                interactions = CreateNewMatchedMapping(interactions, newDefaultInteractions);
                return true;
            }

            bool updatedMappings = false;

            for (int i = 0; i < newDefaultInteractions.Length; i++)
            {
                MixedRealityInteractionMapping currentMapping = interactions[i];
                MixedRealityInteractionMapping currentDefaultMapping = newDefaultInteractions[i];

                if (Equals(currentMapping, currentDefaultMapping))
                {
                    interactions[i] = new MixedRealityInteractionMapping(currentDefaultMapping)
                    {
                        MixedRealityInputAction = currentMapping.MixedRealityInputAction
                    };

                    updatedMappings = true;
                }
            }

            return updatedMappings;
        }

        private MixedRealityInteractionMapping[] CreateNewMatchedMapping(MixedRealityInteractionMapping[] interactions, MixedRealityInteractionMapping[] newDefaultInteractions)
        {
            MixedRealityInteractionMapping[] newDefaultMapping = new MixedRealityInteractionMapping[newDefaultInteractions.Length];

            for (int i = 0; i < newDefaultInteractions.Length; i++)
            {
                for (int j = 0; j < interactions.Length; j++)
                {
                    if (Equals(interactions[j], newDefaultInteractions[i]))
                    {
                        newDefaultMapping[i] = new MixedRealityInteractionMapping(newDefaultInteractions[i])
                        {
                            MixedRealityInputAction = interactions[j].MixedRealityInputAction
                        };
                        break;
                    }
                }

                if (newDefaultMapping[i] == null)
                {
                    newDefaultMapping[i] = new MixedRealityInteractionMapping(newDefaultInteractions[i]);
                }
            }

            return newDefaultMapping;
        }

        private bool Equals(MixedRealityInteractionMapping a, MixedRealityInteractionMapping b)
        {
            return !(a.Description != b.Description ||
                        a.AxisType != b.AxisType ||
                        a.InputType != b.InputType ||
                        a.KeyCode != b.KeyCode ||
                        a.AxisCodeX != b.AxisCodeX ||
                        a.AxisCodeY != b.AxisCodeY ||
                        a.InvertXAxis != b.InvertXAxis ||
                        a.InvertYAxis != b.InvertYAxis);
        }

        private MixedRealityInteractionMapping[] GetDefaultInteractionMappings()
        {
            if (Activator.CreateInstance(controllerType, TrackingState.NotTracked, handedness, null, null) is BaseController detectedController)
            {
                switch (handedness)
                {
                    case Handedness.Left:
                        return detectedController.DefaultLeftHandedInteractions;
                    case Handedness.Right:
                        return detectedController.DefaultRightHandedInteractions;
                    default:
                        return detectedController.DefaultInteractions;
                }
            }

            return null;
        }

        /// <summary>
        /// Synchronizes the input actions of the same physical controller of a different concrete type.
        /// </summary>
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