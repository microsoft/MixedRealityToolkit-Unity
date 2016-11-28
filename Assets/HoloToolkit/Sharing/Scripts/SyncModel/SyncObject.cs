//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

#if UNITY_WINRT && !UNITY_EDITOR
#define USE_WINRT
#endif

using System.Collections.Generic;
using System;
using System.Reflection;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace HoloToolkit.Sharing.SyncModel
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

        // This is the sync element for this object.
        private ObjectElement internalObjectElement;
        public ObjectElement Element
        {
            get { return this.internalObjectElement; }
            internal set
            {
                if (this.internalObjectElement == null)
                {
                    this.internalObjectElement = value;
                    this.NetworkElement = value;

                    if (this.internalObjectElement != null)
                    {
                        CreateSyncListener(this.internalObjectElement);
                    }
                }
            }
        }

        public override object RawValue
        {
            get { return null; }
        }

        // Optional user that owns this object.  
        private HoloToolkit.Sharing.User owner;
        public HoloToolkit.Sharing.User Owner
        {
            get { return owner; }
            set
            {
                if (this.owner == null)
                {
                    this.owner = value;
                }
            }
        }

        public string ObjectType
        {
            get
            {
                if (this.internalObjectElement != null)
                    return this.internalObjectElement.GetObjectType().GetString();
                else
                    return null;
            }
        }

        // Returns the object element registered owner id
        public int OwnerId
        {
            get { return this.internalObjectElement != null ? this.internalObjectElement.GetOwnerID() : int.MaxValue; }
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

        // Returns a list of all child primitives
        public SyncPrimitive[] GetChildren()
        {
            return primitives.ToArray();
        }

        private void InitializePrimitives()
        {
            this.primitiveMap = new Dictionary<long, SyncPrimitive>();
            this.primitives = new List<SyncPrimitive>();

            // Scan the type of object this is a look for the SyncDataAttribute
            System.Type baseType = this.GetType();

#if USE_WINRT
            var typeFields = baseType.GetRuntimeFields();
#else
            var typeFields = baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif

            foreach (FieldInfo typeField in typeFields)
            {
                SyncDataAttribute attribute = null;

#if USE_WINRT
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
                    System.Type fieldType = typeField.FieldType;
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
                            bool hasConstructor = fieldType.GetConstructor(new Type[] { typeof(string) }) != null;
                            if (hasConstructor)
                            {
                                dataPrimitive = (SyncPrimitive)Activator.CreateInstance(fieldType, new object[] { memberName });
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

        // Register a new data model primitive as part of this object.  Can be called multiple times on the same child.
        protected void AddChild(SyncPrimitive data)
        {
            if (data.HasNetworkElement)
            {
                long guid = data.Guid;
                Assert.AreNotEqual(SharingClient.kInvalidXGuid, guid, "A primitive GUID should never be invalid if it is networked.");
                this.primitiveMap.Add(guid, data);
            }

            if (!this.primitives.Contains(data))
            {
                this.primitives.Add(data);
            }
        }

        // Remove a child primitive that belongs to this object
        protected void RemoveChild(SyncPrimitive data)
        {
            // Manually remove from maps
            if (this.primitives.Remove(data) && data.HasNetworkElement)
            {
                this.primitiveMap.Remove(data.NetworkElement.GetGUID());

                // Object has been removed internally, notify network
                ObjectElement parentElement = ObjectElement.Cast(data.NetworkElement.GetParent());
                if (parentElement != null)
                {
                    parentElement.RemoveElement(data.NetworkElement);
                }
            }
        }

        // Handler for if a child is added
        protected virtual void OnElementAdded(Element element)
        {
            // Find the existing element.
            bool primitiveDataChanged = false;
            for (int i = 0; i < this.primitives.Count; i++)
            {
                if (this.primitives[i].XStringFieldName.IsEqual(element.GetName()))
                {
                    // Found it, register the element so it knows where to pull data from
                    this.primitives[i].AddFromRemote(element);

                    // Update the internal map
                    long guid = element.GetGUID();
                    this.primitiveMap[guid] = this.primitives[i];
                    primitiveDataChanged = true;
                    break;
                }
            }

            if (primitiveDataChanged)
            {
                // If there are no more primitives waiting to be added, finish the initialization
                if (this.primitiveMap.Count == this.primitives.Count)
                {
                    if (this.InitializationComplete != null)
                    {
                        this.InitializationComplete(this);
                    }
                }
            }
        }

        // Handler for if a child is deleted
        protected virtual void OnElementDeleted(Element element)
        {
            // If the child exists, then remove it.
            long guid = element.GetGUID();
            if (this.primitiveMap.ContainsKey(guid))
            {
                RemoveChild(this.primitiveMap[guid]);
            }
        }

        // Handler for if a child bool changes
        protected virtual void OnBoolElementChanged(long elementID, bool newValue)
        {
            if (this.primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = this.primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(), typeof(bool));
            }
        }

        // Handler for if a child int changes
        protected virtual void OnIntElementChanged(long elementID, int newValue)
        {
            if (this.primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = this.primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(), typeof(int));
            }
        }

        // Handler for if a child long changes
        protected virtual void OnLongElementChanged(long elementID, long newValue)
        {
            if (this.primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = this.primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(), typeof(long));
            }
        }

        // Handler for if a child float changes
        protected virtual void OnFloatElementChanged(long elementID, float newValue)
        {
            if (this.primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = this.primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(), typeof(float));
            }
        }

        // Handler for if a child double changes
        protected virtual void OnDoubleElementChanged(long elementID, double newValue)
        {
            if (this.primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = this.primitiveMap[elementID];
                primitive.UpdateFromRemote(newValue);
                NotifyPrimitiveChanged(primitive);
            }
            else
            {
                LogUnknownElement(elementID.ToString(), newValue.ToString(), typeof(double));
            }
        }

        // Handler for if a child string changes
        protected virtual void OnStringElementChanged(long elementID, XString newValue)
        {
            if (this.primitiveMap.ContainsKey(elementID))
            {
                SyncPrimitive primitive = this.primitiveMap[elementID];
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
            this.syncListener = new ObjectElementAdapter();
            this.syncListener.ElementAddedEvent += OnElementAdded;
            this.syncListener.ElementDeletedEvent += OnElementDeleted;
            this.syncListener.BoolChangedEvent += OnBoolElementChanged;
            this.syncListener.IntChangedEvent += OnIntElementChanged;
            this.syncListener.LongChangedEvent += OnLongElementChanged;
            this.syncListener.FloatChangedEvent += OnFloatElementChanged;
            this.syncListener.DoubleChangedEvent += OnDoubleElementChanged;
            this.syncListener.StringChangedEvent += OnStringElementChanged;
            element.AddListener(this.syncListener);
        }

        // From SyncPrimitive

        /// <summary>
        /// Initializes this object for local use.  Doesn't wait for network initialization. 
        /// </summary>
        /// <param name="parentElement">Parent element of this SyncObject.</param>
        public override void InitializeLocal(ObjectElement parentElement)
        {
            // Auto create element if needed
            if (this.Element == null)
            {
                this.Element = parentElement.CreateObjectElement(XStringFieldName, GetType().FullName, this.Owner);
                this.NetworkElement = this.Element;
            }

            // Initialize all primitives
            for (int i = 0; i < this.primitives.Count; i++)
            {
                SyncPrimitive data = this.primitives[i];
                data.InitializeLocal(Element);
                this.primitiveMap[data.Guid] = data;
            }

            // Complete the initialization
            if (this.InitializationComplete != null)
            {
                this.InitializationComplete(this);
            }
        }

        public override void AddFromRemote(Element element)
        {
            this.Element = ObjectElement.Cast(element);
            this.NetworkElement = element;
        }
    }
}