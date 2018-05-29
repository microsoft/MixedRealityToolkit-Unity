// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenXR
{
    // TODO - Implement
    public struct OpenXRController : IMixedRealityController
    {
        #region Private properties

        #endregion Private properties

        public OpenXRController(uint sourceId, Handedness handedness)
        {
            ControllerState = ControllerState.None;

            ControllerHandedness = handedness;

            InputSource = null;

            Interactions = new Dictionary<DeviceInputType, InteractionMapping>();
        }

        #region IMixedRealityController Interface Members

        public ControllerState ControllerState { get; private set; }

        public Handedness ControllerHandedness { get; }

        public IMixedRealityInputSource InputSource { get; private set; }

        public Dictionary<DeviceInputType, InteractionMapping> Interactions { get; private set; }

        public void SetupInputSource<T>(IMixedRealityInputSystem inputSystem, T state)
        {
            if (inputSystem != null)
            {
                InputSource = inputSystem.RequestNewGenericInputSource($"OpenXR Controller {ControllerHandedness}");
            }

            // TODO
        }

        public void UpdateInputSource<T>(IMixedRealityInputSystem inputSystem, T state)
        {
            // TODO
        }

        #endregion IMixedRealityInputSource Interface Members

        #region Setup and Update functions

        // TODO

        #endregion Setup and Update functions

        #region Utilities

        // TODO - if required

        #endregion Utilities
    }
}