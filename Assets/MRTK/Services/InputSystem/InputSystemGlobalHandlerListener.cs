// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component ensures that input events are forwarded to this component when focus or gaze is not required.
    /// </summary>
    public abstract class InputSystemGlobalHandlerListener : MonoBehaviour
    {
        // If true, we will try to register ourselves as a global input listener in Start
        private bool lateInitialize = false;

        protected virtual void OnEnable()
        {
            if (CoreServices.InputSystem != null)
            {
                RegisterHandlers();
            }
            else
            {
                // We tried to register for input, but no input system found. Try again at Start
                lateInitialize = true;
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
                RegisterHandlers();
            }
        }

        protected virtual void OnDisable()
        {
            UnregisterHandlers();
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
            if (CoreServices.InputSystem == null)
            {
                await new WaitUntil(() => CoreServices.InputSystem != null);
            }
        }

        /// <summary>
        /// Overload this method to specify, which global events component wants to listen to.
        /// Use RegisterHandler API of InputSystem
        /// </summary>
        protected abstract void RegisterHandlers();

        /// <summary>
        /// Overload this method to specify, which global events component should stop listening to.
        /// Use UnregisterHandler API of InputSystem
        /// </summary>
        protected abstract void UnregisterHandlers();
    }
}