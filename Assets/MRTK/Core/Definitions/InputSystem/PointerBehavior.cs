
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Specifies how a pointer in MRTK's default input system behaves.
    /// </summary>
    public enum PointerBehavior
    {
        /// <summary>
        /// Pointer active state is managed by MRTK input system. If it is a near pointer (grab, poke), it
        /// will be always enabled. If it is not a near pointer, it will get disabled if any near pointer on the 
        /// same hand is active. This is what allows rays to turn off when a hand is near a grabbable.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Pointer is always on, regardless of what other pointers are active.
        /// </summary>
        AlwaysOn,
        /// <summary>
        /// Pointer is always off, regardless of what other pointers are active.
        /// </summary>
        AlwaysOff
    };
}