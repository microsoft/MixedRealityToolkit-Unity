// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem
{
    /// <summary>
    /// Manager interface for a Teleport system in the Mixed Reality Toolkit
    /// All replacement systems for providing Teleportation functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityTeleportSystem
    {
        /// <summary>
        /// Raise a teleportation request event.
        /// </summary>
        /// <param name="pointer"></param>
        void RaiseTeleportRequest(IMixedRealityPointer pointer);

        /// <summary>
        /// Raise a teleportation started event.
        /// </summary>
        /// <param name="pointer"></param>
        void RaiseTeleportStarted(IMixedRealityPointer pointer);

        /// <summary>
        /// Raise a teleportation completed event.
        /// </summary>
        /// <param name="pointer"></param>
        void RaiseTeleportComplete(IMixedRealityPointer pointer);

        /// <summary>
        /// Raise a teleportation canceled event.
        /// </summary>
        /// <param name="pointer"></param>
        void RaiseTeleportCanceled(IMixedRealityPointer pointer);
    }
}
