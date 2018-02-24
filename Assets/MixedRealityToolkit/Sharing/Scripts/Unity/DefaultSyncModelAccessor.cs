// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Sharing.SyncModel;
using UnityEngine;

namespace MixedRealityToolkit.Sharing.Unity
{
    /// <summary>
    /// Default implementation of a MonoBehaviour that allows other components of a game object access the shared data model
    /// as a raw SyncObject instance.
    /// </summary>
    public class DefaultSyncModelAccessor : MonoBehaviour, ISyncModelAccessor
    {
        public SyncObject SyncModel { get; private set; }

        public void SetSyncModel(SyncObject syncObject)
        {
            SyncModel = syncObject;
        }
    }
}
