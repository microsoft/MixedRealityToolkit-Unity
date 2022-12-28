// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Represents handedness; as a bitflag, it can represent left-handedness,
    /// right-handedness, both, or neither.
    /// </summary>
    [Flags]
    public enum Handedness
    {
        /// <summary>
        /// No handedness. Represents a non-handed controller or object.
        /// </summary>
        None = 0 << 0,

        /// <summary>
        /// The user's left hand or controller.
        /// </summary>
        Left = 1 << 0,

        /// <summary>
        /// The user's right hand or controller.
        /// </summary>
        Right = 1 << 1,

        /// <summary>
        /// Both, or either, hands.
        /// </summary>
        Both = Left | Right,
    }
}