// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The event configuration for the PressedNear InteractionState. This state is specific to the 
    /// CompressableButton class.
    /// </summary>
    public class PressedNearEvents : BaseInteractionEventConfiguration
    {
        /// <summary>
        /// Fired when a button is pressed via near interaction. 
        /// </summary>
        public UnityEvent OnButtonPressed = new UnityEvent();

        /// <summary>
        /// Fired when a button press is released via near interaction. 
        /// </summary>
        public UnityEvent OnButtonPressReleased = new UnityEvent();

        /// <summary>
        /// Fired when a button is currently being pressed. 
        /// </summary>
        public UnityEvent OnButtonPressHold = new UnityEvent();
    }
}
