// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the events defined in the SelectFar Interaction Event Configuration.
    /// </summary>
    public class SelectFarReceiver : BaseEventReceiver
    {
        public SelectFarReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private SelectFarEvents SelectFarEventConfig => EventConfiguration as SelectFarEvents;

        private SelectFarInteractionEvent onSelectDown => SelectFarEventConfig.OnSelectDown;

        private SelectFarInteractionEvent onSelectUp => SelectFarEventConfig.OnSelectUp;

        private SelectFarInteractionEvent onSelectHold => SelectFarEventConfig.OnSelectHold;

        private SelectFarInteractionEvent onSelectClicked => SelectFarEventConfig.OnSelectClicked;

        private bool hadFarSelect;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool hasFarSelect = stateManager.GetState(StateName).Value > 0;

            if (hadFarSelect != hasFarSelect)
            {
                if (hasFarSelect)
                {
                    onSelectDown.Invoke(eventData as MixedRealityPointerEventData);
                }
                else
                {
                    onSelectClicked.Invoke(eventData as MixedRealityPointerEventData);
                    onSelectUp.Invoke(eventData as MixedRealityPointerEventData);
                }
            }

            if (hasFarSelect)
            {
                onSelectHold.Invoke(eventData as MixedRealityPointerEventData);
            }

            hadFarSelect = hasFarSelect;
        }
    }
}
