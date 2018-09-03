using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Physics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableFinger : MonoBehaviour
{
    public Interactable Button;
    public bool Focus;
    public bool Down;
    public bool Disabled;
    public bool Clicked;

    private bool? hasFocus;
    private bool? hasDown;
    private bool? isDisabled;
    private bool isClicked;

    private void Update()
    {
        if(hasFocus != Focus)
        {
            Button.SetFocus(Focus);
            hasFocus = Focus;
        }

        if (hasDown != Down)
        {
            Button.SetPress(Down);
            hasDown = Down;
        }

        if (isDisabled != Disabled)
        {
            Button.SetDisabled(Disabled);
            isDisabled = Disabled;
        }

        if (isClicked != Clicked)
        {
            Button.OnInputClicked(null);
            Clicked = isClicked;
        }
    }
}
