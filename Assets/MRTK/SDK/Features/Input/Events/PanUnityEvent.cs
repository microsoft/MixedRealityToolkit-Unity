// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Unity event for a pan / zoom event. Contains the hand pan event data
    /// </summary>
    [System.Serializable]
    public class PanUnityEvent : UnityEvent<HandPanEventData> { }
}
