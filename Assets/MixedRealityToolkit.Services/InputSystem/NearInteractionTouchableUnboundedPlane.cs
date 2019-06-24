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
        private readonly Color handleColor = Color.white;

        [UnityEditor.CustomEditor(typeof(NearInteractionTouchableUnboundedPlane))]
        public class Editor : UnityEditor.Editor
        {

            protected virtual void OnSceneGUI()
            {
                NearInteractionTouchableUnboundedPlane t = (NearInteractionTouchableUnboundedPlane)target;

                if (Event.current.type == EventType.Repaint)
                {
                    UnityEditor.Handles.color = t.handleColor;
                    Vector3 center = t.transform.TransformPoint(t.localPoint);

                    float arrowSize = UnityEditor.HandleUtility.GetHandleSize(center) * 0.75f;
                    UnityEditor.Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(t.transform.rotation * t.localNormal), arrowSize, EventType.Repaint);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = handleColor;

            Vector3 center = transform.TransformPoint(localPoint);
            Vector3 forward = transform.TransformVector(localNormal).normalized;

            Vector3 cross = Vector3.Cross(forward, Vector3.up);

            Vector3 right = cross == Vector3.zero ? Vector3.right : cross.normalized;
            Vector3 up = Vector3.Cross(forward, right).normalized;
            
            Gizmos.DrawRay(center, right + up);
            Gizmos.DrawRay(center, right - up);
            Gizmos.DrawRay(center, -right + up);
            Gizmos.DrawRay(center, -right - up);
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

        public void SetLocalNormal(Vector3 newLocalNormal)
        {
            localNormal = newLocalNormal.normalized;
        }

        public void SetLocalPoint(Vector3 newLocalPoint)
        {
            localPoint = newLocalPoint;
        }

        protected new void OnValidate()
        {
            base.OnValidate();

            localNormal = localNormal.normalized;
        }

        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            normal = transform.TransformDirection(localNormal);

            Vector3 point = samplePoint - transform.TransformPoint(localPoint);

            return Math.Abs(Vector3.Dot(point, normal));
        }
    }
}