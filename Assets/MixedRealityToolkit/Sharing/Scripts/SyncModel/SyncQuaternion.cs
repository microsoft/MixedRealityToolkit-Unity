// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Sharing.SyncModel
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

#if UNITY_EDITOR
        public override object RawValue
        {
            get { return Value; }
        }
#endif

        public Quaternion Value
        {
            get { return new Quaternion(x.Value, y.Value, z.Value, w.Value); }
            set
            {
                x.Value = value.x;
                y.Value = value.y;
                z.Value = value.z;
                w.Value = value.w;
            }
        }

        public SyncQuaternion(string field) : base(field) { }
    }
}
