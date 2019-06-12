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
    /// Add a NearInteractionTouchable to your scene and configure a touchable surface
    /// in order to get PointerDown and PointerUp events whenever a PokePointer touches this surface.
    /// </summary>
    public class NearInteractionTouchable : BaseNearInteractionTouchable
    {
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(NearInteractionTouchable))]
        public class Editor : UnityEditor.Editor
        {
            private readonly Color handleColor = Color.white;
            private readonly Color fillColor = new Color(0, 0, 0, 0);

            protected virtual void OnSceneGUI()
            {
                NearInteractionTouchable t = (NearInteractionTouchable)target;

                if (Event.current.type == EventType.Repaint)
                {
                    UnityEditor.Handles.color = handleColor;

                    Vector3 center = t.transform.TransformPoint(t.localCenter);

                    float arrowSize = UnityEditor.HandleUtility.GetHandleSize(center) * 0.75f;
                    UnityEditor.Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(t.transform.rotation * t.localForward), arrowSize, EventType.Repaint);

                    Vector3 rightDelta = t.transform.localToWorldMatrix.MultiplyVector(t.LocalRight * t.bounds.x / 2);
                    Vector3 upDelta = t.transform.localToWorldMatrix.MultiplyVector(t.localUp * t.bounds.y / 2);

                    Vector3[] points = new Vector3[4];
                    points[0] = center + rightDelta + upDelta;
                    points[1] = center - rightDelta + upDelta;
                    points[2] = center - rightDelta - upDelta;
                    points[3] = center + rightDelta - upDelta;

                    UnityEditor.Handles.DrawSolidRectangleWithOutline(points, fillColor, handleColor);
                }
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                NearInteractionTouchable t = (NearInteractionTouchable)target;
                BoxCollider bc = t.GetComponent<BoxCollider>();
                RectTransform rt = t.GetComponent<RectTransform>();
                if (bc != null)
                {
                    // project size to local coordinate system
                    Vector2 adjustedSize = new Vector2(
                                Math.Abs(Vector3.Dot(bc.size, t.LocalRight)),
                                Math.Abs(Vector3.Dot(bc.size, t.localUp)));

                    // Resize helper
                    if (adjustedSize != t.bounds)
                    {
                        UnityEditor.EditorGUILayout.HelpBox("Bounds do not match the BoxCollider size", UnityEditor.MessageType.Warning);
                        if (GUILayout.Button("Fix Bounds"))
                        {
                            UnityEditor.Undo.RecordObject(t, "Fix Bounds");
                            t.bounds = adjustedSize;
                        }
                    }

                    // Recentre helper
                    if (t.localCenter != bc.center + Vector3.Scale(bc.size / 2.0f, t.localForward))
                    {
                        UnityEditor.EditorGUILayout.HelpBox("Center does not match the BoxCollider center", UnityEditor.MessageType.Warning);
                        if (GUILayout.Button("Fix Center"))
                        {
                            UnityEditor.Undo.RecordObject(t, "Fix Center");
                            t.localCenter = bc.center + Vector3.Scale(bc.size / 2.0f, t.localForward);
                        }
                    }
                }
                else if (rt != null)
                {
                    // Resize Helper
                    if (rt.sizeDelta != t.bounds)
                    {
                        UnityEditor.EditorGUILayout.HelpBox("Bounds do not match the RectTransform size", UnityEditor.MessageType.Warning);
                        if (GUILayout.Button("Fix Bounds"))
                        {
                            UnityEditor.Undo.RecordObject(t, "Fix Bounds");
                            t.bounds = rt.sizeDelta;
                        }
                    }

                    if (t.GetComponentInParent<Canvas>() != null && t.localForward != new Vector3(0, 0, -1))
                    {
                        UnityEditor.EditorGUILayout.HelpBox("Unity UI generally has forward facing away from the front. The LocalForward direction specified does not match the expected forward direction.", UnityEditor.MessageType.Warning);
                        if (GUILayout.Button("Fix Forward Direction"))
                        {
                            UnityEditor.Undo.RecordObject(t, "Fix Forward Direction");
                            t.localForward = new Vector3(0, 0, -1);
                        }
                    }
                }

                // Perpendicular forward/up vectors helpers
                if (Vector3.Dot(t.localForward, t.localUp) != 0)
                {
                    UnityEditor.EditorGUILayout.HelpBox("Local Forward and Local Up are not perpendicular.", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button("Fix Local Up"))
                    {
                        UnityEditor.Undo.RecordObject(t, "Fix Local Up");
                        t.localUp = Vector3.Cross(t.localForward, t.LocalRight).normalized;
                    }
                    if (GUILayout.Button("Fix Local Forward"))
                    {
                        UnityEditor.Undo.RecordObject(t, "Fix Local Forward");
                        t.localForward = Vector3.Cross(t.LocalRight, t.localUp).normalized;
                    }
                }
            }
        }
