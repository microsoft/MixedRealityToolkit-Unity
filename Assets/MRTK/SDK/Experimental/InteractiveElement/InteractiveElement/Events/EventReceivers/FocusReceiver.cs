// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the events defined in the Focus Interaction Event Configuration.
    /// </summary>
    public class FocusReceiver : BaseEventReceiver
    {
        public FocusReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private FocusEvents focusEventConfig => EventConfiguration as FocusEvents;

        private FocusInteractionEvent onFocusOn => focusEventConfig.OnFocusOn;

        private FocusInteractionEvent onFocusOff => focusEventConfig.OnFocusOff;

        private bool hadFocus;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool hasFocus = stateManager.GetState(StateName).Value > 0;

            if (hadFocus != hasFocus)
            {
                if (hasFocus)
                {
                    onFocusOn.Invoke(eventData as FocusEventData);
                }
                else
                {
                    onFocusOff.Invoke(eventData as FocusEventData);
                }
            }

            hadFocus = hasFocus;
        }
    }
}
