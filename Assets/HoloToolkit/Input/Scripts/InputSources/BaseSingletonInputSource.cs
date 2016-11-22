// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Base class for a singleton input source.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSingletonInputSource<T> : BaseInputSource where T : BaseSingletonInputSource<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                return instance;
            }
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
        /// Base awake method that sets the singleton's unique instance.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}", GetType().Name);
            }
            else
            {
                instance = (T)this;
            }
        }

        protected override void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }

            base.OnDestroy();
        }
    }
}
