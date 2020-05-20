// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor(typeof(ToolTip))]
    public class ToolTipInspector : UnityEditor.Editor
    {
        private const float handleSizeMultiplier = 0.35f;

        private const string EditorSettingsFoldoutKey = "MRTK_ToolTip_Inspector_EditorSettings";
        private const string DrawAttachPointsKey = "MRTK_ToopTip_Inspector_DrawAttachPoints";
        private const string DrawHandlesKey = "MRTK_ToopTip_Inspector_DrawHandles";
        private const string EditAttachPointKey = "MRTK_ToopTip_Inspector_EditAttachPoint";
        private const string ContentSettingsFoldoutKey = "MRTK_ToolTip_Inspector_ContentSettings";
        private const string BasicSettingsFoldoutKey = "MRTK_ToolTip_Inspector_BasicSettings";
        private const string GroupSettingsFoldoutKey = "MRTK_ToolTip_Inspector_GroupSettings";
        private const string ObjectSettingsFoldoutKey = "MRTK_ToolTip_Inspector_Objects";

        private static bool editorSettingsFoldout = false;
        private static bool contentSettingsFoldout = true;
        private static bool basicSettingsFoldout = true;
        private static bool groupSettingsFoldout = false;
        private static bool objectSettingsFoldout = false;

        private static readonly GUIContent EditorSettingsContent = new GUIContent("Editor Settings");
        private static readonly GUIContent ContentSettingsContent = new GUIContent("Content Settings");
        private static readonly GUIContent BasicSettingsContent = new GUIContent("Basic Settings");
        private static readonly GUIContent GroupSettingsContent = new GUIContent("Group Display Settings");
        private static readonly GUIContent ObjectSettingsContent = new GUIContent("Object Settings");

        private static bool DrawAttachPoints = false;
        private static bool DrawHandles = true;
        private static bool EditAttachPoint = false;
        private static Vector3[] localAttachPointPositions;

        protected ToolTip toolTip;

        private SerializedProperty toolTipText;
        private SerializedProperty backgroundPadding;
        private SerializedProperty backgroundOffset;
        private SerializedProperty contentScale;
        private SerializedProperty fontSize;
        private SerializedProperty anchor;
        private SerializedProperty pivot;
        private SerializedProperty contentParent;
        private SerializedProperty label;
        private SerializedProperty toolTipLine;
        private SerializedProperty showBackground;
        private SerializedProperty showHighlight;
        private SerializedProperty showConnector;
        private SerializedProperty tipState;
        private SerializedProperty groupTipState;
        private SerializedProperty masterTipState;
        private SerializedProperty attachPointType;
        private SerializedProperty attachPointOffset;

        protected virtual void OnEnable()
        {
            DrawAttachPoints = SessionState.GetBool(DrawAttachPointsKey, DrawAttachPoints);
            DrawHandles = SessionState.GetBool(DrawHandlesKey, DrawHandles);
            EditAttachPoint = SessionState.GetBool(EditAttachPointKey, EditAttachPoint);

            basicSettingsFoldout = SessionState.GetBool(BasicSettingsFoldoutKey, basicSettingsFoldout);
            groupSettingsFoldout = SessionState.GetBool(GroupSettingsFoldoutKey, groupSettingsFoldout);
            contentSettingsFoldout = SessionState.GetBool(ContentSettingsFoldoutKey, contentSettingsFoldout);
            editorSettingsFoldout = SessionState.GetBool(EditorSettingsFoldoutKey, editorSettingsFoldout);
            objectSettingsFoldout = SessionState.GetBool(ObjectSettingsFoldoutKey, objectSettingsFoldout);

            toolTip = (ToolTip)target;

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            if (target != null)
            {
                InspectorUIUtility.RenderHelpURL(target.GetType());
            }

            serializedObject.Update();

            toolTipText = serializedObject.FindProperty("toolTipText");
            backgroundPadding = serializedObject.FindProperty("backgroundPadding");
            backgroundOffset = serializedObject.FindProperty("backgroundOffset");
            contentScale = serializedObject.FindProperty("contentScale");
            fontSize = serializedObject.FindProperty("fontSize");
            anchor = serializedObject.FindProperty("anchor");
            pivot = serializedObject.FindProperty("pivot");
            contentParent = serializedObject.FindProperty("contentParent");
            label = serializedObject.FindProperty("label");
            toolTipLine = serializedObject.FindProperty("toolTipLine");
            showBackground = serializedObject.FindProperty("showBackground");
            showHighlight = serializedObject.FindProperty("showHighlight");
            showConnector = serializedObject.FindProperty("showConnector");
            tipState = serializedObject.FindProperty("tipState");
            groupTipState = serializedObject.FindProperty("groupTipState");
            masterTipState = serializedObject.FindProperty("masterTipState");
            attachPointType = serializedObject.FindProperty("attachPointType");
            attachPointOffset = serializedObject.FindProperty("attachPointOffset");

            bool hasAnchor = anchor.objectReferenceValue != null;
            bool hasPivot = pivot.objectReferenceValue != null;
            bool hasContentParent = contentParent.objectReferenceValue != null;
            bool hasLabel = label.objectReferenceValue != null;
            bool hasToolTipLine = toolTipLine.objectReferenceValue != null;
            bool hasAllObjects = (hasAnchor & hasPivot & hasContentParent & hasLabel & hasToolTipLine);

            editorSettingsFoldout = EditorGUILayout.Foldout(editorSettingsFoldout, EditorSettingsContent, true);
            SessionState.SetBool(EditorSettingsFoldoutKey, editorSettingsFoldout);

            if (editorSettingsFoldout)
            {
                EditorGUI.BeginChangeCheck();
                DrawAttachPoints = EditorGUILayout.Toggle("Draw Attach Points", DrawAttachPoints);
                DrawHandles = EditorGUILayout.Toggle("Draw Handles", DrawHandles);

                if (DrawHandles)
                {
                    EditAttachPoint = EditorGUILayout.Toggle("Edit Attach Point", EditAttachPoint);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    SessionState.SetBool(DrawAttachPointsKey, DrawAttachPoints);
                    SessionState.SetBool(DrawHandlesKey, DrawHandles);
                    SessionState.SetBool(EditAttachPointKey, EditAttachPoint);
                }
            }

            contentSettingsFoldout = EditorGUILayout.Foldout(contentSettingsFoldout, ContentSettingsContent, true);
            SessionState.SetBool(ContentSettingsFoldoutKey, contentSettingsFoldout);

            if (contentSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(toolTipText);
                EditorGUILayout.PropertyField(backgroundPadding);
                EditorGUILayout.PropertyField(backgroundOffset);
                EditorGUILayout.PropertyField(contentScale);
                EditorGUILayout.PropertyField(fontSize);
                EditorGUILayout.PropertyField(attachPointOffset);

                EditorGUI.indentLevel--;
            }

            basicSettingsFoldout = EditorGUILayout.Foldout(basicSettingsFoldout, BasicSettingsContent, true);
            SessionState.SetBool(BasicSettingsFoldoutKey, basicSettingsFoldout);

            if (basicSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(showBackground);
                EditorGUILayout.PropertyField(showConnector);
                EditorGUILayout.PropertyField(showHighlight);
                EditorGUILayout.PropertyField(attachPointType);

                EditorGUI.indentLevel--;
            }

            groupSettingsFoldout = EditorGUILayout.Foldout(groupSettingsFoldout, GroupSettingsContent, true);
            SessionState.SetBool(GroupSettingsFoldoutKey, groupSettingsFoldout);

            if (groupSettingsFoldout)
            {
                EditorGUILayout.HelpBox("Higher states will override lower states unless set to 'None.'", MessageType.Info);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(masterTipState);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(groupTipState);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(tipState);

                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Final Tip State: " + (toolTip.IsOn ? "On" : "Off"), EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            objectSettingsFoldout = EditorGUILayout.Foldout(objectSettingsFoldout, ObjectSettingsContent, true);
            SessionState.SetBool(ObjectSettingsFoldoutKey, objectSettingsFoldout);

            if (objectSettingsFoldout || !hasAllObjects)
            {
                EditorGUI.indentLevel++;

                if (!hasAllObjects)
                {
                    EditorGUILayout.HelpBox("ToolTip will not function unless all objects are present.", MessageType.Warning);
                }

                EditorGUILayout.PropertyField(anchor);
                EditorGUILayout.PropertyField(pivot);
                EditorGUILayout.PropertyField(contentParent);
                EditorGUILayout.PropertyField(label);
                EditorGUILayout.PropertyField(toolTipLine);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        protected virtual void OnSceneGUI()
        {
            if (DrawAttachPoints)
            {
                Handles.color = Color.Lerp(Color.clear, Color.red, 0.5f);
                float scale = toolTip.ContentScale * 0.01f;

                ToolTipUtility.GetAttachPointPositions(ref localAttachPointPositions, toolTip.LocalContentSize);
                foreach (Vector3 attachPoint in localAttachPointPositions)
                {
                    Handles.SphereHandleCap(0, toolTip.ContentParentTransform.TransformPoint(attachPoint), Quaternion.identity, scale, EventType.Repaint);
                }
            }

            if (DrawHandles)
            {
                ToolTip toolTip = (ToolTip)target;
                float handleSize = 0;
                float arrowSize = 0;

                BaseMixedRealityLineDataProvider line = toolTip.GetComponent<BaseMixedRealityLineDataProvider>();
                if (line == null)
                {
                    Handles.color = Color.white;
                    Handles.DrawDottedLine(toolTip.AnchorPosition, toolTip.AttachPointPosition, 5f);
                }

                EditorGUI.BeginChangeCheck();

                Handles.color = Color.cyan;
                handleSize = HandleUtility.GetHandleSize(toolTip.PivotPosition) * handleSizeMultiplier;
                arrowSize = handleSize * 2;
                Vector3 newPivotPosition = Handles.FreeMoveHandle(toolTip.PivotPosition, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
                Handles.ArrowHandleCap(0, newPivotPosition, Quaternion.LookRotation(Vector3.up), arrowSize, EventType.Repaint);
                Handles.ArrowHandleCap(0, newPivotPosition, Quaternion.LookRotation(Vector3.forward), arrowSize, EventType.Repaint);
                Handles.ArrowHandleCap(0, newPivotPosition, Quaternion.LookRotation(Vector3.right), arrowSize, EventType.Repaint);

                Handles.color = Color.white;
                Handles.Label(newPivotPosition + (Vector3.up * arrowSize), "Pivot", EditorStyles.whiteLabel);

                Handles.color = Color.cyan;
                handleSize = HandleUtility.GetHandleSize(toolTip.AnchorPosition) * handleSizeMultiplier;
                arrowSize = handleSize * 2;
                Vector3 newAnchorPosition = Handles.FreeMoveHandle(toolTip.AnchorPosition, Quaternion.identity, HandleUtility.GetHandleSize(toolTip.AnchorPosition) * handleSizeMultiplier, Vector3.zero, Handles.SphereHandleCap);
                Handles.ArrowHandleCap(0, newAnchorPosition, Quaternion.LookRotation(Vector3.up), arrowSize, EventType.Repaint);
                Handles.ArrowHandleCap(0, newAnchorPosition, Quaternion.LookRotation(Vector3.forward), arrowSize, EventType.Repaint);
                Handles.ArrowHandleCap(0, newAnchorPosition, Quaternion.LookRotation(Vector3.right), arrowSize, EventType.Repaint);

                Handles.color = Color.white;
                Handles.Label(newAnchorPosition + (Vector3.up * arrowSize), "Anchor", EditorStyles.whiteLabel);

                if (EditorGUI.EndChangeCheck())
                {
                    if (newAnchorPosition != toolTip.AnchorPosition)
                    {
                        Undo.RegisterCompleteObjectUndo(toolTip.Anchor.transform, "Moved Anchor");
                        toolTip.Anchor.transform.position = newAnchorPosition;
                    }

                    if (newPivotPosition != toolTip.PivotPosition)
                    {
                        Undo.RegisterCompleteObjectUndo(toolTip.Pivot.transform, "Moved Pivot");
                        toolTip.Pivot.transform.position = newPivotPosition;
                    }
                }

                if (EditAttachPoint)
                {
                    EditorGUI.BeginChangeCheck();

                    Handles.color = Color.cyan;
                    handleSize = HandleUtility.GetHandleSize(toolTip.AttachPointPosition) * handleSizeMultiplier;
                    arrowSize = handleSize * 2;
                    Vector3 newAttachPointPosition = Handles.FreeMoveHandle(toolTip.AttachPointPosition, Quaternion.identity, HandleUtility.GetHandleSize(toolTip.AttachPointPosition) * handleSizeMultiplier, Vector3.zero, Handles.SphereHandleCap);
                    Handles.ArrowHandleCap(0, newAttachPointPosition, Quaternion.LookRotation(Vector3.up), arrowSize, EventType.Repaint);
                    Handles.ArrowHandleCap(0, newAttachPointPosition, Quaternion.LookRotation(Vector3.forward), arrowSize, EventType.Repaint);
                    Handles.ArrowHandleCap(0, newAttachPointPosition, Quaternion.LookRotation(Vector3.right), arrowSize, EventType.Repaint);

                    Handles.color = Color.white;
                    Handles.Label(newAttachPointPosition + (Vector3.up * arrowSize), "Attach Point", EditorStyles.whiteLabel);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RegisterCompleteObjectUndo(toolTip, "Moved Attach Point");
                        Undo.RegisterCompleteObjectUndo(toolTip.Anchor.transform, "Moved Attach Point");
                        toolTip.AttachPointPosition = newAttachPointPosition;
                    }
                }
            }
        }
    }
}
