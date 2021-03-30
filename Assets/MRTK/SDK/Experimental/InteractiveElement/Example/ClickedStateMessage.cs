// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement.Examples
{
    /// <summary>
    /// The clicked state for Interactive Element does not light up in the inspector, this script
    /// logs a message as a visual confirmation that the clicked event was fired.
    /// </summary>
    public class ClickedStateMessage : MonoBehaviour
    {
        public BaseInteractiveElement InteractiveElement;

        private InteractionState clickedState;

        void Start()
        {
            clickedState = InteractiveElement.GetState("Clicked");

            if (clickedState != null)
            {
                ClickedEvents clickedEvents = InteractiveElement.GetStateEvents<ClickedEvents>("Clicked");

                clickedEvents.OnClicked.AddListener(() =>
                {
                    Debug.Log($"{gameObject.name} Clicked");
                });
            }
        }
    }
}
