// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// A Unity event with HandTrackingInputEventData. This event is used in the event configuration for the 
    /// Touch state.
    /// </summary>
    [System.Serializable]
    public class TouchInteractionEvent : UnityEvent<HandTrackingInputEventData> { }
}