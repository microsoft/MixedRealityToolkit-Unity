// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// The Controller definition defines the Controller as defined by the SDK / Unity.
    /// </summary>
    public struct Controller
    {
        /// <summary>
        /// The ID assigned to the Controller
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The designated hand that the controller is managing, as defined by the SDK / Unity.
        /// </summary>
        public Handedness Handedness { get; set; }

        /// <summary>
        /// Outputs the current position of the controller, as defined by the SDK / Unity.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Outputs the current rotation of the controller, as defined by the SDK / Unity.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Outputs the current state of the controller, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        public ControllerState ControllerState { get; set; }

        /// <summary>
        /// Details the list of available buttons / interactions available from the controller.
        /// </summary>
        public InteractionDefinition[] Interactions { get; set; }
    }
}