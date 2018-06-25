// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsGaming
{
    // TODO
    internal struct GamepadController : IMixedRealityController
    {
        public GamepadController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource, Dictionary<DeviceInputType, IMixedRealityInteractionMapping> interactions = null) : this()
        {
            ControllerState = controllerState;
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions ?? new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
        }

        public ControllerState ControllerState { get; }

        public Handedness ControllerHandedness { get; }

        public IMixedRealityInputSource InputSource { get; }

        public Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions { get; }

        public void SetupInputSource<T>(T state)
        {
            // TODO
        }

        public void UpdateInputSource<T>(T state)
        {
            //TODO
        }
    }
}
