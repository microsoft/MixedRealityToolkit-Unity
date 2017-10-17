// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A singleton designed to help child singletons take certain actions only after Start has been called.
    /// </summary>
    [Obsolete]
    public abstract class StartAwareSingleton<T> : Singleton<T>
        where T : StartAwareSingleton<T>
    {
        #region MonoBehaviour implementation

        protected virtual void OnEnable()
        {
            if (IsStarted)
            {
                OnEnableAfterStart();
            }
        }

        protected virtual void OnDisable()
        {
            if (IsStarted)
            {
                OnDisableAfterStart();
            }
        }

        protected virtual void Start()
        {
            IsStarted = true;
            OnEnableAfterStart();
        }

        #endregion

        protected bool IsStarted { get; private set; }

        /// <summary>
        /// This method is similar to Unity's OnEnable method, except that it's called only after Start. This
        /// means all <see cref="Singleton{T}"/> classes will have had a chance to run their Awake methods and
        /// <see cref="Singleton{T}.Instance"/> will be safe to use.
        /// </summary>
        protected virtual void OnEnableAfterStart()
        {
            Debug.Assert(IsStarted, "OnEnableAfterStart should only occur after Start.");
        }

        /// <summary>
        /// This method is similar to Unity's OnDisable method, except that it's called only after Start. This
        /// means all <see cref="Singleton{T}"/> classes will have had a chance to run their Awake methods and
        /// <see cref="Singleton{T}.Instance"/> will be safe to use.
        /// </summary>
        protected virtual void OnDisableAfterStart()
        {
            Debug.Assert(IsStarted, "OnDisableAfterStart should only occur after Start.");
        }
    }
}
