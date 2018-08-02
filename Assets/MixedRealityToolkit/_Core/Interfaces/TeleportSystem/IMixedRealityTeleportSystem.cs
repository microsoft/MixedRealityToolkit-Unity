// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Events;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem
{
    /// <summary>
    /// Manager interface for a Teleport system in the Mixed Reality Toolkit
    /// All replacement systems for providing Teleportation functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityTeleportSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Raise a teleportation request event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="target">The teleport target</param>
        void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportTarget target);

        /// <summary>
        /// Raise a teleportation started event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="target">The teleport target</param>
        void RaiseTeleportStarted(IMixedRealityPointer pointer, IMixedRealityTeleportTarget target);

        /// <summary>
        /// Raise a teleportation completed event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="target">The teleport target</param>
        void RaiseTeleportComplete(IMixedRealityPointer pointer, IMixedRealityTeleportTarget target);

        /// <summary>
        /// Raise a teleportation canceled event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="target">The teleport target</param>
        void RaiseTeleportCanceled(IMixedRealityPointer pointer, IMixedRealityTeleportTarget target);
    }
}
