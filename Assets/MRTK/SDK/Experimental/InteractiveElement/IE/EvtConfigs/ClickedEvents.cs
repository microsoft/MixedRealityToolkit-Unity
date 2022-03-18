// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The event configuration for the Clicked InteractionState.
    /// </summary>
    public class ClickedEvents : BaseInteractionEventConfiguration
    {
        /// <summary>
        /// A Unity event that is fired when the Clicked state is active.
        /// </summary>
        public UnityEvent OnClicked = new UnityEvent();
    }
}