#endif

        private enum TouchableSurface
        {
            BoxCollider,
            UnityUI,
            Custom = 100
        }

        /// <summary>
        /// Local space forward direction
        /// </summary>
        [SerializeField]
        protected Vector3 localForward = Vector3.forward;

        public Vector3 LocalForward { get => localForward; }

        /// <summary>
        /// Local space forward direction
        /// </summary>
        [SerializeField]
        protected Vector3 localUp = Vector3.up;

        public Vector3 LocalUp { get => localUp; }

        /// <summary>
        /// Local space object center
        /// </summary>
        [SerializeField]
        protected Vector3 localCenter = Vector3.zero;

        [SerializeField]
        [Tooltip("The type of surface to calculate the touch point on.")]
        private TouchableSurface touchableSurface = TouchableSurface.BoxCollider;

        public Vector3 LocalRight
        {
            get
            {
                Vector3 cross = Vector3.Cross(localUp, localForward);
                if (cross == Vector3.zero)
                {
                    // vectors are collinear return default right
                    return Vector3.right;
                }
                else
                {
                    return cross;
                }
            }
        }

        public Vector3 Forward => transform.TransformDirection(localForward);

        /// <summary>
        /// Local space forward direction
        /// </summary>
        [SerializeField]
        protected Vector2 bounds = Vector2.zero;

        protected new void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(localForward.magnitude > 0);
            Debug.Assert(localUp.magnitude > 0);
            string hierarchy = gameObject.transform.EnumerateAncestors(true).Aggregate("", (result, next) => next.gameObject.name + "=>" + result);
            if (localUp.sqrMagnitude == 1 && localForward.sqrMagnitude == 1)
            {
                Debug.Assert(Vector3.Dot(localForward, localUp) == 0, $"localForward and localUp not perpendicular for object {hierarchy}. Did you set Local Forward correctly?");
            }

            // Check initial setup
            if (bounds == Vector2.zero)
            {
                if (touchableSurface == TouchableSurface.UnityUI)
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        // Initialize bounds to RectTransform SizeDelta
                        bounds = rt.sizeDelta;
                        localForward = new Vector3(0, 0, -1);
                    }
                }
            }

            localForward = localForward.normalized;
            localUp = localUp.normalized;

            bounds.x = Mathf.Max(bounds.x, 0);
            bounds.y = Mathf.Max(bounds.y, 0);
        }

        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            normal = Forward;

            Vector3 localPoint = transform.InverseTransformPoint(samplePoint) - localCenter;

            // Get surface coordinates
            Vector3 planeSpacePoint = new Vector3(
                Vector3.Dot(localPoint, LocalRight),
                Vector3.Dot(localPoint, localUp),
                Vector3.Dot(localPoint, localForward));

            // touchables currently can only be touched within the bounds of the rectangle.
            // We return infinity to ensure that any point outside the bounds does not get touched.
            if (planeSpacePoint.x < -bounds.x / 2 ||
                planeSpacePoint.x > bounds.x / 2 ||
                planeSpacePoint.y < -bounds.y / 2 ||
                planeSpacePoint.y > bounds.y / 2)
            {
                return float.PositiveInfinity;
            }

            // Scale back to 3D space
            planeSpacePoint = transform.TransformSize(planeSpacePoint);

            return Math.Abs(planeSpacePoint.z);
        }

    }
}