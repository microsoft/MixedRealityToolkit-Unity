// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// The Controller definition defines the Controller as defined by the SDK / Unity.
    /// </summary>
    public struct Controller
    {
        public Controller(uint id, Handedness handedness, Vector3 position, Quaternion rotation, ControllerState controllerState, InteractionMapping[] interactions) : this()
        {
            Id = id;
            Handedness = handedness;
            Position = position;
            Rotation = rotation;
            ControllerState = controllerState;
            Interactions = interactions;
        }


        /// <summary>
        /// The ID assigned to the Controller.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// The designated hand that the controller is managing, as defined by the SDK / Unity.
        /// </summary>
        public Handedness Handedness { get; }

        /// <summary>
        /// Outputs the current position of the controller, as defined by the SDK / Unity.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Outputs the current rotation of the controller, as defined by the SDK / Unity.
        /// </summary>
        public Quaternion Rotation { get; }

        /// <summary>
        /// Outputs the current state of the controller, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        public ControllerState ControllerState { get; }

        /// <summary>
        /// Details the list of available buttons / interactions available from the controller.
        /// </summary>
        public InteractionMapping[] Interactions { get; }
    }
}