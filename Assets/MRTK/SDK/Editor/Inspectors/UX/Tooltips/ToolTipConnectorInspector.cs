// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor(typeof(ToolTipConnector))]
    public class ToolTipConnectorInspector : UnityEditor.Editor
    {
        private const string EditorSettingsFoldoutKey = "MRTK_ToolTipConnector_Inspector_EditorSettings";
        private const string DrawManualDirectionHandleKey = "MRTK_ToopTipConnector_Inspector_DrawManualDirectionHandle";

        private static bool editorSettingsFoldout = false;
        private static float pivotDirectionControlWidth = 50;
        private static float pivotDirectionControlHeight = 35;
        private static bool DrawManualDirectionHandle = false;

        protected ToolTipConnector connector;

        private static readonly GUIContent EditorSettingsContent = new GUIContent("Editor Settings");
        private static GUIStyle multiLineHelpBoxStyle;
        private static float connectorPivotDirectionArrowLength = 1f;

        private SerializedProperty connectorTarget;
        private SerializedProperty connectorFollowType;
        private SerializedProperty pivotMode;
        private SerializedProperty pivotDirection;
        private SerializedProperty pivotDirectionOrient;
        private SerializedProperty manualPivotDirection;
        private SerializedProperty manualPivotLocalPosition;
        private SerializedProperty pivotDistance;

        protected virtual void OnEnable()
        {
            DrawManualDirectionHandle = SessionState.GetBool(DrawManualDirectionHandleKey, DrawManualDirectionHandle);

            editorSettingsFoldout = SessionState.GetBool(EditorSettingsFoldoutKey, editorSettingsFoldout);

            connector = (ToolTipConnector)target;

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (multiLineHelpBoxStyle == null)
            {
                multiLineHelpBoxStyle = new GUIStyle(EditorStyles.helpBox);
                multiLineHelpBoxStyle.wordWrap = true;
            }

            connectorTarget = serializedObject.FindProperty("target");
            connectorFollowType = serializedObject.FindProperty("connectorFollowType");
            pivotMode = serializedObject.FindProperty("pivotMode");
            pivotDirection = serializedObject.FindProperty("pivotDirection");
            pivotDirectionOrient = serializedObject.FindProperty("pivotDirectionOrient");
            manualPivotDirection = serializedObject.FindProperty("manualPivotDirection");
            manualPivotLocalPosition = serializedObject.FindProperty("manualPivotLocalPosition");
            pivotDistance = serializedObject.FindProperty("pivotDistance");

            editorSettingsFoldout = EditorGUILayout.Foldout(editorSettingsFoldout, EditorSettingsContent, true);
            SessionState.SetBool(EditorSettingsFoldoutKey, editorSettingsFoldout);

            if (editorSettingsFoldout)
            {
                EditorGUI.BeginChangeCheck();
                DrawManualDirectionHandle = EditorGUILayout.Toggle("Draw Manual Direction Handle", DrawManualDirectionHandle);

                if (EditorGUI.EndChangeCheck())
                {
                    SessionState.SetBool(DrawManualDirectionHandleKey, DrawManualDirectionHandle);
                }
            }

            if (connectorTarget.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("No target set. ToolTip will not use connector component.", MessageType.Info);
                EditorGUILayout.PropertyField(connectorTarget);
            }
            else
            {
                EditorGUILayout.PropertyField(connectorTarget);

                string helpText = string.Empty;
                switch (connector.ConnectorFollowingType)
                {
                    case ConnectorFollowType.AnchorOnly:
                        helpText = "Only the tooltip's anchor will follow the target. Tooltip content will not be altered.";
                        break;

                    case ConnectorFollowType.Position:
                        helpText = "The entire tooltip will follow the target. Tooltip will not rotate.";
                        break;

                    case ConnectorFollowType.PositionAndYRotation:
                        helpText = "The entire tooltip will follow the target. Tooltip will match target's Y rotation.";
                        break;

                    case ConnectorFollowType.PositionAndXYRotation:
                        helpText = "The entire tooltip will follow the target. Tooltip will match target's X and Y rotation.";
                        break;

                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(helpText, EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(connectorFollowType);

                switch (connector.ConnectorFollowingType)
                {
                    case ConnectorFollowType.AnchorOnly:
                        // Pivot mode doesn't apply to anchor-only connections
                        break;

                    default:
                        EditorGUILayout.PropertyField(pivotMode);
                        switch (connector.PivotMode)
                        {
                            case ConnectorPivotMode.Manual:
                                // We just want to set the pivot ourselves
                                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                EditorGUILayout.LabelField("Pivot will not be altered by connector.", EditorStyles.miniLabel);
                                EditorGUILayout.EndVertical();
                                break;

                            case ConnectorPivotMode.Automatic:
                                EditorGUILayout.PropertyField(pivotDirectionOrient);
                                switch (connector.PivotDirectionOrient)
                                {
                                    case ConnectorOrientType.OrientToCamera:
                                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                        EditorGUILayout.LabelField("Pivot will be set in direction relative to camera.", EditorStyles.miniLabel);
                                        EditorGUILayout.EndVertical();
                                        break;

                                    case ConnectorOrientType.OrientToObject:
                                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                        EditorGUILayout.LabelField("Pivot will be set in direction relative to target.", EditorStyles.miniLabel);
                                        EditorGUILayout.EndVertical();
                                        break;
                                }
                                ConnectorPivotDirection newPivotDirection = DrawPivotDirection(connector.PivotDirection);
                                pivotDirection.intValue = (int)newPivotDirection;
                                switch (connector.PivotDirection)
                                {
                                    case ConnectorPivotDirection.Manual:
                                        EditorGUILayout.PropertyField(manualPivotDirection);
                                        break;

                                    default:
                                        break;
                                }
                                EditorGUILayout.PropertyField(pivotDistance);
                                break;

                            case ConnectorPivotMode.LocalPosition:
                                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                EditorGUILayout.LabelField("Pivot will be set to position relative to target.", EditorStyles.miniLabel);
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.PropertyField(manualPivotLocalPosition);
                                break;
                        }
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private ConnectorPivotDirection DrawPivotDirection(ConnectorPivotDirection selection)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical();

            selection = DrawPivotDirectionButton(ConnectorPivotDirection.Manual, "Manual", selection, 3);

            EditorGUILayout.BeginHorizontal();
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.Northeast, "NE", selection);
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.North, "North", selection);
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.Northwest, "NW", selection);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.East, "East", selection);
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.InFront, "Front", selection);
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.West, "West", selection);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.Southeast, "SE", selection);
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.South, "South", selection);
            selection = DrawPivotDirectionButton(ConnectorPivotDirection.Southwest, "SW", selection);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            return selection;
        }

        private ConnectorPivotDirection DrawPivotDirectionButton(ConnectorPivotDirection direction, string displayName, ConnectorPivotDirection selection, float widthMultiplier = 1)
        {
            GUI.color = (direction == selection) ? Color.white : Color.gray;
            if (GUILayout.Button(
                displayName,
                EditorStyles.miniButtonMid,
                GUILayout.MinWidth(pivotDirectionControlWidth * widthMultiplier),
                GUILayout.MinHeight(pivotDirectionControlHeight),
                GUILayout.MaxWidth(pivotDirectionControlWidth * widthMultiplier),
                GUILayout.MaxHeight(pivotDirectionControlHeight)))
            {
                return direction;
            }
            GUI.color = Color.white;
            return selection;
        }

        protected virtual void OnSceneGUI()
        {
            if (connector.Target == null)
                return;


            ToolTip toolTip = connector.GetComponent<ToolTip>();
            if (toolTip == null)
                return;

            switch (connector.PivotMode)
            {
                case ConnectorPivotMode.Automatic:
                    switch (connector.PivotDirection)
                    {
                        case ConnectorPivotDirection.Manual:
                            // If we're using an automatic / manual combination, draw a handle that lets us set a manual pivot direction
                            Transform targetTransform = connector.Target.transform;
                            Vector3 targetPosition = targetTransform.position;
                            Vector3 pivotPosition = toolTip.PivotPosition;

                            float handleSize = HandleUtility.GetHandleSize(pivotPosition) * connectorPivotDirectionArrowLength;
                            Handles.color = MixedRealityInspectorUtility.LineVelocityColor;
                            Handles.ArrowHandleCap(0, pivotPosition, Quaternion.LookRotation(connector.ManualPivotDirection, targetTransform.up), handleSize, EventType.Repaint);
                            Vector3 newPivotPosition = Handles.PositionHandle(pivotPosition, targetTransform.rotation);

                            if ((newPivotPosition - pivotPosition).sqrMagnitude > 0)
                            {
                                manualPivotDirection = serializedObject.FindProperty("manualPivotDirection");
                                manualPivotDirection.vector3Value = (newPivotPosition - targetPosition).normalized;

                                serializedObject.ApplyModifiedProperties();
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
