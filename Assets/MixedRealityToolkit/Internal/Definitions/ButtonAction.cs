// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Internal.Definitions
{
    /// <summary>
    /// The ButtonAction defines the set of actions exposed by a controller.
    /// Denoting the available buttons / interactions that a controller supports.
    /// </summary>
    [System.Flags]
    public enum ButtonAction
    {
        Trigger             = 0,
        Grab                = 1,
        ThumbStick          = 2,
        ThumbStickButton    = 4,
        Touchpad            = 8,
        TouchpadTouch       = 16,
        TouchpadButton      = 32,
        Start               = 64,
        Menu                = 128,
        ButtonOne           = 256,
        ButtonTwo           = 512,
        ButtonThree         = 1024,
        ButtonFour          = 2048,
        ButtonFive          = 4096,
        ButtonSix           = 8192,
        ButtonSeven         = 16384,
        ButtonEight         = 32768,
        ButtonNine          = 65536,
        ButtonTen           = 131072
    }
}