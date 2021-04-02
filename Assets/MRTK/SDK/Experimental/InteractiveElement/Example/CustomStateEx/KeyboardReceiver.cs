// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement.Examples
{
    /// <summary>
    /// Example receiver class for the Keyboard state
    /// </summary>
    public class KeyboardReceiver : BaseEventReceiver
    {
        /// <summary>
        /// Example constructor for the Keyboard State
        /// </summary>
        /// <param name="eventConfiguration">The event configuration for the Keyboard state</param>
        public KeyboardReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private KeyboardEvents KeyboardEventConfig => EventConfiguration as KeyboardEvents;

        // Set reference to the event defined in KeyboardEvents
        private UnityEvent onKKeyPressed => KeyboardEventConfig.OnKKeyPressed;

        // The OnUpdate method is called using: 
        // InteractiveElement.SetStateAndInvokeEvent(stateName, stateValue, optional eventData)
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool keyPressed = stateManager.GetState(StateName).Value > 0;

            // If the state was set to on, then invoke the event
            if (keyPressed)
            {
                onKKeyPressed.Invoke();
            }
        }
    }
}

