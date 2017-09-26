using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;


/// <summary>
/// A usable object is one that can be "used" or actiavated while being grabbed/carried
/// A gun and a remote control are examples: first grab, then press a different button to use
/// </summary>

public abstract class BaseUsable : MonoBehaviour
{

    //assign a controller button to "use" the object
    public ButtonChoice ActivateUseButton;

    //fires an event called UseActive
    public delegate void UseActivated();
    public static event UseActivated UseStartedEvent;

    public delegate void UseDeactivated();
    public static event UseDeactivated UseEndedEvent;

    //Subscribe GrabStart and GrabEnd to InputEvents for Grip
    protected virtual void OnEnable()
    {
        InteractionManager.InteractionSourcePressed += UseStarted;
        InteractionManager.InteractionSourceReleased += UseEnded;
    }

    protected virtual void OnDisable()
    {
        InteractionManager.InteractionSourcePressed -= UseStarted;
        InteractionManager.InteractionSourceReleased -= UseEnded;
    }

    protected virtual void UseStarted(InteractionSourcePressedEventArgs obj)
    {
        if (ActivateUseButton.Equals(ButtonChoice.Touchpad))
            touchPadActive = true;
    }

    protected virtual void UseEnded(InteractionSourceReleasedEventArgs obj)
    {
        if (ActivateUseButton.Equals(ButtonChoice.Touchpad))
            touchPadActive = false;
    }


    protected bool touchPadActive;

}
