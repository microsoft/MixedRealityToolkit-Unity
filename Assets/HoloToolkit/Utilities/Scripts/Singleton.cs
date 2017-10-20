// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Singleton behaviour class, used for components that should only have one instance
    /// </summary>
    /// <typeparam name="T">The Singleton Type</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        private static List<Action> actionsWaitingForInitialization = new List<Action>();

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
                if (searchForInstance && instance == null)
                {
                    searchForInstance = false;
                    T[] objects = FindObjectsOfType<T>();
                    if (objects.Length == 1)
                    {
                        instance = objects[0];
                        actionsWaitingForInitialization.ForEach(action => action());
                        actionsWaitingForInitialization = null;
                    }
                    else if (objects.Length > 1)
                    {
                        Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}.", typeof(T).ToString(), objects.Length);
                    }
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

        /// <summary>
        /// Base Awake method that sets the Singleton's unique instance.
        /// Called by Unity when initializing a MonoBehaviour.
        /// Scripts that extend Singleton should be sure to call base.Awake() to ensure the
        /// static Instance reference is properly created.
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
                instance = (T)this;
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

        /// <summary>
        /// Performs an action when the singleton is initialized.
        /// If the singleton is already initialized, the action will be called instantanly.
        /// Actions can be registered multiple times. In this case, the actions will also be called multiple times.
        /// </summary>
        /// <param name="action">Action to perform when the singleton is initialized.</param>
        public static void WhenInitialized(Action action)
        {
            if (IsInitialized)
            {
                action();
            }
            else
            {
                actionsWaitingForInitialization.Add(action);
            }
        }
    }
}
