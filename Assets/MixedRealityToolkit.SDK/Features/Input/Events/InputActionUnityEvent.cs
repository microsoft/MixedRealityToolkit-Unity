// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Unity event for input action events. Contains the data of the input event that triggered the action.
    /// </summary>
    [System.Serializable]
    public class InputActionUnityEvent : UnityEvent<BaseInputEventData> { }
}
