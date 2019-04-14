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
        private static bool editingEnabled = true;
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

            Transform buttonContentTransform = transform;
            if (movingButtonVisuals.objectReferenceValue != null)
                buttonContentTransform = (movingButtonVisuals.objectReferenceValue as GameObject).transform;

            // Get the start and end pos for moving content
            Vector3 startPos = buttonContentTransform.localPosition;
            Vector3 endPos = buttonContentTransform.localPosition;
            endPos.z += maxPushDistance.floatValue;
            // Get the start pos for touch
            Vector3 touchStartPos = touchCageLocalBounds.center;
            touchStartPos.z -= touchCageLocalBounds.extents.z;

            // This is where we'll store the results of our manipulations
            float newMaxPushDistance = maxPushDistance.floatValue;
            float newPressDistance = pressDistance.floatValue;
            float newReleaseDistanceDelta = releaseDistanceDelta.floatValue;
            Vector3 newStartPos = startPos;
            Vector3 newEndPos = endPos;
            Vector3 newPressDistPos = startPos + (Vector3.forward * newPressDistance) / transform.lossyScale.z;
            Vector3 newTouchStartPos = touchStartPos;
            Vector3 newReleaseDistPos = newPressDistPos + (Vector3.back * newReleaseDistanceDelta) / transform.lossyScale.z;
            Vector3 newTouchCageSize = touchCageSize;
            Vector3 newTouchCageCenter = touchCageCenter;

            if (editingEnabled)
            {
                EditorGUI.BeginChangeCheck();
            }

            float handleSize = touchCageSize.x * 0.065f;

            // END POINT
            Handles.color = Color.cyan;
            DrawPlaneAndHandle(targetEndPlane, touchCageSize * 0.5f, handleSize, ref newEndPos, "Max move distance");

            if (editingEnabled)
            {
                // Clamp the z value to start position
                newEndPos.z = Mathf.Max(startPos.z, newEndPos.z);
                newMaxPushDistance = Vector3.Distance(startPos, newEndPos);
            }

            // PRESS DISTANCE
            Handles.color = Color.yellow;
            DrawPlaneAndHandle(pressDistancePlane, touchCageLocalBounds.size * 0.35f, handleSize, ref newPressDistPos, "Press event");

            if (editingEnabled)
            {
                // Clamp the z values to target start / end
                newPressDistPos.z = Mathf.Max(startPos.z, newPressDistPos.z);
                newPressDistPos.z = Mathf.Min(newEndPos.z, newPressDistPos.z);
                // Set based on distance from start
                // Adjust for scaled objects
                newPressDistance = Vector3.Distance(newPressDistPos, startPos) * transform.lossyScale.z;
            }

            // RELEASE DISTANCE DELTA
            Handles.color = Color.red;
            DrawPlaneAndHandle(releasePlane, touchCageLocalBounds.size * 0.3f, handleSize, ref newReleaseDistPos, "Release event");

            if (editingEnabled)
            {
                // Clamp the z values to start / press distance
                newReleaseDistPos.z = Mathf.Max(startPos.z, newReleaseDistPos.z);
                newReleaseDistPos.z = Mathf.Min(newPressDistPos.z, newReleaseDistPos.z);
                // Set based on distance from press distance
                // Adjust for scaled objects
                newReleaseDistanceDelta = Vector3.Distance(newReleaseDistPos, newPressDistPos) * transform.lossyScale.z;
            }

            // BUTTON CONTENT ORIGIN
            Handles.color = Color.green;
            DrawPlaneAndHandle(pressStartPlane, touchCageLocalBounds.size * 0.4f, handleSize, ref newStartPos, "Button begins moving (content origin)");

            if (editingEnabled)
            {
                if (buttonContentTransform != transform)
                {
                    newStartPos.z = Mathf.Max(newTouchStartPos.z, newStartPos.z);
                    buttonContentTransform.localPosition = newStartPos;
                }
            }

            // START POINT
            Handles.color = Color.cyan;
            DrawPlaneAndHandle(targetStartPlane, touchCageLocalBounds.size * 0.5f, handleSize, ref newTouchStartPos, "Touch event");

            if (editingEnabled)
            {
                // The touch event is defined by the collider bounds
                // If we've moved the start pos, we've moved the bounds
                float difference = touchStartPos.z - newTouchStartPos.z;
                newTouchCageCenter.z -= difference / 2;
                newTouchCageSize.z += difference;
            }

            if (editingEnabled && EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changing push button properties");

                maxPushDistance.floatValue = newMaxPushDistance;
                pressDistance.floatValue = newPressDistance;
                releaseDistanceDelta.floatValue = newReleaseDistanceDelta;

                boxColliderSize.vector3Value = newTouchCageSize;
                boxColliderCenter.vector3Value = newTouchCageCenter;
                boxColliderObject.ApplyModifiedProperties();
            }

            // Draw dotted lines showing path from beginning to end of button path
            Handles.color = Color.Lerp(Color.cyan, Color.clear, 0.25f);
            Handles.DrawDottedLine(targetStartPlane[0], targetEndPlane[0], 2.5f);
            Handles.DrawDottedLine(targetStartPlane[1], targetEndPlane[1], 2.5f);
            Handles.DrawDottedLine(targetStartPlane[2], targetEndPlane[2], 2.5f);
            Handles.DrawDottedLine(targetStartPlane[3], targetEndPlane[3], 2.5f);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPlaneAndHandle(Vector3[] plane, Vector3 planeSize, float handleSize, ref Vector3 position, string label)
        {
            MakePlaneFromPoint(plane, position, planeSize, transform);
            Handles.DrawSolidRectangleWithOutline(plane, Color.Lerp(Handles.color, Color.clear, 0.65f), Handles.color);

            Vector3 mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToViewportPoint(Event.current.mousePosition);
            mousePosition.y = 1f - mousePosition.y;
            mousePosition.z = 0;
            Vector3 handlePos = SceneView.currentDrawingSceneView.camera.WorldToViewportPoint(plane[1]);
            handlePos.z = 0;

            if (Vector3.Distance(mousePosition, handlePos) < labelMouseOverDistance)
            {
                DrawLabel(plane[1], transform.up - transform.right, label, labelStyle);
                SceneView.RepaintAll();
            }

            if (editingEnabled)
            {
                position = Handles.FreeMoveHandle(plane[1], Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
                // Draw forward / backward arrows so people know they can drag
                Handles.ArrowHandleCap(0, plane[1], Quaternion.LookRotation(transform.forward, Vector3.up), handleSize * 2, EventType.Repaint);
                Handles.ArrowHandleCap(0, plane[1], Quaternion.LookRotation(-transform.forward, Vector3.up), handleSize * 2, EventType.Repaint);
                position = InverseTransformAndConstrain(position, transform);
            }

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            editingEnabled = EditorGUILayout.Toggle("Show Handles", editingEnabled);
        }

        private Vector3 InverseTransformAndConstrain(Vector3 value, Transform transform)
        {
            value = transform.InverseTransformPoint(value);
            value.x = 0;
            value.y = 0;
            return value;
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
