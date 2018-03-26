// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;

namespace ARCA
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Backing field for the Instance property
        /// </summary>
        static private T _Instance;

        /// <summary>
        /// Instance accessor
        /// </summary>
        static public T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    throw new System.InvalidOperationException(string.Format("No instance of {0} has been created, please DO NOT USE != null or == null with Instance, Check Availability through the .Exists property.", typeof(T).Name));
                }

                return _Instance;
            }
        }

        /// <summary>
        /// Query if this singleton exists
        /// </summary>
        static public bool Exists
        {
            get
            {
                return _Instance != null;
            }
        }

        /// <summary>
        /// First call that unity will make into this object
        /// </summary>
        protected virtual void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this as T;
            }
            else
            {
                throw new System.InvalidOperationException(string.Format("An instance of {0} already exists on {1}", typeof(T).Name, _Instance.gameObject.name));
            }
        }

        /// <summary>
        /// Final call unity will make to this object
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_Instance == this)
            {
                _Instance = null;
            }
            else
            {
                throw new System.InvalidOperationException(string.Format("This is not the active instance, this object {0}, instance object {1}", this.gameObject.name, _Instance ? _Instance.gameObject.name : "null"));
            }
        }
    }
}
