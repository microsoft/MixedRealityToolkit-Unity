// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    public struct ValveKnucklesController : IMixedRealityController
    {
        public ValveKnucklesController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource, Dictionary<DeviceInputType, InteractionMapping> interactions) : this()
        {
            ControllerState = controllerState;
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions;
        }

        /// <inheritdoc />
        public ControllerState ControllerState { get; }

        /// <inheritdoc />
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSource { get; }

        /// <inheritdoc />
        public Dictionary<DeviceInputType, InteractionMapping> Interactions { get; }

        /// <inheritdoc />
        public void SetupInputSource<T>(IMixedRealityInputSystem inputSystem, T state)
        {
            // TODO
        }

        /// <inheritdoc />
        public void UpdateInputSource<T>(IMixedRealityInputSystem inputSystem, T state)
        {
            //TODO
        }
    }
}
