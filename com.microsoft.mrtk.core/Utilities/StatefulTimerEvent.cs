// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit
{
    [System.Serializable]
    /// <summary>
    /// The timer event fired when a <see cref="TimedFlag"/> is triggered.
    /// Passes a single float argument, representing the timestamp at which
    /// the event (entered, exited) occurred.
    /// </summary>
    public class StatefulTimerEvent : UnityEvent<float> { }
}
