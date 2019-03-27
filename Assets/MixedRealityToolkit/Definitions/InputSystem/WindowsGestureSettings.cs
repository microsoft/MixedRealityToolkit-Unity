// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Windows.Input
{
    /// <summary>
    /// Copy of Unity's <see href="https://docs.unity3d.com/ScriptReference/XR.WSA.Input.GestureSettings.html">GestureSettings</see>
    /// </summary>
    [Flags]
    public enum WindowsGestureSettings
    {
        /// <summary>
        ///   <para>Enable support for the tap gesture.</para>
        /// </summary>
        Tap = 1 << 0, // HEX: 0x00000001 | Decimal: 1

        /// <summary>
        ///   <para>Enable support for the double-tap gesture.</para>
        /// </summary>
        DoubleTap = 1 << 1, // HEX: 0x00000002 | Decimal: 2

        /// <summary>
        ///   <para>Enable support for the hold gesture.</para>
        /// </summary>
        Hold = 1 << 2, // HEX: 0x00000004 | Decimal: 4

        /// <summary>
        ///   <para>Enable support for the manipulation gesture which tracks changes to the hand's position.  This gesture is relative to the start position of the gesture and measures an absolute movement through the world.</para>
        /// </summary>
        ManipulationTranslate = 1 << 3, // HEX: 0x00000008 | Decimal: 8

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the horizontal axis.</para>
        /// </summary>
        NavigationX = 1 << 4, // HEX: 0x00000010 | Decimal: 16

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the vertical axis.</para>
        /// </summary>
        NavigationY = 1 << 5, // HEX: 0x00000020 | Decimal: 32

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the depth axis.</para>
        /// </summary>
        NavigationZ = 1 << 6, // HEX: 0x00000040 | Decimal: 64

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the horizontal axis using rails (guides).</para>
        /// </summary>
        NavigationRailsX = 1 << 7, // HEX: 0x00000080 | Decimal: 128

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the vertical axis using rails (guides).</para>
        /// </summary>
        NavigationRailsY = 1 << 8, // HEX: 0x00000100 | Decimal: 256

        /// <summary>
        ///   <para>Enable support for the navigation gesture, in the depth axis using rails (guides).</para>
        /// </summary>
        NavigationRailsZ = 1 << 9, // HEX: 0x00000200 | Decimal: 512
    }
}
