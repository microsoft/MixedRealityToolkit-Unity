// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UX;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(PressableButton), true)]
    public class PressableButtonEditor : StatefulInteractableEditor
    {
        // Struct used to store state of preview.
        // This lets us display accurate info while button is being pressed.
        // All vectors / distances are in local space.
        private struct ButtonInfo
        {
            public Vector3 LocalCenter;
            public Vector2 PlaneExtents;

            // The actual values that the button uses
            public float StartPushPlane;
            public float EndPushPlane;
        }

        const string EditingEnabledKey = "MRTK_PressableButtonInspector_EditingEnabledKey";
        const string VisiblePlanesKey = "MRTK_PressableButtonInspector_VisiblePlanesKey";
        private static bool EditingEnabled = false;
        private static bool VisiblePlanes = true;

        private const float labelMouseOverDistance = 0.025f;

        private static GUIStyle labelStyle;

        private PressableButton button;
        private Transform transform;

        private ButtonInfo currentInfo;

        private SerializedProperty distanceSpaceMode;
        private SerializedProperty startPushPlane;
        private SerializedProperty endPushPlane;

        private SerializedProperty smoothSelectedness;
        private SerializedProperty returnSpeed;
        private SerializedProperty extendSpeed;
        private SerializedProperty enforceFrontPush;
        private SerializedProperty rejectXYRolloff;
        private SerializedProperty rolloffXYDepth;
        private SerializedProperty rejectZRolloff;

        private static readonly Vector3[] startPlaneVertices = new Vector3[4];
        private static readonly Vector3[] endPlaneVertices = new Vector3[4];

        protected override void OnEnable()
        {
            base.OnEnable();

            button = (PressableButton)target;
            transform = button.transform;

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.white;
            }

            distanceSpaceMode = serializedObject.FindProperty("distanceSpaceMode");
            startPushPlane = serializedObject.FindProperty("startPushPlane");
            endPushPlane = serializedObject.FindProperty("endPushPlane");

            smoothSelectedness = serializedObject.FindProperty("smoothSelectedness");
            extendSpeed = serializedObject.FindProperty("extendSpeed");
            returnSpeed = serializedObject.FindProperty("returnSpeed");

            enforceFrontPush = serializedObject.FindProperty("enforceFrontPush");
            rejectXYRolloff = serializedObject.FindProperty("rejectXYRolloff");
            rolloffXYDepth = serializedObject.FindProperty("rolloffXYDepth");
            rejectZRolloff = serializedObject.FindProperty("rejectZRolloff");
        }

        [DrawGizmo(GizmoType.Selected)]
        private void OnSceneGUI()
        {

            if (!VisiblePlanes)
            {
                return;
            }

            if (button == null)
            {
                return;
            }

            serializedObject.Update();
            currentInfo = GatherCurrentInfo();
            DrawButtonInfo(currentInfo, EditingEnabled);
        }

        private ButtonInfo GatherCurrentInfo()
        {
            BoxCollider collider = button.GetComponentInChildren<BoxCollider>();
            return new ButtonInfo
            {
                // null coalesce safe as we're checking it in the same frame as we get it!
                LocalCenter = collider?.center ?? Vector3.zero,
                PlaneExtents = collider?.size ?? Vector3.zero,
                StartPushPlane = startPushPlane.floatValue,
                EndPushPlane = endPushPlane.floatValue
            };
        }

        private void DrawButtonInfo(ButtonInfo info, bool editingEnabled)
        {
            if (editingEnabled)
            {
                EditorGUI.BeginChangeCheck();
            }

            var targetBehaviour = (MonoBehaviour)target;
            bool isOpaque = targetBehaviour.isActiveAndEnabled;
            float alpha = (isOpaque) ? 1.0f : 0.5f;

            // START PUSH
            Handles.color = ApplyAlpha(Color.cyan, alpha);
            float newStartPushDistance = DrawPlaneAndHandle(startPlaneVertices, info.PlaneExtents * 0.5f, info.StartPushPlane, info, "Start Push Plane", editingEnabled);
            if (editingEnabled && newStartPushDistance != info.StartPushPlane)
            {
                info.StartPushPlane = Mathf.Min(newStartPushDistance, info.EndPushPlane);
            }

            // MAX PUSH
            var purple = new Color(0.28f, 0.0f, 0.69f);
            Handles.color = ApplyAlpha(purple, alpha);
            float newMaxPushDistance = DrawPlaneAndHandle(endPlaneVertices, info.PlaneExtents * 0.5f, info.EndPushPlane, info, "End Push Plane", editingEnabled);
            if (editingEnabled && newMaxPushDistance != info.EndPushPlane)
            {
                info.EndPushPlane = Mathf.Max(newMaxPushDistance, info.StartPushPlane);
            }

            if (editingEnabled && EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, string.Concat("Modify Button Planes of ", button.name));

                startPushPlane.floatValue = info.StartPushPlane;
                endPushPlane.floatValue = info.EndPushPlane;

                serializedObject.ApplyModifiedProperties();
            }

            // Draw dotted lines showing path from beginning to end of button path
            Handles.color = Color.Lerp(Color.cyan, Color.clear, 0.25f);
            Handles.DrawDottedLine(startPlaneVertices[0], endPlaneVertices[0], 2.5f);
            Handles.DrawDottedLine(startPlaneVertices[1], endPlaneVertices[1], 2.5f);
            Handles.DrawDottedLine(startPlaneVertices[2], endPlaneVertices[2], 2.5f);
            Handles.DrawDottedLine(startPlaneVertices[3], endPlaneVertices[3], 2.5f);
        }

        private float DrawPlaneAndHandle(Vector3[] vertices, Vector2 halfExtents, float distance, ButtonInfo info, string label, bool editingEnabled)
        {
            Vector3 centerWorld = transform.TransformPoint(new Vector3(info.LocalCenter.x, info.LocalCenter.y, button.GetLocalPositionAlongPushDirection(distance).z));
            MakeQuadFromPoint(vertices, centerWorld, halfExtents, info);

            if (VisiblePlanes)
            {
                Handles.DrawSolidRectangleWithOutline(vertices, Color.Lerp(Handles.color, Color.clear, 0.65f), Handles.color);
            }

            // Label
            {
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                var dist = HandleUtility.DistancePointLine(vertices[1], ray.origin, ray.origin + ray.direction * 100.0f);

                if (dist < labelMouseOverDistance)
                {
                    DrawLabel(vertices[1], transform.up - transform.right, label, labelStyle);
                    HandleUtility.Repaint();
                }
            }

            // Draw forward / backward arrows so people know they can drag
            if (editingEnabled)
            {
                float handleSize = HandleUtility.GetHandleSize(vertices[1]) * 0.15f;

                Vector3 planeNormal = button.transform.forward;
                Handles.ArrowHandleCap(0, vertices[1], Quaternion.LookRotation(planeNormal), handleSize * 2, EventType.Repaint);
                Handles.ArrowHandleCap(0, vertices[1], Quaternion.LookRotation(-planeNormal), handleSize * 2, EventType.Repaint);

#if UNITY_2022_1_OR_NEWER
                Vector3 newPosition = Handles.FreeMoveHandle(vertices[1], handleSize, Vector3.zero, Handles.SphereHandleCap);
#else
                Vector3 newPosition = Handles.FreeMoveHandle(vertices[1], Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
#endif

                if (!newPosition.Equals(vertices[1]))
                {
                    distance = button.GetDistanceAlongPushDirection(newPosition);
                }
            }

            return distance;
        }

        static bool advancedButtonFoldout = false;
        static bool editorFoldout = false;

        protected override void DrawProperties()
        {
            base.DrawProperties();

            if (distanceSpaceMode == null) { return; }

            serializedObject.Update();

            advancedButtonFoldout = EditorGUILayout.Foldout(advancedButtonFoldout, EditorGUIUtility.TrTempContent("Volumetric Press Settings"), true, EditorStyles.foldoutHeader);
            if (advancedButtonFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.BeginChangeCheck();
                    var currentMode = distanceSpaceMode.intValue;
                    EditorGUILayout.PropertyField(distanceSpaceMode);

                    // EndChangeCheck returns true when something was selected in the dropdown, but
                    // doesn't necessarily mean that the value itself changed. Check for that too.
                    if (EditorGUI.EndChangeCheck() && currentMode != distanceSpaceMode.intValue)
                    {
                        // Changing the DistanceSpaceMode requires updating the plane distance values so they stay in the same relative ratio positions
                        Undo.RecordObject(target, string.Concat("Trigger Plane Distance Conversion of ", button.name));
                        button.DistanceSpaceMode = (PressableButton.SpaceMode)distanceSpaceMode.intValue;
                        serializedObject.Update();
                    }

                    // Push settings
                    EditorGUILayout.PropertyField(startPushPlane);
                    EditorGUILayout.PropertyField(endPushPlane);

                    // Other settings
                    EditorGUILayout.PropertyField(smoothSelectedness);
                    EditorGUILayout.PropertyField(extendSpeed);
                    EditorGUILayout.PropertyField(returnSpeed);

                    // Rolloff rejection
                    EditorGUILayout.PropertyField(enforceFrontPush);
                    EditorGUILayout.PropertyField(rejectXYRolloff);
                    if (rejectXYRolloff.boolValue)
                    {
                        EditorGUILayout.PropertyField(rolloffXYDepth);
                    }
                    EditorGUILayout.PropertyField(rejectZRolloff);
                }
            }

            // editor settings
            {
                EditorGUI.BeginDisabledGroup(Application.isPlaying == true);
                editorFoldout = EditorGUILayout.Foldout(editorFoldout, EditorGUIUtility.TrTempContent("Button Editor Settings"), true, EditorStyles.foldoutHeader);
                if (editorFoldout)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        var prevVisiblePlanes = SessionState.GetBool(VisiblePlanesKey, true);
                        VisiblePlanes = EditorGUILayout.Toggle("Show Button Event Planes", prevVisiblePlanes);
                        if (VisiblePlanes != prevVisiblePlanes)
                        {
                            SessionState.SetBool(VisiblePlanesKey, VisiblePlanes);
                            EditorUtility.SetDirty(target);
                        }

                        // enable plane editing
                        {
                            EditorGUI.BeginDisabledGroup(VisiblePlanes == false);
                            var prevEditingEnabled = SessionState.GetBool(EditingEnabledKey, false);
                            EditingEnabled = EditorGUILayout.Toggle("Make Planes Editable", EditingEnabled);
                            if (EditingEnabled != prevEditingEnabled)
                            {
                                SessionState.SetBool(EditingEnabledKey, EditingEnabled);
                                EditorUtility.SetDirty(target);
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLabel(Vector3 origin, Vector3 direction, string content, GUIStyle labelStyle)
        {
            Color colorOnEnter = Handles.color;

            float handleSize = HandleUtility.GetHandleSize(origin);
            Vector3 handlePos = origin + (2 * handleSize * direction.normalized);
            Handles.Label(handlePos + (0.1f * handleSize * Vector3.up), content, labelStyle);
            Handles.color = Color.Lerp(colorOnEnter, Color.clear, 0.25f);
            Handles.DrawDottedLine(origin, handlePos, 5f);

            Handles.color = colorOnEnter;
        }

        private void MakeQuadFromPoint(Vector3[] vertices, Vector3 centerWorld, Vector2 halfExtents, ButtonInfo info)
        {
            vertices[0] = transform.TransformVector((new Vector3(-halfExtents.x, -halfExtents.y, 0.0f))) + centerWorld;
            vertices[1] = transform.TransformVector((new Vector3(-halfExtents.x, +halfExtents.y, 0.0f))) + centerWorld;
            vertices[2] = transform.TransformVector((new Vector3(+halfExtents.x, +halfExtents.y, 0.0f))) + centerWorld;
            vertices[3] = transform.TransformVector((new Vector3(+halfExtents.x, -halfExtents.y, 0.0f))) + centerWorld;
        }

        private static Color ApplyAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, color.a * alpha);
        }
    }
}
