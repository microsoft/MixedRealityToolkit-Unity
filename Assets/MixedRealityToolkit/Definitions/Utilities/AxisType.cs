// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The AxisType identifies the type of button or input being sent to the framework from a controller.
    /// This is mainly information only or for advanced users to understand the input coming directly from the controller.
    /// </summary>
    public enum AxisType
    {
        /// <summary>
        /// No Specified type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Raw stream from input (proxy only).
        /// </summary>
        Raw,
        /// <summary>
        /// Digital On/Off input.
        /// </summary>
        Digital,
        /// <summary>
        /// Single Axis analogue input.
        /// </summary>
        SingleAxis,
        /// <summary>
        /// Dual Axis analogue input.
        /// </summary>
        DualAxis,
        /// <summary>
        /// Position only Axis analogue input.
        /// </summary>
        ThreeDofPosition,
        /// <summary>
        /// Rotation only Axis analogue input.
        /// </summary>
        ThreeDofRotation,
        /// <summary>
        /// Position AND Rotation analogue input.
        /// </summary>
        SixDof
    }
}