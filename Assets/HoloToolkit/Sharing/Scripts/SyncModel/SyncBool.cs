//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the boolean primitive for the syncing system.
    /// It does the heavy lifting to make adding new bools to a class easy.
    /// </summary>
    public class SyncBool : SyncPrimitive
    {
        private BoolElement element;
        private bool value;

        public override object RawValue
        {
            get { return this.value; }
        }

        public bool Value
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

        public SyncBool(string field)
            : base(field)
        {
        }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            this.element = parentElement.CreateBoolElement(XStringFieldName, this.value);
            this.NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, bool value)
        {
            InitializeLocal(parentElement);
            this.Value = value;
        }

        public override void AddFromRemote(Element element)
        {
            this.NetworkElement = element;
            this.element = BoolElement.Cast(element);
            this.value = this.element.GetValue();
        }

        public override void UpdateFromRemote(bool value)
        {
            // Change the value
            this.value = value;
        }
    }
}