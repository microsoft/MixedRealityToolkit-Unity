// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Register this game object on the InputManager as a global listener.
    /// </summary>
    public class SetGlobalListener : MonoBehaviour
    {
        private bool started = false;

        private void Start()
        {
            started = true;
            if (InputManager.IsInitialized)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }

        private void OnEnable()
        {
            if (started && InputManager.IsInitialized)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }

        private void OnDisable()
        {
            if (InputManager.IsInitialized)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (InputManager.IsInitialized)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }
    }
}