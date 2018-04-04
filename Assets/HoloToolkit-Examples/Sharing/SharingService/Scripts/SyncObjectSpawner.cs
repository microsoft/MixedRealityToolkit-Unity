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

        public bool SpawnSyncObject(SyncSpawnedObject o, Vector3 position, Quaternion rotation)
        {
            if (!spawnManager.Spawn(o, position, rotation, spawnParentTransform.gameObject, o.Name.Value, false))
                return false;
            return true;
        }

        public SyncSpawnedObject SearchSyncObject(Type type)
        {
            foreach (var elt in SharingStage.Instance.Root.InstantiatedPrefabs.GetDataArray())
                if (elt.GetType() == type)
                    return elt;
            return null;
        }

        public void DeleteSyncObject(GameObject o)
        {
            spawnManager.Delete((SyncSpawnedObject)o.GetComponent<DefaultSyncModelAccessor>().SyncModel);
        }

        public void DeleteSyncObject(SyncSpawnedObject o)
        {
            spawnManager.Delete(o);
        }
    }
}
