//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the double primitive for the syncing system.
    /// It does the heavy lifting to make adding new doubles to a class easy.
    /// </summary>
    public class SyncDouble : SyncPrimitive
    {
        private DoubleElement element;
        private double value;

        public override object RawValue
        {
            get { return this.value; }
        }

        public double Value
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
                        this.element.SetValue(value);
                    }
                }
            }
        }

        public SyncDouble(string field)
            : base(field)
        {
        }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            this.element = parentElement.CreateDoubleElement(XStringFieldName, this.value);
            this.NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, double value)
        {
            InitializeLocal(parentElement);
            this.Value = value;
        }

        public override void AddFromRemote(Element element)
        {
            this.NetworkElement = element;
            this.element = DoubleElement.Cast(element);
            this.value = this.element.GetValue();
        }

        public override void UpdateFromRemote(double value)
        {
            // Change the value
            this.value = value;
        }
    }
}