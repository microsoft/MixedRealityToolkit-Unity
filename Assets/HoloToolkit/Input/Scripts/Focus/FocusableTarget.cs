using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class FocusableTarget : MonoBehaviour, IFocusTarget, IInputHandler
{
    public new GameObject gameObject;

    public virtual void OnFocusEnter(FocusEventData eventData)
    {
        Debug.Log("Focuser Enter: " + eventData.Focuser);
        focusers.Add(eventData.Focuser);
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

    public bool FocusEnabled { get; set; }

    public bool HasFocus
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    private List<IFocuser> focusers;

    public List<IFocuser> Focusers
    {
        get
        {
            return focusers;
        }
    }

}
