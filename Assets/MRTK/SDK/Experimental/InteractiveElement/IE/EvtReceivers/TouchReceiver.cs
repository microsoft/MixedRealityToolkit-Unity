// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the events defined in the TouchEvents Configuration.
    /// </summary>
    public class TouchReceiver : BaseEventReceiver
    {
        public TouchReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private TouchEvents touchEventConfig => EventConfiguration as TouchEvents;

        private TouchInteractionEvent onTouchStarted => touchEventConfig.OnTouchStarted;

        private TouchInteractionEvent onTouchCompleted => touchEventConfig.OnTouchCompleted;

        private TouchInteractionEvent onTouchUpdated => touchEventConfig.OnTouchUpdated;

        private bool wasTouching;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool isTouching = stateManager.GetState(StateName).Value > 0;

            if (wasTouching != isTouching)
            {
                if (isTouching)
                {
                    onTouchStarted.Invoke(eventData as HandTrackingInputEventData);
                }
                else
                {
                    onTouchCompleted.Invoke(eventData as HandTrackingInputEventData);
                }
            }

            if (isTouching)
            {
                onTouchUpdated.Invoke(eventData as HandTrackingInputEventData);
            }

            wasTouching = isTouching;
        }
    }
}
