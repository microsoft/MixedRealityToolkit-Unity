// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing
{
    /// <summary>
    /// Delegate called when a new spatial coordinate state payload has been created
    /// </summary>
    /// <param name="payload"></param>
    public delegate void SpatialCoordinateStateUpdateHandler(byte[] payload);

    /// <summary>
    /// Interface implemented by classes that establish a shared coordinate system
    /// </summary>
    public interface ISpatialCoordinateServiceOld
    {
        /// <summary>
        /// Obtains a transform from the local application origin to the shared application origin
        /// </summary>
        /// <param name="localOriginToSharedOrigin">Transform from the local application origin to the shared application origin</param>
        /// <returns>Returns true if the transform is known, otherwise false</returns>
        bool TryGetLocalOriginToSharedOrigin(out Matrix4x4 localOriginToSharedOrigin);

        /// <summary>
        /// Event called when a new spatial coordinate state payload has been created
        /// </summary>
        event SpatialCoordinateStateUpdateHandler SpatialCoordinateStateUpdated;

        /// <summary>
        /// Provides external spatial coordinate state payloads to the local spatial coordinate service
        /// </summary>
        /// <param name="playerId">Unique player id</param>
        /// <param name="payload">New external payload related to spatial coordinate services</param>
        void Sync(string playerId, byte[] payload);
    }
}
