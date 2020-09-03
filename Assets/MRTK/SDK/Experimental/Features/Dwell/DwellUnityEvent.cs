// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    /// <summary>
    /// Unity event for a dwell event. Contains the pointer reference.
    /// </summary>
    [Serializable]
    public class DwellUnityEvent : UnityEvent<IMixedRealityPointer> { }
}
