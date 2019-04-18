// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(PressableButton))]
    public class PressableButtonInspector : UnityEditor.Editor
    {
        // Struct used to store state of preview
        // This lets us display accurate info while button is being pressed
        private struct ButtonInfo
        {
            // Convenience fields for box collider info
            public Bounds TouchCageLocalBounds;
            // Start pos for touch, also the origin of our cage
            public Vector3 TouchStartOrigin;
            // Start and end pos for moving content
            public float StartPos;
            public float EndPos;
            // Press, touch and release positions in z axis
            public float PressDistPos;
            public float TouchStartPos;
            public float ReleaseDistPos;
            // Cage values
            public float TouchCageCenter;
            public float TouchCageSize;
            // The actual values that the button uses
            public float MaxPushDistance;
            public float PressDistance;
            public float ReleaseDistanceDelta;
        }

        const string EditingEnabledKey = "MRTK_PressableButtonInspector_EditingEnabledKey";
        const string VisiblePlanesKey = "MRTK_PressableButtonInspector_VisiblePlanesKey";
        private static bool EditingEnabled = false;
        private static bool VisiblePlanes = true;

        private const float labelMouseOverDistance = 0.025f;

        private static GUIStyle labelStyle;

        private PressableButton button;
        private Transform transform;
        private Transform buttonContentTransform;
        private BoxCollider touchCage;

        private ButtonInfo currentInfo;

        private SerializedProperty maxPushDistance;
        private SerializedProperty pressDistance;
        private SerializedProperty releaseDistanceDelta;
        private SerializedProperty movingButtonVisuals;
        private SerializedObject boxColliderObject;
        private SerializedProperty boxColliderSize;
        private SerializedProperty boxColliderCenter;

        private static Vector3[] targetStartPlane = new Vector3[4];
        private static Vector3[] targetEndPlane = new Vector3[4];
        private static Vector3[] pressDistancePlane = new Vector3[4];
        private static Vector3[] pressStartPlane = new Vector3[4];
        private static Vector3[] releasePlane = new Vector3[4];

        private void OnEnable()
        {
            button = (PressableButton)target;
            transform = button.transform;

            touchCage = button.GetComponent<BoxCollider>();

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.white;
            }

            maxPushDistance = serializedObject.FindProperty("maxPushDistance");
            pressDistance = serializedObject.FindProperty("pressDistance");
            releaseDistanceDelta = serializedObject.FindProperty("releaseDistanceDelta");
            movingButtonVisuals = serializedObject.FindProperty("movingButtonVisuals");

            boxColliderObject = new SerializedObject(touchCage);
            boxColliderSize = boxColliderObject.FindProperty("m_Size");
            boxColliderCenter = boxColliderObject.FindProperty("m_Center");

            buttonContentTransform = transform;
            if (movingButtonVisuals.objectReferenceValue != null)
            {
                buttonContentTransform = (movingButtonVisuals.objectReferenceValue as GameObject).transform;
            }
        }

        private void OnSceneGUI()
        {
            // Only display on selection
            if (Selection.activeObject != button.gameObject)
            {
                return;
            }

            if (!VisiblePlanes)
            {
                return;
            }

            // If the button is being pressed, don't gather new info
            // Just display the info we already gathered
            // This lets people view button presses in real-time
            if (button.IsTouching)
            {
                DrawButtonInfo(currentInfo, false);
            }
            else
            {
                currentInfo = GatherCurrentInfo();
                DrawButtonInfo(currentInfo, EditingEnabled);
            }
        }

        private ButtonInfo GatherCurrentInfo()
        {
            ButtonInfo info = new ButtonInfo();

            info.TouchCageLocalBounds = new Bounds(touchCage.center, touchCage.size);
            // Get the start pos for touch
            Vector3 touchStartOrigin = info.TouchCageLocalBounds.center;
            touchStartOrigin.z -= info.TouchCageLocalBounds.extents.z;
            info.TouchStartOrigin = touchStartOrigin;
            info.TouchCageCenter = info.TouchCageLocalBounds.center.z;
            info.TouchCageSize = info.TouchCageLocalBounds.size.z;

            // Get the start and end pos for moving content
            info.StartPos = buttonContentTransform.localPosition.z;
            info.EndPos = buttonContentTransform.localPosition.z;
            info.EndPos += maxPushDistance.floatValue / transform.lossyScale.z;

            info.MaxPushDistance = maxPushDistance.floatValue;
            info.PressDistance = pressDistance.floatValue;
            info.ReleaseDistanceDelta = releaseDistanceDelta.floatValue;

            info.PressDistPos = info.StartPos + (info.PressDistance / transform.lossyScale.z);
            info.TouchStartPos = info.TouchStartOrigin.z;
            info.ReleaseDistPos = info.PressDistPos - (info.ReleaseDistanceDelta / transform.lossyScale.z);

            return info;
        }

        private void DrawButtonInfo(ButtonInfo info, bool editingEnabled)
        {
            // This is where we'll store our new values to compare against
            ButtonInfo newInfo = info;

            if (editingEnabled)
            {
                EditorGUI.BeginChangeCheck();
            }

            // PRESS END
            Handles.color = Color.cyan;
            newInfo.EndPos = DrawPlaneAndHandle(targetEndPlane, info.TouchCageLocalBounds.size * 0.5f, newInfo.EndPos, info.TouchStartOrigin, "Max move distance", editingEnabled);

            if (editingEnabled)
            {
                // Clamp the z value to start position
                newInfo.EndPos = Mathf.Max(newInfo.StartPos, newInfo.EndPos);
                newInfo.MaxPushDistance = (newInfo.EndPos - newInfo.StartPos) * transform.lossyScale.z;
            }

            // PRESS DISTANCE
            Handles.color = Color.yellow;
            newInfo.PressDistPos = DrawPlaneAndHandle(pressDistancePlane, info.TouchCageLocalBounds.size * 0.35f, newInfo.PressDistPos, info.TouchStartOrigin, "Press event", editingEnabled);

            if (editingEnabled)
            {
                // Clamp the z values to target start / end
                newInfo.PressDistPos = Mathf.Max(newInfo.StartPos, newInfo.PressDistPos);
                newInfo.PressDistPos = Mathf.Min(newInfo.EndPos, newInfo.PressDistPos);
                // Set based on distance from start
                // Adjust for scaled objects
                newInfo.PressDistance = Mathf.Abs(newInfo.PressDistPos - newInfo.StartPos) * transform.lossyScale.z;
            }

            // RELEASE DISTANCE DELTA
            Handles.color = Color.red;
            newInfo.ReleaseDistPos = DrawPlaneAndHandle(releasePlane, info.TouchCageLocalBounds.size * 0.3f, newInfo.ReleaseDistPos, info.TouchStartOrigin, "Release event", editingEnabled);

            if (editingEnabled)
            {
                // Clamp the z values to press distance
                newInfo.ReleaseDistPos = Mathf.Min(newInfo.PressDistPos, newInfo.ReleaseDistPos);
                // Set based on distance from press distance
                // Adjust for scaled objects
                newInfo.ReleaseDistanceDelta = (newInfo.PressDistPos - newInfo.ReleaseDistPos) * transform.lossyScale.z;
            }

            // BUTTON CONTENT ORIGIN
            // Don't allow editing of button position
            Handles.color = Color.green;
            DrawPlaneAndHandle(pressStartPlane, info.TouchCageLocalBounds.size * 0.4f, newInfo.StartPos, info.TouchStartOrigin, "Moving button visuals", false);

            // START POINT
            // Start point doesn't need a display offset because it's based on the touch cage center
            Handles.color = Color.cyan;
            newInfo.TouchStartPos = DrawPlaneAndHandle(targetStartPlane, info.TouchCageLocalBounds.size * 0.5f, newInfo.TouchStartPos, info.TouchStartOrigin, "Touch event", editingEnabled);

            if (editingEnabled)
            {
                // The touch event is defined by the collider bounds
                // If we've moved the start pos, we've moved the bounds
                float difference = (info.TouchStartPos - newInfo.TouchStartPos);
                if (Mathf.Abs(difference) > 0)
                {
                    newInfo.TouchCageCenter -= difference / 2;
                    newInfo.TouchCageSize += difference;
                }
            }

            if (editingEnabled && EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changing push button properties");

                maxPushDistance.floatValue = newInfo.MaxPushDistance;
                pressDistance.floatValue = newInfo.PressDistance;
                releaseDistanceDelta.floatValue = newInfo.ReleaseDistanceDelta;

                boxColliderSize.vector3Value = new Vector3(info.TouchCageLocalBounds.size.x, info.TouchCageLocalBounds.size.y, newInfo.TouchCageSize);
                boxColliderCenter.vector3Value = new Vector3(info.TouchCageLocalBounds.center.x, info.TouchCageLocalBounds.center.y, newInfo.TouchCageCenter);
                boxColliderObject.ApplyModifiedProperties();

                serializedObject.ApplyModifiedProperties();
            }

            // Draw dotted lines showing path from beginning to end of button path
            Handles.color = Color.Lerp(Color.cyan, Color.clear, 0.25f);
            Handles.DrawDottedLine(targetStartPlane[0], targetEndPlane[0], 2.5f);
            Handles.DrawDottedLine(targetStartPlane[1], targetEndPlane[1], 2.5f);
            Handles.DrawDottedLine(targetStartPlane[2], targetEndPlane[2], 2.5f);
            Handles.DrawDottedLine(targetStartPlane[3], targetEndPlane[3], 2.5f);
        }

        private float DrawPlaneAndHandle(Vector3[] plane, Vector3 planeSize, float zPosition, Vector3 cagePosition, string label, bool editingEnabled)
        {
            cagePosition.z = zPosition;
            MakePlaneFromPoint(plane, cagePosition, planeSize, transform);

            if (VisiblePlanes)
            {
                Handles.DrawSolidRectangleWithOutline(plane, Color.Lerp(Handles.color, Color.clear, 0.65f), Handles.color);
            }

            Vector3 mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToViewportPoint(Event.current.mousePosition);
            mousePosition.y = 1f - mousePosition.y;
            mousePosition.z = 0;
            Vector3 handleVisiblePos = SceneView.currentDrawingSceneView.camera.WorldToViewportPoint(plane[1]);
            handleVisiblePos.z = 0;

            if (Vector3.Distance(mousePosition, handleVisiblePos) < labelMouseOverDistance)
            {
                DrawLabel(plane[1], transform.up - transform.right, label, labelStyle);
                SceneView.RepaintAll();
            }

            float handleSize = HandleUtility.GetHandleSize(plane[1]) * 0.15f;

            Handles.ArrowHandleCap(0, plane[1], Quaternion.LookRotation(transform.forward, Vector3.up), handleSize * 2, EventType.Repaint);
            Handles.ArrowHandleCap(0, plane[1], Quaternion.LookRotation(-transform.forward, Vector3.up), handleSize * 2, EventType.Repaint);

            // Draw forward / backward arrows so people know they can drag
            if (editingEnabled)
            {
                Vector3 handlePosition = Handles.FreeMoveHandle(plane[1], Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
                handlePosition = transform.InverseTransformPoint(handlePosition);
                zPosition = handlePosition.z;
            }

            return zPosition;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            VisiblePlanes = SessionState.GetBool(VisiblePlanesKey, true);
            VisiblePlanes = EditorGUILayout.Toggle("Show Button Event Planes", VisiblePlanes);
            SessionState.SetBool(VisiblePlanesKey, VisiblePlanes);

            if (VisiblePlanes)
            {
                EditingEnabled = SessionState.GetBool(EditingEnabledKey, false);
                EditingEnabled = EditorGUILayout.Toggle("Make Planes Editable", EditingEnabled);
                SessionState.SetBool(EditingEnabledKey, EditingEnabled);
            }

            EditorUtility.SetDirty(target);
        }

        private void DrawLabel(Vector3 origin, Vector3 direction, string content, GUIStyle labelStyle)
        {
            Color colorOnEnter = Handles.color;

            float handleSize = HandleUtility.GetHandleSize(origin);
            Vector3 handlePos = origin + direction.normalized * handleSize * 2;
            Handles.Label(handlePos + (Vector3.up * handleSize * 0.1f), content, labelStyle);
            Handles.color = Color.Lerp(colorOnEnter, Color.clear, 0.25f);
            Handles.DrawDottedLine(origin, handlePos, 5f);

            Handles.color = colorOnEnter;
        }

        private void MakePlaneFromPoint(Vector3[] plane, Vector3 pos, Vector3 size, Transform targetTransform)
        {
            plane[0] = targetTransform.TransformPoint(new Vector3(pos.x - size.x, pos.y - size.y, pos.z));
            plane[1] = targetTransform.TransformPoint(new Vector3(pos.x - size.x, pos.y + size.y, pos.z));
            plane[2] = targetTransform.TransformPoint(new Vector3(pos.x + size.x, pos.y + size.y, pos.z));
            plane[3] = targetTransform.TransformPoint(new Vector3(pos.x + size.x, pos.y - size.y, pos.z));
        }
    }
}
