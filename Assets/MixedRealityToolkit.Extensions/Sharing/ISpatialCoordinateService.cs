// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public delegate void SpatialCoordinateStateUpdateHandler(byte[] payload);

    public interface ISpatialCoordinateService
    {
        bool TryGetLocalOriginToSharedOrigin(out Matrix4x4 localOriginToSharedOrigin);

        // TODO - unclear how player id and payloads should be associated
        event SpatialCoordinateStateUpdateHandler SpatialCoordinateStateUpdated;
        void Sync(string playerId, byte[] payload);
    }
}
