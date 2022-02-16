// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// A Unity event with MixedRealityPointerEventData. This event is used in the event configuration for the 
    /// SelectFar state.
    /// </summary>
    [System.Serializable]
    public class SelectFarInteractionEvent : UnityEvent<MixedRealityPointerEventData> { }
}