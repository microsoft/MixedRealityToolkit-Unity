// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component ensures that input events are forwarded to this component when focus or gaze is not required.
    /// </summary>
    public abstract class InputSystemGlobalHandlerListener : MonoBehaviour
    {
        private bool lateInitialize = true;

        private readonly Type eventSystemHandlerType = typeof(IEventSystemHandler);
        private readonly MethodInfo registerHandlerMethod = typeof(IMixedRealityEventSystem).GetMethod("RegisterHandler");
        private readonly MethodInfo unregisterHandlerMethod = typeof(IMixedRealityEventSystem).GetMethod("UnregisterHandler");

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
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

        protected virtual void OnEnable()
        {
            if (InputSystem != null && !lateInitialize)
            {
                RegisterHandlers();
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
            if (InputSystem == null)
            {
                await new WaitUntil(() => InputSystem != null);
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

        /// <summary>
        /// Utility method, which registers all handlers implemented by 'this' using Reflection.
        /// T should be a derived class type
        /// Avoid using this method unless class implements so many interfaces that enumerating them manually becomes unclear for the reader
        /// </summary>
        protected void RegisterAllHandlers<T>() where T: class, IEventSystemHandler
        {
            if (InputSystem == null)
            {
                return;
            }

            if (registerHandlerMethod == null)
            {
                Debug.Assert(registerHandlerMethod != null, "Unable to extract RegisterHandler method info from reflection.");
                return;
            }

            // This call is here to force compile error in case event system API changes,
            // because reflection uses a string to get method definition.
            InputSystem.RegisterHandler<IEventSystemHandler>(null);

            var actualType = typeof(T);

            foreach (var iface in actualType.GetInterfaces())
            {
                if (eventSystemHandlerType.IsAssignableFrom(iface) && !eventSystemHandlerType.Equals(iface))
                {
                    registerHandlerMethod.MakeGenericMethod(iface).Invoke(InputSystem, new object[] { this as T });
                }
            }
        }

        /// <summary>
        /// Utility method, which unregisters all handlers implemented by 'this' using Reflection.
        /// T should be a derived class type
        /// Avoid calling this method unless class implements so many interfaces that enumerating them manually becomes unclear for the reader
        /// </summary>
        protected void UnregisterAllHandlers<T>() where T: class, IEventSystemHandler
        {
            if(InputSystem == null)
            {
                return;
            }

            if (unregisterHandlerMethod == null)
            {
                Debug.Assert(unregisterHandlerMethod != null, "Unable to extract UnregisterHandler method info from reflection.");
                return;
            }

            // This call is here to force compile error in case event system API changes,
            // because reflection uses a string to get method definition.
            InputSystem.UnregisterHandler<IEventSystemHandler>(null);

            var actualType = typeof(T);

            foreach (var iface in actualType.GetInterfaces())
            {
                if (eventSystemHandlerType.IsAssignableFrom(iface) && !eventSystemHandlerType.Equals(iface))
                {
                    unregisterHandlerMethod.MakeGenericMethod(iface).Invoke(InputSystem, new object[] { this as T });
                }
            }
        }
    }
}
