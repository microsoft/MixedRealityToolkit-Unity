//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine;
using HoloToolkit.Sharing.SyncModel;

/// <summary>
/// Default implementation of a behaviour that allows other components of a game object access the shared data model
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
