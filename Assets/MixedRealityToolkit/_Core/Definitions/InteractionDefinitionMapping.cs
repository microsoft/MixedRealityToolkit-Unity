// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    // TODO - Need to think about adding filtering to the Controller definition?
    /// <summary>
    /// A mapping construct, linking the Physical inputs of a controller to a Logical construct in a runtime project
    /// </summary>
    public struct InteractionDefinitionMapping
    {
        /// <summary>
        /// The physical button / control you want to map
        /// </summary>
        public DeviceInputType InputType { get; set; }

        /// <summary>
        /// The Axis of the Action to be used in your project 
        /// *Note list will be filtered to only those controller inputs that support this Axis
        /// </summary>
        public AxisType AxisType { get; set; }

        /// <summary>
        /// The logical Action to be performed in your project
        /// E.G. Click, select, interact, pick-up, etc.
        /// </summary>
        public InputAction InputAction { get; set; }
    }
}