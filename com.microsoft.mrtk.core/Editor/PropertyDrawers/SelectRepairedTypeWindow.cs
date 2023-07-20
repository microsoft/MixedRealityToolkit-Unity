// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A Unity editor window for repairing <see cref="SystemType"/> field serializations. 
    /// </summary>
    public class SelectRepairedTypeWindow : EditorWindow
    {
        private static Type[] repairedTypeOptions;
        private static SerializedProperty property;
        private static SelectRepairedTypeWindow window;

        /// <summary>
        /// Get if a <see cref="SelectRepairedTypeWindow"/> window is opened.
        /// </summary>
        public static bool WindowOpen 
        { 
            get => window != null; 
        }

        /// <summary>
        /// Create and open a new <see cref="SelectRepairedTypeWindow"/> window.
        /// </summary>
        /// <remarks>
        /// Closes a previously opened <see cref="SelectRepairedTypeWindow"/> window.
        /// </remarks>
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

        /// <summary>
        /// A function called by Unity to render and handle GUI events.
        /// </summary>
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

        /// <summary>
        /// A Unity event function that is called when the script component has been disabled.
        /// </summary>
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
