// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Unity event for a pointer event. Contains the pointer event data.
    /// </summary>
    [System.Serializable]
    public class PointerUnityEvent : UnityEvent<MixedRealityPointerEventData> { }
}
