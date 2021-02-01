// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// A Unity event with an Interaction State as event data.  The Interaction State represents the state that was activated or set on.
    /// This event is used in the StateManager when a state is set on/activated.
    /// </summary>
    [System.Serializable]
    public class InteractionStateActiveEvent : UnityEvent<InteractionState> { }
}
