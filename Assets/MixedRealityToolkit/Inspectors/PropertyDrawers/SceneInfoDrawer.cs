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
        const string errorIconContent = "d_console.erroricon.sml";
        const string warningIconContent = "d_console.warnicon.sml";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assetProperty = property.FindPropertyRelative("Asset");
            SerializedProperty nameProperty = property.FindPropertyRelative("Name");
            SerializedProperty pathProperty = property.FindPropertyRelative("Path");
            SerializedProperty buildIndexProperty = property.FindPropertyRelative("BuildIndex");

            Rect fieldPosition = position;

            if (Application.isPlaying)
            {   // Don't allow this field to be edited, just draw the object property
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(position, label, assetProperty.objectReferenceValue, typeof(SceneAsset), false);
                EditorGUI.EndDisabledGroup();
                return;
            }

            bool changed = false;

            UnityEngine.Object asset = assetProperty.objectReferenceValue;

            if (asset == null)
            {
                string missingSceneName = nameProperty.stringValue;
                if (!string.IsNullOrEmpty (missingSceneName))
                {
                    var errorContent = EditorGUIUtility.IconContent(errorIconContent);
                    GUI.Label(new Rect(position.width, position.y, position.width, position.height), errorContent);
                    fieldPosition = new Rect(position.x, position.y, position.width - iconWidth, position.height);

                    asset = EditorGUI.ObjectField(fieldPosition, missingSceneName + " (Missing)", asset, typeof(SceneAsset), false);
                }
                else
                {
                    var errorContent = EditorGUIUtility.IconContent(warningIconContent);
                    GUI.Label(new Rect(position.width, position.y, position.width, position.height), errorContent);
                    fieldPosition = new Rect(position.x, position.y, position.width - iconWidth, position.height);

                    asset = EditorGUI.ObjectField(fieldPosition, "(Empty)", asset, typeof(SceneAsset), false);
                }
            }
            else
            {
                if (buildIndexProperty.intValue >= 0)
                {
                    asset = EditorGUI.ObjectField(fieldPosition, nameProperty.stringValue + " (" + buildIndexProperty.intValue + ")", asset, typeof(SceneAsset), false);
                }
                else
                {
                    var errorContent = EditorGUIUtility.IconContent(warningIconContent);
                    GUI.Label(new Rect(position.width, position.y, position.width, position.height), errorContent);
                    fieldPosition = new Rect(position.x, position.y, position.width - iconWidth, position.height);

                    asset = EditorGUI.ObjectField(fieldPosition, nameProperty.stringValue + " (No build index)", asset, typeof(SceneAsset), false);
                }
            }

            if (asset != assetProperty.objectReferenceValue)
            {
                assetProperty.objectReferenceValue = asset;
                changed = true;
            }

            changed |= RefreshSceneInfo(asset, nameProperty, pathProperty, buildIndexProperty);

            if (changed)
            {
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private static bool RefreshSceneInfo(UnityEngine.Object asset, SerializedProperty nameProperty, SerializedProperty pathProperty, SerializedProperty buildIndexProperty)
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
            }

            return changed;
        }
    }
}