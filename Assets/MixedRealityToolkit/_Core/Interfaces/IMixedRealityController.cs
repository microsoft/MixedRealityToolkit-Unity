// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// The Controller definition defines the Controller as defined by the SDK / Unity.
    /// </summary>
    public interface IMixedRealityController<T> : IMixedRealityInputSource
    {
        /// <summary>
        /// The designated hand that the controller is managing, as defined by the SDK / Unity.
        /// </summary>
        Handedness Handedness { get; }

        /// <summary>
        /// Outputs the current position of the controller, as defined by the SDK / Unity.
        /// </summary>
        T Controller { get; }

        /// <summary>
        /// Outputs the current Pointer position of the controller, as defined by the SDK / Unity.
        /// </summary>
        T Pointer { get; }

        /// <summary>
        /// Outputs the current Grip position of the controller, as defined by the SDK / Unity.
        /// </summary>
        T Grip { get;}

        /// <summary>
        /// Outputs the current state of the controller, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        ControllerState ControllerState { get; }

        /// <summary>
        /// Details the list of available buttons / interactions available from the controller.
        /// </summary>
        InteractionDefinition[] Interactions { get; }

        /// <summary>
        /// The model to instantiate in the scene for this controller
        /// </summary>
        GameObject ControllerModel { get; set; }
    }
}