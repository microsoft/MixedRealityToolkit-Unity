// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing
{
    /// <summary>
    /// Helper base class for <see cref="ISpatialCoordinateService"/> implementations.
    /// </summary>
    /// <typeparam name="TKey">They key for the <see cref="ISpatialCoordinate"/>.</typeparam>
    public abstract class SpatialCoordinateServiceBase<TKey> : DisposableBase, ISpatialCoordinateService
    {
        /// <inheritdoc />
        public event Action<ISpatialCoordinate> CoordinatedDiscovered;

        private CancellationTokenSource trackingCTS = null;
        private bool isTracking;

        /// <inheritdoc />
        public bool IsTracking
        {
            get
            {
                ThrowIfDisposed();
                return isTracking;
            }
            set
            {
                ThrowIfDisposed();

                if (isTracking != value)
                {
                    trackingCTS?.Cancel();
                    trackingCTS?.Dispose();
                    trackingCTS = null;

                    if (value)
                    {
                        CancellationTokenSource cts = new CancellationTokenSource();
                        trackingCTS = cts;

                        RunTrackingAsync(cts.Token).FireAndForget();
                    }

                    isTracking = value;
                }
            }
        }

        protected readonly Dictionary<TKey, ISpatialCoordinate> knownCoordinates = new Dictionary<TKey, ISpatialCoordinate>();

        /// <inheritdoc />
        public IEnumerable<ISpatialCoordinate> KnownCoordinates
        {
            get
            {
                ThrowIfDisposed();

                return knownCoordinates.Values.Cast<ISpatialCoordinate>();
            }
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            knownCoordinates.Clear();
        }

        /// <summary>
        /// Adds a coordinate to be tracked by this service.
        /// </summary>
        protected void OnNewCoordinate(TKey id, ISpatialCoordinate spatialCoordinate)
        {
            ThrowIfDisposed();

            knownCoordinates.Add(id, spatialCoordinate);
            CoordinatedDiscovered?.Invoke(spatialCoordinate);
        }

        /// <inheritdoc />
        public virtual Task<ISpatialCoordinate> TryCreateCoordinateAsync(Vector3 worldPosition, Quaternion worldRotation, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return Task.FromResult<ISpatialCoordinate>(null);
        }

        /// <inheritdoc />
        public virtual Task<bool> TryDeleteCoordinateAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public virtual Task<ISpatialCoordinate> TryGetCoordinateByIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return Task.FromResult<ISpatialCoordinate>(null);
        }

        /// <summary>
        /// Implement this method for the logic begin and end tracking (when <see cref="CancellationToken"/> is cancelled).
        /// </summary>
        protected abstract Task RunTrackingAsync(CancellationToken cancellationToken);
    }
}
