// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    [CustomEditor(typeof(States))]
    public class StatesInspector : UnityEditor.Editor
    {
        protected States instance;
        protected SerializedProperty stateList;

        private static GUIContent RemoveStateLabel;
        private static readonly GUIContent AddStateLabel = new GUIContent("+", "Add State");

        protected virtual void OnEnable()
        {
            instance = (States)target;

            RemoveStateLabel = new GUIContent(InspectorUIUtility.Minus, "Remove State");
            stateList = serializedObject.FindProperty("stateList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorUIUtility.DrawTitle("States");
            EditorGUILayout.HelpBox("Manage state configurations to drive Interactables or Transitions", MessageType.None);

            SerializedProperty stateModelClassName = serializedObject.FindProperty("StateModelClassName");
            SerializedProperty assemblyQualifiedName = serializedObject.FindProperty("AssemblyQualifiedName");

            var stateModelTypes = TypeCacheUtility.GetSubClasses<BaseStateModel>();
            var stateModelClassNames = stateModelTypes.Select(t => t?.Name).ToArray();
            int id = Array.IndexOf(stateModelClassNames, stateModelClassName.stringValue);

            Rect stateModelPos = EditorGUILayout.GetControlRect();
            using (new EditorGUI.PropertyScope(stateModelPos, new GUIContent("State Model"), stateModelClassName))
            {
                int newId = EditorGUILayout.Popup("State Model", id, stateModelClassNames);
                if (id != newId)
                {
                    Type newType = stateModelTypes[newId];
                    stateModelClassName.stringValue = newType.Name;
                    assemblyQualifiedName.stringValue = newType.AssemblyQualifiedName;
                }
            }

            for (int i = 0; i < stateList.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty stateItem = stateList.GetArrayElementAtIndex(i);

                    SerializedProperty name = stateItem.FindPropertyRelative("Name");
                    SerializedProperty activeIndex = stateItem.FindPropertyRelative("ActiveIndex");
                    SerializedProperty bit = stateItem.FindPropertyRelative("Bit");
                    SerializedProperty index = stateItem.FindPropertyRelative("Index");

                    // assign the bitcount based on location in the list as power of 2
                    bit.intValue = 1 << i;

                    activeIndex.intValue = i;

                    Rect position = EditorGUILayout.GetControlRect();
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var label = new GUIContent(name.stringValue + " (" + bit.intValue + ")");
                        using (new EditorGUI.PropertyScope(position, new GUIContent(), name))
                        {
                            string[] stateEnums = Enum.GetNames(typeof(InteractableStates.InteractableStateEnum));
                            int enumIndex = Array.IndexOf(stateEnums, name.stringValue);

                            int newEnumIndex = EditorGUILayout.Popup(label, enumIndex, stateEnums);
                            if (newEnumIndex == -1) { newEnumIndex = 0; }

                            name.stringValue = stateEnums[newEnumIndex];
                            index.intValue = newEnumIndex;
                        }

                        if (InspectorUIUtility.SmallButton(RemoveStateLabel))
                        {
                            stateList.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                }
            }

            if (InspectorUIUtility.FlexButton(AddStateLabel))
            {
                stateList.InsertArrayElementAtIndex(stateList.arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
