// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Copy of Unity's XR.WSA.GestureSettings.
    /// </summary>
    [Flags]
    public enum GestureSettings
    {
        /// <summary>
        ///   <para>Disable support for gestures.</para>
        /// </summary>
        None = 0,

        /// <summary>
        ///   <para>Enable support for the tap gesture.</para>
        /// </summary>
        Tap = 1,

        /// <summary>
        ///   <para>Enable support for the double-tap gesture.</para>
        /// </summary>
        DoubleTap = 2,

        /// <summary>
        ///   <para>Enable support for the hold gesture.</para>
        /// </summary>
        Hold = 4,

        /// <summary>
        ///   <para>Enable support for the manipulation gesture which tracks changes to the hand's position.  This gesture is relative to the start position of the gesture and measures an absolute movement through the world.</para>
        /// </summary>
        ManipulationTranslate = 8,

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the horizontal axis.</para>
        /// </summary>
        NavigationX = 16, // 0x00000010

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the vertical axis.</para>
        /// </summary>
        NavigationY = 32, // 0x00000020

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the depth axis.</para>
        /// </summary>
        NavigationZ = 64, // 0x00000040

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the horizontal axis using rails (guides).</para>
        /// </summary>
        NavigationRailsX = 128, // 0x00000080

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the vertical axis using rails (guides).</para>
        /// </summary>
        NavigationRailsY = 256, // 0x00000100

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the depth axis using rails (guides).</para>
        /// </summary>
        NavigationRailsZ = 512, // 0x00000200
    }
}
