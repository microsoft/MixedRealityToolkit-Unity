// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The event configuration for the Focus InteractionState.
    /// </summary>
    public class FocusEvents : BaseInteractionEventConfiguration
    {
        /// <summary>
        /// A Unity event with FocusEventData. This event is fired when focus enters an object.
        /// </summary>
        public FocusInteractionEvent OnFocusOn = new FocusInteractionEvent();

        /// <summary>
        /// A Unity event with FocusEventData. This event is fired when focus exits an object.
        /// </summary>
        public FocusInteractionEvent OnFocusOff = new FocusInteractionEvent();
    }
}
