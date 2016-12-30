// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// Register this game object on the InputManager as a global listener.
    /// </summary>
    public class SetGlobalListener : MonoBehaviour
    {
        private void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        private void OnDestroy()
        {
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }
    }
}
