// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    // TODO - Implement
    public class OculusTouchController : BaseController
    {
        public OculusTouchController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource, List<IInteractionMapping> interactions)
                : base(controllerState, controllerHandedness, inputSource, interactions) { }
    }
}
