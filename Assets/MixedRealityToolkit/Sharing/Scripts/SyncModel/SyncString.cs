// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the string primitive for the syncing system. 
    /// It does the heavy lifting to make adding new strings to a class easy.
    /// </summary>
    public class SyncString : SyncPrimitive
    {
        private StringElement element;
        private string value = "";

#if UNITY_EDITOR
        public override object RawValue
        {
            get { return value; }
        }
#endif

        public string Value
        {
            get { return value; }

            set
            {
                // Has the value actually changed?
                if (this.value != value)
                {
                    // Change the value
                    this.value = value;

                    if (element != null)
                    {
                        // Notify network that the value has changed
                        element.SetValue(new XString(value));
                    }
                }
            }
        }

        public SyncString(string field) : base(field) { }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            element = parentElement.CreateStringElement(XStringFieldName, new XString(value));
            NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, string localValue)
        {
            InitializeLocal(parentElement);
            Value = localValue;
        }

        public override void AddFromRemote(Element remoteElement)
        {
            NetworkElement = remoteElement;
            element = StringElement.Cast(remoteElement);
            value = element.GetValue().GetString();
        }

        public override void UpdateFromRemote(XString remoteValue)
        {
            value = remoteValue.GetString();
        }
    }
}