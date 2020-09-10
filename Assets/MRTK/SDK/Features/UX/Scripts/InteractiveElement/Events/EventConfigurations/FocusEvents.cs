// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The event configuration for the Focus state.
    /// </summary>
    [CreateAssetMenu(fileName = "FocusEvents", menuName = "Mixed Reality Toolkit/Interactive Element/Event Configurations/Focus Event Configuration")]
    public class FocusEvents : BaseInteractionEventConfiguration
    {
        public override string StateName => "Focus";

        /// <summary>
        /// A Unity event with FocusEventData. This event is fired when focus enters an object.
        /// </summary>
        public FocusInteractionEvent OnFocusOn = new FocusInteractionEvent();

        /// <summary>
        /// A Unity event with FocusEventData. This event is fired when focus exits an object.
        /// </summary>
        public FocusInteractionEvent OnFocusOff = new FocusInteractionEvent();

        ///<inheritdoc/>
        public override BaseEventReceiver InitializeRuntimeEventReceiver()
        {
            FocusReceiver focusReceiver = new FocusReceiver(this);

            EventReceiver = focusReceiver;

            return focusReceiver;
        }
    }
}
