using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInputStack : MonoBehaviour, IMixedRealityTouchHandler
{
    public GameObject focusedObject;

     private BoxCollider myCollider;

     private IMixedRealityPointer currentPointer;

    void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        if (TryGetPokePointer(eventData.InputSource.Pointers, out currentPointer))
        {
            HandTrackingInputEventData newTouchData = eventData;
            eventData.Sender = this;

            currentPointer.Result.CurrentPointerTarget.GetComponentInChildren<IMixedRealityTouchHandler>().OnTouchCompleted(newTouchData);
        }
    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        if (TryGetPokePointer(eventData.InputSource.Pointers, out currentPointer))
        {
            HandTrackingInputEventData newTouchData = eventData;
            eventData.Sender = this;

            currentPointer.Result.CurrentPointerTarget.GetComponentInChildren<IMixedRealityTouchHandler>().OnTouchStarted(newTouchData);
        }
    }

    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        if (TryGetPokePointer(eventData.InputSource.Pointers, out currentPointer))
        {
            HandTrackingInputEventData newTouchData = eventData;
            eventData.Sender = this;

            currentPointer.Result.CurrentPointerTarget.GetComponentInChildren<IMixedRealityTouchHandler>().OnTouchUpdated(newTouchData);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider>();

        PressableButton[] childPressables = GetComponentsInChildren<PressableButton>();
        foreach (PressableButton pb in childPressables)
        {
            pb.PassThroughMode = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private bool TryGetPokePointer(IMixedRealityPointer[] pointers, out IMixedRealityPointer pokePointer)
    {
        pokePointer = null;

        for (int i = 0; i < pointers.Length; i++)
        {
            if (pointers[i].GetType() == typeof(PokePointer))
            {
                pokePointer = pointers[i];
            }
        }

        return (pokePointer != null) ? true : false;
    }

}
