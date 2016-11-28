//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine;

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the Quaternion object primitive for the syncing system.
    /// It does the heavy lifting to make adding new Quaternion to a class easy.
    /// </summary>
    public class SyncQuaternion : SyncObject
    {
        [SyncData] private SyncFloat x = null;
        [SyncData] private SyncFloat y = null;
        [SyncData] private SyncFloat z = null;
        [SyncData] private SyncFloat w = null;

        public override object RawValue
        {
            get { return new Quaternion(this.x.Value, this.y.Value, this.z.Value, this.w.Value); }
        }

        public Quaternion Value
        {
            get { return new Quaternion(this.x.Value, this.y.Value, this.z.Value, this.w.Value); }
            set
            {
                this.x.Value = value.x;
                this.y.Value = value.y;
                this.z.Value = value.z;
                this.w.Value = value.w;
            }
        }

        public SyncQuaternion(string field)
            : base(field)
        {
        }
    }
}
