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
    public abstract class BaseInputDeviceManager : BaseDataProvider<IMixedRealityInputSystem>, IMixedRealityInputDeviceManager
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
        { }

        /// <summary>
        /// The input system configuration profile in use in the application.
        /// </summary>
        protected MixedRealityInputSystemProfile InputSystemProfile => Service != null ? Service.InputSystemProfile : null;

        /// <inheritdoc />
        public virtual IMixedRealityController[] GetActiveControllers() => System.Array.Empty<IMixedRealityController>();

        private struct PointerConfig
        {
            public PointerOption profile;
            public Stack<IMixedRealityPointer> cache;
        }

        private PointerConfig[] pointerConfigurations = System.Array.Empty<PointerConfig>();

        private class PointerEqualityComparer : IEqualityComparer<IMixedRealityPointer>
        {
            private static PointerEqualityComparer defaultComparer;

            internal static PointerEqualityComparer Default
            {
                get =>  defaultComparer ?? (defaultComparer = new PointerEqualityComparer());
            }

            public bool Equals(IMixedRealityPointer p1, IMixedRealityPointer p2)
            {
                return ReferenceEquals(p1, p2);
            }

            public int GetHashCode(IMixedRealityPointer pointer)
            {
                if (pointer is MonoBehaviour pointerObj)
                {
                    return pointerObj.GetInstanceID();
                }
                else
                {
                    return pointer.GetHashCode();
                }
            }
        }

        // TODO: Troy - Some reason IMixedRealityPointer keys do not return true on containskey
        // TODO: Troy - problem with pointer/pointerids is destroyed pointers won't be cleared*
        // Active pointers associated with the config index they were spawned from
        private Dictionary<IMixedRealityPointer, uint> activePointersToConfig = new Dictionary<IMixedRealityPointer, uint>(PointerEqualityComparer.Default);
        //private Dictionary<uint, uint> activePointersToConfig = new Dictionary<uint, uint>();

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (InputSystemProfile != null && InputSystemProfile.PointerProfile != null)
            {
                var initPointerOptions = InputSystemProfile.PointerProfile.PointerOptions;

                pointerConfigurations = new PointerConfig[initPointerOptions.Length];
                activePointersToConfig.Clear();

                for (int i = 0; i < initPointerOptions.Length; i++)
                {
                    pointerConfigurations[i].profile = initPointerOptions[i];
                    pointerConfigurations[i].cache = new Stack<IMixedRealityPointer>();
                }
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            for (int i = 0; i < pointerConfigurations.Length; i++)
            {
                while (pointerConfigurations[i].cache.Count > 0)
                {
                    var ptr = pointerConfigurations[i].cache.Pop();
                    var pointerComponent = ptr as MonoBehaviour;
                    if (pointerComponent != null)
                    {
                        GameObjectExtensions.DestroyGameObject(pointerComponent.gameObject);
                    }
                }
            }

            // TODO: Troy also loop through activePointersToconfig and delete gameobjects there?

            pointerConfigurations = System.Array.Empty<PointerConfig>();
            activePointersToConfig.Clear();
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
                            Debug.Log($"{p.PointerName} retrieved from cache in request");

                            pointerGameObject.gameObject.SetActive(true);

                            // Track pointer for recycling
                            //activePointersToConfig.Add(p.PointerId, (uint)i);
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
                        Debug.Log($"{pointer.PointerId} created after cache miss in request");

                        // Track pointer for recycling
                        //activePointersToConfig.Add(pointer.PointerId, (uint)i);
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
                        // TODO: Troy - look at reset method or other properties?
                        p.Controller = null;
                        pGameObject.gameObject.SetActive(false);

                        if (activePointersToConfig.ContainsKey(p))
                        {
                            uint pointerOptionIndex = activePointersToConfig[p];
                            activePointersToConfig.Remove(p);

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
