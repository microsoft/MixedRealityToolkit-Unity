//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using System;
using System.Collections.Generic;
using System.Reflection;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Root of the synchronization data model used by this application.
    /// </summary>
    public class SyncRoot : SyncObject
    {
        // Children of root
        [SyncData]
        public SyncArray<SyncSpawnedObject> InstantiatedPrefabs;

        public SyncRoot(ObjectElement rootElement)
        {
            Element = rootElement;
            FieldName = Element.GetName().GetString();
            InitializeSyncSettings();
            InitializeDataModel();
        }

        private void InitializeSyncSettings()
        {
            SyncSettings.Instance.Initialize();
        }

        /// <summary>
        /// Initializes any data models that need to have a local state.
        /// </summary>
        private void InitializeDataModel()
        {
            InstantiatedPrefabs.InitializeLocal(Element);
        }       
    }
}
