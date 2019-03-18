// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MRTKPrefix.Utilities;
using UnityEngine;

namespace MRTKPrefix.Input
{
    /// <summary>
    /// This component ensures that all input events are forwarded to this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> when focus or gaze is not required.
    /// </summary>
    public class InputSystemGlobalListener : MonoBehaviour
    {
        private bool lateInitialize = true;

        protected readonly WaitUntil WaitUntilInputSystemValid = new WaitUntil(() => MixedRealityToolkit.InputSystem != null);

        protected virtual void OnEnable()
        {
            if (MixedRealityToolkit.IsInitialized && MixedRealityToolkit.InputSystem != null && !lateInitialize)
            {
                MixedRealityToolkit.InputSystem.Register(gameObject);
            }
        }

        protected virtual async void Start()
        {
            if (lateInitialize)
            {
                if (MixedRealityToolkit.InputSystem == null)
                {
                    await WaitUntilInputSystemValid;
                }

                if (this == null)
                {
                    // We've been destroyed during the await.
                    return;
                }

                lateInitialize = false;
                MixedRealityToolkit.InputSystem.Register(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            MixedRealityToolkit.InputSystem?.Unregister(gameObject);
        }
    }
}
