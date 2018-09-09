// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
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

        private readonly WaitUntil waitUntilInputSystemValid = new WaitUntil(() => MixedRealityManager.Instance != null && InputSystem != null);

        protected virtual async void OnEnable()
        {
            await waitUntilInputSystemValid;
            InputSystem.Register(gameObject);
        }

        protected virtual void OnDisable()
        {
            InputSystem?.Unregister(gameObject);
        }
    }
}
