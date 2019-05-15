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
    /// Add a NearInteractionTouchableUnboundedPlane to your scene and configure a touchable volume
    /// in order to get PointerDown and PointerUp events whenever a PokePointer touches the
    /// attached collider through this surface.
    /// </summary>
    public class NearInteractionTouchableUnboundedPlane : BaseNearInteractionTouchable
    {
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(NearInteractionTouchableUnboundedPlane))]
        public class Editor : UnityEditor.Editor
        {
            private readonly Color handleColor = Color.white;

            protected virtual void OnSceneGUI()
            {
                NearInteractionTouchableUnboundedPlane t = (NearInteractionTouchableUnboundedPlane)target;

                if (Event.current.type == EventType.Repaint)
                {
                    UnityEditor.Handles.color = handleColor;

                    Vector3 center = t.transform.TransformPoint(t.localPoint);

                    float arrowSize = UnityEditor.HandleUtility.GetHandleSize(center) * 0.75f;
                    UnityEditor.Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(t.transform.rotation * t.localNormal), arrowSize, EventType.Repaint);
                }
            }
        }
#endif

        /// <summary>
        /// Local space forward direction
        /// </summary>
        [SerializeField]
        protected Vector3 localNormal = Vector3.forward;

        /// <summary>
        /// Local space forward direction
        /// </summary>
        [SerializeField]
        protected Vector3 localPoint = Vector3.zero;

        protected void OnValidate()
        {
            localNormal = localNormal.normalized;

            touchableCollider = GetComponent<Collider>();
            usesCollider = touchableCollider != null;
        }

        public override float DistanceToSurface(Vector3 samplePoint, out Vector3 normal)
        {
            normal = transform.TransformDirection(localNormal);

            Vector3 point = samplePoint - transform.TransformPoint(localPoint);

            return Math.Abs(Vector3.Dot(point, normal));
        }
    }
}