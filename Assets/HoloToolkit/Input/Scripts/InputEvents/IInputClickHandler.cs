// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to simple click input.
    /// </summary>
    public interface IInputClickHandler : IEventSystemHandler
    {
#if UNITY_WSA
        void OnInputClicked(InputEventData eventData);
#endif
    }
}

