// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The default event configuration for an InteractionState.
    /// </summary>
    public class StateEvents : BaseInteractionEventConfiguration
    {
        /// <summary>
        /// Fired when a state is set to on. 
        /// </summary>
        public UnityEvent OnStateOn = new UnityEvent();

        /// <summary>
        /// Fired when a state is set to off.
        /// </summary>
        public UnityEvent OnStateOff = new UnityEvent();
    }
}
