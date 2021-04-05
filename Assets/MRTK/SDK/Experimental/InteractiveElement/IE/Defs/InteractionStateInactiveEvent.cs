// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// A Unity event with two Interaction States as event data.  The first Interaction State represents the state
    /// that was deactivated and the second is the state that is currently active.  This event is used in the StateManager when 
    /// a state is set to off/deactivated.
    /// </summary>
    [System.Serializable]
    public class InteractionStateInactiveEvent : UnityEvent<InteractionState, InteractionState> { }
}
