//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using HoloToolkit.Sharing.SyncModel;

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
