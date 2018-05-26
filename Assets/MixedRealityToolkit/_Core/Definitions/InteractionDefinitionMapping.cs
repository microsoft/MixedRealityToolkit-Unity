// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    public struct InteractionDefinitionMapping
    {
        public DeviceInputType InputType { get; set; }
        public AxisType AxisType { get; set; }
        public InputAction InputAction { get; set; }
        public InputAction InputHoldAction { get; set; }
    }
}
