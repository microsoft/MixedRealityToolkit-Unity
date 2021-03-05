// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This class implements <see cref="IMixedRealitySystem"/> and ensures proper state management.
    /// </summary>
    public abstract class BaseSystem : IMixedRealitySystem
    {
        public const uint DefaultPriority = 10;

        /// <inheritdoc />
        public virtual string Name { get; protected set; }

        /// <inheritdoc />
        public virtual uint Priority { get; protected set; } = DefaultPriority;

        /// <inheritdoc />
        public virtual BaseMixedRealityProfile ConfigurationProfile { get; protected set; } = null;

        private bool isInitialized = false;

        #region State

        /// <inheritdoc />
        public virtual bool IsInitialized
        {
            get => isInitialized;
            protected set => isInitialized = value;
        }

        private bool isEnabled = false;

        /// <inheritdoc />
        public virtual bool IsEnabled
        {
            get => isEnabled;
            protected set => isEnabled = value;
        }

        private bool isMarkedDestroyed = false;

        /// <inheritdoc />
        public virtual bool IsMarkedDestroyed
        {
            get => isMarkedDestroyed;
            protected set => isMarkedDestroyed = value;
        }

        #endregion State

        ///// <inheritdoc />
        //public virtual void Initialize()
        //{
        //    IsInitialized = true;
        //}

        ///// <inheritdoc />
        //public virtual void Destroy()
        //{
        //    IsInitialized = false;
        //    IsEnabled = false;
        //    IsMarkedDestroyed = true;
        //}

        /// <inheritdoc />
        public void Enable()
        {
            if (!IsEnabled)
            {
                IsEnabled = TryEnable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryEnable()
        {
            // todo
            return true;
        }

        /// <inheritdoc />
        public void Disable()
        {
            if (IsEnabled)
            {
                IsEnabled = !TryDisable();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryDisable()
        {
            // todo
            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            // todo
            // IsInitialized = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryReset()
        {
            // todo
            return true;
        }

        /// <inheritdoc />
        // todo
        public virtual void Update() { }

        /// <inheritdoc />
        // todo
        public virtual void LateUpdate() { }

        #region IDisposable Implementation

        /// <summary>
        /// Value indicating if the object has completed disposal.
        /// </summary>
        /// <remarks>
        /// Set by derived classes to indicate that disposal has been completed.
        /// </remarks>
        protected bool disposed = false;

        /// <summary>
        /// Finalizer
        /// </summary>
        ~BaseService()
        {
            Dispose();
        }

        /// <summary>
        /// Cleanup resources used by this object.
        /// </summary>
        public void Dispose()
        {
            // Clean up our resources (managed and unmanaged resources)
            Dispose(true);

            // Suppress finalization as the finalizer also calls our cleanup code.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleanup resources used by the object
        /// </summary>
        /// <param name="disposing">Are we fully disposing the object? 
        /// True will release all managed resources, unmanaged resources are always released.
        /// </param>
        protected virtual void Dispose(bool disposing) { }

        #endregion IDisposable Implementation
    }
}
