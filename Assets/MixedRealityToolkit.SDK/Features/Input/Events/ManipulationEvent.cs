// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A UnityEvent callback containing a ManipulationEventData payload.
    /// </summary>
    [System.Serializable]
    public class ManipulationEvent : UnityEvent<ManipulationEventData> { }
}
