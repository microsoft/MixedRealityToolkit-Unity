// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement.Editor
{
    /// <summary>
    /// Inspector for CompressableButton, includes the logic for the plane distance setting editor tool which allows for modification 
    /// of the Start Push Distance, Max Push Distance, Press Distance and Release Distance.  
    /// 
    /// Note: This is the inspector for PressableButtonHoloLens2.
    /// </summary>
    [CustomEditor(typeof(CompressableButton))]
    public class CompressableButtonInspector : BaseInteractiveElementInspector
    {
        // Struct used to store state of preview.
        // This lets us display accurate info while button is being pressed.
        // All vectors / distances are in local space.
        private struct ButtonInfo
        {
            public Vector3 LocalCenter;
            public Vector2 PlaneExtents;

            // The rotation of the push space.
            public Quaternion PushRotationLocal;

            // The actual values that the button uses
            public float StartPushDistance;
            public float MaxPushDistance;
            public float PressDistance;
            public float ReleaseDistance;
        }

        const string EditingEnabledKey = "MRTK_CompressableButtonInspector_EditingEnabledKey";
        const string VisiblePlanesKey = "MRTK_CompressableButtonInspector_VisiblePlanesKey";
        private static bool EditingEnabled = false;
        private static bool VisiblePlanes = true;

        private const float labelMouseOverDistance = 0.025f;

        private static GUIStyle labelStyle;

        private CompressableButton button;
        private Transform transform;
        private NearInteractionTouchableSurface touchable;

        private ButtonInfo currentInfo;

        private SerializedProperty movingButtonVisuals;
        private SerializedProperty distanceSpaceMode;
        private SerializedProperty startPushDistance;
        private SerializedProperty maxPushDistance;
        private SerializedProperty pressDistance;
        private SerializedProperty releaseDistanceDelta;

        private static readonly Vector3[] startPlaneVertices = new Vector3[4];
        private static readonly Vector3[] endPlaneVertices = new Vector3[4];
        private static readonly Vector3[] pressPlaneVertices = new Vector3[4];
        private static readonly Vector3[] pressStartPlaneVertices = new Vector3[4];
        private static readonly Vector3[] releasePlaneVertices = new Vector3[4];

        private static readonly GUIContent DistanceSpaceModeLabel = new GUIContent("Coordinate Space Mode");
        private static readonly string[] excludeProperties = new string[] { "distanceSpaceMode", "movingButtonVisuals", "m_Script", "active", "states" };

        protected override void OnEnable()
        {
            base.OnEnable();

            button = (CompressableButton)target;
            transform = button.transform;

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.white;
            }

            movingButtonVisuals = serializedObject.FindProperty("movingButtonVisuals");
            distanceSpaceMode = serializedObject.FindProperty("distanceSpaceMode");
            startPushDistance = serializedObject.FindProperty("startPushDistance");
            maxPushDistance = serializedObject.FindProperty("maxPushDistance");
            pressDistance = serializedObject.FindProperty("pressDistance");
            releaseDistanceDelta = serializedObject.FindProperty("releaseDistanceDelta");

            touchable = button.GetComponent<NearInteractionTouchableSurface>();
        }

        [DrawGizmo(GizmoType.Selected)]
        private void OnSceneGUI()
        {
            if (touchable == null)
            {
                // The inspector code will prompt a developer to add a touchable.
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
            Vector3 pressDirLocal = (touchable != null) ? touchable.LocalPressDirection : Vector3.forward;
            Vector3 upDirLocal = Vector3.up;

            if (touchable is NearInteractionTouchable touchableConcrete)
            {
                upDirLocal = touchableConcrete.LocalUp;
            }

            return new ButtonInfo
            {
                LocalCenter = touchable.LocalCenter,
                PlaneExtents = touchable.Bounds,
                PushRotationLocal = Quaternion.LookRotation(pressDirLocal, upDirLocal),
                StartPushDistance = startPushDistance.floatValue,
                MaxPushDistance = maxPushDistance.floatValue,
                PressDistance = pressDistance.floatValue,
                ReleaseDistance = pressDistance.floatValue - releaseDistanceDelta.floatValue
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
            float newStartPushDistance = DrawPlaneAndHandle(startPlaneVertices, info.PlaneExtents * 0.5f, info.StartPushDistance, info, "Start Push Distance", editingEnabled);
            if (editingEnabled && newStartPushDistance != info.StartPushDistance)
            {
                EnforceDistanceOrdering(ref info);
                info.StartPushDistance = ClampStartPushDistance(Mathf.Min(newStartPushDistance, info.ReleaseDistance));
            }

            // RELEASE DISTANCE
            Handles.color = ApplyAlpha(Color.red, alpha);
            float newReleaseDistance = DrawPlaneAndHandle(releasePlaneVertices, info.PlaneExtents * 0.3f, info.ReleaseDistance, info, "Release Distance", editingEnabled);
            if (editingEnabled && newReleaseDistance != info.ReleaseDistance)
            {
                EnforceDistanceOrdering(ref info);
                info.ReleaseDistance = Mathf.Clamp(newReleaseDistance, info.StartPushDistance, info.PressDistance);
            }

            // PRESS DISTANCE
            Handles.color = ApplyAlpha(Color.yellow, alpha);
            float newPressDistance = DrawPlaneAndHandle(pressPlaneVertices, info.PlaneExtents * 0.35f, info.PressDistance, info, "Press Distance", editingEnabled);
            if (editingEnabled && newPressDistance != info.PressDistance)
            {
                EnforceDistanceOrdering(ref info);
                info.PressDistance = Mathf.Clamp(newPressDistance, info.ReleaseDistance, info.MaxPushDistance);
            }

            // MAX PUSH
            var purple = new Color(0.28f, 0.0f, 0.69f);
            Handles.color = ApplyAlpha(purple, alpha);
            float newMaxPushDistance = DrawPlaneAndHandle(endPlaneVertices, info.PlaneExtents * 0.5f, info.MaxPushDistance, info, "Max Push Distance", editingEnabled);
            if (editingEnabled && newMaxPushDistance != info.MaxPushDistance)
            {
                EnforceDistanceOrdering(ref info);
                info.MaxPushDistance = Mathf.Max(newMaxPushDistance, info.PressDistance);
            }

            if (editingEnabled && EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, string.Concat("Modify Button Planes of ", button.name));

                startPushDistance.floatValue = info.StartPushDistance;
                maxPushDistance.floatValue = info.MaxPushDistance;
                pressDistance.floatValue = info.PressDistance;
                releaseDistanceDelta.floatValue = info.PressDistance - info.ReleaseDistance;

                serializedObject.ApplyModifiedProperties();
            }

            // Draw dotted lines showing path from beginning to end of button path
            Handles.color = Color.Lerp(Color.cyan, Color.clear, 0.25f);
            Handles.DrawDottedLine(startPlaneVertices[0], endPlaneVertices[0], 2.5f);
            Handles.DrawDottedLine(startPlaneVertices[1], endPlaneVertices[1], 2.5f);
            Handles.DrawDottedLine(startPlaneVertices[2], endPlaneVertices[2], 2.5f);
            Handles.DrawDottedLine(startPlaneVertices[3], endPlaneVertices[3], 2.5f);
        }

        private void EnforceDistanceOrdering(ref ButtonInfo info)
        {
            info.StartPushDistance = ClampStartPushDistance(Mathf.Min(new[] { info.StartPushDistance, info.ReleaseDistance, info.PressDistance, info.MaxPushDistance }));
            info.ReleaseDistance = Mathf.Min(new[] { info.ReleaseDistance, info.PressDistance, info.MaxPushDistance });
            info.PressDistance = Mathf.Min(info.PressDistance, info.MaxPushDistance);
        }

        private float DrawPlaneAndHandle(Vector3[] vertices, Vector2 halfExtents, float distance, ButtonInfo info, string label, bool editingEnabled)
        {
            Vector3 centerWorld = button.GetWorldPositionAlongPushDirection(distance);
            MakeQuadFromPoint(vertices, centerWorld, halfExtents, info);

            if (VisiblePlanes)
            {
                Handles.DrawSolidRectangleWithOutline(vertices, Color.Lerp(Handles.color, Color.clear, 0.65f), Handles.color);
            }

            // Label
            {
                Vector3 mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToViewportPoint(Event.current.mousePosition);
                mousePosition.y = 1f - mousePosition.y;
                mousePosition.z = 0;
                Vector3 handleVisiblePos = SceneView.currentDrawingSceneView.camera.WorldToViewportPoint(vertices[1]);
                handleVisiblePos.z = 0;

                if (Vector3.Distance(mousePosition, handleVisiblePos) < labelMouseOverDistance)
                {
                    DrawLabel(vertices[1], transform.up - transform.right, label, labelStyle);
                    HandleUtility.Repaint();
                }
            }

            // Draw forward / backward arrows so people know they can drag
            if (editingEnabled)
            {
                float handleSize = HandleUtility.GetHandleSize(vertices[1]) * 0.15f;

                Vector3 dir = (touchable != null) ? touchable.LocalPressDirection : Vector3.forward;
                Vector3 planeNormal = button.transform.TransformDirection(dir);
                Handles.ArrowHandleCap(0, vertices[1], Quaternion.LookRotation(planeNormal), handleSize * 2, EventType.Repaint);
                Handles.ArrowHandleCap(0, vertices[1], Quaternion.LookRotation(-planeNormal), handleSize * 2, EventType.Repaint);

                Vector3 newPosition = Handles.FreeMoveHandle(vertices[1], Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
                if (!newPosition.Equals(vertices[1]))
                {
                    distance = button.GetDistanceAlongPushDirection(newPosition);
                }
            }

            return distance;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            // Ensure there is a touchable.
            if (touchable == null)
            {
                EditorGUILayout.HelpBox($"{target.GetType().Name} requires a {nameof(NearInteractionTouchableSurface)}-derived component on this game object to function.", MessageType.Warning);

                bool isUnityUI = (button.GetComponent<RectTransform>() != null);
                var typeToAdd = isUnityUI ? typeof(NearInteractionTouchableUnityUI) : typeof(NearInteractionTouchable);

                if (GUILayout.Button($"Add {typeToAdd.Name} component"))
                {
                    Undo.RecordObject(target, string.Concat($"Add {typeToAdd.Name}"));
                    var addedComponent = button.gameObject.AddComponent(typeToAdd);
                    touchable = (NearInteractionTouchableSurface)addedComponent;
                }
                else
                {
                    // It won't work without it, return to avoid nullrefs.
                    return;
                }
            }

            // Ensure that the touchable has EventsToReceive set to Touch
            if (touchable.EventsToReceive != TouchableEventType.Touch)
            {
                EditorGUILayout.HelpBox($"The {nameof(NearInteractionTouchableSurface)}-derived component on this game object currently has its EventsToReceive set to '{touchable.EventsToReceive}'.  It must be set to 'Touch' in order for PressableButton to function properly.", MessageType.Warning);

                if (GUILayout.Button("Set EventsToReceive to 'Touch'"))
                {
                    Undo.RecordObject(touchable, string.Concat("Set EventsToReceive to Touch on ", touchable.name));
                    touchable.EventsToReceive = TouchableEventType.Touch;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(movingButtonVisuals);

            // Ensure that there is a moving button visuals in the UnityUI case.  Even if it is not visible, it must be present to receive GraphicsRaycasts.
            if (touchable is NearInteractionTouchableUnityUI)
            {
                if (movingButtonVisuals.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox($"When used with a NearInteractionTouchableUnityUI, a MovingButtonVisuals is required, as it receives the GraphicsRaycast that allows pressing the button with near/hand interactions.  It does not need to be visible, but it must be able to receive GraphicsRaycasts.", MessageType.Warning);
                }
                else
                {
                    var movingVisualGameObject = (GameObject)movingButtonVisuals.objectReferenceValue;
                    var movingGraphic = movingVisualGameObject.GetComponentInChildren<UnityEngine.UI.Graphic>();
                    if (movingGraphic == null)
                    {
                        EditorGUILayout.HelpBox($"When used with a NearInteractionTouchableUnityUI, the MovingButtonVisuals must contain an Image, RawImage, or other Graphic element so that it can receive a GraphicsRaycast.", MessageType.Warning);
                    }
                }
            }

            EditorGUILayout.LabelField("Press Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var currentMode = distanceSpaceMode.intValue;
            EditorGUILayout.PropertyField(distanceSpaceMode);
            // EndChangeCheck returns true when something was selected in the dropdown, but
            // doesn't necessarily mean that the value itself changed. Check for that too.
            if (EditorGUI.EndChangeCheck() && currentMode != distanceSpaceMode.intValue)
            {
                // Changing the DistanceSpaceMode requires updating the plane distance values so they stay in the same relative ratio positions
                Undo.RecordObject(target, string.Concat("Trigger Plane Distance Conversion of ", button.name));
                button.DistanceSpaceMode = (CompressableButton.SpaceMode)distanceSpaceMode.enumValueIndex;
                serializedObject.Update();
            }

            DrawPropertiesExcluding(serializedObject, excludeProperties);

            startPushDistance.floatValue = ClampStartPushDistance(startPushDistance.floatValue);

            // show button state in play mode
            {
                EditorGUI.BeginDisabledGroup(Application.isPlaying == false);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Button State", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Current Push Distance", button.CurrentPushDistance.ToString());
                EditorGUILayout.Toggle("Touching", button.IsTouching);
                EditorGUILayout.Toggle("Pressing", button.IsPressing);
                EditorGUI.EndDisabledGroup();
            }

            // editor settings
            {
                EditorGUI.BeginDisabledGroup(Application.isPlaying == true);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
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

                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool IsMouseOverQuad(ButtonInfo info, Vector3 halfExtents, Vector3 centerLocal)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
            Ray mouseRay = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(mousePosition);

            // Transform to local object space.
            mouseRay.direction = button.transform.InverseTransformDirection(mouseRay.direction);
            mouseRay.origin = button.transform.InverseTransformPoint(mouseRay.origin);

            // Transform to plane space, which transform the plane into the XY plane.
            Quaternion quadRotationInverse = Quaternion.Inverse(info.PushRotationLocal);
            mouseRay.direction = quadRotationInverse * mouseRay.direction;
            mouseRay.origin = quadRotationInverse * (mouseRay.origin - centerLocal);

            // Intersect ray with XY plane.
            Plane xyPlane = new Plane(Vector3.forward, 0.0f);
            if (xyPlane.Raycast(mouseRay, out float intersectionDistance))
            {
                Vector3 intersection = mouseRay.GetPoint(intersectionDistance);
                return (Mathf.Abs(intersection.x) <= halfExtents.x && Mathf.Abs(intersection.y) <= halfExtents.y);
            }

            return false;
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

        private void MakeQuadFromPoint(Vector3[] vertices, Vector3 centerWorld, Vector2 halfExtents, ButtonInfo info)
        {
            Vector3 touchCageOrigin = touchable.LocalCenter;
            touchCageOrigin.z = 0.0f;
            vertices[0] = transform.TransformVector(info.PushRotationLocal * (new Vector3(-halfExtents.x, -halfExtents.y, 0.0f) + touchCageOrigin)) + centerWorld;
            vertices[1] = transform.TransformVector(info.PushRotationLocal * (new Vector3(-halfExtents.x, +halfExtents.y, 0.0f) + touchCageOrigin)) + centerWorld;
            vertices[2] = transform.TransformVector(info.PushRotationLocal * (new Vector3(+halfExtents.x, +halfExtents.y, 0.0f) + touchCageOrigin)) + centerWorld;
            vertices[3] = transform.TransformVector(info.PushRotationLocal * (new Vector3(+halfExtents.x, -halfExtents.y, 0.0f) + touchCageOrigin)) + centerWorld;
        }

        private float ClampStartPushDistance(float startDistance)
        {
            // If the touchable is UnityUI based, then the start distance must be positive.
            if (touchable is NearInteractionTouchableUnityUI && startDistance < 0.0f)
            {
                return 0.0f;
            }
            else
            {
                return startDistance;
            }
        }

        private static Color ApplyAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, color.a * alpha);
        }
    }
}