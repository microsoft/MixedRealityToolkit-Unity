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
        const string EditingEnabledKey = "MRTK_PressableButtonInspector_EditingEnabledKey";
        private static bool EditingEnabled = false;

        private const float labelMouseOverDistance = 0.025f;

        private static GUIStyle labelStyle;

        private PressableButton button;
        private Transform transform;
        private BoxCollider touchCage;

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
        }

        private void OnSceneGUI()
        {
            // Only display on selection
            if (Selection.activeObject != button.gameObject)
                return;

            // Instruct the button to create its path markers
            button.FindOrCreatePathMarkers();
            
            Vector3 touchCageSize = touchCage.size;
            Vector3 touchCageCenter = touchCage.center;
            Bounds touchCageLocalBounds = new Bounds(touchCageCenter, touchCageSize);
            // Get the start pos for touch
            Vector3 touchStartPos = touchCageLocalBounds.center;
            touchStartPos.z -= touchCageLocalBounds.extents.z;

            Transform buttonContentTransform = transform;
            if (movingButtonVisuals.objectReferenceValue != null)
                buttonContentTransform = (movingButtonVisuals.objectReferenceValue as GameObject).transform;

            // Get the start and end pos for moving content
            float startPos = buttonContentTransform.localPosition.z;
            float endPos = buttonContentTransform.localPosition.z;
            endPos += maxPushDistance.floatValue;

            // This is where we'll store the results of our manipulations
            float newMaxPushDistance = maxPushDistance.floatValue;
            float newPressDistance = pressDistance.floatValue;
            float newReleaseDistanceDelta = releaseDistanceDelta.floatValue;

            float newEndPos = endPos;
            float newPressDistPos = startPos + (newPressDistance / transform.lossyScale.z);
            float newTouchStartPos = touchStartPos.z;
            float newReleaseDistPos = newPressDistPos - (newReleaseDistanceDelta / transform.lossyScale.z);
            float newTouchCageSize = touchCageSize.z;
            float newTouchCageCenter = touchCageCenter.z;

            if (EditingEnabled)
            {
                EditorGUI.BeginChangeCheck();
            }

            float handleSize = Mathf.Max(touchCageSize.x * 0.065f, 0.0025f);

            // PRESS END
            Handles.color = Color.cyan;
            DrawPlaneAndHandle(targetEndPlane, touchCageSize * 0.5f, handleSize, ref newEndPos, touchStartPos, "Max move distance");

            if (EditingEnabled)
            {
                // Clamp the z value to start position
                newEndPos = Mathf.Max(startPos, newEndPos);
                newMaxPushDistance = Mathf.Abs(startPos - newEndPos);
            }

            // PRESS DISTANCE
            Handles.color = Color.yellow;
            DrawPlaneAndHandle(pressDistancePlane, touchCageLocalBounds.size * 0.35f, handleSize, ref newPressDistPos, touchStartPos, "Press event");

            if (EditingEnabled)
            {
                // Clamp the z values to target start / end
                newPressDistPos = Mathf.Max(startPos, newPressDistPos);
                newPressDistPos = Mathf.Min(newEndPos, newPressDistPos);
                // Set based on distance from start
                // Adjust for scaled objects
                newPressDistance = Mathf.Abs(newPressDistPos - startPos) * transform.lossyScale.z;
            }

            // RELEASE DISTANCE DELTA
            Handles.color = Color.red;
            DrawPlaneAndHandle(releasePlane, touchCageLocalBounds.size * 0.3f, handleSize, ref newReleaseDistPos, touchStartPos, "Release event");

            if (EditingEnabled)
            {
                // Clamp the z values to press distance
                newReleaseDistPos = Mathf.Min(newPressDistPos, newReleaseDistPos);
                // Set based on distance from press distance
                // Adjust for scaled objects
                newReleaseDistanceDelta = Mathf.Abs(newReleaseDistPos - newPressDistPos) * transform.lossyScale.z;
            }

            // BUTTON CONTENT ORIGIN
            // Don't allow editing of button position
            Handles.color = Color.green;
            float editStartPos = startPos;
            DrawPlaneAndHandle(pressStartPlane, touchCageLocalBounds.size * 0.4f, handleSize, ref editStartPos, touchStartPos, "Moving Button Visuals", false);

            // START POINT
            // Start point doesn't need a display offset because it's based on the touch cage center
            Handles.color = Color.cyan;
            DrawPlaneAndHandle(targetStartPlane, touchCageLocalBounds.size * 0.5f, handleSize, ref newTouchStartPos, touchStartPos, "Touch event");

            if (EditingEnabled)
            {
                // The touch event is defined by the collider bounds
                // If we've moved the start pos, we've moved the bounds
                float difference = (touchStartPos.z - newTouchStartPos);
                if (Mathf.Abs(difference) > 0)
                {
                    newTouchCageCenter -= difference / 2;
                    newTouchCageSize += difference;
                }
            }

            if (EditingEnabled && EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changing push button properties");

                maxPushDistance.floatValue = newMaxPushDistance;
                pressDistance.floatValue = newPressDistance;
                releaseDistanceDelta.floatValue = newReleaseDistanceDelta;

                boxColliderSize.vector3Value = new Vector3(touchCageSize.x, touchCageSize.y, newTouchCageSize);
                boxColliderCenter.vector3Value = new Vector3(touchCageCenter.x, touchCageCenter.y, newTouchCageCenter);
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

        private void DrawPlaneAndHandle(Vector3[] plane, Vector3 planeSize, float handleSize, ref float zPosition, Vector3 cagePosition, string label, bool drawArrows = true)
        {
            cagePosition.z = zPosition;
            MakePlaneFromPoint(plane, cagePosition, planeSize, transform);
            Handles.DrawSolidRectangleWithOutline(plane, Color.Lerp(Handles.color, Color.clear, 0.65f), Handles.color);

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

            if (EditingEnabled)
            {
                Vector3 handlePosition = Handles.FreeMoveHandle(plane[1], Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
                // Draw forward / backward arrows so people know they can drag
                if (drawArrows)
                {
                    Handles.ArrowHandleCap(0, handlePosition, Quaternion.LookRotation(transform.forward, Vector3.up), handleSize * 2, EventType.Repaint);
                    Handles.ArrowHandleCap(0, handlePosition, Quaternion.LookRotation(-transform.forward, Vector3.up), handleSize * 2, EventType.Repaint);
                }
                // Remove the offset from the handle position
                handlePosition = transform.InverseTransformPoint(handlePosition);
                zPosition = handlePosition.z;
            }

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            EditingEnabled = SessionState.GetBool(EditingEnabledKey, false);
            EditingEnabled = EditorGUILayout.Toggle("Show Handles", EditingEnabled);
            SessionState.SetBool(EditingEnabledKey, EditingEnabled);
        }

        private void DrawLabel(Vector3 origin, Vector3 direction, string content, GUIStyle labelStyle)
        {
            Color colorOnEnter = Handles.color;

            Vector3 handlePos = origin + direction.normalized * HandleUtility.GetHandleSize(origin);
            Handles.Label(handlePos, content, labelStyle);
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
