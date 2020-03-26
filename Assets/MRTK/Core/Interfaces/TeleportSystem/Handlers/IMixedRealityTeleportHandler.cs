// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    /// <summary>
    /// Interface to implement for teleport events.
    /// </summary>
    public interface IMixedRealityTeleportHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when a pointer requests a teleport target, but no teleport has begun.
        /// </summary>
        void OnTeleportRequest(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport has started.
        /// </summary>
        void OnTeleportStarted(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport has successfully completed.
        /// </summary>
        void OnTeleportCompleted(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport request has been canceled.
        /// </summary>
        void OnTeleportCanceled(TeleportEventData eventData);
    }
}
