// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Unity event for input action events. Contains the data of the input event that triggered the action.
    /// </summary>
    [System.Serializable]
    public class InputActionUnityEvent : UnityEvent<BaseInputEventData> { }
}
