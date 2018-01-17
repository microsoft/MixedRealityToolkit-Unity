// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Sharing.Spawning;
using MixedRealityToolkit.Sharing.SyncModel;

namespace MixedRealityToolkit.Examples.Sharing
{
    /// <summary>
    /// Class that demonstrates a custom class using sync model attributes.
    /// </summary>
    [SyncDataClass]
    public class SyncSpawnTestSphere : SyncSpawnedObject
    {
        [SyncData]
        public SyncFloat TestFloat;
    }
}

