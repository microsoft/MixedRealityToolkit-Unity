//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// Base primitive used to define an element within the data model.
    /// The primitive is defined by a field and a value.
    /// </summary>
    abstract public class SyncPrimitive
    {
        protected string fieldName = null;
        private XString xStringFieldName = null;
        protected Element internalElement = null;

        // Unique identifier for primitive.  Returns kInvalidXGuid if uninitialized.
        public long Guid
        {
            get
            {
                if (this.internalElement != null)
                {
                    return this.internalElement.GetGUID();
                }
                else
                {
                    return SharingClient.kInvalidXGuid;
                }
            }
        }

        public virtual Element NetworkElement
        {
            get { return internalElement; }
            protected set { internalElement = value; }
        }

        // Indicates if the primitive has a network element.  The primitive can only be modified if this returns true.
        public bool HasNetworkElement
        {
            get { return (this.internalElement != null); }
        }

        // The field name of the primitive
        public XString XStringFieldName
        {
            get { return this.xStringFieldName; }
        }

        // The field name of the primitive
        public string FieldName
        {
            get { return fieldName; }

            set
            {
                this.fieldName = value;
                this.xStringFieldName = new XString(value);
            }
        }

        // Returns the raw boxed object this primitive holds
        public abstract object RawValue
        {
            get;
        }

        public SyncPrimitive(string field)
        {
            FieldName = field;
        }

        // Initializes this object for local use.  Doesn't wait for network initialization.
        abstract public void InitializeLocal(ObjectElement parentElement);

        // Called when being remotely initialized
        abstract public void AddFromRemote(Element element);

        // Called when the primitive value has changed from a remote action
        virtual public void UpdateFromRemote(XString value) { }
        virtual public void UpdateFromRemote(float value) { }
        virtual public void UpdateFromRemote(double value) { }
        virtual public void UpdateFromRemote(int value) { }
        virtual public void UpdateFromRemote(long value) { }
        virtual public void UpdateFromRemote(bool value) { }
    }
}