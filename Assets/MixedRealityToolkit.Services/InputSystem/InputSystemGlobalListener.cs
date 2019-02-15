// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Services.InputSystem
{
    /// <summary>
    /// This component ensures that all input events are forwarded to this <see cref="GameObject"/> when focus or gaze is not required.
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
