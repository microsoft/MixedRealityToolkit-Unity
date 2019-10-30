// Copyright (c) Microsoft Corporation. All rights reserved.
// Copyright(c) 2019 Takahiro Miyaura
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// The type of displays on which an application may run.
    /// </summary>
    public enum DisplayType
    {
        /// <summary>
        /// The display is opaque. Devices on the digital reality (ex: VR) side of the Mixed Reality 
        /// spectrum generally have opaque displays.
        /// </summary>
        Opaque = 0,

        /// <summary>
        /// The display is transparent. Devices on the physical reality (ex: Microsoft HoloLens) side 
        /// of the Mixed Reality spectrum generally have transparent displays.
        /// </summary>
        Transparent
    }
}