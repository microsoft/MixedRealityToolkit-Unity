//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System;
using HoloToolkit.Sharing.SyncModel;
using UnityEngine;

namespace HoloToolkit.Sharing.Spawning
{
    /// <summary>
    /// This array is meant to hold SyncSpawnedObject and objects of subclasses.
    /// Compared to SyncArray, this supports dynamic types for objects.
    /// </summary>
    /// <typeparam name="T">Type of object that the array contains</typeparam>
    public class SyncSpawnArray<T> : SyncArray<T> where T : SyncSpawnedObject, new()
    {
        public SyncSpawnArray(string field)
            : base(field)
        {
        }

        public SyncSpawnArray()
            : base(string.Empty)
        {
        }

        protected override T CreateObject(ObjectElement objectElement)
        {
            string objectType = objectElement.GetObjectType();

            Type typeToInstantiate = Type.GetType(objectType);
            if (typeToInstantiate == null)
            {
                Debug.LogErrorFormat("Could not find the SyncModel type to instantiate.");
                return null;
            }

            System.Object createdObject = Activator.CreateInstance(typeToInstantiate);

            T spawnedDataModel = createdObject as T;
            spawnedDataModel.Element = objectElement;
            spawnedDataModel.FieldName = objectElement.GetName();
            spawnedDataModel.Owner = this.Owner; // TODO: this should really come from objectElement.GetOwnerID().  It not safe to assume that this class and its child have the same owner

            return spawnedDataModel;
        }
    }
}