//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;

namespace HoloToolkit.Sharing.SyncModel
{
    /// <summary>
    /// This class implements the Vector3 object primitive for the syncing system.
    /// It does the heavy lifting to make adding new Vector3 to a class easy.
    /// </summary>
    public class SyncVector3 : SyncObject
    {
        [SyncData] private SyncFloat x = null;
        [SyncData] private SyncFloat y = null;
        [SyncData] private SyncFloat z = null;

        public override object RawValue
        {
            get { return new Vector3(this.x.Value, this.y.Value, this.z.Value); }
        }

        public Vector3 Value
        {
            get { return new Vector3(this.x.Value, this.y.Value, this.z.Value); }
            set
            {
                this.x.Value = value.x;
                this.y.Value = value.y;
                this.z.Value = value.z;
            }
        }

        public SyncVector3(string field)
            : base(field)
        {
        }
    }
}
