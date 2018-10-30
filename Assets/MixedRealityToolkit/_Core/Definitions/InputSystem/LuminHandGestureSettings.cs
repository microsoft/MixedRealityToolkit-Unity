// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Copy of Unity's <see cref="UnityEngine.XR.MagicLeap.MLHandKeyPose"/>
    /// </summary>
    [Flags]
    public enum LuminHandGestureSettings
    {
        /// <summary>
        /// Enable support for hand Finger gesture.
        /// </summary>
        Finger = 1 << 0,
        /// <summary>
        /// Enable support for hand Fist gesture.
        /// </summary>
        Fist = 1 << 1,
        /// <summary>
        /// Enable support for hand Pinch gesture.
        /// </summary>
        Pinch = 1 << 2,
        /// <summary>
        /// Enable support for hand Thumb gesture.
        /// </summary>
        Thumb = 1 << 3,
        /// <summary>
        /// Enable support for hand L gesture.
        /// </summary>
        L = 1 << 4,
        /// <summary>
        /// Enable support for hand OpenHandBack gesture.
        /// </summary>
        OpenHandBack = 1 << 5,
        /// <summary>
        /// Enable support for hand Ok gesture.
        /// </summary>
        Ok = 1 << 6,
        /// <summary>
        /// Enable support for hand C gesture.
        /// </summary>
        C = 1 << 7,
        /// <summary>
        /// Enable support for hand NoPose gesture.
        /// </summary>
        NoPose = 1 << 8
    }
}