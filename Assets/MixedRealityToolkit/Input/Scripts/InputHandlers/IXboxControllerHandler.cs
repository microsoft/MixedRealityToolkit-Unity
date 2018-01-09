// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Input.EventData;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.Input.InputHandlers
{
    public interface IXboxControllerHandler : IEventSystemHandler
    {
        void OnXboxInputUpdate(XboxControllerEventData eventData);
    }
}
