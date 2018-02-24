// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace MixedRealityToolkit.Sharing.SyncModel
{
    /// <summary>
    /// The SyncObject class is a container object that can hold multiple SyncPrimitives.
    /// </summary>
    public class SyncObject : SyncPrimitive
    {
        private ObjectElementAdapter syncListener;                 // The sync object that we use to get sync events for this element
        private Dictionary<long, SyncPrimitive> primitiveMap;      // A dictionary of all of the primitives that are children of this object.  Maps element GUID to data
        private List<SyncPrimitive> primitives;                    // Raw list of child primitives

        public event Action<SyncObject> ObjectChanged;             // Invoked when one of the child primitives has changed
        public event Action<SyncObject> InitializationComplete;    // Invoked when this object has been fully initialized

        private ObjectElement internalObjectElement;
        /// <summary>
        /// This is the sync element for this object.
        /// </summary>
        public ObjectElement Element
        {
            get { return internalObjectElement; }
            internal set
            {
                if (internalObjectElement == null)
                {
                    internalObjectElement = value;
                    NetworkElement = value;

                    if (internalObjectElement != null)
                    {
                        CreateSyncListener(internalObjectElement);
                    }
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Raw Value used when displaying sync objects in the inspector.
        /// </summary>
        public override object RawValue
        {
            get { return null; }
        }
#endif

        private User owner;
        /// <summary>
        /// Optional user that owns this object.
        /// </summary>
        public User Owner
        {
            get { return owner; }
            set
            {
                if (owner == null)
                {
                    owner = value;
                }
            }
        }

        /// <summary>
        /// The Object Type as string.
        /// </summary>
        public string ObjectType
        {
            get
            {
                return internalObjectElement != null ? internalObjectElement.GetObjectType().GetString() : null;
            }
        }

        /// <summary>
        /// Returns the object element registered owner id.
        /// </summary>
        public int OwnerId
        {
            get { return internalObjectElement != null ? internalObjectElement.GetOwnerID() : int.MaxValue; }
        }

        public SyncObject(string field)
            : base(field)
        {
            InitializePrimitives();
        }

        public SyncObject()
            : base(string.Empty)
        {
            InitializePrimitives();
        }

        /// <summary>
        /// Returns a list of all child primitives.
        /// </summary>
        /// <returns>Array of SyncPrimitives.</returns>
        public SyncPrimitive[] GetChildren()
        {
            return primitives.ToArray();
        }

        private void InitializePrimitives()
        {
            primitiveMap = new Dictionary<long, SyncPrimitive>();
            primitives = new List<SyncPrimitive>();

            // Scan the type of object this is a look for the SyncDataAttribute
            Type baseType = GetType();

#if WINDOWS_UWP
            var typeFields = baseType.GetRuntimeFields();
#else
            var typeFields = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif

            foreach (FieldInfo typeField in typeFields)
            {
                SyncDataAttribute attribute = null;

#if WINDOWS_UWP
                attribute = typeField.GetCustomAttribute<SyncDataAttribute>(true);
#else
                object[] customAttributes = typeField.GetCustomAttributes(typeof(SyncDataAttribute), true);
                if (customAttributes.Length > 0)
                {
                    attribute = customAttributes[0] as SyncDataAttribute;
                }
#endif

                if (attribute != null)
                {
                    Type fieldType = typeField.FieldType;
                    string memberName = typeField.Name;

                    // Override the member name if provided
                    if (!string.IsNullOrEmpty(attribute.CustomFieldName))
                    {
                        memberName = attribute.CustomFieldName;
                    }

                    // Auto instantiate the primitive if it doesn't already exist
                    SyncPrimitive dataPrimitive = typeField.GetValue(this) as SyncPrimitive;
                    if (dataPrimitive == null)
                    {
                        try
                        {
                            // Constructors are not inherited, as per Section 1.6.7.1 of the C# Language Specification.
                            // This means that if a class subclasses Object or Primitive, they must either declare a constructor
                            // that takes the "memberName" property or use the default (parameter less constructor).

                            // First check if there is a constructor that takes the member name and if so call it
                            bool hasConstructor = fieldType.GetConstructor(new[] { typeof(string) }) != null;
                            if (hasConstructor)
                            {
                                dataPrimitive = (SyncPrimitive)Activator.CreateInstance(fieldType, memberName);
                            }
                            else
                            {
                                // Fallback on using the default constructor and manually assign the member name
                                dataPrimitive = (SyncPrimitive)Activator.CreateInstance(fieldType, null);
                                dataPrimitive.FieldName = memberName;
                            }

                            typeField.SetValue(this, dataPrimitive);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarningFormat("Unable to create SyncPrimitive of type {0}.  Exception: {1}", memberName, ex);
                        }
                    }

                    if (dataPrimitive != null)
                    {
                        // Register the child
                        AddChild(dataPrimitive);
                    }
                }
            }
        }

        /// <summary>
        /// Register a new data model primitive as part of this object.  Can be called multiple times on the same child.
        /// </summary>
        /// <param name="data">Sync Primitive Data</param>
        protected void AddChild(SyncPrimitive data)
        {
            if (data.HasNetworkElement)
            {
                long guid = data.Guid;
                Assert.AreNotEqual(SharingClient.kInvalidXGuid, guid, "A primitive GUID should never be invalid if it is networked.");
                primitiveMap.Add(guid, data);
            }

            if (!primitives.Contains(data))
            {
                primitives.Add(data);
            }
        }

        /// <summary>
        /// Remove a child primitive that belongs to this object.
        /// </summary>
        /// <param name="data">Sync Primitive Data</param>
        protected void RemoveChild(SyncPrimitive data)
        {
            // Manually remove from maps
            if (primitives.Remove(data) && data.HasNetworkElement)
            {
                primitiveMap.Remove(data.NetworkElement.GetGUID());

                // Object has been removed internally, notify network
                ObjectElement parentElement = ObjectElement.Cast(data.NetworkElement.GetParent());
                if (parentElement != null)
                {
                    parentElement.RemoveElement(data.NetworkElement);
                }
            }
        }

        /// <summary>
        /// Handler for if a child is added.
        /// </summary>
        /// <param name="element">Added Element</param>
        protected virtual void OnElementAdded(Element element)
        {
            // Find the existing element.
            bool primitiveDataChanged = false;
            for (int i = 0; i < primitives.Count; i++)
            {
                if (primitives[i].XStringFieldName.IsEqual(element.GetName()))
                {
                    // Found it, register the element so it knows where to pull data from
                    primitives[i].AddFromRemote(element);

                    // Update the internal map
                    long guid = element.GetGUID();
                    primitiveMap[guid] = primitives[i];
                    primitiveDataChanged = true;
                    break;
                }
            }

            if (primitiveDataChanged)
            {
                // If there are no more primitives waiting to be added, finish the initialization
                if (primitiveMap.Count == primitives.Count)
                {
                    if (InitializationComplete != null)
                    {
                        InitializationComplete(this);
                    }
                }
            }
        }

        /// <summary>
        /// Handler for if a child is deleted.
        /// </summary>
        /// <param name="element">Deleted Element</param>
        protected virtual void OnElementDeleted(Element element)
        {
            // If the child exists, then remove it.
            long guid = element.GetGUID();
            if (primitiveMap.ContainsKey(guid))
            {
                RemoveChild(primitiveMap[guid]);
            }
        }

        /// <summary>
        /// Handler for if a child bool changes.
        /// </summary>
        /// <param name="elementID">Element Id</param>
        /// <param name="newValue">New Value</param>
        protected virtual void OnBoolElementChanged(long elementID, bool newValue)
        {
            if (primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(), typeof(bool));
            }
        }

        /// <summary>
        /// Handler for if a child int changes.
        /// </summary>
        /// <param name="elementID">Element Id</param>
        /// <param name="newValue">New Value</param>
        protected virtual void OnIntElementChanged(long elementID, int newValue)
        {
            if (primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(), typeof(int));
            }
        }

        /// <summary>
        /// Handler for if a child long changes
        /// </summary>
        /// <param name="elementID">Element Id</param>
        /// <param name="newValue">New Value</param>
        protected virtual void OnLongElementChanged(long elementID, long newValue)
        {
            if (primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(), typeof(long));
            }
        }

        /// <summary>
        /// Handler for if a child float changes
        /// </summary>
        /// <param name="elementID">Element Id</param>
        /// <param name="newValue">New Value</param>
        protected virtual void OnFloatElementChanged(long elementID, float newValue)
        {
            if (primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(CultureInfo.InvariantCulture), typeof(float));
            }
        }

        /// <summary>
        /// Handler for if a child double changes
        /// </summary>
        /// <param name="elementID">Element Id</param>
        /// <param name="newValue">New Value</param>
        protected virtual void OnDoubleElementChanged(long elementID, double newValue)
        {
            if (primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(CultureInfo.InvariantCulture), typeof(double));
            }
        }

        /// <summary>
        /// Handler for if a child string changes
        /// </summary>
        /// <param name="elementID">Element Id</param>
        /// <param name="newValue">New Value</param>
        protected virtual void OnStringElementChanged(long elementID, XString newValue)
        {
            if (primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.GetString(), typeof(string));
            }
        }

        private void LogUnknownElement(string elementName, string elementValue, Type elementType)
        {
            Debug.LogWarningFormat("Error: Trying to update an unknown child element!  Discarding update.  Type: {0}, Value: {1}, Id: {2}", elementType, elementValue, elementName);
        }

        protected virtual void NotifyPrimitiveChanged(SyncPrimitive primitive)
        {
            ObjectChanged.RaiseEvent(this);
        }

        private void CreateSyncListener(ObjectElement element)
        {
            // Create a listener for this
            syncListener = new ObjectElementAdapter();
            syncListener.ElementAddedEvent += OnElementAdded;
            syncListener.ElementDeletedEvent += OnElementDeleted;
            syncListener.BoolChangedEvent += OnBoolElementChanged;
            syncListener.IntChangedEvent += OnIntElementChanged;
            syncListener.LongChangedEvent += OnLongElementChanged;
            syncListener.FloatChangedEvent += OnFloatElementChanged;
            syncListener.DoubleChangedEvent += OnDoubleElementChanged;
            syncListener.StringChangedEvent += OnStringElementChanged;
            element.AddListener(syncListener);
        }

        #region From SyncPrimitive

        /// <summary>
        /// Initializes this object for local use.  Doesn't wait for network initialization. 
        /// </summary>
        /// <param name="parentElement">Parent element of this SyncObject.</param>
        public override void InitializeLocal(ObjectElement parentElement)
        {
            // Auto create element if needed
            if (Element == null)
            {
                Element = parentElement.CreateObjectElement(XStringFieldName, GetType().FullName, Owner);
                NetworkElement = Element;
            }

            // Initialize all primitives
            for (int i = 0; i < primitives.Count; i++)
            {
                SyncPrimitive data = primitives[i];
                data.InitializeLocal(Element);
                primitiveMap[data.Guid] = data;
            }

            // Complete the initialization
            if (InitializationComplete != null)
            {
                InitializationComplete(this);
            }
        }

        public override void AddFromRemote(Element remoteElement)
        {
            Element = ObjectElement.Cast(remoteElement);
            NetworkElement = remoteElement;
        }

        #endregion //From SyncPrimitive
    }
}