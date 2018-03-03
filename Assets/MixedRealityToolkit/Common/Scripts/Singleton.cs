// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common.Extensions;
using UnityEngine;

namespace MixedRealityToolkit.Common
{
    /// <summary>
    /// Singleton behaviour class, used for components that should only have one instance.
    /// <remarks>Singleton classes live on through scene transitions and will mark their 
    /// parent root GameObject with <see cref="Object.DontDestroyOnLoad"/></remarks>
    /// </summary>
    /// <typeparam name="T">The Singleton Type</typeparam>
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        /// <summary>
        /// Returns the Singleton instance of the classes type.
        /// If no instance is found, then we search for an instance
        /// in the scene.
        /// If more than one instance is found, we throw an error and
        /// no instance is returned.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (IsInitialized)
                {
                    return instance;
                }

                if (!searchForInstance)
                {
                    return null;
                }

                // likely first time
                T[] objects = FindObjectsOfType<T>();
                searchForInstance = false;

                if (objects.Length == 1)
                {
                    objects[0].Initialize();
                }
                else if (objects.Length > 1)
                {
                    Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}.", typeof(T).Name, objects.Length);
                }

                return instance;
            }
        }

        private static bool searchForInstance = true;

        public static void AssertIsInitialized()
        {
            Debug.Assert(IsInitialized, string.Format("The {0} singleton has not been initialized.", typeof(T).Name));
        }

        /// <summary>
        /// Returns whether the instance has been initialized or not.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return instance != null;
            }
        }

        public static bool ConfirmInitialized()
        {
            T access = Instance; // assigning the Instance to access is used.
            return IsInitialized;
        }

        [SerializeField]
        private bool dontDestroyParentRootOnLoad = true;

        private readonly object initializedLock = new object();

        protected void Initialize()
        {
            lock (initializedLock)
            {
                if (!IsInitialized)
                {
                    instance = (T)this;
                    InitializeInternal();
                    if (dontDestroyParentRootOnLoad)
                    {
                        instance.transform.root.DontDestroyOnLoad();
                    }
                }
            }
        }

        /// <summary>
        /// Function called when singleton instance is assigned.
        /// Enables lazy initialization. 
        /// Important for ensuring auto-sorting of singleton initialization.
        /// </summary>
        protected virtual void InitializeInternal()
        {
            // No overall singleton initialization needed.
        }

        /// <summary>
        /// Base Awake method that sets the Singleton's unique instance.
        /// Called by Unity when initializing a MonoBehaviour.
        /// Scripts that extend Singleton should be sure to call base.Awake() unless they want
        /// lazy initialization
        /// </summary>
        protected virtual void Awake()
        {
            if (IsInitialized && instance != this)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(this);
                }
                else
                {
                    Destroy(this);
                }

                Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}. Additional Instance was destroyed", GetType().Name);
            }
            else if (!IsInitialized)
            {
                Initialize();
                searchForInstance = false;
            }
        }

        /// <summary>
        /// Base OnDestroy method that destroys the Singleton's unique instance.
        /// Called by Unity when destroying a MonoBehaviour. Scripts that extend
        /// Singleton should be sure to call base.OnDestroy() to ensure the
        /// underlying static Instance reference is properly cleaned up.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }
    }
}
