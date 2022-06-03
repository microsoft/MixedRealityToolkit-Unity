// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines in which hand a controller is currently being used.
    /// </summary>
    [Flags]
    public enum Handedness
    {
        /// <summary>
        /// No hand specified by the SDK for the controller.
        /// </summary>
        /// <remarks>
        /// This value is used for controllers such as a gamepad or a mouse.
        /// </remarks>
        None = 0 << 0,

        /// <summary>
        /// The controller is identified as being provided in a Left hand
        /// </summary>
        Left = 1 << 0,

        /// <summary>
        /// The controller is identified as being provided in a Right hand
        /// </summary>
        Right = 1 << 1
    }
}