// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Teleport;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface IMixedRealityTeleportPointer : IMixedRealityPointer
    {
        /// <summary>
        /// True when teleport pointer has raised a request with the teleport manager.
        /// </summary>
        bool TeleportRequestRaised { get; }

        /// <summary>
        /// The currently active teleport hotspot.
        /// </summary>
        IMixedRealityTeleportHotspot TeleportHotspot { get; set; }

        /// <summary>
        /// The Y orientation of the pointer - used for touchpad rotation and navigation
        /// </summary>
        float PointerOrientation { get; }
    }
}