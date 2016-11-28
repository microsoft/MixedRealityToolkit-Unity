//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
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

        public override object RawValue
        {
            get { return this.value; }
        }

        public float Value
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

        public SyncFloat(string field)
            : base(field)
        {
        }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            this.element = parentElement.CreateFloatElement(XStringFieldName, this.value);
            this.NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, float value)
        {
            InitializeLocal(parentElement);
            this.Value = value;
        }

        public override void AddFromRemote(Element element)
        {
            this.NetworkElement = element;
            this.element = FloatElement.Cast(element);
            this.value = this.element.GetValue();
        }

        public override void UpdateFromRemote(float value)
        {
            // Change the value
            this.value = value;
        }
    }
}