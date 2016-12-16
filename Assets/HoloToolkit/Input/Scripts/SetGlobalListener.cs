// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// Register on the InputManger this game object as a global listerner.
    /// </summary>
    public class SetGlobalListener : MonoBehaviour
    {
        private void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }
    }
}
