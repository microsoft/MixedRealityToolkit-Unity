// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Sharing.SyncModel;

namespace MixedRealityToolkit.Sharing.Unity
{
    /// <summary>
    /// Interface that allows a components of a game object access the shared data model set by a SpawnManager.
    /// </summary>
    public interface ISyncModelAccessor
    {
        /// <summary>
        /// Sets the synchronized data model to use for this object.
        /// </summary>
        /// <param name="syncObject">Sync object to set as the model.</param>
        void SetSyncModel(SyncObject syncObject);
    }
}
