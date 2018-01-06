// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to simple pointer input.
    /// </summary>
    public interface IPointerHandler : IEventSystemHandler
    {
        void OnPointerUp(ClickEventData eventData);
        void OnPointerDown(ClickEventData eventData);
        void OnPointerClicked(ClickEventData eventData);
    }
}