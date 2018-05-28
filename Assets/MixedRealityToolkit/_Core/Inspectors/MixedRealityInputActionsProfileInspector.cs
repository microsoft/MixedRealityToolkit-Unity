// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityInputActionsProfile))]
    public class MixedRealityInputActionsProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Remove Action");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Add a New Action", "Add New Action");
        private static readonly GUIContent PointerContent = new GUIContent("Pointer Action", "The action to use for pointing events:\nOnPointerUp, OnPointerDown, OnPointerClick, etc.");

        private static Vector2 scrollPosition = Vector2.zero;

        private MixedRealityInputActionsProfile profile;

        private SerializedProperty inputActionList;
        private SerializedProperty pointerAction;
        private SerializedProperty pointerActionId;

        private void OnEnable()
        {
            profile = (MixedRealityInputActionsProfile)target;
            inputActionList = serializedObject.FindProperty("inputActions");
            pointerAction = serializedObject.FindProperty("pointerAction");
            pointerActionId = pointerAction.FindPropertyRelative("id");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

            EditorGUILayout.LabelField("Input Actions", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Input Actions are any/all actions your users will be able to make when interacting with your application.\n\n" +
                                    "After defining all your actions, you can then wire up these actions to hardware sensors, controllers, and other input devices.", MessageType.Info);

            RenderList(inputActionList);

            EditorGUILayout.LabelField("Input Action Handlers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("These actions raise specific events via the Mixed Reality Input System.", MessageType.Info);

            GUIContent[] actionLabels = profile.InputActions.Select(action => new GUIContent(action.Description)).Prepend(new GUIContent("None")).ToArray();
            int[] actionIds = profile.InputActions.Select(action => (int)action.Id).Prepend(0).ToArray();

            pointerActionId.intValue = EditorGUILayout.IntPopup(PointerContent, CheckValue(pointerActionId.intValue, profile.InputActions.Length), actionLabels, actionIds);

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Checks if value is above a certain amount, then sets it back to zero if true.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="against"></param>
        /// <returns></returns>
        private static int CheckValue(int value, int against)
        {
            if (value > against)
            {
                value = 0;
            }

            return value;
        }

        private static void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            if (GUILayout.Button(AddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var inputAction = list.GetArrayElementAtIndex(list.arraySize - 1);
                var inputActionId = inputAction.FindPropertyRelative("id");
                var inputActionDescription = inputAction.FindPropertyRelative("description");
                inputActionDescription.stringValue = $"New Action {inputActionId.intValue = list.arraySize}";
            }

            GUILayout.Space(12f);

            var lineHeight = list.arraySize * 20f;
            if (lineHeight < 64f) { lineHeight = 64f; }
            if (lineHeight > 128f) { lineHeight = 128f; }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(false), GUILayout.Height(lineHeight));

            GUILayout.BeginVertical();
            for (int i = 0; i < list?.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var previousLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 64f;
                SerializedProperty inputAction = list.GetArrayElementAtIndex(i);
                SerializedProperty inputActionDescription = inputAction.FindPropertyRelative("description");
                SerializedProperty inputActionId = inputAction.FindPropertyRelative("id");
                EditorGUILayout.PropertyField(inputActionDescription, new GUIContent($"Action {inputActionId.intValue = i + 1}"));
                EditorGUIUtility.labelWidth = previousLabelWidth;

                if (GUILayout.Button(MinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
