// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to focus enter/exit.
    /// </summary>
    public interface IFocusHandler : IEventSystemHandler
    {
        void OnFocusEnter(FocusEventData eventData);
        void OnFocusExit(FocusEventData eventData);
        void OnFocusChanged(FocusEventData eventData);
    }
}
