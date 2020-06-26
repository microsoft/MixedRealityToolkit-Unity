// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A UnityEvent callback containing a ManipulationEventData payload.
    /// </summary>
    [System.Serializable]
    public class ManipulationEvent : UnityEvent<ManipulationEventData> { }
}
