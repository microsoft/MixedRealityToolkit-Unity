// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the event defined in the ToggleOff Interaction Event Configuration.
    /// </summary>
    public class ToggleOffReceiver : BaseEventReceiver
    {
        public ToggleOffReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private ToggleOffEvents ToggleOffEventConfig => EventConfiguration as ToggleOffEvents;

        private UnityEvent onToggleOff => ToggleOffEventConfig.OnToggleOff;

        private bool wasToggledOff;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool isToggleOff = stateManager.GetState(StateName).Value > 0;

            if (wasToggledOff != isToggleOff)
            {
                if (isToggleOff)
                {
                    onToggleOff.Invoke();
                }
            }

            wasToggledOff = isToggleOff;
        }
    }
}
