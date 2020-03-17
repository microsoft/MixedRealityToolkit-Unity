// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public class SelectRepairedTypeWindow : EditorWindow
    {
        private static Type[] repairedTypeOptions;
        private static SerializedProperty property;
        private static SelectRepairedTypeWindow window;

        public static bool WindowOpen { get { return window != null; } }

        public static void Display(Type[] repairedTypeOptions, SerializedProperty property)
        {
            if (window != null)
            {
                window.Close();
            }

            SelectRepairedTypeWindow.repairedTypeOptions = repairedTypeOptions;
            SelectRepairedTypeWindow.property = property;

            window = ScriptableObject.CreateInstance(typeof(SelectRepairedTypeWindow)) as SelectRepairedTypeWindow;
            window.titleContent = new GUIContent("Select repaired type");
            window.ShowUtility();
        }

        private void OnGUI()
        {
            for (int i = 0; i < repairedTypeOptions.Length; i++)
            {
                if (GUILayout.Button(repairedTypeOptions[i].FullName, EditorStyles.miniButton))
                {
                    property.stringValue = SystemType.GetReference(repairedTypeOptions[i]);
                    property.serializedObject.ApplyModifiedProperties();
                    Close();
                }
            }
        }

        private void OnDisable()
        {
            window = null;
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
