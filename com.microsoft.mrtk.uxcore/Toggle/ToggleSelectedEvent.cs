// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A Unity event raised by <see cref="Microsoft.MixedReality.Toolkit.UX.ToggleCollection">ToggleCollection</see>
    /// when any of the toggle buttons are selected. The event data is the index of the toggle button within the
    /// <see cref="ToggleCollection"/>.
    /// </summary>
    [Serializable]
    public class ToggleSelectedEvent : UnityEvent<int> { }
}