// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System.Collections.ObjectModel;

public class FocusTarget : MonoBehaviour, IFocusTarget
{
    public virtual bool FocusEnabled { get { return focusEnabled; } set { focusEnabled = value; } }

    public virtual bool HasFocus
    {
        get
        {
            return FocusEnabled && focusers.Count > 0;
        }
    }

    public List<IFocuser> Focusers
    {
        get
        {
            // TODO: potentially cache a readonly collection to prevent manipulation
            return focusers;
        }
    }

    private List<IFocuser> focusers = new List<IFocuser>();

    [SerializeField]
    private bool focusEnabled = true;

    public virtual void OnFocusEnter(FocusEventData eventData)
    {
        //Debug.Log("Focuser Enter: " + eventData.Focuser);
        if (!focusers.Contains(eventData.Focuser))
        {
            focusers.Add(eventData.Focuser);
        }
    }

    public virtual void OnFocusExit(FocusEventData eventData)
    {
        //Debug.Log("Focuser Exit: " + eventData.Focuser);
        focusers.Remove(eventData.Focuser);
    }

    public void ResetFocus()
    {
        focusers.Clear();
    }

}
