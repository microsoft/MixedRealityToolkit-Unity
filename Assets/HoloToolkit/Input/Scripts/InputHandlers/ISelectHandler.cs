// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to select pressed changes.
    /// </summary>
    public interface ISelectHandler : IEventSystemHandler
    {
        void OnSelectPressedAmountChanged(SelectPressedEventData eventData);
    }
}