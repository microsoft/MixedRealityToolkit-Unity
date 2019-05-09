using Boo.Lang;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Draws the scene info struct and populates its hidden fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneInfo))]
    public class SceneInfoDrawer : PropertyDrawer
    {
        /// <summary>
        /// Used to control whether to draw the tag property.
        /// All scenes can have tags, but they're not always relevant based on how the scene is being used.
        /// Not sure how much I like this method of controlling property drawing since it could result in unpredictable behavior in inspectors.
        /// We could add an enum or bool to the SceneInfo struct to control this, but that seemed like unnecessary clutter.
        /// </summary>
        public static bool DrawTagProperty { get; set; }

        const float iconWidth = 20f;
        const float totalPropertyWidth = 410;
        const float assetPropertyWidth = 400;
        const float tagPropertyWidth = 400;
        const float buttonPropertyWidth = 400;
        const float assetLabelWidth = 150;
        const float tagLabelWidth = 40;

        const string enabledIconContent = "TestPassed";
        const string missingIconContent = "TestIgnored";
        const string disabledIconContent = "TestNormal";
        const string warningIconContent = "TestInconclusive";
        const string errorIconContent = "TestFailed";

        static readonly RectOffset boxOffset = EditorStyles.helpBox.padding;
        static readonly GUIStyle italicStyle = EditorStyles.label;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (DrawTagProperty)
            {
                return ((EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * 4);
            }
            else
            {
                return ((EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * 3);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assetProperty = property.FindPropertyRelative("Asset");
            SerializedProperty nameProperty = property.FindPropertyRelative("Name");
            SerializedProperty pathProperty = property.FindPropertyRelative("Path");
            SerializedProperty buildIndexProperty = property.FindPropertyRelative("BuildIndex");
            SerializedProperty enabledProperty = property.FindPropertyRelative("Enabled");
            SerializedProperty tagProperty = property.FindPropertyRelative("Tag");

            bool lastMode = EditorGUIUtility.wideMode;
            int lastIndentLevel = EditorGUI.indentLevel;
            EditorGUIUtility.wideMode = true;
            EditorGUI.BeginProperty(position, label, property);

            // Clamp widths so they don't go off screen
            //float propertyWidth = Mathf.Min(totalPropertyWidth, position.width);
            //float assetWidth = Mathf.Min(assetPropertyWidth, position.width);
            //float tagWidth = Mathf.Min(tagPropertyWidth, position.width);
            //float buttonWidth = Mathf.Min(buttonPropertyWidth, position.width);

            // Indent our rect, then reset indent to 0 so sub-properties don't get doubly indented
            position = EditorGUI.IndentedRect(position);
            //position.width = propertyWidth;
            EditorGUI.indentLevel = 0;

            // Draw a box around our item
            Rect boxPosition = position;
            boxPosition.height = (EditorGUIUtility.singleLineHeight * 4) - EditorGUIUtility.standardVerticalSpacing;
            GUI.Box(boxPosition, GUIContent.none, EditorStyles.helpBox);

            position = boxOffset.Remove(position);

            Rect iconRect = position;
            iconRect.width = iconWidth;

            Rect assetRect = position;
            assetRect.width = position.width - iconWidth;
            assetRect.height = EditorGUIUtility.singleLineHeight;
            assetRect.x += iconWidth;
           
            Rect tagRect = position;
            //tagRect.width = tagWidth;
            tagRect.height = EditorGUIUtility.singleLineHeight;
            tagRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            Rect buttonRect = position;
            //buttonRect.width = buttonWidth;
            buttonRect.height = EditorGUIUtility.singleLineHeight;
            buttonRect.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;

            bool changed = false;

            UnityEngine.Object asset = assetProperty.objectReferenceValue;

            GUIContent labelContent = null;
            GUIContent iconContent = null;
            italicStyle.fontStyle = FontStyle.Normal;
            bool buttonsDisabled = false;
            bool tagDisabled = false;

            if (asset == null)
            {
                string missingSceneName = nameProperty.stringValue;
                if (!string.IsNullOrEmpty(missingSceneName))
                {
                    labelContent = new GUIContent(" (" + missingSceneName + ")");
                    labelContent.tooltip = "The scene " + missingSceneName + " is missing. It will not be available to load.";
                    italicStyle.fontStyle = FontStyle.Italic;
                    buttonsDisabled = true;
                    tagDisabled = true;
                    iconContent = EditorGUIUtility.IconContent(missingIconContent);
                }
                else
                {
                    labelContent = new GUIContent(" (Empty)");
                    labelContent.tooltip = "This scene is empty. You should assign a scene object before building.";
                    buttonsDisabled = true;
                    tagDisabled = true;
                    iconContent = EditorGUIUtility.IconContent(missingIconContent);
                }
            }
            else
            {
                if (buildIndexProperty.intValue >= 0)
                {
                    if (enabledProperty.boolValue)
                    {
                        labelContent = new GUIContent(" Build index: " + buildIndexProperty.intValue);
                        labelContent.tooltip = "This scene is in build settings at index " + buildIndexProperty.intValue;
                        iconContent = EditorGUIUtility.IconContent(enabledIconContent);
                    }
                    else
                    {
                        labelContent = new GUIContent(" Build index: " + buildIndexProperty.intValue + " (Disabled)");
                        labelContent.tooltip = "This scene is in build settings at index " + buildIndexProperty.intValue + ", but it has been disabled and will not be available to load.";
                        iconContent = EditorGUIUtility.IconContent(disabledIconContent);
                    }
                }
                else
                {
                    labelContent = new GUIContent(" (Not included in build)");
                    labelContent.tooltip = "This scene is not included in build settings and will not be available to load.";
                    iconContent = EditorGUIUtility.IconContent(errorIconContent);
                }
            }

            // Draw our icon
            EditorGUI.LabelField(iconRect, iconContent);

            // Draw our object field
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUIUtility.labelWidth = assetLabelWidth;
            asset = EditorGUI.ObjectField(assetRect, labelContent, assetProperty.objectReferenceValue, typeof(SceneAsset), false);
            EditorGUI.EndDisabledGroup();

            if (asset != assetProperty.objectReferenceValue)
            {
                assetProperty.objectReferenceValue = asset;
                changed = true;
            }

            if (DrawTagProperty)
            {
                // Draw our tag field
                EditorGUI.BeginDisabledGroup(tagDisabled || Application.isPlaying);
                EditorGUIUtility.labelWidth = tagLabelWidth;
                changed |= EditorGUI.PropertyField(tagRect, tagProperty);
                EditorGUI.EndDisabledGroup();
            }

            // Draw our button
            EditorGUI.BeginDisabledGroup(buttonsDisabled || Application.isPlaying);
            if (buildIndexProperty.intValue < 0)
            {
                // Show add to build button
                if (GUI.Button(buttonRect, "Add to build settings", EditorStyles.toolbarButton) && asset != null)
                {
                    List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                    scenes.Add(new EditorBuildSettingsScene(pathProperty.stringValue, true));
                    enabledProperty.boolValue = true;
                    EditorBuildSettings.scenes = scenes.ToArray();
                    changed = true;
                }
            }
            else
            {
                // Show add / remove toggle
                if (GUI.Button(buttonRect, enabledProperty.boolValue ? "Disable in build settings" : "Enable in build settings", EditorStyles.toolbarButton))
                {
                    enabledProperty.boolValue = !enabledProperty.boolValue;
                    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                    scenes[buildIndexProperty.intValue].enabled = enabledProperty.boolValue;
                    EditorBuildSettings.scenes = scenes;
                    changed = true;
                }
            }
            EditorGUI.EndDisabledGroup();

            if (!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling)
            {   // This is expensive so don't refresh during play mode or while other stuff is going on
                changed |= RefreshSceneInfo(asset, nameProperty, pathProperty, buildIndexProperty, enabledProperty, tagProperty);
            }

            if (changed)
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUIUtility.wideMode = lastMode;
            EditorGUI.indentLevel = lastIndentLevel;
            EditorGUI.EndProperty();
        }

        private static bool RefreshSceneInfo(
            UnityEngine.Object asset, 
            SerializedProperty nameProperty, 
            SerializedProperty pathProperty,
            SerializedProperty buildIndexProperty,
            SerializedProperty includedProperty,
            SerializedProperty tagProperty)
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
                // Refreshing these values is very expensive
                // Especially getting build scenes
                // We may want to move this out of the property drawer
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

            if (string.IsNullOrEmpty(tagProperty.stringValue))
            {
                tagProperty.stringValue = "Untagged";
                changed = true;
            }

            return changed;
        }
    }
}