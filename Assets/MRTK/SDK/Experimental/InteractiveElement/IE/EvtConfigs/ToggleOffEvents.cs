// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The event configuration for the ToggleOff InteractionState.
    /// </summary>
    public class ToggleOffEvents : BaseInteractionEventConfiguration
    {
        /// <summary>
        /// A Unity event that is fired when the ToggleOff state is active.
        /// </summary>
        public UnityEvent OnToggleOff = new UnityEvent();
    }
}

