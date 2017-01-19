//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the integer primitive for the syncing system.
    /// It does the heavy lifting to make adding new integers to a class easy.
    /// </summary>
    public class SyncInteger : SyncPrimitive
    {
        private IntElement element;
        private int value;

        public override object RawValue
        {
            get { return this.value; }
        }

        public int Value
        {
            get { return this.value; }

            set
            {
                if (this.value != value)
                {
                    // Change the value
                    this.value = value;

                    if (this.element != null)
                    {
                        // Notify network that the value has changed
                        this.element.SetValue(value);
                    }
                }
            }
        }

        public SyncInteger(string field)
            : base(field)
        {
        }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            this.element = parentElement.CreateIntElement(XStringFieldName, this.value);
            this.NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, int value)
        {
            InitializeLocal(parentElement);
            this.Value = value;
        }

        public override void AddFromRemote(Element element)
        {
            this.NetworkElement = element;
            this.element = IntElement.Cast(element);
            this.value = this.element.GetValue();
        }

        public override void UpdateFromRemote(int value)
        {
            // Change the value
            this.value = value;
        }
    }
}