// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the long primitive for the syncing system.
    /// It does the heavy lifting to make adding new longs to a class easy.
    /// </summary>
    public class SyncLong : SyncPrimitive
    {
        private LongElement element;
        private long value;

#if UNITY_EDITOR
        public override object RawValue
        {
            get { return value; }
        }
#endif

        public long Value
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

        public SyncLong(string field)
            : base(field)
        {
        }

        public override void InitializeLocal(ObjectElement parentElement)
        {
            element = parentElement.CreateLongElement(XStringFieldName, value);
            NetworkElement = element;
        }

        public void AddFromLocal(ObjectElement parentElement, long localValue)
        {
            InitializeLocal(parentElement);
            Value = localValue;
        }

        public override void AddFromRemote(Element remoteElement)
        {
            NetworkElement = remoteElement;
            element = LongElement.Cast(remoteElement);
            value = element.GetValue();
        }

        public override void UpdateFromRemote(long remoteValue)
        {
            value = remoteValue;
        }
    }
}