// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    public class GamepadInputSource : BaseGenericInputSource
    {
        public GamepadInputSource(string name, Dictionary<Internal.Definitions.Devices.DeviceInputType, InteractionDefinition> interactions, IMixedRealityPointer[] pointers = null) : base(name, interactions, pointers) { }
    }
}
