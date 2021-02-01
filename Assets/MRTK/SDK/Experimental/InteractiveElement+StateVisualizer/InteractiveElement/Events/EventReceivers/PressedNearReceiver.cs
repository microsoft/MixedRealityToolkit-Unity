// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the events defined in the PressedNear Interaction Event Configuration.
    /// The PressedNear state is specific to the CompressableButton class. 
    /// </summary>
    public class PressedNearReceiver : BaseEventReceiver
    {
        /// <inheritdoc />
        public PressedNearReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private PressedNearEvents pressedNearEventConfig => EventConfiguration as PressedNearEvents;

        private UnityEvent onButtonPressed => pressedNearEventConfig.OnButtonPressed;

        private UnityEvent onButtonPressReleased => pressedNearEventConfig.OnButtonPressReleased;

        private UnityEvent onButtonPressHold => pressedNearEventConfig.OnButtonPressHold;

        private bool wasButtonPressedNear;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool isButtonPressedNear = stateManager.GetState(StateName).Value > 0;

            if (isButtonPressedNear != wasButtonPressedNear)
            {
                if (isButtonPressedNear)
                {
                    onButtonPressed.Invoke();
                }
                else
                {
                    onButtonPressReleased.Invoke();
                }
            }

            if (isButtonPressedNear)
            {
                onButtonPressHold.Invoke();
            }

            wasButtonPressedNear = isButtonPressedNear;
        }
    }
}
