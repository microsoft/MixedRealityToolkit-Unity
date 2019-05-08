using Microsoft.MixedReality.Toolkit.SceneSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Draws the scene info struct and populates its hidden fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneInfo))]
    public class SceneInfoDrawer : PropertyDrawer
    {
        const float iconWidth = 40f;
        const float objectFieldRelativeWidth = 0.35f;
        const string errorIconContent = "d_console.erroricon.sml";
        const string warningIconContent = "d_console.warnicon.sml";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assetProperty = property.FindPropertyRelative("Asset");
            SerializedProperty nameProperty = property.FindPropertyRelative("Name");
            SerializedProperty pathProperty = property.FindPropertyRelative("Path");
            SerializedProperty buildIndexProperty = property.FindPropertyRelative("BuildIndex");
            SerializedProperty includedProperty = property.FindPropertyRelative("Included");

            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            EditorGUI.BeginProperty(position, label, property);

            // Indent our rect
            position = EditorGUI.IndentedRect(position);

            // Make room for a toggle control
            Rect toggleRect = position;
            toggleRect.width -= position.width * objectFieldRelativeWidth;

            // Make room for an object field
            Rect objectFieldRect = position;
            objectFieldRect.x += position.width * objectFieldRelativeWidth;
            objectFieldRect.width -= position.width * objectFieldRelativeWidth;

            // Make an icon rect in case we need one
            Rect iconRect = objectFieldRect;
            iconRect.x = objectFieldRect.x + objectFieldRect.width - iconWidth;
            iconRect.width = iconWidth;

            if (Application.isPlaying)
            {   // Don't allow this field to be edited, just draw the object property
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.Toggle(toggleRect, label, includedProperty.boolValue);
                EditorGUI.ObjectField(objectFieldRect, label, assetProperty.objectReferenceValue, typeof(SceneAsset), false);
                EditorGUI.EndDisabledGroup();
                return;
            }

            bool changed = false;

            UnityEngine.Object asset = assetProperty.objectReferenceValue;

            if (asset == null)
            {
                string missingSceneName = nameProperty.stringValue;
                if (!string.IsNullOrEmpty(missingSceneName))
                {
                    // Draw a disabled toggle
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Toggle(toggleRect, includedProperty.boolValue, missingSceneName + " (Missing)");
                    EditorGUI.EndDisabledGroup();

                    var errorContent = EditorGUIUtility.IconContent(errorIconContent);
                    GUI.Label(iconRect, errorContent);

                    objectFieldRect.width -= iconWidth;
                    asset = EditorGUI.ObjectField(objectFieldRect, asset, typeof(SceneAsset), false);
                }
                else
                {
                    // Draw a disabled toggle
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Toggle(toggleRect, false, "(Empty)");
                    EditorGUI.EndDisabledGroup();

                    var errorContent = EditorGUIUtility.IconContent(warningIconContent);
                    GUI.Label(iconRect, errorContent);

                    objectFieldRect.width -= iconWidth;
                    asset = EditorGUI.ObjectField(objectFieldRect, asset, typeof(SceneAsset), false);
                }
            }
            else
            {
                if (buildIndexProperty.intValue >= 0)
                {
                    // Draw a functional toggle
                    string content = nameProperty.stringValue + (includedProperty.boolValue ? " (Build index #" + buildIndexProperty.intValue + ")" : " (Disabled)");
                    bool included = GUI.Toggle(toggleRect, includedProperty.boolValue, content);

                    if (included != includedProperty.boolValue)
                    {
                        // Change the editor build settings right now
                        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                        scenes[buildIndexProperty.intValue].enabled = included;
                        EditorBuildSettings.scenes = scenes;
                    }
                    
                    asset = EditorGUI.ObjectField(objectFieldRect, asset, typeof(SceneAsset), false);
                }
                else
                {
                    // Draw a disabled toggle
                    EditorGUI.BeginDisabledGroup(true);
                    // Draw a functional toggle
                    GUI.Toggle(toggleRect, includedProperty.boolValue, nameProperty.stringValue + " (Not included in build)");
                    EditorGUI.EndDisabledGroup();

                    var errorContent = EditorGUIUtility.IconContent(warningIconContent);
                    GUI.Label(iconRect, errorContent);

                    objectFieldRect.width -= iconWidth;
                    asset = EditorGUI.ObjectField(objectFieldRect, asset, typeof(SceneAsset), false);
                }
            }

            if (asset != assetProperty.objectReferenceValue)
            {
                assetProperty.objectReferenceValue = asset;
                changed = true;
            }

            changed |= RefreshSceneInfo(asset, nameProperty, pathProperty, buildIndexProperty, includedProperty);

            if (changed)
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUIUtility.wideMode = lastMode;
            EditorGUI.EndProperty();
        }

        private static bool RefreshSceneInfo(
            UnityEngine.Object asset, 
            SerializedProperty nameProperty, 
            SerializedProperty pathProperty,
            SerializedProperty buildIndexProperty,
            SerializedProperty includedProperty)
        {
            bool changed = false;

            if (asset == null)
            {
                // Leave the name and path alone, but reset the build index
                if (buildIndexProperty.intValue >= 0)
                {
                    buildIndexProperty.intValue = -1;
                    changed = true;
                }
            }
            else
            {
                if (nameProperty.stringValue != asset.name)
                {
                    nameProperty.stringValue = asset.name;
                    changed = true;
                }

                string scenePath = AssetDatabase.GetAssetPath(asset);
                if (pathProperty.stringValue != scenePath)
                {
                    pathProperty.stringValue = scenePath;
                    changed = true;
                }

                Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
                int buildIndex = scene.buildIndex;
                if (buildIndexProperty.intValue != buildIndex)
                {
                    buildIndexProperty.intValue = buildIndex;
                    changed = true;
                }

                bool included = false;
                if (buildIndex >= 0)
                {
                    EditorBuildSettingsScene buildSettingsScene = EditorBuildSettings.scenes[buildIndex];
                    included = buildSettingsScene.enabled;
                }
                if (included != includedProperty.boolValue)
                {
                    includedProperty.boolValue = included;
                    changed = true;
                }


            }

            return changed;
        }
    }
}