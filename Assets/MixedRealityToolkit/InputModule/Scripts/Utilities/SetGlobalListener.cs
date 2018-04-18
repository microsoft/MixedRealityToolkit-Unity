// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.InputModule.Utilities
{
    /// <summary>
    /// Register this game object on the InputManager as a global listener.
    /// </summary>
    public class SetGlobalListener : MonoBehaviour
    {
        private void OnEnable()
        {
            InputManager.AddGlobalListener(gameObject);
        }

        private void OnDisable()
        {
            InputManager.RemoveGlobalListener(gameObject);
        }
    }
}
