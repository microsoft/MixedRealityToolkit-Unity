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

#if UNITY_EDITOR
        public override object RawValue
        {
            get { return value; }
        }
#endif

        public double Value
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
                        element.SetValue(value);
                    }
                }
            }
        }

        public SyncDouble(string field) : base(field) { }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            element = parentElement.CreateDoubleElement(XStringFieldName, value);
            NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, double localValue)
        {
            InitializeLocal(parentElement);
            Value = localValue;
        }

        public override void AddFromRemote(Element remoteElement)
        {
            NetworkElement = remoteElement;
            element = DoubleElement.Cast(remoteElement);
            value = element.GetValue();
        }

        public override void UpdateFromRemote(double remoteValue)
        {
            value = remoteValue;
        }
    }
}