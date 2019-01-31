// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities.ToolTips
{
    [CustomEditor(typeof(ToolTip))]
    public class ToolTipInspector : Editor
    {
        private const string EditorSettingsFoldoutKey = "MRTK_ToolTip_Inspector_EditorSettings";
        private const string DrawAttachPointsKey = "MRTK_ToopTip_Inspector_DrawAttachPoints";
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

        protected virtual void OnEnable()
        {
            DrawAttachPoints = SessionState.GetBool(DrawAttachPointsKey, DrawAttachPoints);

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

                if (EditorGUI.EndChangeCheck())
                {
                    SessionState.SetBool(DrawAttachPointsKey, DrawAttachPoints);
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
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Higher states will override lower states unless set to 'None.'", EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();

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
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("Warning: ToolTip will not function unless all objects are present.", EditorStyles.miniLabel);
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.PropertyField(anchor);
                EditorGUILayout.PropertyField(pivot);
                EditorGUILayout.PropertyField(contentParent);
                EditorGUILayout.PropertyField(label);
                EditorGUILayout.PropertyField(toolTipLine);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(toolTip);
        }

        protected virtual void OnSceneGUI()
        {
            if (DrawAttachPoints)
            {
                Handles.color = Color.Lerp (Color.clear, Color.red, 0.5f);
                float scale = toolTip.ContentScale * 0.01f;

                ToolTipUtility.GetAttachPointPositions(ref localAttachPointPositions, toolTip.LocalContentSize);
                foreach (Vector3 attachPoint in localAttachPointPositions)
                {
                    Handles.SphereHandleCap(0, toolTip.ContentParentTransform.TransformPoint(attachPoint), Quaternion.identity, scale, EventType.Repaint);
                }
            }
        }
    }
}
