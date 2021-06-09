// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomEditor(typeof(NearInteractionTouchable), true)]
    public class NearInteractionTouchableInspector : NearInteractionTouchableInspectorBase
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (NearInteractionTouchable)target;
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
                    EditorGUILayout.HelpBox("Bounds do not match the BoxCollider size", MessageType.Warning);
                    if (GUILayout.Button("Fix Bounds"))
                    {
                        Undo.RecordObject(t, "Fix Bounds");
                        t.SetBounds(adjustedSize);
                    }
                }

                // Recentre helper
                if (t.LocalCenter != bc.center + Vector3.Scale(bc.size / 2.0f, t.LocalForward))
                {
                    EditorGUILayout.HelpBox("Center does not match the BoxCollider center", MessageType.Warning);
                    if (GUILayout.Button("Fix Center"))
                    {
                        Undo.RecordObject(t, "Fix Center");
                        t.SetLocalCenter(bc.center + Vector3.Scale(bc.size / 2.0f, t.LocalForward));
                    }
                }
            }
            else if (rt != null)
            {
                // Resize Helper
                if (rt.sizeDelta != t.Bounds)
                {
                    EditorGUILayout.HelpBox("Bounds do not match the RectTransform size", MessageType.Warning);
                    if (GUILayout.Button("Fix Bounds"))
                    {
                        Undo.RecordObject(t, "Fix Bounds");
                        t.SetBounds(rt.sizeDelta);
                    }
                }

                if (t.GetComponentInParent<Canvas>() != null && t.LocalForward != new Vector3(0, 0, -1))
                {
                    EditorGUILayout.HelpBox("Unity UI generally has forward facing away from the front. The LocalForward direction specified does not match the expected forward direction.", MessageType.Warning);
                    if (GUILayout.Button("Fix Forward Direction"))
                    {
                        Undo.RecordObject(t, "Fix Forward Direction");
                        t.SetLocalForward(new Vector3(0, 0, -1));
                    }
                }
            }

            // Perpendicular forward/up vectors helpers
            if (!t.AreLocalVectorsOrthogonal)
            {
                EditorGUILayout.HelpBox("Local Forward and Local Up are not perpendicular.", MessageType.Warning);
                if (GUILayout.Button("Fix Local Up"))
                {
                    Undo.RecordObject(t, "Fix Local Up");
                    t.SetLocalForward(t.LocalForward);
                }
                if (GUILayout.Button("Fix Local Forward"))
                {
                    Undo.RecordObject(t, "Fix Local Forward");
                    t.SetLocalUp(t.LocalUp);
                }
            }

            // Warn users when using a non-box collider
            if (t.TouchableCollider != null && !(t.TouchableCollider is BoxCollider))
            {
                if (t.Bounds == Vector2.zero)
                {
                    EditorGUILayout.HelpBox("When using a non-box collider the bounds must be manually specified. The default value (0, 0) will "
                    + "not function as intended.\nWith NearInteractionTouchable touch is only detected when the collider is within range of the pointer "
                    + "and the pointer is within the 2D bounds specified above. To detect touch with the whole collider (whenever the collider is "
                    + "within range of the pointer) use NearInteractionTouchableVolume instead.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("With NearInteractionTouchable touch is only detected when the collider is within range of the pointer "
                    + "and the pointer is within the 2D bounds specified above. To detect touch with the whole collider (whenever the collider is "
                    + "within range of the pointer) use NearInteractionTouchableVolume instead.", MessageType.Info);
                }
            }
        }
    }

    [CustomEditor(typeof(BaseNearInteractionTouchable), true)]
    public class NearInteractionTouchableInspectorBase : UnityEditor.Editor
    {
        private readonly Color handleColor = Color.white;
        private readonly Color fillColor = new Color(0, 0, 0, 0);

        protected virtual void OnSceneGUI()
        {
            var t = (NearInteractionTouchableSurface)target;

            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = handleColor;

                Vector3 center = t.transform.TransformPoint(t.LocalCenter);

                float arrowSize = HandleUtility.GetHandleSize(center) * 0.75f;
                Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(t.transform.rotation * -t.LocalPressDirection), arrowSize, EventType.Repaint);

                var localRight = Vector3.right;
                var localUp = Vector3.up;

                if (t is NearInteractionTouchable touchableConcrete)
                {
                    localRight = touchableConcrete.LocalRight;
                    localUp = touchableConcrete.LocalUp;
                }

                Vector3 rightDelta = t.transform.localToWorldMatrix.MultiplyVector(localRight * t.Bounds.x / 2);
                Vector3 upDelta = t.transform.localToWorldMatrix.MultiplyVector(localUp * t.Bounds.y / 2);

                Vector3[] points = new Vector3[4];
                points[0] = center + rightDelta + upDelta;
                points[1] = center - rightDelta + upDelta;
                points[2] = center - rightDelta - upDelta;
                points[3] = center + rightDelta - upDelta;

                Handles.DrawSolidRectangleWithOutline(points, fillColor, handleColor);
            }
        }
    }
}
