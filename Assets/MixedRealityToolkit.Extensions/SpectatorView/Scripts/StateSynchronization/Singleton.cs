// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// MonoBehaviour base class used to ensure only one instance of the class exists in the application/scene.
    /// </summary>
    /// <typeparam name="T">Desired type</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _Instance;

        /// <summary>
        /// Returns true if the class has been instantiated, otherwise false
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return _Instance != null;
            }
        }

        protected virtual void Awake()
        {
            _Instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            _Instance = null;
        }

        /// <summary>
        /// Returns the global instance for the class
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<T>();
                }
                return _Instance;
            }
        }
    }
}