// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsGaming
{
    // TODO
    public struct ArcadeStickController : IMixedRealityController
    {
        public ArcadeStickController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource, List<IMixedRealityInteractionMapping> interactions = null) : this()
        {
            ControllerState = controllerState;
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions ?? new List<IMixedRealityInteractionMapping>();
        }

        public ControllerState ControllerState { get; }

        public Handedness ControllerHandedness { get; }

        public IMixedRealityInputSource InputSource { get; }

        public List<IMixedRealityInteractionMapping> Interactions { get; }

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