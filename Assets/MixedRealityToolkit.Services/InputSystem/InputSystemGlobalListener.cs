// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component ensures that all input events are forwarded to this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> when focus or gaze is not required.
    /// </summary>
    public class InputSystemGlobalListener : MonoBehaviour
    {
        private Implementation implementation = new Implementation();

        /// <summary>
        /// This class handles InputSystemGlobalListenerImplementation registration. Classes which cannot derive from InputSystemGlobalListener 
        /// can use this implementation class to aide in registration. 
        /// </summary>
        public class Implementation
        {
            private bool lateInitialize = true;
            private IMixedRealityInputSystem inputSystem = null;

            /// <summary>
            /// The active instance of the input system.
            /// </summary>
            public IMixedRealityInputSystem InputSystem
            {
                get
                {
                    if (inputSystem == null)
                    {
                        MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                    }

                    return inputSystem;
                }
            }

            /// <summary>
            /// Registers a GameObject as a global listener.
            /// </summary>
            /// <param name="gameObject">GameObject to register.</param>
            public void OnEnable(GameObject gameObject)
            {
                if (MixedRealityToolkit.IsInitialized && InputSystem != null && !lateInitialize)
                {
                    InputSystem.Register(gameObject);
                }
            }

            /// <summary>
            /// Waits until the input system is valid to registers a GameObject as a global listener.
            /// </summary>
            /// <param name="gameObject">GameObject to register.</param>
            /// <returns></returns>
            public async Task Start(GameObject gameObject)
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
                    InputSystem.Register(gameObject);
                }
            }

            /// <summary>
            /// Unregisters a GameObject as a global listener.
            /// </summary>
            /// <param name="gameObject">GameObject to unregister.</param>
            public virtual void OnDisable(GameObject gameObject)
            {
                InputSystem?.Unregister(gameObject);
            }

            /// <summary>
            /// A task that will only complete when the input system has in a valid state.
            /// </summary>
            /// <remarks>
            /// It's possible for this object to have been destroyed after the await, which
            /// implies that callers should check that this != null after awaiting this task.
            /// </remarks>
            public async Task EnsureInputSystemValid()
            {
                if (InputSystem == null)
                {
                    await new WaitUntil(() => InputSystem != null);
                }
            }
        }

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                return implementation.InputSystem;
            }
        }

        protected virtual void OnEnable()
        {
            implementation.OnEnable(gameObject);
        }

        protected virtual async void Start()
        {
            await implementation.Start(gameObject);
        }

        protected virtual void OnDisable()
        {
            implementation.OnDisable(gameObject);
        }

        protected async Task EnsureInputSystemValid()
        {
            await implementation.EnsureInputSystemValid();
        }
    }
}
