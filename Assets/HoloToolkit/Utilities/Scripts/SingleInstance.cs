// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A simplified version of the Singleton class which doesn't depend on the Instance being set in Awake
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleInstance<T> : MonoBehaviour where T : SingleInstance<T>
    {
        private static T _Instance;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    T[] objects = FindObjectsOfType<T>();
                    if (objects.Length == 1)
                    {
                        _Instance = objects[0];
                    }
                    else if (objects.Length > 1)
                    {
                        Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}", typeof(T).ToString(), objects.Length);
                    }
                }
                return _Instance;
            }
        }

        /// <summary>
        /// Called by Unity when destroying a MonoBehaviour. Scripts that extend
        /// SingleInstance should be sure to call base.OnDestroy() to ensure the
        /// underlying static _Instance reference is properly cleaned up.
        /// </summary>
        protected virtual void OnDestroy()
        {
            _Instance = null;
        }
    }
}