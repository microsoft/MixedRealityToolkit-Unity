// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Unity event for a pointer event. Contains the pointer event data.
    /// </summary>
    [System.Serializable]
    public class PointerUnityEvent : UnityEvent<MixedRealityPointerEventData> { }
}
