// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// Helper base class for <see cref="ISpatialCoordinateService"/> implementations.
    /// </summary>
    /// <typeparam name="TKey">They key for the <see cref="ISpatialCoordinate"/>.</typeparam>
    public abstract class SpatialCoordinateServiceUnityBase<TKey> : SpatialCoordinateServiceBase<TKey>
    {
        /// <inheritdoc />
        public override sealed Task<ISpatialCoordinate> TryCreateCoordinateAsync(Vector3 worldPosition, Quaternion worldRotation, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return TryCreateCoordinateAsync(worldPosition.AsUnityVector(), worldRotation.AsUnityQuaternion(), cancellationToken);
        }

        protected virtual Task<ISpatialCoordinate> TryCreateCoordinateAsync(UnityEngine.Vector3 worldPosition, UnityEngine.Quaternion worldRotation, CancellationToken cancellationToken)
        {
            return Task.FromResult<ISpatialCoordinate>(null);
        }
    }
}
