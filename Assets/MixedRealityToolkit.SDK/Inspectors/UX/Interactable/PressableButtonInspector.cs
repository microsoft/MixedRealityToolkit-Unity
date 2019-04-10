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
        // Struct use to store state of preview
        // This lets us display accurate info while button is being pressed
        private struct ButtonInfo
        {
            // Convenience fields for box collider info
            public Bounds touchCageLocalBounds;
            // Start pos for touch, also the origin of our cage
            public Vector3 touchStartOrigin;
            // Start and end pos for moving content
            public float startPos;
            public float endPos;
            // Press, touch and release positions in z axis
            public float pressDistPos;
            public float touchStartPos;
            public float releaseDistPos;
            // Cage values
            public float touchCageCenter;
            public float touchCageSize;
            // The actual values that the button uses
            public float maxPushDistance;
            public float pressDistance;
            public float releaseDistanceDelta;
        }

        const string EditingEnabledKey = "MRTK_PressableButtonInspector_EditingEnabledKey";
        private static bool EditingEnabled = false;

        private const float labelMouseOverDistance = 0.025f;

        private static GUIStyle labelStyle;

        private PressableButton button;
        private Transform transform;
        private BoxCollider touchCage;

        private ButtonInfo currentInfo;

        private SerializedProperty maxPushDistance;
        private SerializedProperty pressDistance;
        private SerializedProperty releaseDistanceDelta;
        private SerializedProperty movingButtonVisuals;
        private SerializedProperty isTouching;
        private SerializedProperty isPressing;
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
            isTouching = serializedObject.FindProperty("isTouching");
            isPressing = serializedObject.FindProperty("isPressing");

            boxColliderObject = new SerializedObject(touchCage);
            boxColliderSize = boxColliderObject.FindProperty("m_Size");
            boxColliderCenter = boxColliderObject.FindProperty("m_Center");
        }

        private void OnSceneGUI()
        {
            // Only display on selection
            if (Selection.activeObject != button.gameObject)
                return;

            // If the button is being pressed, don't gather new info
            // Just display the info we already gathered
            // This lets people view button presses in real-time
            if (isTouching.boolValue || isPressing.boolValue)
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

            info.touchCageLocalBounds = new Bounds(touchCage.center, touchCage.size);
            // Get the start pos for touch
            Vector3 touchStartOrigin = info.touchCageLocalBounds.center;
            touchStartOrigin.z -= info.touchCageLocalBounds.extents.z;
            info.touchStartOrigin = touchStartOrigin;
            info.touchCageCenter = info.touchCageLocalBounds.center.z;
            info.touchCageSize = info.touchCageLocalBounds.size.z;

            Transform buttonContentTransform = transform;
            if (movingButtonVisuals.objectReferenceValue != null)
                buttonContentTransform = (movingButtonVisuals.objectReferenceValue as GameObject).transform;

            // Get the start and end pos for moving content
            info.startPos = buttonContentTransform.localPosition.z;
            info.endPos = buttonContentTransform.localPosition.z;
            info.endPos += maxPushDistance.floatValue;

            info.maxPushDistance = maxPushDistance.floatValue;
            info.pressDistance = pressDistance.floatValue;
            info.releaseDistanceDelta = releaseDistanceDelta.floatValue;

            info.pressDistPos = info.startPos + (info.pressDistance / transform.lossyScale.z);
            info.touchStartPos = info.touchStartOrigin.z;
            info.releaseDistPos = info.pressDistPos - (info.releaseDistanceDelta / transform.lossyScale.z);

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

            float handleSize = Mathf.Max(info.touchCageLocalBounds.size.x * 0.065f, 0.0025f);

            // PRESS END
            Handles.color = Color.cyan;
            DrawPlaneAndHandle(targetEndPlane, info.touchCageLocalBounds.size * 0.5f, handleSize, ref newInfo.endPos, info.touchStartOrigin, "Max move distance");

            if (editingEnabled)
            {
                // Clamp the z value to start position
                newInfo.endPos = Mathf.Max(newInfo.startPos, newInfo.endPos);
                newInfo.maxPushDistance = Mathf.Abs(newInfo.startPos - newInfo.endPos);
            }

            // PRESS DISTANCE
            Handles.color = Color.yellow;
            DrawPlaneAndHandle(pressDistancePlane, info.touchCageLocalBounds.size * 0.35f, handleSize, ref newInfo.pressDistPos, info.touchStartOrigin, "Press event");

            if (editingEnabled)
            {
                // Clamp the z values to target start / end
                newInfo.pressDistPos = Mathf.Max(newInfo.startPos, newInfo.pressDistPos);
                newInfo.pressDistPos = Mathf.Min(newInfo.endPos, newInfo.pressDistPos);
                // Set based on distance from start
                // Adjust for scaled objects
                newInfo.pressDistance = Mathf.Abs(newInfo.pressDistPos - newInfo.startPos) * transform.lossyScale.z;
            }

            // RELEASE DISTANCE DELTA
            Handles.color = Color.red;
            DrawPlaneAndHandle(releasePlane, info.touchCageLocalBounds.size * 0.3f, handleSize, ref newInfo.releaseDistPos, info.touchStartOrigin, "Release event");

            if (editingEnabled)
            {
                // Clamp the z values to press distance
                newInfo.releaseDistPos = Mathf.Min(newInfo.pressDistPos, newInfo.releaseDistPos);
                // Set based on distance from press distance
                // Adjust for scaled objects
                newInfo.releaseDistanceDelta = Mathf.Abs(newInfo.releaseDistPos - newInfo.pressDistPos) * transform.lossyScale.z;
            }

            // BUTTON CONTENT ORIGIN
            // Don't allow editing of button position
            Handles.color = Color.green;
            float editStartPos = newInfo.startPos;
            DrawPlaneAndHandle(pressStartPlane, info.touchCageLocalBounds.size * 0.4f, handleSize, ref editStartPos, info.touchStartOrigin, "Moving Button Visuals", false);

            // START POINT
            // Start point doesn't need a display offset because it's based on the touch cage center
            Handles.color = Color.cyan;
            DrawPlaneAndHandle(targetStartPlane, info.touchCageLocalBounds.size * 0.5f, handleSize, ref newInfo.touchStartPos, info.touchStartOrigin, "Touch event");

            if (editingEnabled)
            {
                // The touch event is defined by the collider bounds
                // If we've moved the start pos, we've moved the bounds
                float difference = (info.touchStartPos - newInfo.touchStartPos);
                if (Mathf.Abs(difference) > 0)
                {
                    newInfo.touchCageCenter -= difference / 2;
                    newInfo.touchCageSize += difference;
                }
            }

            if (editingEnabled && EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changing push button properties");

                maxPushDistance.floatValue = newInfo.maxPushDistance;
                pressDistance.floatValue = newInfo.pressDistance;
                releaseDistanceDelta.floatValue = newInfo.releaseDistanceDelta;

                boxColliderSize.vector3Value = new Vector3(info.touchCageLocalBounds.size.x, info.touchCageLocalBounds.size.y, newInfo.touchCageSize);
                boxColliderCenter.vector3Value = new Vector3(info.touchCageLocalBounds.center.x, info.touchCageLocalBounds.center.y, newInfo.touchCageCenter);
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
                Color startColor = Handles.color;
                Handles.color = drawArrows ? startColor : Color.Lerp(startColor, Color.clear, 0.5f);
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
                Handles.color = startColor;
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
