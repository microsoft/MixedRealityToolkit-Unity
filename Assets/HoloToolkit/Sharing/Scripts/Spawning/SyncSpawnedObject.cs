//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using HoloToolkit.Sharing.SyncModel;
using UnityEngine;

namespace HoloToolkit.Sharing.Spawning
{
    /// <summary>
    /// A SpawnedObject contains all the information needed for another device to spawn an object in the same location
    /// as where it was originally created on this device.
    /// </summary>
    [SyncDataClass]
    public class SyncSpawnedObject : SyncObject
    {
        /// <summary>
        /// Transform (position, orientation and scale) for the object.
        /// </summary>
        [SyncData] public SyncTransform Transform;

        /// <summary>
        /// Name of the object.
        /// </summary>
        [SyncData] public SyncString Name;

        /// <summary>
        /// Path to the parent object in the game object.
        /// </summary>
        [SyncData] public SyncString ParentPath;

        /// <summary>
        /// Path to the object
        /// </summary>
        [SyncData] public SyncString ObjectPath;


        public GameObject GameObject { get; set; }

        public virtual void Initialize(string name, string parentPath)
        {
            this.Name.Value = name;
            this.ParentPath.Value = parentPath;

            this.ObjectPath.Value = string.Empty;
            if (!string.IsNullOrEmpty(this.ParentPath.Value))
            {
                this.ObjectPath.Value = this.ParentPath.Value + "/";
            }

            this.ObjectPath.Value += this.Name.Value;
        }
    }
}
