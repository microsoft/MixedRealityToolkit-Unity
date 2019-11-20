// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
