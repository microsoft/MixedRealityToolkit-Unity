// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using HoloToolkit.Unity.InputModule;
using System.Collections.ObjectModel;

public interface IFocusTarget : IEventSystemHandler
{
    void OnFocusEnter(FocusEventData eventData);

    void OnFocusExit(FocusEventData eventData);

    bool HasFocus { get; }

    bool FocusEnabled { get; set; }

    void ResetFocus();

    ReadOnlyCollection<IFocuser> Focusers { get; }

    // This will be automatically implemented by MonoBehavior
    GameObject gameObject { get; }
}
