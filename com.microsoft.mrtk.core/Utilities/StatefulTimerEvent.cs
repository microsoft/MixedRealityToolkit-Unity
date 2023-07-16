// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A Unity event used by <see cref="TimedFlag"/> instances.
    /// </summary>
    /// <remarks>
    /// The timer event fired when a <see cref="TimedFlag"/> is triggered.
    /// Passes a single float argument, representing the timestamp at which
    /// the event (entered, exited) occurred.
    /// </remarks>
    [System.Serializable]
    public class StatefulTimerEvent : UnityEvent<float> { }
}
