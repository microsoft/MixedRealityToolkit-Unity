// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the event defined in the ToggleOn Interaction Event Configuration.
    /// </summary>
    public class ToggleOnReceiver : BaseEventReceiver
    {
        public ToggleOnReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private ToggleOnEvents ToggleOnEventConfig => EventConfiguration as ToggleOnEvents;

        private UnityEvent onToggleOn => ToggleOnEventConfig.OnToggleOn;

        private bool wasToggledOn;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool isToggleOn = stateManager.GetState(StateName).Value > 0;

            if (wasToggledOn != isToggleOn)
            {
                if (isToggleOn)
                {
                    onToggleOn.Invoke();
                }
            }

            wasToggledOn = isToggleOn;
        }
    }
}
