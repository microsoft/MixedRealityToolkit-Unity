//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the float primitive for the syncing system.
    /// It does the heavy lifting to make adding new floats to a class easy.
    /// </summary>
    public class SyncFloat : SyncPrimitive
    {
        private FloatElement element;
        private float value;

#if UNITY_EDITOR
        public override object RawValue
        {
            get { return value; }
        }
#endif

        public float Value
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

        public SyncFloat(string field) : base(field) { }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            element = parentElement.CreateFloatElement(XStringFieldName, value);
            NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, float localValue)
        {
            InitializeLocal(parentElement);
            Value = localValue;
        }

        public override void AddFromRemote(Element remoteElement)
        {
            NetworkElement = remoteElement;
            element = FloatElement.Cast(remoteElement);
            value = element.GetValue();
        }

        public override void UpdateFromRemote(float remoteValue)
        {
            // Change the value
            value = remoteValue;
        }
    }
}