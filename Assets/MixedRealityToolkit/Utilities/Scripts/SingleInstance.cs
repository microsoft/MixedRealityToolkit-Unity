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
                    if (objects.Length != 1)
                    {
                        Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}", typeof(T).ToString(), objects.Length);
                    }
                    else
                    {
                        _Instance = objects[0];
                    }
                }
                return _Instance;
            }
        }

        protected void OnDestroy()
        {
            _Instance = null;
        }
    }
}