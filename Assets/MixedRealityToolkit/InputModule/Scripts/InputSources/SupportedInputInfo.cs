// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.InputModule.InputSources
{
    /// <summary>
    /// Flags used to indicate which input information is supported by an input source.
    /// </summary>
    [Flags]
    public enum SupportedInputInfo
    {
        None = 0,
        [Obsolete("use PointerPosition")]
        Position = (1 << 0),
        PointerPosition = (1 << 0),
        [Obsolete("use PointerRotation")]
        Rotation = (1 << 1),
        PointerRotation = (1 << 1),
        Pointing = (1 << 2),
        Thumbstick = (1 << 3),
        Touchpad = (1 << 4),
        Select = (1 << 5),
        Menu = (1 << 6),
        Grasp = (1 << 7),
        GripPosition = (1 << 8),
        GripRotation = (1 << 9)
    }
}