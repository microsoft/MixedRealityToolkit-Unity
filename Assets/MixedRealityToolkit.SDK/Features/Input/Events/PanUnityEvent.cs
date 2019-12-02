// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Unity event for a pan / zoom event. Contains the hand pan event data
    /// </summary>
    [System.Serializable]
    public class PanUnityEvent : UnityEvent<HandPanEventData> { }
}
