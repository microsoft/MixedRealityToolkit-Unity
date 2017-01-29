//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// Base primitive used to define an element within the data model.
    /// The primitive is defined by a field and a value.
    /// </summary>
    public abstract class SyncPrimitive
    {
        protected string fieldName;
        private XString xStringFieldName;
        protected Element internalElement;

        /// <summary>
        /// Unique identifier for primitive.  Returns kInvalidXGuid if uninitialized.
        /// </summary>
        public long Guid
        {
            get
            {
                return internalElement != null ? internalElement.GetGUID() : SharingClient.kInvalidXGuid;
            }
        }

        public virtual Element NetworkElement
        {
            get { return internalElement; }
            protected set { internalElement = value; }
        }

        /// <summary>
        /// Indicates if the primitive has a network element.
        /// The primitive can only be modified if this returns true.
        /// </summary>
        public bool HasNetworkElement
        {
            get { return internalElement != null; }
        }

        /// <summary>
        /// The field name of the primitive
        /// </summary>
        public XString XStringFieldName
        {
            get { return xStringFieldName; }
        }

        /// <summary>
        /// The field name of the primitive
        /// </summary>
        public string FieldName
        {
            get { return fieldName; }

            set
            {
                fieldName = value;
                xStringFieldName = new XString(value);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns the raw boxed object this primitive holds.
        /// Used by SharingStageEditor.cs
        /// </summary>
        public abstract object RawValue
        {
            get;
        }
#endif

        public SyncPrimitive(string field)
        {
            FieldName = field;
        }

        // Initializes this object for local use.  Doesn't wait for network initialization.
        public abstract void InitializeLocal(ObjectElement parentElement);

        // Called when being remotely initialized
        public abstract void AddFromRemote(Element remoteElement);

        // Called when the primitive value has changed from a remote action
        public virtual void UpdateFromRemote(XString remoteValue) { }
        public virtual void UpdateFromRemote(float remoteValue) { }
        public virtual void UpdateFromRemote(double remoteValue) { }
        public virtual void UpdateFromRemote(int remoteValue) { }
        public virtual void UpdateFromRemote(long remoteValue) { }
        public virtual void UpdateFromRemote(bool remoteValue) { }
    }
}