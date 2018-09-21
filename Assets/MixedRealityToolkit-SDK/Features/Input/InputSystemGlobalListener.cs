// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input
{
    /// <summary>
    /// This component ensures that all input events are forwarded to this <see cref="GameObject"/> when focus or gaze is not required.
    /// </summary>
    public class InputSystemGlobalListener : MonoBehaviour
    {
        private static IMixedRealityInputSystem inputSystem = null;
        protected static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());

        private bool lateInitialize = true;

        protected virtual void OnEnable()
        {
            if (MixedRealityManager.IsInitialized && InputSystem != null && !lateInitialize)
            {
                InputSystem.Register(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            InputSystem?.Unregister(gameObject);
        }
        
        protected virtual void Start()
        {
            if (lateInitialize)
            {
                StartCoroutine(Register());
            }
        }

        private IEnumerator Register() 
        {
            while (InputSystem == null) yield return null;
            lateInitialize = false;
            InputSystem.Register(gameObject);
        }
    }
}
