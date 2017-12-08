// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System.Collections.ObjectModel;

public class FocusTarget : MonoBehaviour, IFocusTarget, IInputHandler
{
    public new GameObject gameObject;

    [SerializeField]
    private bool focusEnabled = true;

    public virtual void OnFocusEnter(FocusEventData eventData)
    {
        Debug.Log("Focuser Enter: " + eventData.Focuser);
        if (!focusers.Contains(eventData.Focuser))
        {
            focusers.Add(eventData.Focuser);
        }
    }

    public virtual void OnFocusExit(FocusEventData eventData)
    {
        Debug.Log("Focuser Exit: " + eventData.Focuser);
        if(focusers.Contains(eventData.Focuser))
        {
            focusers.Remove(eventData.Focuser);
        }
    }

    public void ResetFocus()
    {

    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log("Down");
    }

    public void OnInputUp(InputEventData eventData)
    {
        Debug.Log("Up");
    }

    public bool FocusEnabled { get { return focusEnabled; } set {  FocusEnabled = value; } }

    public bool HasFocus
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    private List<IFocuser> focusers = new List<IFocuser>();

    public ReadOnlyCollection<IFocuser> Focusers
    {
        get
        {
            return focusers.AsReadOnly();
        }
    }

}
