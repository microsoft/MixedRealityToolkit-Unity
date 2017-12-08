using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HoloToolkit.Unity.InputModule;

public interface IFocusTarget : IEventSystemHandler
{
    void OnFocusEnter(FocusEventData eventData);

    void OnFocusExit(FocusEventData eventData);

    bool HasFocus { get; }

    bool FocusEnabled { get; set; }

    void ResetFocus();

    List<IFocuser> Focusers { get; }

    // This will be automatically implemented by MonoBehavior
    GameObject gameObject { get; }
}
