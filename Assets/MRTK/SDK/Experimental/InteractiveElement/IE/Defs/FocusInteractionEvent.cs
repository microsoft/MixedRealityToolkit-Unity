// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// A Unity event with FocusEventData. This event is used in the event configuration for the 
    /// Focus state.
    /// </summary>
    [System.Serializable]
    public class FocusInteractionEvent : UnityEvent<FocusEventData> { }
}