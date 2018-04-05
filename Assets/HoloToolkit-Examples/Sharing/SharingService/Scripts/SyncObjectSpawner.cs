// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Sharing.Tests
{
    /// <summary>
    /// Class that handles spawning sync objects on keyboard presses, for the SpawningTest scene.
    /// </summary>
    public class SyncObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        private PrefabSpawnManager spawnManager = null;

        [SerializeField]
        [Tooltip("Optional transform target, for when you want to spawn the object on a specific parent.  If this value is not set, then the spawned objects will be spawned on this game object.")]
        private Transform spawnParentTransform;

        private void Awake()
        {
            if (spawnManager == null)
            {
                Debug.LogError("You need to reference the spawn manager on SyncObjectSpawner.");
            }

            // If we don't have a spawn parent transform, then spawn the object on this transform.
            if (spawnParentTransform == null)
            {
                spawnParentTransform = transform;
            }
        }

        /// <summary>
        /// Spawn a sync object at the position and with the rotation you want.
        /// </summary>
        public bool SpawnSyncObject(SyncSpawnedObject syncObject, Vector3 position, Quaternion rotation)
        {
            if (!spawnManager.Spawn(syncObject, position, rotation, spawnParentTransform.gameObject, syncObject.Name.Value, false)) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Search a type of SyncObject and return it.
        /// </summary>
        public SyncSpawnedObject SearchSyncObject(Type type)
        {
            foreach (var syncObject in SharingStage.Instance.Root.InstantiatedPrefabs.GetDataArray()) {
                if (syncObject.GetType() == type) {
                     return syncObject;
                } 
            }
            return null;
        }

        /// <summary>
        /// Deletes any sync object that inherits from SyncSpawnObject.
        /// </summary>
        public void DeleteSyncObject(GameObject gameObject)
        {
            DeleteSyncObject(gameObject.GetComponent<DefaultSyncModelAccessor>());
        }

        /// <summary>
        /// Deletes any sync object that inherits from SyncSpawnObject.
        /// </summary>
        public void DeleteSyncObject(DefaultSyncModelAccessor gameObject)
        {
            DeleteSyncObject((SyncSpawnedObject)(gameObject.SyncModel);
        }

        /// <summary>
        /// Deletes any sync object that inherits from SyncSpawnObject.
        /// </summary>
        public void DeleteSyncObject(SyncSpawnedObject syncObject)
        {
            spawnManager.Delete(syncObject);
        }
    }
}
