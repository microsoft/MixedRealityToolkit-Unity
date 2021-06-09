// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The base service implements <see cref="IMixedRealityService"/> and provides default properties for all services.
    /// </summary>
    public abstract class BaseService : IMixedRealityService, IMixedRealityServiceState
    {
        public const uint DefaultPriority = 10;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public virtual string Name { get; protected set; }

        /// <inheritdoc />
        public virtual uint Priority { get; protected set; } = DefaultPriority;

        /// <inheritdoc />
        public virtual BaseMixedRealityProfile ConfigurationProfile { get; protected set; } = null;

        /// <inheritdoc />
        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        /// <inheritdoc />
        public virtual void Reset()
        {
            IsInitialized = false;
        }

        /// <inheritdoc />
        public virtual void Enable()
        {
            IsEnabled = true;
        }

        /// <inheritdoc />
        public virtual void Update() { }

        /// <inheritdoc />
        public virtual void LateUpdate() { }

        /// <inheritdoc />
        public virtual void Disable()
        {
            IsEnabled = false;
        }

        /// <inheritdoc />
        public virtual void Destroy()
        {
            IsInitialized = false;
            IsEnabled = false;
            IsMarkedDestroyed = true;
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealityServiceState Implementation

        private bool? isInitialized = null;

        /// <inheritdoc />
        public virtual bool IsInitialized
        {
            get
            {
                Debug.Assert(isInitialized.HasValue, $"{GetType()} has not set a value for IsInitialized, returning false.");
                return isInitialized ?? false;
            }

            protected set => isInitialized = value;
        }

        private bool? isEnabled = null;

        /// <inheritdoc />
        public virtual bool IsEnabled
        {
            get
            {
                Debug.Assert(isEnabled.HasValue, $"{GetType()} has not set a value for IsEnabled, returning false.");
                return isEnabled ?? false;
            }

            protected set => isEnabled = value;
        }

        private bool? isMarkedDestroyed = null;

        /// <inheritdoc />
        public virtual bool IsMarkedDestroyed
        {
            get
            {
                Debug.Assert(isMarkedDestroyed.HasValue, $"{GetType()} has not set a value for IsMarkedDestroyed, returning false.");
                return isMarkedDestroyed ?? false;
            }

            protected set => isMarkedDestroyed = value;
        }

        #endregion IMixedRealityServiceState Implementation

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
