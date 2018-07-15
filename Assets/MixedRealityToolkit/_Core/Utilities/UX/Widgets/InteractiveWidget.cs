// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.UX.Widgets
{
    /// <summary>
    /// InteractiveState can exist on a child element of the game object containing the Interactive component.
    /// Extend this class to make custom behaviors that listen from state updates from Interactive
    /// </remarks>
    public class InteractiveWidget : MonoBehaviour
    {
        [Tooltip("The Interactive that will update the widget, optional: use if the widget is a sibling of the Interactive or if the parent Interactive is child of another Interactive")]
        public Interactive InteractiveHost;

        // the Interactive state
        protected ButtonStateEnum State;

        /// <summary>
        /// register if the InteractiveHost was not manually set
        /// </summary>
        protected virtual void OnEnable()
        {
            if (InteractiveHost != null)
            {
                InteractiveHost.RegisterWidget(this);
            }
        }

        /// <summary>
        /// Unregister when disabled
        /// </summary>
        protected virtual void OnDisable()
        {
            if (InteractiveHost != null)
            {
                InteractiveHost.UnregisterWidget(this);
            }
        }

        /// <summary>
        /// Interactive calls this method on state change
        /// </summary>
        /// <param name="state">
        /// Enum containing the following states:
        /// DefaultState: normal state of the button
        /// FocusState: gameObject has gaze
        /// PressState: currently being pressed
        /// SelectedState: selected and has no other interaction
        /// FocusSelected: selected with gaze
        /// PressSelected: selected and pressed
        /// Disabled: button is disabled
        /// DisabledSelected: the button is not interactive, but in it's alternate state (toggle button)
        /// </param>
        public virtual void SetState(ButtonStateEnum state)
        {
            State = state;
        }
    }
}
