// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component ensures that all input events are forwarded to this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> when focus or gaze is not required.
    /// </summary>
    public class InputSystemGlobalListener : MonoBehaviour
    {
        private bool lateInitialize = true;

        /// <summary>
        /// Prefer using EnsureInputSystemValid(), which will only await if the input system isn't
        /// yet valid.
        /// </summary>
        [Obsolete("Prefer using EnsureInputSystemValid()")]
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
                await EnsureInputSystemValid();

                // We've been destroyed during the await.
                if (this == null)
                {
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

        /// <summary>
        /// A task that will only complete when the input system has in a valid state.
        /// </summary>
        /// <remarks>
        /// It's possible for this object to have been destroyed after the await, which
        /// implies that callers should check that this != null after awaiting this task.
        /// </remarks>
        protected async Task EnsureInputSystemValid()
        {
            if (MixedRealityToolkit.InputSystem == null)
            {
#pragma warning disable CS0618 // WaitUntilInputSystemValid is obsolete for outside-of-class usage.
                await WaitUntilInputSystemValid;
#pragma warning restore CS0618
            }
        }
    }
}
