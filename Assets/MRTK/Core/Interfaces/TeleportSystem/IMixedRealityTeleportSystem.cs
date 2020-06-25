// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    /// <summary>
    /// Manager interface for a Teleport system in the Mixed Reality Toolkit
    /// All replacement systems for providing Teleportation functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityTeleportSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// The duration of the teleport in seconds.
        /// </summary>
        float TeleportDuration { get; set; }

        /// <summary>
        /// Raise a teleportation request event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        void RaiseTeleportRequest(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);

        /// <summary>
        /// Raise a teleportation started event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        void RaiseTeleportStarted(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);

        /// <summary>
        /// Raise a teleportation canceled event.
        /// </summary>
        /// <param name="pointer">The pointer that raised the event.</param>
        /// <param name="hotSpot">The teleport target</param>
        void RaiseTeleportCanceled(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot hotSpot);
    }
}
