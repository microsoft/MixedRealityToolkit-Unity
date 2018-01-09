// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    [Obsolete("Use ISourceStateHandler")]
    public interface IGamePadHandler : IEventSystemHandler
    {
        void OnGamePadDetected(GamePadEventData eventData);
        void OnGamePadLost(GamePadEventData eventData);
    }
}
