// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// The SyncArray class provides the functionality of an array in the data model.
    /// The array holds entire objects, not primitives, since each object is indexed by unique name.
    /// Note that this array is unordered.
    /// </summary>
    /// <typeparam name="T">Type of SyncObject in the array.</typeparam>
    public class SyncArray<T> : SyncObject, IEnumerable<T> where T : SyncObject, new()
    {
        /// <summary>
        /// Called when a new object has been added to the array.
        /// </summary>
        public event Action<T> ObjectAdded;

        /// <summary>
        /// Called when an existing object has been removed from the array
        /// </summary>
        public event Action<T> ObjectRemoved;

        /// <summary>
        /// // Maps the unique id to object
        /// </summary>
        private readonly Dictionary<string, T> dataArray;

        /// <summary>
        /// Type of objects in the array.
        /// This is cached so that we don't have to call typeof(T) more than once.
        /// </summary>
        protected readonly Type arrayType;

        public SyncArray(string field)
            : base(field)
        {
            dataArray = new Dictionary<string, T>();
            arrayType = typeof(T);
        }

        public SyncArray() : this(string.Empty) { }

        /// <summary>
        /// Creates the object in the array, based on its underlying object element that came from the sync system.
        /// </summary>
        /// <param name="objectElement">Object element on which the data model object is based.</param>
        /// <returns>The created data model object of the appropriate type.</returns>
        protected virtual T CreateObject(ObjectElement objectElement)
        {
            Type objectType = SyncSettings.Instance.GetDataModelType(objectElement.GetObjectType()).AsType();
            if (!objectType.IsSubclassOf(arrayType) && objectType != arrayType)
            {
                throw new InvalidCastException(string.Format("Object of incorrect type added to SyncArray: Expected {0}, got {1} ", objectType, objectElement.GetObjectType().GetString()));
            }

            object createdObject = Activator.CreateInstance(objectType);

            T spawnedDataModel = (T)createdObject;
            spawnedDataModel.Element = objectElement;
            spawnedDataModel.FieldName = objectElement.GetName();

            // TODO: this should not query SharingStage, but instead query the underlying session layer
            spawnedDataModel.Owner = SharingStage.Instance.SessionUsersTracker.GetUserById(objectElement.GetOwnerID());

            return spawnedDataModel;
        }

        /// <summary>
        /// Adds a new entry into the array.
        /// </summary>
        /// <param name="newSyncObject">New object to add.</param>
        /// <param name="owner">Owner the object. Set to null if the object has no owner.</param>
        /// <returns>Object that was added, with its networking elements setup.</returns>
        public T AddObject(T newSyncObject, User owner = null)
        {
            // Create our object element for our target
            string id = System.Guid.NewGuid().ToString();
            string dataModelName = SyncSettings.Instance.GetDataModelName(newSyncObject.GetType());
            ObjectElement existingElement = Element.CreateObjectElement(new XString(id), dataModelName, owner);

            // Create a new object and assign the element
            newSyncObject.Element = existingElement;
            newSyncObject.FieldName = id;
            newSyncObject.Owner = owner;

            // Register the child with the object
            AddChild(newSyncObject);

            // Update internal map
            dataArray[id] = newSyncObject;

            // Initialize it so it can be used immediately.
            newSyncObject.InitializeLocal(Element);

            // Notify listeners that an object was added.
            if (ObjectAdded != null)
            {
                ObjectAdded(newSyncObject);
            }

            return newSyncObject;
        }

        /// <summary>
        /// Removes an entry from the array
        /// </summary>
        /// <param name="existingObject">Object to remove.</param>
        /// <returns>True if removal succeeded, false if not.</returns>
        public bool RemoveObject(T existingObject)
        {
            bool success = false;
            if (existingObject != null)
            {
                string uniqueName = existingObject.Element.GetName();
                if (dataArray.Remove(uniqueName))
                {
                    RemoveChild(existingObject);

                    if (ObjectRemoved != null)
                    {
                        ObjectRemoved(existingObject);
                    }

                    success = true;
                }
            }

            return success;
        }

        // Returns a full list of the objects
        public T[] GetDataArray()
        {
            var childrenList = new List<T>(dataArray.Count);
            foreach (KeyValuePair<string, T> pair in dataArray)
            {
                childrenList.Add(pair.Value);
            }

            return childrenList.ToArray();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return dataArray.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dataArray.Values.GetEnumerator();
        }

        public void Clear()
        {
            // TODO: We need a way of resetting in a consistent way across all object types and hierarchies.
            T[] array = GetDataArray();
            for (int i = 0; i < array.Length; i++)
            {
                RemoveObject(array[i]);
            }
        }

        protected override void OnElementAdded(Element element)
        {
            if (element.GetElementType() == ElementType.ObjectType)
            {
                // Add the new object and listen for when the initialization has fully completed since it can take time.
                T newObject = AddObject(ObjectElement.Cast(element));
                newObject.InitializationComplete += OnInitializationComplete;
            }
            else
            {
                Debug.LogError("Error: Adding unknown element to SyncArray<T>");
            }

            base.OnElementAdded(element);
        }

        protected override void OnElementDeleted(Element element)
        {
            base.OnElementDeleted(element);

            string uniqueName = element.GetName();
            if (dataArray.ContainsKey(uniqueName))
            {
                T obj = dataArray[uniqueName];
                RemoveObject(obj);
            }
        }

        private void OnInitializationComplete(SyncObject obj)
        {
            // Notify listeners know that an object was added
            if (ObjectAdded != null)
            {
                ObjectAdded(obj as T);
            }
        }

        /// <summary>
        /// Adds a new entry into the array.
        /// </summary>
        /// <param name="existingElement">Element from which the object should be created.</param>
        /// <returns></returns>
        private T AddObject(ObjectElement existingElement)
        {
            string id = existingElement.GetName();

            // Create a new object and assign the element
            T newObject = CreateObject(existingElement);

            // Register the child with the object
            AddChild(newObject);

            // Update internal map
            dataArray[id] = newObject;

            return newObject;
        }
    }
}