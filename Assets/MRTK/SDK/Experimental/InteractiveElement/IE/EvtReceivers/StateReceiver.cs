// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the events defined in the State Interaction Event Configuration.
    /// </summary>
    public class StateReceiver : BaseEventReceiver
    {
        /// <inheritdoc />
        public StateReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private StateEvents stateEventConfig => EventConfiguration as StateEvents;

        private UnityEvent onStateOn => stateEventConfig.OnStateOn;

        private UnityEvent onStateOff => stateEventConfig.OnStateOff;

        private bool wasStateOn;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool isStateOn = stateManager.GetState(StateName).Value > 0;

            // If the current state value does not equal the previous state value
            if (isStateOn != wasStateOn)
            {
                if (isStateOn)
                {
                    onStateOn.Invoke();
                }
                else
                {
                    onStateOff.Invoke();
                }
            }

            wasStateOn = isStateOn;
        }
    }
}
