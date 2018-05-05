// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers
{
    public interface IMixedRealityTeleportHandler : IEventSystemHandler
    {
        void OnTeleportIntent(TeleportEventData eventData);
        void OnTeleportStarted(TeleportEventData eventData);
        void OnTeleportCompleted(TeleportEventData eventData);
        void OnTeleportCanceled(TeleportEventData eventData);
    }
}
