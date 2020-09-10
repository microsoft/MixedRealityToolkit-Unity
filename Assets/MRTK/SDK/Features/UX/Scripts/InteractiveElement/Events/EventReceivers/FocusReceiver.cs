// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The internal event receiver for the events defined in the Focus Interaction Event Configuration.
    /// </summary>
    internal class FocusReceiver : BaseEventReceiver
    {
        /// <inheritdoc />
        public FocusReceiver(FocusEvents focusEventConfiguration) : base(focusEventConfiguration, "FocusReceiver") 
        {
            EventConfiguration = focusEventConfiguration;

            focusEventConfig = focusEventConfiguration;
        }

        private readonly FocusEvents focusEventConfig;

        private FocusInteractionEvent onFocusOn => focusEventConfig.OnFocusOn;

        private FocusInteractionEvent onFocusOff => focusEventConfig.OnFocusOff;

        private bool hadFocus;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool hasFocus = stateManager.GetState(CoreInteractionState.Focus).Value > 0;

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
