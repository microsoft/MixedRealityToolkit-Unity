// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

#if UNITY_EDITOR
        public override object RawValue
        {
            get { return Value; }
        }
#endif

        public Vector3 Value
        {
            get { return new Vector3(x.Value, y.Value, z.Value); }
            set
            {
                x.Value = value.x;
                y.Value = value.y;
                z.Value = value.z;
            }
        }

        public SyncVector3(string field) : base(field) { }
    }
}
