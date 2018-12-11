using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.SDK.UX;
using Microsoft.MixedReality.Toolkit.SDK.UX.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Physics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using System;

public class doubleGripReport : MonoBehaviour, IMixedRealityInputHandler
{
 

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

    }


    public void OnInputDown(InputEventData eventData)
    {
        if (eventData.MixedRealityInputAction.Description.ToUpper() != "NONE")
        {
            TextMesh tm = GameObject.Find("DebugInput").GetComponent<TextMesh>();
            tm.text = "action:" + eventData.MixedRealityInputAction.Description + " id:" + eventData.SourceId.ToString() + "  time:" + Time.unscaledDeltaTime.ToString() + "\n" + tm.text;
            if (tm.text.Length > 200)
            {
                tm.text = tm.text.Substring(0, 200);
            }
        }
    }

    public void OnInputPressed(InputEventData<float> eventData)
    {
    }

    public void OnInputUp(InputEventData eventData)
    {
    }

    public void OnPositionInputChanged(InputEventData<Vector2> eventData)
    {
    }

}
