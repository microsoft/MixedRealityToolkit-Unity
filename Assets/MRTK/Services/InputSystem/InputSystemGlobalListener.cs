// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component ensures that all input events are forwarded to this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> when focus or gaze is not required.
    /// </summary>
    [Obsolete("InputSystemGlobalListener uses obsolete global input event registration API. " +
        "Use RegisterHandler/UnregisterHandler API directly (preferred) or InputSystemGlobalHandlerListener instead.")]
    [AddComponentMenu("Scripts/MRTK/Obsolete/InputSystemGlobalListener")]
    public class InputSystemGlobalListener : MonoBehaviour
    {
        private bool lateInitialize = true;

        protected virtual void OnEnable()
        {
            if (CoreServices.InputSystem != null && !lateInitialize)
            {
                CoreServices.InputSystem.Register(gameObject);
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
                CoreServices.InputSystem.Register(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            CoreServices.InputSystem?.Unregister(gameObject);
        }

        /// <summary>
        /// A task that will only complete when the input system has in a valid state.
        /// </summary>
        /// <remarks>
        /// <para>It's possible for this object to have been destroyed after the await, which
        /// implies that callers should check that this != null after awaiting this task.</para>
        /// </remarks>
        protected async Task EnsureInputSystemValid()
        {
            if (CoreServices.InputSystem == null)
            {
                await new WaitUntil(() => CoreServices.InputSystem != null);
            }
        }
    }
}
