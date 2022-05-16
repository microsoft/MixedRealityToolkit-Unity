// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    /// <summary>
    /// Inspector for themes, and used by Interactable
    /// </summary>
    [CustomEditor(typeof(Theme))]
    public class ThemeInspector : UnityEditor.Editor
    {
        protected SerializedProperty themeDefinitions;
        protected SerializedProperty states;
        protected Theme theme;
        protected State[] themeStates;

        private const float ThemeStateFontScale = 1.1f;
        private const int ThemeBoxMargin = 25;

        private static readonly GUIContent AddThemePropertyLabel = new GUIContent("Add Theme Definition", "Add Theme Definition");
        private static readonly GUIContent RemoveThemePropertyContent = new GUIContent("Delete", "Remove Theme Definition");
        private static readonly GUIContent CreateAnimationsContent = new GUIContent("Create Animations", "Create and add an Animator with AnimationClips");
        private static readonly GUIContent EasingContent = new GUIContent("Easing", "should the theme animate state values");

        public void OnEnable()
        {
            theme = target as Theme;
            themeDefinitions = serializedObject.FindProperty("definitions");
            states = serializedObject.FindProperty("states");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (theme != null && theme.States != null)
            {
                themeStates = theme.States.StateList.ToArray();
            }

            if (themeStates == null)
            {
                themeStates = Array.Empty<State>();
            }

            // If no theme properties assigned, add a default one
            if (themeDefinitions.arraySize < 1)
            {
                AddThemeDefinition();
            }

            RenderTheme();

            serializedObject.ApplyModifiedProperties();
        }

        #region Rendering Methods

        public virtual void RenderTheme()
        {
            if (!RenderStates())
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.Space();

            RenderThemeDefinitions();

        }

        /// <summary>
        /// draw the states property field for assigning states
        /// Set the default state if one does not exist
        /// </summary>
        protected bool RenderStates()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(states, new GUIContent("States", "The States this Interactable is based on"));
                    if (check.changed)
                    {
                        theme.States = states.objectReferenceValue as States;
                        theme.ValidateDefinitions();
                        serializedObject.Update();
                    }
                }

                if (states.objectReferenceValue == null || themeStates.Length < 1)
                {
                    InspectorUIUtility.DrawError("Please assign a valid States object!");
                    return false;
                }
            }

            return true;
        }

        public void RenderThemeDefinitions()
        {
            GUIStyle box = InspectorUIUtility.HelpBox(EditorGUI.indentLevel * ThemeBoxMargin);

            // Loop through all InteractableThemePropertySettings of Theme
            for (int index = 0; index < themeDefinitions.arraySize; index++)
            {
                using (new EditorGUILayout.VerticalScope(box))
                {
                    SerializedProperty themeDefinition = themeDefinitions.GetArrayElementAtIndex(index);
                    SerializedProperty className = themeDefinition.FindPropertyRelative("ClassName");

                    string themeDefinition_prefKey = theme.name + "_Definitions" + index;
                    bool show = false;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        show = InspectorUIUtility.DrawSectionFoldoutWithKey(className.stringValue, themeDefinition_prefKey, MixedRealityStylesUtility.BoldFoldoutStyle);

                        if (RenderDeleteButton(index))
                        {
                            return;
                        }
                    }

                    if (show)
                    {
                        EditorGUILayout.Space();

                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);

                            using (new EditorGUILayout.HorizontalScope())
                            {
                                var themeTypes = TypeCacheUtility.GetSubClasses<InteractableThemeBase>();
                                var themeClassNames = themeTypes.Select(t => t?.Name).ToArray();
                                int id = Array.IndexOf(themeClassNames, className.stringValue);
                                int newId = EditorGUILayout.Popup("Theme Runtime", id, themeClassNames);

                                // Some old Themes did not properly save a value here
                                SerializedProperty assemblyQualifiedName = themeDefinition.FindPropertyRelative("AssemblyQualifiedName");
                                if (string.IsNullOrEmpty(assemblyQualifiedName.stringValue) && newId != -1)
                                {
                                    assemblyQualifiedName.stringValue = themeTypes[newId].AssemblyQualifiedName;
                                }

                                // If user changed the theme type for current themeDefinition
                                if (id != newId && newId != -1)
                                {
                                    Type oldType = id != -1 ? themeTypes[id] : null;
                                    Type newType = themeTypes[newId];
                                    ChangeThemeDefinitionType(index, oldType, newType);
                                    return;
                                }
                            }

                            var themeType = theme.Definitions[index].ThemeType;
                            if (themeType != null)
                            {
                                SerializedProperty customProperties = themeDefinition.FindPropertyRelative("customProperties");
                                RenderCustomProperties(customProperties);

                                var themeExample = (InteractableThemeBase)Activator.CreateInstance(themeType);

                                if (themeExample.IsEasingSupported)
                                {
                                    RenderEasingProperties(themeDefinition);
                                }

                                if (themeExample.AreShadersSupported)
                                {
                                    RenderShaderProperties(themeDefinition);
                                }

                                EditorGUILayout.Space();

                                RenderThemeStates(themeDefinition);
                            }
                            else
                            {
                                InspectorUIUtility.DrawError("Theme Runtime Type is not valid");
                            }
                        }
                    }
                }
            }

            // If no theme properties assigned, add a default one
            if (themeDefinitions.arraySize < 1 || GUILayout.Button(AddThemePropertyLabel))
            {
                AddThemeDefinition();
            }
        }

        private void RenderThemeStates(SerializedProperty themeDefinition)
        {
            EditorGUILayout.LabelField("State Properties", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope())
            {
                for (int n = 0; n < themeStates.Length; n++)
                {
                    InspectorUIUtility.DrawLabel(themeStates[n].Name, (int)(InspectorUIUtility.DefaultFontSize * ThemeStateFontScale), InspectorUIUtility.ColorTint50);
                    SerializedProperty stateProperties = themeDefinition.FindPropertyRelative("stateProperties");
                    using (new EditorGUI.IndentLevelScope())
                    {
                        for (int i = 0; i < stateProperties.arraySize; i++)
                        {
                            SerializedProperty propertyItem = stateProperties.GetArrayElementAtIndex(i);
                            SerializedProperty values = propertyItem.FindPropertyRelative("values");

                            if (n >= values.arraySize)
                            {
                                // This property does not have the correct number of state values
                                continue;
                            }

                            SerializedProperty name = propertyItem.FindPropertyRelative("name");
                            SerializedProperty type = propertyItem.FindPropertyRelative("type");
                            SerializedProperty statePropertyValue = values.GetArrayElementAtIndex(n);

                            RenderValue(statePropertyValue, new GUIContent(name.stringValue, ""), (ThemePropertyTypes)type.intValue);
                        }
                    }
                }
                GUILayout.Space(5);
            }
            GUILayout.Space(5);
        }

        /// <summary>
        /// Renders easing and related time properties for InteractableThemePropertySettings
        /// </summary>
        /// <param name="themeDefinition">Serialized property of a ThemeDefinition object</param>
        private static void RenderEasingProperties(SerializedProperty themeDefinition)
        {
            SerializedProperty easing = themeDefinition.FindPropertyRelative("easing");
            SerializedProperty enabled = easing.FindPropertyRelative("Enabled");

            enabled.boolValue = EditorGUILayout.Toggle(EasingContent, enabled.boolValue);

            if (enabled.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    SerializedProperty time = easing.FindPropertyRelative("LerpTime");
                    SerializedProperty curve = easing.FindPropertyRelative("Curve");

                    EditorGUILayout.PropertyField(time, new GUIContent("Duration", "Duration for easing between values in seconds"));
                    EditorGUILayout.PropertyField(curve, new GUIContent("Animation Curve", "Curve that defines rate of easing between values"));
                }
            }
        }

        private static void RenderShaderProperties(SerializedProperty themeDefinition)
        {
            SerializedProperty stateProperties = themeDefinition.FindPropertyRelative("stateProperties");

            for (int i = 0; i < stateProperties.arraySize; i++)
            {
                SerializedProperty stateProperty = stateProperties.GetArrayElementAtIndex(i);
                SerializedProperty type = stateProperty.FindPropertyRelative("type");

                if (ThemeStateProperty.IsShaderPropertyType((ThemePropertyTypes)type.intValue))
                {
                    SerializedProperty statePropertyName = stateProperty.FindPropertyRelative("name");
                    SerializedProperty shader = stateProperty.FindPropertyRelative("targetShader");
                    SerializedProperty shaderPropertyname = stateProperty.FindPropertyRelative("shaderPropertyName");

                    // Temporary workaround to help migrate old ThemeDefinitions to new model if applicable
                    MigrateShaderData(stateProperty, shader, shaderPropertyname);

                    EditorGUILayout.PropertyField(shader, new GUIContent(statePropertyName.stringValue + " Shader"), false);

                    var propertyList = GetShaderPropertyList(shader.objectReferenceValue as Shader, GetShaderPropertyFilter((ThemePropertyTypes)type.intValue));
                    int selectedIndex = propertyList.IndexOf(shaderPropertyname.stringValue);

                    Rect pos = EditorGUILayout.GetControlRect();
                    using (new EditorGUI.PropertyScope(pos, new GUIContent(statePropertyName.stringValue + " Property"), shaderPropertyname))
                    {
                        int newIndex = EditorGUILayout.Popup(statePropertyName.stringValue + " Property", selectedIndex, propertyList.ToArray());
                        if (newIndex != selectedIndex)
                        {
                            shaderPropertyname.stringValue = propertyList[newIndex];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Temporary utility function to migrate shader data from deprecated properties to new valid properties
        /// </summary>
        private static void MigrateShaderData(SerializedProperty stateProperty, SerializedProperty shader, SerializedProperty shaderPropertyname)
        {
            if (shader.objectReferenceValue == null)
            {
                SerializedProperty shaderOptions = stateProperty.FindPropertyRelative("ShaderOptions");
                if (shaderOptions.arraySize > 0)
                {
                    var shaderName = stateProperty.FindPropertyRelative("ShaderName");
                    var shaderOptionNames = stateProperty.FindPropertyRelative("ShaderOptionNames");
                    var shaderOptionIndex = stateProperty.FindPropertyRelative("PropId");
                    var shaderOption = shaderOptionNames.GetArrayElementAtIndex(shaderOptionIndex.intValue);

                    // Migrate data over to new model
                    shader.objectReferenceValue = Shader.Find(shaderName.stringValue);
                    shaderPropertyname.stringValue = shaderOption.stringValue;

                    // Wipe old data from triggering this again
                    shaderOptions.ClearArray();

                    stateProperty.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    shader.objectReferenceValue = StandardShaderUtility.MrtkStandardShader;

                    SerializedProperty type = stateProperty.FindPropertyRelative("type");
                    if (type.intValue == (int)ThemePropertyTypes.Color)
                    {
                        shaderPropertyname.stringValue = "_Color";
                    }
                    else if (type.intValue == (int)ThemePropertyTypes.Texture)
                    {
                        shaderPropertyname.stringValue = "_MainTex";
                    }
                }
            }
        }

        /// <summary>
        /// Render list of custom settings part of a InteractableThemePropertySettings object
        /// </summary>
        /// <param name="customProperties">SerializedProperty for InteractableThemePropertySettings.CustomSettings</param>
        private static void RenderCustomProperties(SerializedProperty customProperties)
        {
            for (int p = 0; p < customProperties.arraySize; p++)
            {
                SerializedProperty item = customProperties.GetArrayElementAtIndex(p);
                SerializedProperty name = item.FindPropertyRelative("Name");
                SerializedProperty tooltip = item.FindPropertyRelative("Tooltip");
                SerializedProperty propType = item.FindPropertyRelative("Type");
                SerializedProperty value = item.FindPropertyRelative("Value");
                ThemePropertyTypes type = (ThemePropertyTypes)propType.intValue;

                RenderValue(value, new GUIContent(name.stringValue, tooltip?.stringValue), type);
            }
        }

        /// <summary>
        /// Render a single property value
        /// </summary>
        public static void RenderValue(SerializedProperty item, GUIContent label, ThemePropertyTypes type)
        {
            SerializedProperty floatValue = item.FindPropertyRelative("Float");
            SerializedProperty vector2Value = item.FindPropertyRelative("Vector2");
            SerializedProperty stringValue = item.FindPropertyRelative("String");

            switch (type)
            {
                case ThemePropertyTypes.Float:
                    floatValue.floatValue = EditorGUILayout.FloatField(label, floatValue.floatValue);
                    break;
                case ThemePropertyTypes.Int:
                    SerializedProperty intValue = item.FindPropertyRelative("Int");
                    intValue.intValue = EditorGUILayout.IntField(label, intValue.intValue);
                    break;
                case ThemePropertyTypes.Color:
                    SerializedProperty colorValue = item.FindPropertyRelative("Color");
                    colorValue.colorValue = EditorGUILayout.ColorField(label, colorValue.colorValue);
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    floatValue.floatValue = EditorGUILayout.FloatField(label, floatValue.floatValue);
                    break;
                case ThemePropertyTypes.ShaderRange:
                    vector2Value.vector2Value = EditorGUILayout.Vector2Field(label, vector2Value.vector2Value);
                    break;
                case ThemePropertyTypes.Vector2:
                    vector2Value.vector2Value = EditorGUILayout.Vector2Field(label, vector2Value.vector2Value);
                    break;
                case ThemePropertyTypes.Vector3:
                    SerializedProperty vector3Value = item.FindPropertyRelative("Vector3");
                    vector3Value.vector3Value = EditorGUILayout.Vector3Field(label, vector3Value.vector3Value);
                    break;
                case ThemePropertyTypes.Vector4:
                    SerializedProperty vector4Value = item.FindPropertyRelative("Vector4");
                    vector4Value.vector4Value = EditorGUILayout.Vector4Field(label, vector4Value.vector4Value);
                    break;
                case ThemePropertyTypes.Quaternion:
                    SerializedProperty quaternionValue = item.FindPropertyRelative("Quaternion");
                    Vector4 vect4 = new Vector4(quaternionValue.quaternionValue.x, quaternionValue.quaternionValue.y, quaternionValue.quaternionValue.z, quaternionValue.quaternionValue.w);
                    vect4 = EditorGUILayout.Vector4Field(label, vect4);
                    quaternionValue.quaternionValue = new Quaternion(vect4.x, vect4.y, vect4.z, vect4.w);
                    break;
                case ThemePropertyTypes.Texture:
                    SerializedProperty texture = item.FindPropertyRelative("Texture");
                    EditorGUILayout.PropertyField(texture, label, false);
                    break;
                case ThemePropertyTypes.Material:
                    SerializedProperty material = item.FindPropertyRelative("Material");
                    EditorGUILayout.PropertyField(material, label, false);
                    break;
                case ThemePropertyTypes.AudioClip:
                    SerializedProperty audio = item.FindPropertyRelative("AudioClip");
                    EditorGUILayout.PropertyField(audio, label, false);
                    break;
                case ThemePropertyTypes.Animaiton:
                    SerializedProperty animation = item.FindPropertyRelative("Animation");
                    EditorGUILayout.PropertyField(animation, label, false);
                    break;
                case ThemePropertyTypes.GameObject:
                    SerializedProperty gameObjectValue = item.FindPropertyRelative("GameObject");
                    EditorGUILayout.PropertyField(gameObjectValue, label, false);
                    break;
                case ThemePropertyTypes.String:
                    stringValue.stringValue = EditorGUILayout.TextField(label, stringValue.stringValue);
                    break;
                case ThemePropertyTypes.Bool:
                    SerializedProperty boolValue = item.FindPropertyRelative("Bool");
                    boolValue.boolValue = EditorGUILayout.Toggle(label, boolValue.boolValue);
                    break;
                case ThemePropertyTypes.AnimatorTrigger:
                    stringValue.stringValue = EditorGUILayout.TextField(label, stringValue.stringValue);
                    break;
                case ThemePropertyTypes.Shader:
                    SerializedProperty shaderObjectValue = item.FindPropertyRelative("Shader");
                    EditorGUILayout.PropertyField(shaderObjectValue, label, false);
                    break;
                default:
                    break;
            }
        }

        protected bool RenderDeleteButton(int index)
        {
            // Create Delete button if we have an array of themes
            if (themeDefinitions.arraySize > 1 && InspectorUIUtility.SmallButton(RemoveThemePropertyContent))
            {
                ClearHistoryCache(index);
                DeleteThemeDefinition((uint)index);

                serializedObject.Update();
                EditorUtility.SetDirty(theme);
                return true;
            }

            return false;
        }

        #endregion

        #region Theme Definition Management

        protected virtual void AddThemeDefinition()
        {
            Type defaultType = typeof(InteractableActivateTheme);

            ThemeDefinition newDefinition = ThemeDefinition.GetDefaultThemeDefinition(defaultType).Value;
            if (theme.Definitions == null)
            {
                theme.Definitions = new List<ThemeDefinition>();
            }
            theme.Definitions.Add(newDefinition);
            theme.History.Add(new Dictionary<Type, ThemeDefinition>());
            theme.ValidateDefinitions();

            serializedObject.Update();
            EditorUtility.SetDirty(theme);
        }

        protected void DeleteThemeDefinition(uint index)
        {
            if (!(theme != null && theme.Definitions != null && index < theme.Definitions.Count))
            {
                Debug.LogError("Cannot delete ThemeDefinition. Invalid Theme object");
                return;
            }

            theme.Definitions.RemoveAt((int)index);
        }

        private void ChangeThemeDefinitionType(int index, Type oldType, Type newType)
        {
            // Save theme definition to cache
            SaveThemeDefinitionHistory(index, oldType);

            // Try to load theme from history cache
            ThemeDefinition? definition = LoadThemeDefinitionHistory(index, newType);
            if (definition == null)
            {
                // if not available, then create a new one
                definition = ThemeDefinition.GetDefaultThemeDefinition(newType);
            }

            theme.Definitions[index] = definition.Value;
            theme.ValidateDefinitions();

            themeDefinitions.serializedObject.Update();
            EditorUtility.SetDirty(theme);
        }

        #endregion

        #region Theme Definition History

        protected void ClearHistoryCache(int index)
        {
            if (theme == null || theme.History == null || index >= theme.History.Count)
            {
                return;
            }

            theme.History.RemoveAt(index);
        }

        /// <summary>
        /// Check that access for the provided index is valid into the definitions and history of the provided Theme
        /// </summary>
        /// <param name="target">Theme container object to inspector</param>
        /// <param name="index">index of ThemeDefinintion and History cache to access</param>
        /// <returns>true if access at index is possible, false otherwise</returns>
        private static bool ValidThemeHistoryAccess(Theme target, uint index)
        {
            return target != null && target.History != null && target.Definitions != null
                    && index < target.History.Count;
        }

        protected void SaveThemeDefinitionHistory(int index, Type definitionClassType)
        {
            if (definitionClassType == null)
            {
                return;
            }

            if (theme == null || theme.History == null || theme.Definitions == null)
            {
                Debug.LogWarning("Could not save ThemeDefinition to history cache");
                return;
            }

            // If cache list is out of sync for some reason, wipe and start fresh
            if (theme.History.Count != theme.Definitions.Count)
            {
                theme.History.Clear();
                for (int i = 0; i < theme.Definitions.Count; i++)
                {
                    theme.History.Add(new Dictionary<Type, ThemeDefinition>());
                }
            }

            var definition = theme.Definitions[index];
            var cache = theme.History[index];
            cache[definitionClassType] = definition;
        }

        protected ThemeDefinition? LoadThemeDefinitionHistory(int index, Type newDefinitionClassType)
        {
            if (!ValidThemeHistoryAccess(theme, (uint)index))
            {
                Debug.LogWarning("Could not load ThemeDefinition to history cache");
                return null;
            }

            var cache = theme.History[index];
            if (cache.ContainsKey(newDefinitionClassType))
            {
                return cache[newDefinitionClassType];
            }
            else
            {
                return null;
            }
        }

        #endregion

        private static ShaderUtil.ShaderPropertyType[] GetShaderPropertyFilter(ThemePropertyTypes shaderPropertyType)
        {
            ShaderUtil.ShaderPropertyType[] shaderTypes = null;
            switch (shaderPropertyType)
            {
                case ThemePropertyTypes.Color:
                    shaderTypes = new ShaderUtil.ShaderPropertyType[] { ShaderUtil.ShaderPropertyType.Color };
                    break;
                case ThemePropertyTypes.Texture:
                    shaderTypes = new ShaderUtil.ShaderPropertyType[] { ShaderUtil.ShaderPropertyType.TexEnv };
                    break;
                case ThemePropertyTypes.ShaderFloat:
                case ThemePropertyTypes.ShaderRange:
                    shaderTypes = new ShaderUtil.ShaderPropertyType[] { ShaderUtil.ShaderPropertyType.Float, ShaderUtil.ShaderPropertyType.Range };
                    break;
            }

            return shaderTypes;
        }

        private static List<string> GetShaderPropertyList(Shader shader, ShaderUtil.ShaderPropertyType[] filterTypes = null)
        {
            List<string> results = new List<string>();

            if (shader == null) return results;

            int count = ShaderUtil.GetPropertyCount(shader);
            results.Capacity = count;

            for (int i = 0; i < count; i++)
            {
                bool isHidden = ShaderUtil.IsShaderPropertyHidden(shader, i);
                bool isValidPropertyType = filterTypes == null || filterTypes.Contains(ShaderUtil.GetPropertyType(shader, i));
                if (!isHidden && isValidPropertyType)
                {
                    results.Add(ShaderUtil.GetPropertyName(shader, i));
                }
            }

            results.Sort();
            return results;
        }
    }
}
