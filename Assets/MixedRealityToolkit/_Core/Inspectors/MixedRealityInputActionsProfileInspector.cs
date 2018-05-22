// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    //TODO - make it prettier
    [CustomEditor(typeof(MixedRealityInputActionsProfile))]
    public class MixedRealityInputActionsProfileInspector : Editor
    {
        Texture2D logo = null;
        private void Awake()
        {
            logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MRTK_Logo.png", typeof(Texture2D));
        }

        public override void OnInspectorGUI()
        {
            //Show MRTK Logo
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logo, GUILayout.MaxHeight(150));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Display InputActions items
            serializedObject.Update();
            //GUILayout.Label("Input Actions");
            EditorList.Show(serializedObject.FindProperty("inputActions"));
            serializedObject.ApplyModifiedProperties();
        }

    }

    public static class EditorList
    {
        public static void Show(SerializedProperty list)
        {
            EditorGUILayout.PropertyField(list);

            //EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            }
            if (GUILayout.Button(addButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
            }
        }
        private static GUIContent addButtonContent = new GUIContent("+", "add element");
    }
}
