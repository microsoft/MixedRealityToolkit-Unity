// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Copy of Unity's <see cref="UnityEngine.XR.MagicLeap.MLInputControllerTouchpadGestureType"/>
    /// </summary>
    [Flags]
    public enum LuminControllerGestureSettings
    {
        //None = 1 << 0, We don't keep this one so we can hide it in then enum drop down.
        /// <summary>
        /// Enable support for controller touchpad Tap gesture.
        /// </summary>
        Tap = 1 << 1,
        /// <summary>
        /// Enable support for controller touchpad ForceTapDown gesture.
        /// </summary>
        ForceTapDown = 1 << 2,
        /// <summary>
        /// Enable support for controller touchpad ForceTapUp gesture.
        /// </summary>
        ForceTapUp = 1 << 3,
        /// <summary>
        /// Enable support for controller touchpad ForceDwell gesture.
        /// </summary>
        ForceDwell = 1 << 4,
        /// <summary>
        /// Enable support for controller touchpad SecondForceDown gesture.
        /// </summary>
        SecondForceDown = 1 << 5,
        /// <summary>
        /// Enable support for controller touchpad LongHold gesture.
        /// </summary>
        LongHold = 1 << 6,
        /// <summary>
        /// Enable support for controller touchpad RadialScroll gesture.
        /// </summary>
        RadialScroll = 1 << 7,
        /// <summary>
        /// Enable support for controller touchpad Swipe gesture.
        /// </summary>
        Swipe = 1 << 8,
        /// <summary>
        /// Enable support for controller touchpad Scroll gesture.
        /// </summary>
        Scroll = 1 << 9,
        /// <summary>
        /// Enable support for controller touchpad Pinch gesture.
        /// </summary>
        Pinch = 1 << 10,
    }
}