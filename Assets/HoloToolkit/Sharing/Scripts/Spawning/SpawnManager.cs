// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing.SyncModel;

namespace HoloToolkit.Sharing.Spawning
{
    /// <summary>
    /// A SpawnManager is in charge of spawning the appropriate objects based on changes to an array of data model objects
    /// to which it is registered.
    /// It also manages the lifespan of these spawned objects.
    /// </summary>
    /// <typeparam name="T">Type of SyncObject in the array being monitored by the SpawnManager.</typeparam>
    public abstract class SpawnManager<T> : MonoBehaviour where T : SyncObject, new()
    {
        protected SharingStage NetworkManager { get; private set; }

        protected SyncArray<T> SyncSource;

        protected List<GameObject> SyncSpawnObjectListInternal = new List<GameObject>(0);

        public bool IsSpawningObjects { get; protected set; }

        public List<GameObject> SyncSpawnObjectList { get { return SyncSpawnObjectListInternal; } }

        protected virtual void Start()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            NetworkManager = SharingStage.Instance;
            if (NetworkManager.IsConnected)
            {
                Connected();
            }
            else
            {
                NetworkManager.SharingManagerConnected += Connected;
            }
        }

        protected virtual void Connected(object sender = null, EventArgs e = null)
        {
            if (SyncSource != null)
            {
                IsSpawningObjects = true;
                UnRegisterToDataModel();

                for (var i = 0; i < SyncSpawnObjectListInternal.Count; i++)
                {
                    Destroy(SyncSpawnObjectListInternal[i]);
                }

                SyncSpawnObjectListInternal.Clear();
            }

            SetDataModelSource();
            RegisterToDataModel();

            if (IsSpawningObjects)
            {
                ReSpawnObjects();
                IsSpawningObjects = false;
            }
        }

        /// <summary>
        /// Sets the data model source for the spawn manager.
        /// </summary>
        protected abstract void SetDataModelSource();

        /// <summary>
        /// Register to data model updates;
        /// </summary>
        private void RegisterToDataModel()
        {
            SyncSource.ObjectAdded += OnObjectAdded;
            SyncSource.ObjectRemoved += OnObjectRemoved;
        }

        private void UnRegisterToDataModel()
        {
            SyncSource.ObjectAdded -= OnObjectAdded;
            SyncSource.ObjectRemoved -= OnObjectRemoved;
        }

        private void OnObjectAdded(T addedObject)
        {
            InstantiateFromNetwork(addedObject);
        }

        private void OnObjectRemoved(T removedObject)
        {
            RemoveFromNetwork(removedObject);
        }

        private void ReSpawnObjects()
        {
            T[] objs = SyncSource.GetDataArray();

            for (var i = 0; i < objs.Length; i++)
            {
                InstantiateFromNetwork(objs[i]);
            }
        }

        /// <summary>
        /// Delete the data model for an object and all its related game objects.
        /// </summary>
        /// <param name="objectToDelete">Object that needs to be deleted.</param>
        public abstract void Delete(T objectToDelete);

        /// <summary>
        /// Instantiate game objects based on data model that was created on the network.
        /// </summary>
        /// <param name="addedObject">Object that was added to the data model.</param>
        protected abstract void InstantiateFromNetwork(T addedObject);

        /// <summary>
        /// Remove an object based on data model that was removed on the network.
        /// </summary>
        /// <param name="removedObject">Object that was removed from the data model.</param>
        protected abstract void RemoveFromNetwork(T removedObject);
    }
}
