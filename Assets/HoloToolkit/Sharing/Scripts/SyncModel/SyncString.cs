//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the string primitive for the syncing system. 
    /// It does the heavy lifting to make adding new strings to a class easy.
    /// </summary>
    public class SyncString : SyncPrimitive
    {
        private StringElement element;
        private string value = "";

        public override object RawValue
        {
            get { return this.value; }
        }

        public string Value
        {
            get { return this.value; }

            set
            {
                // Has the value actually changed?
                if (this.value != value)
                {
                    // Change the value
                    this.value = value;

                    if (this.element != null)
                    {
                        // Notify network that the value has changed
                        this.element.SetValue(new XString(value));
                    }
                }
            }
        }

        public SyncString(string field)
            : base(field)
        {
        }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            this.element = parentElement.CreateStringElement(XStringFieldName, new XString(this.value));
            this.NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, string value)
        {
            InitializeLocal(parentElement);
            Value = value;
        }

        override public void AddFromRemote(Element element)
        {
            this.NetworkElement = element;
            this.element = StringElement.Cast(element);
            this.value = this.element.GetValue().GetString();
        }

        override public void UpdateFromRemote(XString value)
        {
            // Change the value
            this.value = value.GetString();
        }
    }
}