using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

// Simple example script that subscribes to primary pointer changes and applies a cursor highlight to the current one.
public class PrimaryPointerHandlerExample : MonoBehaviour
{
    public GameObject CursorHighlight;

    private void OnEnable()
    {
        MixedRealityToolkit.InputSystem?.FocusProvider?.SubscribeToPrimaryPointerChanged(OnPrimaryPointerChanged, true);
    }

    private void OnPrimaryPointerChanged(IMixedRealityPointer oldPointer, IMixedRealityPointer newPointer)
    {
        if (CursorHighlight != null)
        {
            if (newPointer != null)
            {
                Transform parentTransform = newPointer.BaseCursor?.GameObjectReference?.transform;

                // If there's no cursor try using the controller pointer transform instead
                if (parentTransform == null)
                {
                    var controllerPointer = newPointer as BaseControllerPointer;
                    parentTransform = controllerPointer?.transform;
                }

                if (parentTransform != null)
                {
                    CursorHighlight.transform.SetParent(parentTransform, false);
                    CursorHighlight.SetActive(true);
                    return;
                }
            }
            
            CursorHighlight.SetActive(false);
            CursorHighlight.transform.SetParent(null, false);
        }
    }

    private void OnDisable()
    {
        MixedRealityToolkit.InputSystem?.FocusProvider?.UnsubscribeFromPrimaryPointerChanged(OnPrimaryPointerChanged);
        OnPrimaryPointerChanged(null, null);
    }
}
