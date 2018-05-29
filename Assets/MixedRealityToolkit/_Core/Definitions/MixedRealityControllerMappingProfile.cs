// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mapping Profile")]
    public class MixedRealityControllerMappingProfile : ScriptableObject
    {
        //TODO - Does this also need to track Axis?

        [SerializeField]
        [Tooltip("Enable the Motion Controllers on Startup")]
        private Handedness controllingHand;

        public Handedness ControllingHand
        {
            get { return controllingHand; }
            private set { controllingHand = value; }
        }

        [SerializeField]
        [Tooltip("Controller type to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.None)]
        private SystemType controllerType;

        /// <summary>
        /// Controller Type to instantiate at runtime.
        /// </summary>
        public SystemType ControllerType
        {
            get { return controllerType; }
            private set { controllerType = value; }
        }

        [SerializeField]
        [Tooltip("Input System Class to instantiate at runtime.")]
        private InteractionMapping[] interactions = null;

        public InteractionMapping[] Interactions => interactions ?? new InteractionMapping[0];

        public InteractionMapping GetInteractionMapping(DeviceInputType inputType)
        {
            return default(InteractionMapping);
        }
    }
}