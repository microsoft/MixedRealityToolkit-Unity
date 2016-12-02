//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using HoloToolkit.Sharing.Spawning;

namespace HoloToolkit.Sharing.SyncModel
{
    [SyncDataClass]
    public class SyncSpawnTestSphere : SyncSpawnedObject
    {
        [SyncData]
        public SyncFloat TestFloat;
    }
}
