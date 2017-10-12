// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Register this game object on the InputManager as a global listener.
    /// </summary>
    public class SetGlobalListener : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(AddGlobalListener());
        }

        private void OnDisable()
        {
            InputManager.AssertIsInitialized();

            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        private void OnDestroy()
        {
            InputManager.AssertIsInitialized();

            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        private IEnumerator AddGlobalListener()
        {
            while (!InputManager.IsInitialized)
            {
                yield return null;
            }

            InputManager.AssertIsInitialized();

            InputManager.Instance.AddGlobalListener(gameObject);
        }
    }
}
