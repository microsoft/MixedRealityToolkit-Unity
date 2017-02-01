//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System;
using UnityEngine;
using HoloToolkit.Sharing.SyncModel;

namespace HoloToolkit.Sharing.Spawning
{
    /// <summary>
    /// This array is meant to hold SyncSpawnedObject and objects of subclasses.
    /// Compared to SyncArray, this supports dynamic types for objects.
    /// </summary>
    /// <typeparam name="T">Type of object that the array contains</typeparam>
    public class SyncSpawnArray<T> : SyncArray<T> where T : SyncSpawnedObject, new()
    {
        public SyncSpawnArray(string field) : base(field) { }

        public SyncSpawnArray() : base(string.Empty) { }

        protected override T CreateObject(ObjectElement objectElement)
        {
            string objectType = objectElement.GetObjectType();

            Type typeToInstantiate = Type.GetType(objectType);
            if (typeToInstantiate == null)
            {
                Debug.LogError("Could not find the SyncModel type to instantiate.");
                return null;
            }

            object createdObject = Activator.CreateInstance(typeToInstantiate);

            T spawnedDataModel = (T)createdObject;
            spawnedDataModel.Element = objectElement;
            spawnedDataModel.FieldName = objectElement.GetName();
            // TODO: this should not query SharingStage, but instead query the underlying session layer
            spawnedDataModel.Owner = SharingStage.Instance.SessionUsersTracker.GetUserById(objectElement.GetOwnerID());

            return spawnedDataModel;
        }
    }
}