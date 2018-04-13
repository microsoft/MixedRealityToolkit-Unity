// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Utilities
{
    /// <summary>
    /// Register this game object on the InputManager as a global listener.
    /// </summary>
    [DisallowMultipleComponent]
    public class SetGlobalListener : MonoBehaviour
    {
        private void OnEnable()
        {
            MixedRealityInputManager.AddGlobalListener(gameObject);
        }

        private void OnDisable()
        {
            MixedRealityInputManager.RemoveGlobalListener(gameObject);
        }
    }
}
