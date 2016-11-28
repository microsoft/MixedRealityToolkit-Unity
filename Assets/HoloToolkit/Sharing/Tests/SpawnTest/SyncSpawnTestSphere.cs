//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
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
