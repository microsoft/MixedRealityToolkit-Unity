﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Class providing a base implementation of the <see cref="IMixedRealityInputDeviceManager"/> interface.
    /// </summary>
    public abstract class BaseInputDeviceManager : BaseDataProvider<IMixedRealityInputSystem>, IMixedRealityInputDeviceManager
    {
        private bool enablePointerCache = true;

        /// <summary>
        /// Control mechanism to enable/disable use of Pointer Cache in request/recycling of pointers by Input System
        /// </summary>
        public bool EnablePointerCache
        {
            get => enablePointerCache;
            set
            {
                if (enablePointerCache != value)
                {
                    enablePointerCache = value;

                    if (!enablePointerCache)
                    {
                        DestroyPointerCache();
                    }
                }
            }
        }

        /// <summary>
        /// The input system configuration profile in use in the application.
        /// </summary>
        protected MixedRealityInputSystemProfile InputSystemProfile => Service?.InputSystemProfile;

        /// <inheritdoc />
        public virtual IMixedRealityController[] GetActiveControllers() => System.Array.Empty<IMixedRealityController>();

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
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile) { }

        #region Private members

        private struct PointerConfig
        {
            public PointerOption profile;
            public Stack<IMixedRealityPointer> cache;
        }

        private PointerConfig[] pointerConfigurations = System.Array.Empty<PointerConfig>();

        private class PointerEqualityComparer : IEqualityComparer<IMixedRealityPointer>
        {
            private static PointerEqualityComparer defaultComparer;

            internal static PointerEqualityComparer Default => defaultComparer ?? (defaultComparer = new PointerEqualityComparer());

            /// <summary>
            /// Check that references equals for two pointers
            /// </summary>
            public bool Equals(IMixedRealityPointer p1, IMixedRealityPointer p2)
            {
                return ReferenceEquals(p1, p2);
            }

            /// <summary>
            /// Unity objects have unique equals comparison and to check keys in a dictionary,
            /// we want the hash code match to be Unity's unique InstanceID to compare objects.
            /// </summary>
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

        private static readonly ProfilerMarker RequestPointersPerfMarker = new ProfilerMarker("[MRTK] BaseInputDeviceManager.RequestPointers");

        // Active pointers associated with the config index they were spawned from
        private readonly Dictionary<IMixedRealityPointer, uint> activePointersToConfig
            = new Dictionary<IMixedRealityPointer, uint>(PointerEqualityComparer.Default);

        #endregion

        #region IMixedRealityService implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            if (InputSystemProfile != null && InputSystemProfile.PointerProfile != null)
            {
                var initPointerOptions = InputSystemProfile.PointerProfile.PointerOptions;

                // If we were previously initialized, then clear our old pointer cache
                if (pointerConfigurations != null && pointerConfigurations.Length > 0)
                {
                    DestroyPointerCache();
                }

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
            DestroyPointerCache();

            // Loop through active pointers in scene, destroy all gameobjects and clear our tracking dictionary
            foreach (var pointer in activePointersToConfig.Keys)
            {
                if (pointer.TryGetMonoBehaviour(out MonoBehaviour pointerComponent))
                {
                    GameObjectExtensions.DestroyGameObject(pointerComponent.gameObject);
                }
            }

            pointerConfigurations = System.Array.Empty<PointerConfig>();
            activePointersToConfig.Clear();

            base.Destroy();
        }

        #endregion

        #region Pointer utilization and caching

        /// <summary>
        /// Request an array of pointers for the controller type.
        /// </summary>
        /// <param name="controllerType">The controller type making the request for pointers.</param>
        /// <param name="controllingHand">The handedness of the controller making the request.</param>
        /// <param name="useSpecificType">Only register pointers with a specific type.</param>
        protected virtual IMixedRealityPointer[] RequestPointers(SupportedControllerType controllerType, Handedness controllingHand)
        {
            using (RequestPointersPerfMarker.Auto())
            {
                var returnPointers = new List<IMixedRealityPointer>();

                CleanActivePointers();

                for (int i = 0; i < pointerConfigurations.Length; i++)
                {
                    var option = pointerConfigurations[i].profile;
                    if (option.ControllerType.IsMaskSet(controllerType) && option.Handedness.IsMaskSet(controllingHand))
                    {
                        IMixedRealityPointer requestedPointer = null;

                        if (EnablePointerCache)
                        {
                            var pointerCache = pointerConfigurations[i].cache;
                            while (pointerCache.Count > 0)
                            {
                                var p = pointerCache.Pop();
                                if (p.TryGetMonoBehaviour(out MonoBehaviour pointerComponent))
                                {
                                    pointerComponent.gameObject.SetActive(true);

                                    // We got pointer from cache, continue to next pointer option to review
                                    requestedPointer = p;

                                    DebugUtilities.LogVerboseFormat("RequestPointers: Reusing a cached pointer {0} for controller type {1} and handedness {2}",
                                        requestedPointer,
                                        controllerType,
                                        controllingHand);
                                    break;
                                }
                            }
                        }

                        if (requestedPointer == null)
                        {
                            // We couldn't obtain a pointer from our cache, resort to creating a new one
                            requestedPointer = CreatePointer(ref option);
                        }

                        if (requestedPointer != null)
                        {
                            // Track pointer for recycling
                            activePointersToConfig.Add(requestedPointer, (uint)i);

                            returnPointers.Add(requestedPointer);
                        }
                    }
                }

                return returnPointers.Count == 0 ? null : returnPointers.ToArray();
            }
        }

        private static readonly ProfilerMarker RecyclePointersPerfMarker = new ProfilerMarker("[MRTK] BaseInputDeviceManager.RecyclePointers");

        /// <summary>
        /// Recycle all pointers associated with the provided <see cref="IMixedRealityInputSource"/>. 
        /// This involves reseting the pointer, disabling the pointer GameObject, and possibly caching it for re-use.
        /// </summary>
        protected virtual void RecyclePointers(IMixedRealityInputSource inputSource)
        {
            using (RecyclePointersPerfMarker.Auto())
            {
                if (inputSource != null)
                {
                    CleanActivePointers();

                    var pointers = inputSource.Pointers;
                    for (int i = 0; i < pointers.Length; i++)
                    {
                        var pointer = pointers[i];
                        if (pointers[i].TryGetMonoBehaviour(out MonoBehaviour pointerComponent))
                        {
                            // Unfortunately, it's possible the gameobject source is *being* destroyed so we are not null now but will be soon.
                            // At least if this is a controller we know about and we expect it to be destroyed, skip
                            if (pointer is IMixedRealityControllerPoseSynchronizer controller && controller.DestroyOnSourceLost)
                            {
                                continue;
                            }

                            if (EnablePointerCache)
                            {
                                pointer.Reset();
                                pointerComponent.gameObject.SetActive(false);

                                if (EnablePointerCache && activePointersToConfig.ContainsKey(pointer))
                                {
                                    uint pointerOptionIndex = activePointersToConfig[pointer];
                                    activePointersToConfig.Remove(pointer);

                                    // Add our pointer back to our cache
                                    pointerConfigurations[(int)pointerOptionIndex].cache.Push(pointer);
                                }
                            }
                            else
                            {
                                GameObjectExtensions.DestroyGameObject(pointerComponent.gameObject);
                            }
                        }
                    }
                }
            }
        }

        private static readonly ProfilerMarker CreatePointerPerfMarker = new ProfilerMarker("[MRTK] BaseInputDeviceManager.CreatePointer");

        /// <summary>
        /// Instantiate the Pointer prefab with supplied PointerOption details. If there is no IMixedRealityPointer on the prefab, then destroy and log error
        /// </summary>
        /// <remarks>
        /// PointerOption is passed by ref to reduce copy overhead of struct
        /// </remarks>
        private IMixedRealityPointer CreatePointer(ref PointerOption option)
        {
            using (CreatePointerPerfMarker.Auto())
            {
                var pointerObject = Object.Instantiate(option.PointerPrefab);
                MixedRealityPlayspace.AddChild(pointerObject.transform);
                var pointer = pointerObject.GetComponent<IMixedRealityPointer>();
                if (pointer == null)
                {
                    Debug.LogError($"Ensure that the prefab '{option.PointerPrefab.name}' listed under Input -> Pointers -> Pointer Options has an {typeof(IMixedRealityPointer).Name} component.\nThis prefab can't be used as a pointer as configured and won't be instantiated.");

                    GameObjectExtensions.DestroyGameObject(pointerObject);
                }

                // Make sure we init the pointer with the correct raycast LayerMasks, if needed
                if (pointer.PrioritizedLayerMasksOverride == null || pointer.PrioritizedLayerMasksOverride.Length == 0)
                {
                    pointer.PrioritizedLayerMasksOverride = option.PrioritizedLayerMasks;
                }

                return pointer;
            }
        }

        private static readonly ProfilerMarker CleanActivePointersPerfMarker = new ProfilerMarker("[MRTK] BaseInputDeviceManager.CleanActivePointers");

        /// <summary>
        /// This class tracks pointers that have been requested and thus are considered "active" GameObjects in the scene. 
        /// As GameObjects, these pointers may be destroyed and thus their entry becomes "null" although the managed object is not destroyed
        /// This helper loops through all dictionary entries and checks if it is null, if so it is removed
        /// </summary>
        private void CleanActivePointers()
        {
            using (CleanActivePointersPerfMarker.Auto())
            {
                var removal = new List<IMixedRealityPointer>();

                var enumerator = activePointersToConfig.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var pointer = enumerator.Current.Key;
                    if (pointer.IsNull())
                    {
                        removal.Add(pointer);
                    }
                }

                for (int i = 0; i < removal.Count; i++)
                {
                    activePointersToConfig.Remove(removal[i]);
                }
            }
        }

        /// <summary>
        /// Wipes references to cached pointers for every pointer configuration option. All GameObject references are likewise destroyed
        /// </summary>
        private void DestroyPointerCache()
        {
            for (int i = 0; i < pointerConfigurations.Length; i++)
            {
                while (pointerConfigurations[i].cache.Count > 0)
                {
                    if (pointerConfigurations[i].cache.Pop().TryGetMonoBehaviour(out MonoBehaviour pointerComponent))
                    {
                        GameObjectExtensions.DestroyGameObject(pointerComponent.gameObject);
                    }
                }
            }
        }

        #endregion
    }
}
