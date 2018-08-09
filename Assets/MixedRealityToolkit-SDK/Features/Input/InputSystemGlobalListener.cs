// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
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

        protected virtual void OnEnable()
        {
            Debug.Assert(MixedRealityManager.IsInitialized, "No Mixed Reality Manager found in the scene.  Be sure to run the Mixed Reality Configuration.");
            Debug.Assert(InputSystem != null, "No Input System found, Did you set it up in your configuration profile?");
            InputSystem.Register(gameObject);
        }

        protected virtual void OnDisable()
        {
            InputSystem.Unregister(gameObject);
        }
    }
}
