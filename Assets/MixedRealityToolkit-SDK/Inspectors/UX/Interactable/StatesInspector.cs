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

        // list of InteractableStates
        protected Type[] stateTypes;

        // list of State names
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

            // get the list of options and InteractableStates
            stateOptions = instance.StateOptions;
            stateTypes = instance.StateTypes;
            
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

                int newEnumIndex = EditorGUILayout.Popup(name.stringValue + " (" + bitCount + ")", enumIndex, stateEnums);
                if (enumIndex != newEnumIndex)
                {
                    name.stringValue = stateEnums[newEnumIndex];
                }

                SmallButton(new GUIContent(minus, "Remove State"), i, RemoveState);
                
                EditorGUILayout.EndHorizontal();

                // assign the bitcount based on location in the list
                bit.intValue = bitCount;
                
                EditorGUILayout.EndVertical();
            }

            FlexButton(new GUIContent("+", "Add Theme Property"), 0, AddState);

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Add a state to the list
        /// </summary>
        /// <param name="index"></param>
        protected void AddState(int index)
        {
            stateList.InsertArrayElementAtIndex(stateList.arraySize);
        }

        /// <summary>
        /// Remove a state from the list
        /// </summary>
        /// <param name="index"></param>
        protected void RemoveState(int index)
        {
            stateList.DeleteArrayElementAtIndex(index);
        }

        /// <summary>
        /// Get a list of state names
        /// </summary>
        /// <returns></returns>
        protected string[] GetStateOptions()
        {
            return Enum.GetNames(typeof(InteractableStates.InteractableStateEnum));
        }
    }
#endif
}
