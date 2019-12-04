// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Class providing a base implementation of the <see cref="IMixedRealityInputDeviceManager"/> interface.
    /// </summary>
    public abstract class BaseInputDeviceManager : BaseDataProvider, IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        protected BaseInputDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name, 
            uint priority, 
            BaseMixedRealityProfile profile) : this(inputSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        protected BaseInputDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile)
        {
            if (inputSystem == null)
            {
                Debug.LogError($"{name} requires a valid input system instance.");
            }

            InputSystem = inputSystem;
        }

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem { get; private set; }

        /// <summary>
        /// The input system configuration profile in use in the application.
        /// </summary>
        protected MixedRealityInputSystemProfile InputSystemProfile => InputSystem?.InputSystemProfile;

        /// <inheritdoc />
        public virtual IMixedRealityController[] GetActiveControllers() => System.Array.Empty<IMixedRealityController>();

        private struct PointerConfig
        {
            public PointerOption profile;

            public Stack<IMixedRealityPointer> cache;
        }

        private PointerConfig[] pointerConfigurations = new PointerConfig[0];

        // Active pointers associated with the config index they were spawned from
        private Dictionary<IMixedRealityPointer, uint> activePointersToConfig;

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (InputSystemProfile != null && InputSystemProfile.PointerProfile != null)
            {
                var initPointerOptions = InputSystemProfile.PointerProfile.PointerOptions;

                pointerConfigurations = new PointerConfig[initPointerOptions.Length];
                activePointersToConfig = new Dictionary<IMixedRealityPointer, uint>();

                for (int i = 0; i < initPointerOptions.Length; i++)
                {
                    pointerConfigurations[i].profile = initPointerOptions[i];
                    pointerConfigurations[i].cache = new Stack<IMixedRealityPointer>();
                }
            }
        }

        /// <summary>
        /// Request an array of pointers for the controller type.
        /// </summary>
        /// <param name="controllerType">The controller type making the request for pointers.</param>
        /// <param name="controllingHand">The handedness of the controller making the request.</param>
        /// <param name="useSpecificType">Only register pointers with a specific type.</param>
        protected virtual IMixedRealityPointer[] RequestPointers(SupportedControllerType controllerType, Handedness controllingHand)
        {
            var returnPointers = new List<IMixedRealityPointer>();

            for (int i = 0; i < pointerConfigurations.Length; i++)
            {
                var option = pointerConfigurations[i].profile;

                if (option.ControllerType.HasFlag(controllerType) && option.Handedness.HasFlag(controllingHand))
                {
                    var pointerCache = pointerConfigurations[i].cache;
                    if (pointerCache.Count > 0)
                    {
                        var p = pointerCache.Pop();
                        var pointerGameObject = p as MonoBehaviour;
                        if (p != null && pointerGameObject != null)
                        {
                            pointerGameObject.gameObject.SetActive(true);

                            // Track pointer for recycling
                            activePointersToConfig.Add(p, (uint)i);

                            returnPointers.Add(p);

                            // We got pointer from cache, continue to next pointer option to review
                            continue;
                        }
                    }
                    
                    // We couldn't obtain a pointer from our cache, resort to creating a new one
                    var pointer = CreatePointer(in option);
                    if (pointer != null)
                    {
                        // Track pointer for recycling
                        activePointersToConfig.Add(pointer, (uint)i);

                        returnPointers.Add(pointer);
                    }
                }
            }

            return returnPointers.Count == 0 ? null : returnPointers.ToArray();
        }

        protected virtual void RecyclePointers(IMixedRealityInputSource inputSource)
        {
            if (inputSource != null)
            {
                var pointers = inputSource.Pointers;
                for (int i = 0; i < pointers.Length; i++)
                {
                    var p = pointers[i];
                    var pGameObject = p as MonoBehaviour;
                    if (p != null && pGameObject != null)
                    {
                        if (activePointersToConfig.ContainsKey(p))
                        {
                            uint pointerOptionIndex = activePointersToConfig[p];

                            p.Controller = null;
                            pGameObject.gameObject.SetActive(false);

                            // Add our pointer back to our cache
                            pointerConfigurations[(int)pointerOptionIndex].cache.Push(p);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Instantiate the Pointer prefab with supplied PointerOption details. If there is no IMixedRealityPointer on the prefab, then destroy and log error
        /// </summary>
        /// <remarks>
        /// PointerOption is passed by ref to reduce copy overhead of struct
        /// </remarks>
        private IMixedRealityPointer CreatePointer(in PointerOption option)
        {
            var pointerObject = Object.Instantiate(option.PointerPrefab);
            MixedRealityPlayspace.AddChild(pointerObject.transform);
            var pointer = pointerObject.GetComponent<IMixedRealityPointer>();
            if (pointer == null)
            {
                Debug.LogError($"{option.PointerPrefab} does not have {typeof(IMixedRealityPointer).Name} component. Cannot create and utilize pointer");

                GameObjectExtensions.DestroyGameObject(pointerObject);
            }

            return pointer;
        }
    }
}
