// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Internal.Definitions
{
    /// <summary>
    /// The ControllerInputType identifies the type of button or input being sent to the framework from a controller.
    /// This is mainly information only or for advanced users to understand the input coming directly from the controller.
    /// </summary>
    [System.Flags]
    public enum ControllerInputType
    {
        /// <summary>
        /// No Specified type.
        /// </summary>
        None        = 0,
        /// <summary>
        /// Digital On/Off input.
        /// </summary>
        Digital     = 1,
        /// <summary>
        /// Single Axis analogue input.
        /// </summary>
        SingleAxis  = 2,
        /// <summary>
        /// Dual Axis analogue input.
        /// </summary>
        DualAxis    = 4,
        /// <summary>
        /// Dual Axis analogue input.
        /// </summary>
        ThreeDoF    = 8,
        /// <summary>
        /// Position AND Rotation analogue input.
        /// </summary>
        SixDoF      = 16,
        /// <summary>
        /// Raw stream from input (proxy only).
        /// </summary>
        Raw         = 32,
    }
}