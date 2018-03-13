// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Internal.Definitons
{
    /// <summary>
    /// The InputType identifies the type of button or input being sent to the framework from a controller.
    /// This is mainly information only or for advanced users to understand the input coming directly from the controller.
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// No Specified type.
        /// </summary>
        None,
        /// <summary>
        /// Single Axis analogue input.
        /// </summary>
        Analogue1X,
        /// <summary>
        /// Dual Axis analogue input.
        /// </summary>
        Analogue2X,
        /// <summary>
        /// Digital On/Off input.
        /// </summary>
        Digital,
        /// <summary>
        /// Raw stream from input (proxy only).
        /// </summary>
        Raw,
    }
}