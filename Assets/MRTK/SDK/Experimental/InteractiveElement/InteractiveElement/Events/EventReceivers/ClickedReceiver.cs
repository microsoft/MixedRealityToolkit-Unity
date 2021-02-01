// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the event defined in the Clicked Interaction Event Configuration.
    /// </summary>
    public class ClickedReceiver : BaseEventReceiver
    {
        public ClickedReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private ClickedEvents ClickedEventConfig => EventConfiguration as ClickedEvents;

        private UnityEvent onClicked => ClickedEventConfig.OnClicked;
        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool clicked = stateManager.GetState(StateName).Value > 0;

            if (clicked)
            {
                onClicked.Invoke();
            }
        }
    }
}
