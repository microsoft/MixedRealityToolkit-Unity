// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.InputHandlers
{
    public interface ITeleportHandler : IEventSystemHandler
    {
        void OnTeleportIntent(TeleportEventData eventData);
        void OnTeleportStarted(TeleportEventData eventData);
        void OnTeleportCompleted(TeleportEventData eventData);
        void OnTeleportCanceled(TeleportEventData eventData);
    }
}
