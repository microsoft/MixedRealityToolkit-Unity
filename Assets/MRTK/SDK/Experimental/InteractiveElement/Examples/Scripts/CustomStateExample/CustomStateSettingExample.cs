// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement.Examples
{
    /// <summary>
    /// Example custom state setting for the Keyboard state
    /// </summary>
    public class CustomStateSettingExample : MonoBehaviour
    {
        private BaseInteractiveElement interactiveElement;

        private InteractionState keyboardState;

        void Start()
        {
            interactiveElement = GetComponent<InteractiveElement>();

            if (interactiveElement != null)
            {
                keyboardState = interactiveElement.GetState("Keyboard");

                if (keyboardState != null)
                {
                    KeyboardEvents keyboardEvents = interactiveElement.GetStateEvents<KeyboardEvents>("Keyboard");

                    // Add listener to the new custom state
                    keyboardEvents.OnKKeyPressed.AddListener(() =>
                    {
                        Debug.Log("K Key has been pressed");
                    });
                }
            }
        }

        void Update()
        {
            if (keyboardState != null)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.K))
                {
                    // Set the state on and invoke the events in KeyboardEvents
                    interactiveElement.SetStateAndInvokeEvent("Keyboard", 1);
                }

                // Press the the L key to set the Keyboard state to off
                if (UnityEngine.Input.GetKeyDown(KeyCode.L))
                {
                    // Set the state off 
                    interactiveElement.SetStateAndInvokeEvent("Keyboard", 0);
                }
            }
        }
    }
}

