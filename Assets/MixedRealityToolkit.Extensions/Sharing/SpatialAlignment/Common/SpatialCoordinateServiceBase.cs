// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// Helper base class for <see cref="ISpatialCoordinateService"/> implementations.
    /// </summary>
    /// <typeparam name="TKey">They key for the <see cref="ISpatialCoordinate"/>.</typeparam>
    public abstract class SpatialCoordinateServiceBase<TKey> : DisposableBase, ISpatialCoordinateService
    {
        /// <inheritdoc />
        public event Action<ISpatialCoordinate> CoordinatedDiscovered;

        private readonly object discoveryLockObject = new object();
        private readonly CancellationTokenSource disposedCTS = new CancellationTokenSource();

        private bool isTracking;

        protected readonly ConcurrentDictionary<TKey, ISpatialCoordinate> knownCoordinates = new ConcurrentDictionary<TKey, ISpatialCoordinate>();

        /// <inheritdoc />
        public bool IsTracking
        {
            get
            {
                ThrowIfDisposed();
                return isTracking;
            }
        }

        protected virtual bool SupportsDiscovery => true;

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

            // Notify of dispose to any existing operations
            disposedCTS.Cancel();
            disposedCTS.Dispose();

            knownCoordinates.Clear();
        }

        /// <summary>
        /// Adds a coordinate to be tracked by this service.
        /// </summary>
        protected void OnNewCoordinate(TKey id, ISpatialCoordinate spatialCoordinate)
        {
            ThrowIfDisposed();

            lock (discoveryLockObject)
            {
                if (!isTracking)
                {
                    throw new InvalidOperationException("We aren't tracking, and shouldn't expect additional coordinates to be discovered.");
                }
            }

            if (knownCoordinates.TryAdd(id, spatialCoordinate))
            {
                CoordinatedDiscovered?.Invoke(spatialCoordinate);
            }
            else
            {
                throw new InvalidOperationException($"Coordinate with id '{id}' already discovered.");
            }
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
        public async Task<bool> TryDiscoverCoordinatesAsync(CancellationToken cancellationToken, string[] idsToLocate = null)
        {
            if (!SupportsDiscovery)
            {
                return false;
            }

            lock (discoveryLockObject)
            {
                ThrowIfDisposed();

                if (isTracking)
                {
                    return false;
                }

                isTracking = true;
            }

            try
            {
                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(disposedCTS.Token, cancellationToken))
                {
                    await OnDiscoverCoordinatesAsync(cts.Token).IgnoreCancellation();
                }

                return true;
            }
            finally
            {
                isTracking = false;
            }
        }

        protected abstract bool TryParse(string id, out TKey result);

        /// <summary>
        /// Implement this method for the logic begin and end tracking (when <see cref="CancellationToken"/> is cancelled).
        /// </summary>
        protected abstract Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken, TKey[] idsToLocate = null);
    }
}
