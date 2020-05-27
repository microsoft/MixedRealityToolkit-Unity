// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SceneSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

        static RectOffset boxOffset;
        static GUIStyle italicStyle;

        public static float GetPropertyHeight(bool drawTagProperty)
        {
            return (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * (drawTagProperty ? 4 : 3);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetPropertyHeight(DrawTagProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawProperty(position, property, label);
        }

        public static void DrawProperty(Rect position, SerializedProperty property, GUIContent label, bool isActive = false, bool isSelected = false)
        {
            SerializedProperty assetProperty, nameProperty, pathProperty, buildIndexProperty, includedProperty, tagProperty;
            SceneInfoUtils.GetSceneInfoRelativeProperties(property, out assetProperty, out nameProperty, out pathProperty, out buildIndexProperty, out includedProperty, out tagProperty);

            // Set up our properties and settings
            boxOffset = EditorStyles.helpBox.padding;
            if (italicStyle == null) { italicStyle = new GUIStyle(EditorStyles.label); }
            bool lastMode = EditorGUIUtility.wideMode;
            int lastIndentLevel = EditorGUI.indentLevel;
            EditorGUIUtility.wideMode = true;
            EditorGUI.BeginProperty(position, label, property);

            GUI.color = isActive ? Color.gray : GUI.backgroundColor;
            GUI.color = isSelected ? Color.blue : GUI.backgroundColor;

            // Indent our rect, then reset indent to 0 so sub-properties don't get doubly indented
            position = EditorGUI.IndentedRect(position);
            EditorGUI.indentLevel = 0;

            // Draw a box around our item
            Rect boxPosition = position;
            boxPosition.height = (EditorGUIUtility.singleLineHeight * (DrawTagProperty ? 4 : 3)) - EditorGUIUtility.standardVerticalSpacing;
            GUI.Box(boxPosition, GUIContent.none, EditorStyles.helpBox);

            position = boxOffset.Remove(position);

            Rect iconRect = position;
            iconRect.width = iconWidth;

            Rect assetRect = position;
            assetRect.width = position.width - iconWidth;
            assetRect.height = EditorGUIUtility.singleLineHeight;
            assetRect.x += iconWidth;
           
            Rect buttonRect = position;
            buttonRect.height = EditorGUIUtility.singleLineHeight;
            buttonRect.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            Rect tagRect = position;
            tagRect.height = EditorGUIUtility.singleLineHeight;
            tagRect.y += ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2) + EditorGUIUtility.standardVerticalSpacing;

            bool changed = false;

            UnityEngine.Object asset = assetProperty.objectReferenceValue;

            if (!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling)
            {   // This is expensive so don't refresh during play mode or while other stuff is going on
                changed = SceneInfoUtils.RefreshSceneInfo(asset, nameProperty, pathProperty, buildIndexProperty, includedProperty, tagProperty);
            }

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
                if (includedProperty.boolValue)
                {
                    if (buildIndexProperty.intValue >= 0)
                    {
                        labelContent = new GUIContent(" Build index: " + buildIndexProperty.intValue);
                        labelContent.tooltip = "This scene is in build settings at index " + buildIndexProperty.intValue;
                        iconContent = EditorGUIUtility.IconContent(enabledIconContent);
                    }
                    else
                    {
                        labelContent = new GUIContent(" (Disabled)");
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
            if (!string.IsNullOrEmpty(pathProperty.stringValue) && asset == null)
            {
                // The scene is missing
                // This may be due to a local file ID mismatch
                // Try to find it based on guid first
                asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathProperty.stringValue);
            }


            if (!string.IsNullOrEmpty (nameProperty.stringValue) && asset == null)
            {
                // If we still can't find it, draw a button that lets people attempt to recover it
                if (GUI.Button(buttonRect, "Search for missing scene", EditorStyles.toolbarButton))
                {
                   changed |= SceneInfoUtils.FindScene(nameProperty, pathProperty, ref asset);
                }
            }
            else
            {
                // It's not included in build settings
                if (!includedProperty.boolValue)
                {
                    // The scene exists but it isn't in our build settings
                    // Show a button that lets us add it
                    if (GUI.Button(buttonRect, "Add to build settings", EditorStyles.toolbarButton) && asset != null)
                    {
                        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                        scenes.Add(new EditorBuildSettingsScene(pathProperty.stringValue, true));
                        includedProperty.boolValue = true;
                        EditorBuildSettings.scenes = scenes.ToArray();
                        SceneInfoUtils.RefreshCachedScenes();
                        changed = true;
                    }
                }
                else
                {
                    bool enabledInBuild = buildIndexProperty.intValue >= 0;
                    // The scene exists and is in build settings
                    // Show enable / disable toggle
                    if (GUI.Button(buttonRect, enabledInBuild ? "Disable in build settings" : "Enable in build settings", EditorStyles.toolbarButton))
                    {
                        enabledInBuild = !enabledInBuild;
                        // Modify a local copy of our scenes instead of using the cached scenes
                        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                        // Find the scene in our build settings and enable / disable it
                        int sceneCount = 0;
                        int buildIndex = -1;
                        for (int i = 0; i < SceneInfoUtils.CachedScenes.Length; i++)
                        {
                            if (scenes[i].path == pathProperty.stringValue)
                            {
                                scenes[i].enabled = enabledInBuild;
                                if (scenes[i].enabled)
                                {   // Only store the build index if it's enabled
                                    buildIndex = sceneCount;
                                }
                                break;
                            }

                            if (scenes[i].enabled)
                            {   // Disabled scenes don't count toward scene count
                                sceneCount++;
                            }
                        }
                        EditorBuildSettings.scenes = scenes;
                        SceneInfoUtils.RefreshCachedScenes();
                        buildIndexProperty.intValue = buildIndex;
                        changed = true;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            if (asset != assetProperty.objectReferenceValue)
            {
                assetProperty.objectReferenceValue = asset;
                changed = true;
            }

            if (changed)
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUIUtility.wideMode = lastMode;
            EditorGUI.indentLevel = lastIndentLevel;
            EditorGUI.EndProperty();
        }
    }
}