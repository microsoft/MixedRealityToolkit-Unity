// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using UnityEngine.Events;

namespace MixedRealityToolkit.Examples.UX
{
    /// <summary>
    /// InteractiveToggleButton expands InteractiveToggle to expose a gaze, down and up state events in the inspector.
    /// 
    /// Beyond the basic button functionality, Interactive also maintains the notion of selection and enabled, which allow for more robust UI features.
    /// InteractiveEffects are behaviors that listen for updates from Interactive, which allows for visual feedback to be customized and placed on
    /// individual elements of the Interactive GameObject
    /// </summary>
    public class InteractiveToggleButton : InteractiveToggle
    {

        public UnityEvent OnGazeEnterEvents;
        public UnityEvent OnGazeLeaveEvents;
        public UnityEvent OnDownEvents;
        public UnityEvent OnUpEvents;

        /// <summary>
        /// The gameObject received gaze
        /// </summary>
        public override void OnFocusEnter()
        {
            base.OnFocusEnter();

            OnGazeEnterEvents.Invoke();
        }

        /// <summary>
        /// The gameObject no longer has gaze
        /// </summary>
        public override void OnFocusExit()
        {
            base.OnFocusExit();
            OnGazeLeaveEvents.Invoke();
        }

        /// <summary>
        /// The user is initiating a tap or hold
        /// </summary>
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

            OnDownEvents.Invoke();
        }

        /// <summary>
        /// All tab, hold, and gesture events are completed
        /// </summary>
        public override void OnInputUp(InputEventData eventData)
        {
            bool ignoreRelease = mCheckRollOff;
            base.OnInputUp(eventData);
            if (!ignoreRelease)
            {
                OnUpEvents.Invoke();
            }
        }
    }
}
