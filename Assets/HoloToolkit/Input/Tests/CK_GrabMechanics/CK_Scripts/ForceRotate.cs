using System.Collections;
using UnityEngine.XR.WSA.Input;


/// <summary>
/// ForceRotate inherits from BaseUsable because the object to be manipulated must first be
/// pick up (grabbed) and is then "usable"
/// </summary>

public class ForceRotate : BaseUsable
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    /// <summary>
    /// In the BaseUsable class that this class inherits from, UseStarted begins checking for usage
    /// after the object is grabbed/picked up
    /// </summary>
    /// <param name="obj"></param>
    protected override void UseStarted(InteractionSourcePressedEventArgs obj)
    {
        base.UseStarted(obj);
        switch (ActivateUseButton)
        {
            case ButtonChoice.Trigger:

                break;
            case ButtonChoice.Touchpad:
                if (obj.state.thumbstickPressed)
                {
                    StartCoroutine(MakeRotate(obj));
                }
                break;
        }
    }

    protected override void UseEnded(InteractionSourceReleasedEventArgs obj)
    {
        base.UseEnded(obj);
        switch (ActivateUseButton)
        {
            case ButtonChoice.Trigger:

                break;
            case ButtonChoice.Touchpad:
                break;
        }
    }

    private IEnumerator MakeRotate(InteractionSourcePressedEventArgs obj)
    {
        //touchpadActive is set to true is the object is grabbed and has selected TouchPad for the "use" button
            while (touchPadActive)
            {
                transform.Rotate(obj.state.thumbstickPosition.x, obj.state.thumbstickPosition.y, 0.01f);
                yield return 0;
            }
        yield return null;
    }


}
