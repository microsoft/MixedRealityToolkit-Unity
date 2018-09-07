// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
#if UNITY_EDITOR
    [CustomEditor(typeof(States))]
    public class StatesInspector : InspectorBase
    {
        protected States instance;
        protected SerializedProperty stateList;

        protected Type[] stateTypes;
        protected string[] stateOptions;
        
        protected virtual void OnEnable()
        {
            instance = (States)target;
            
            stateList = serializedObject.FindProperty("StateList");
            AdjustListSettings(stateList.arraySize);
            instance.SetupStateOptions();
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            
            DrawTitle("States");
            DrawNotice("Manage state configurations to drive Interactables or Tansitions");

            // get the list of options
            stateOptions = instance.StateOptions;// SerializedPropertyToOptions(options);

            stateTypes = instance.StateTypes;// serializedObject.FindProperty("StateTypes");
            
            SerializedProperty stateLogicName = serializedObject.FindProperty("StateLogicName");

            int option = States.ReverseLookup(stateLogicName.stringValue, stateOptions);

            int newLogic = EditorGUILayout.Popup("State Model", option, stateOptions);
            if (option != newLogic)
            {
                stateLogicName.stringValue = stateOptions[newLogic];
            }

            int bitCount = 0;
            for (int i = 0; i < stateList.arraySize; i++)
            {
                if (i == 0)
                {
                    bitCount += 1;
                }
                else
                {
                    bitCount += bitCount;
                }

                EditorGUILayout.BeginVertical("Box");
                SerializedProperty stateItem = stateList.GetArrayElementAtIndex(i);

                SerializedProperty name = stateItem.FindPropertyRelative("Name");
                SerializedProperty index = stateItem.FindPropertyRelative("ActiveIndex");
                SerializedProperty bit = stateItem.FindPropertyRelative("Bit");

                index.intValue = i;

                EditorGUILayout.BeginHorizontal();
                string[] stateEnums = GetStateOptions();
                int enumIndex = States.ReverseLookup(name.stringValue, stateEnums);

                int newEnumIndex = EditorGUILayout.Popup("State: " + name.stringValue, enumIndex, stateEnums);
                if (enumIndex != newEnumIndex)
                {
                    name.stringValue = stateEnums[newEnumIndex];
                }

                //name.stringValue = EditorGUILayout.TextField(new GUIContent("Name", "Display name for the state"), name.stringValue);
                SmallButton(new GUIContent(minus, "Remove State"), i, RemoveState);

                /*
                if (GUILayout.Button(new GUIContent(minus, "Remove State"), smallButton, GUILayout.Width(minusButtonWidth)))
                {
                    RemoveState(i);
                }*/
                EditorGUILayout.EndHorizontal();

                bit.intValue = bitCount;// EditorGUILayout.IntField(new GUIContent("Bit", "Bitwise value of the state, used for comparing state combinations"), bit.intValue);
                
                EditorGUILayout.EndVertical();
            }

            RemoveButton(new GUIContent("+", "Add Theme Property"), 0, AddState);

            serializedObject.ApplyModifiedProperties();
        }

        protected void AddState(int index)
        {
            stateList.InsertArrayElementAtIndex(stateList.arraySize);
        }

        protected void RemoveState(int index)
        {
            stateList.DeleteArrayElementAtIndex(index);
        }

        protected string[] GetStateOptions()
        {
            return Enum.GetNames(typeof(InteractableStates.InteractableStateEnum));
        }
    }
#endif
}
