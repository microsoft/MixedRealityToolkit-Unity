// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [UnityEditor.CustomEditor(typeof(NearInteractionTouchable), true)]
    public class NearInteractionTouchableInspector : UnityEditor.Editor
    {
        private readonly Color handleColor = Color.white;
        private readonly Color fillColor = new Color(0, 0, 0, 0);

        protected virtual void OnSceneGUI()
        {
            NearInteractionTouchable t = (NearInteractionTouchable)target;

            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Handles.color = handleColor;

                Vector3 center = t.transform.TransformPoint(t.LocalCenter);

                float arrowSize = UnityEditor.HandleUtility.GetHandleSize(center) * 0.75f;
                UnityEditor.Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(t.transform.rotation * t.LocalForward), arrowSize, EventType.Repaint);

                Vector3 rightDelta = t.transform.localToWorldMatrix.MultiplyVector(t.LocalRight * t.Bounds.x / 2);
                Vector3 upDelta = t.transform.localToWorldMatrix.MultiplyVector(t.LocalUp * t.Bounds.y / 2);

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
                            Math.Abs(Vector3.Dot(bc.size, t.LocalUp)));

                // Resize helper
                if (adjustedSize != t.Bounds)
                {
                    UnityEditor.EditorGUILayout.HelpBox("Bounds do not match the BoxCollider size", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button("Fix Bounds"))
                    {
                        UnityEditor.Undo.RecordObject(t, "Fix Bounds");
                        t.Bounds = adjustedSize;
                    }
                }

                // Recentre helper
                if (t.LocalCenter != bc.center + Vector3.Scale(bc.size / 2.0f, t.LocalForward))
                {
                    UnityEditor.EditorGUILayout.HelpBox("Center does not match the BoxCollider center", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button("Fix Center"))
                    {
                        UnityEditor.Undo.RecordObject(t, "Fix Center");
                        t.LocalCenter = bc.center + Vector3.Scale(bc.size / 2.0f, t.LocalForward);
                    }
                }
            }
            else if (rt != null)
            {
                // Resize Helper
                if (rt.sizeDelta != t.Bounds)
                {
                    UnityEditor.EditorGUILayout.HelpBox("Bounds do not match the RectTransform size", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button("Fix Bounds"))
                    {
                        UnityEditor.Undo.RecordObject(t, "Fix Bounds");
                        t.Bounds = rt.sizeDelta;
                    }
                }

                if (t.GetComponentInParent<Canvas>() != null && t.LocalForward != new Vector3(0, 0, -1))
                {
                    UnityEditor.EditorGUILayout.HelpBox("Unity UI generally has forward facing away from the front. The LocalForward direction specified does not match the expected forward direction.", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button("Fix Forward Direction"))
                    {
                        UnityEditor.Undo.RecordObject(t, "Fix Forward Direction");
                        t.SetLocalForward(new Vector3(0, 0, -1));
                    }
                }
            }

            // Perpendicular forward/up vectors helpers
            if (!t.AreLocalVectorsOrthogonal)
            {
                UnityEditor.EditorGUILayout.HelpBox("Local Forward and Local Up are not perpendicular.", UnityEditor.MessageType.Warning);
                if (GUILayout.Button("Fix Local Up"))
                {
                    UnityEditor.Undo.RecordObject(t, "Fix Local Up");
                    t.SetLocalForward(t.LocalForward);
                }
                if (GUILayout.Button("Fix Local Forward"))
                {
                    UnityEditor.Undo.RecordObject(t, "Fix Local Forward");
                    t.SetLocalUp(t.LocalUp);
                }
            }
        }
    }
}