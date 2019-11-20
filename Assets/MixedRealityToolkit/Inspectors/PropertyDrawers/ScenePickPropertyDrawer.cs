// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Creates a custom picker based on the list of scene in the build settings.
    /// </summary>
    /// <example>
    /// <code>
    /// [ScenePick]
    /// public int SceneId;
    /// </code>
    /// </example>
    [CustomPropertyDrawer(typeof(ScenePickAttribute))]
    public class ScenePickPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// List of Options extracted from the Editor
        /// </summary>
        private static GUIContent[] Options;

        /// <summary>
        /// List of Scene GUIDS for the scenes
        /// </summary>
        private static string[] PropertyData;

        /// <summary>
        /// Select this option to remove the event string
        /// </summary>
        private static readonly string UnselectedText = "-- None --";

        /// <summary>
        /// Text to display when an entry is missing
        /// </summary>
        private static readonly string MissingText = "-- Missing --";

        /// <summary>
        /// Function called by unity to draw the GUI for this property
        /// We are replacing the int value of the backing field with a dropdown list of scene names
        /// </summary>
        /// <param name="position">See base class</param>
        /// <param name="property">See base class</param>
        /// <param name="label">See base class</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            BuildOptions();

            var currentGuid = property.stringValue.Split(';')[0];

            var currentId = System.Array.FindIndex(PropertyData, (x) => x.Contains(currentGuid));

            if (currentId == -1)
            {
                // Not found, display the missing text
                currentId = Options.Length - 1;
            }
            else if (currentId > 0 && property.stringValue != PropertyData[currentId])
            {
                // If the string has changed, update the property.
                // This will happen if the scene is renamed.
                property.stringValue = PropertyData[currentId];
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.BeginProperty(position, new GUIContent(property.name), property);
            var newId = EditorGUI.Popup(position, label, currentId, Options);

            if (newId != currentId)
            {
                property.stringValue = PropertyData[newId];
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();

        }

        /// <summary>
        /// Build the list of scene names
        /// Note: Scene 0 is the no-scene option.
        /// </summary>
        private static void BuildOptions()
        {
            var scenes = EditorBuildSettings.scenes;

            if (scenes.Length > 0)
            {
                Options = new GUIContent[scenes.Length + 2];
                PropertyData = new string[scenes.Length + 2];

                Options[0] = new GUIContent(UnselectedText);
                PropertyData[0] = string.Empty;

                for (int i = 0; i < scenes.Length; i++)
                {
                    // Right, replace '/' with '\' otherwise the list displays like a menu where '/' denotes a sub-menu.
                    Options[i + 1] = new GUIContent(scenes[i].path.Replace("/", "\\"));
                    PropertyData[i + 1] = scenes[i].guid.ToString() + ";" + scenes[i].path;
                }

                Options[scenes.Length + 1] = new GUIContent(MissingText);
                PropertyData[scenes.Length + 1] = MissingText;
            }
            else
            {
                Options = new GUIContent[2];
                PropertyData = new string[2];

                Options[0] = new GUIContent(UnselectedText);
                PropertyData[0] = string.Empty;
                Options[1] = new GUIContent(MissingText);
                PropertyData[1] = MissingText;
            }
        }
    }
}