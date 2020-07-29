// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    /// <summary>
    /// Class controls inspector rendering logic for the InteractableOnFocus class
    /// </summary>
    [CustomEditor(typeof(InteractableOnFocus))]
    public class InteractableOnFocusInspector : UnityEditor.Editor
    {
        private static readonly GUIContent AddProfileContent = new GUIContent("+ Add New Profile", "Add Visual Profile");
        private static readonly GUIContent RemoveProfileContent = new GUIContent("-", "Remove Profile");

        protected SerializedProperty profilesProperty;
        protected List<InspectorUIUtility.ListSettings> listSettings;

        protected virtual void OnEnable()
        {
            profilesProperty = serializedObject.FindProperty("Profiles");
            listSettings = InspectorUIUtility.AdjustListSettings(null, profilesProperty.arraySize);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InspectorUIUtility.DrawTitle("Profiles");

            if (profilesProperty.arraySize == 0)
            {
                AddProfile();
            }

            for (int i = 0; i < profilesProperty.arraySize; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty profile = profilesProperty.GetArrayElementAtIndex(i);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        SerializedProperty targetGameObject = profile.FindPropertyRelative("Target");
                        EditorGUILayout.PropertyField(targetGameObject, new GUIContent("Target", "Target gameObject for this theme properties to manipulate"));

                        if (InspectorUIUtility.SmallButton(RemoveProfileContent))
                        {
                            profilesProperty.DeleteArrayElementAtIndex(i);
                            serializedObject.ApplyModifiedProperties();
                            continue;
                        }
                    }

                    SerializedProperty theme = profile.FindPropertyRelative("Theme");
                    EditorGUILayout.PropertyField(theme, new GUIContent("Theme", "Theme properties for interaction feedback"));

                    // Render Theme Settings
                    if (theme.objectReferenceValue != null)
                    {
                        InspectorUIUtility.ListSettings settings = listSettings[i];
                        settings.Show = InspectorUIUtility.DrawSectionFoldout("Theme Settings (Click to edit)", listSettings[i].Show);
                        if (settings.Show)
                        {
                            UnityEditor.Editor themeEditor = UnityEditor.Editor.CreateEditor(theme.objectReferenceValue);
                            themeEditor.OnInspectorGUI();
                        }

                        listSettings[i] = settings;
                    }
                }
            }// profile for loop

            if (InspectorUIUtility.RenderIndentedButton(AddProfileContent, EditorStyles.miniButton))
            {
                AddProfile();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AddProfile()
        {
            profilesProperty.InsertArrayElementAtIndex(profilesProperty.arraySize);
            SerializedProperty newProfile = profilesProperty.GetArrayElementAtIndex(profilesProperty.arraySize - 1);

            var theme = newProfile.FindPropertyRelative("Theme");
            theme.objectReferenceValue = null;

            listSettings = InspectorUIUtility.AdjustListSettings(null, profilesProperty.arraySize);
        }
    }
}