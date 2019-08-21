// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Add a NearInteractionTouchableVolume to your scene and configure a touchable volume
    /// in order to get PointerDown and PointerUp events whenever a PokePointer collides with this volume.
    /// </summary>
    public class NearInteractionTouchableVolume : ColliderNearInteractionTouchable
    {
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(NearInteractionTouchableVolume))]
        public class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                NearInteractionTouchableVolume t = (NearInteractionTouchableVolume)target;
                Collider c = t.GetComponent<Collider>();
                if (c == null)
                {
                    UnityEditor.EditorGUILayout.HelpBox("A collider is required in order to compute the touchable volume.", UnityEditor.MessageType.Warning);
                }
            }
        }
#endif

        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            Vector3 closest = TouchableCollider.ClosestPoint(samplePoint);

            normal = (samplePoint - closest);
            if (normal == Vector3.zero)
            {
                // inside object, use vector to centre as normal
                normal = samplePoint - transform.TransformVector(TouchableCollider.bounds.center);
                normal.Normalize();
                return 0;
            }
            else
            {
                float dist = normal.magnitude;
                normal.Normalize();
                return dist;
            }
        }
    }
}