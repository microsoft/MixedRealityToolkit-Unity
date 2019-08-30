// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
#if UNITY_EDITOR
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

        private const float ThemeStateFontScale = 1.2f;
        private const int ThemeBoxMargin = 30;

        private static readonly GUIContent AddThemePropertyLabel = new GUIContent("+ Add Theme Definition", "Add Theme Definition");
        private static readonly GUIContent RemoveThemePropertyContent = new GUIContent("-", "Remove Theme Definition");
        private static readonly GUIContent CreateAnimationsContent = new GUIContent("Create Animations", "Create and add an Animator with AnimationClips");
        private static readonly GUIContent EasingContent = new GUIContent("Easing", "should the theme animate state values");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            theme = target as Theme;

            themeDefinitions = serializedObject.FindProperty("Definitions");
            states = serializedObject.FindProperty("States");
            themeStates = theme.GetStates();

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

            // If no theme properties assigned, add a default one
            if (themeDefinitions.arraySize < 1 || InspectorUIUtility.FlexButton(AddThemePropertyLabel))
            {
                AddThemeDefinition();
                return;
            }

            RenderThemeSettings();

            RenderThemeStates();
        }

        /// <summary>
        /// draw the states property field for assigning states
        /// Set the default state if one does not exist
        /// </summary>
        /// <returns></returns>
        protected bool RenderStates()
        {
            GUIStyle box = InspectorUIUtility.Box(EditorGUI.indentLevel * ThemeBoxMargin);
            using (new EditorGUILayout.VerticalScope(box))
            {
                GUI.enabled = !(EditorApplication.isPlaying || EditorApplication.isPaused);
                EditorGUILayout.PropertyField(states, new GUIContent("States", "The States this Interactable is based on"));
                GUI.enabled = true;

                if (states.objectReferenceValue == null || themeStates.Length < 1)
                {
                    InspectorUIUtility.DrawError("Please assign a valid States object!");
                    return false;
                }
            }

            return true;
        }

        public void RenderThemeSettings()
        {
            GUIStyle box = InspectorUIUtility.Box(EditorGUI.indentLevel * ThemeBoxMargin);

            // Loop through all InteractableThemePropertySettings of Theme
            for (int index = 0; index < themeDefinitions.arraySize; index++)
            {
                SerializedProperty themeDefinition = themeDefinitions.GetArrayElementAtIndex(index);
                using (new EditorGUILayout.VerticalScope(box))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        SerializedProperty className = themeDefinition.FindPropertyRelative("ClassName");

                        var themeTypes = typeof(InteractableThemeBase).GetAllSubClassesOf();
                        var themeClassNames = themeTypes.Select(t => t.Name).ToArray();
                        int id = Array.IndexOf(themeClassNames, className.stringValue);
                        int newId = EditorGUILayout.Popup("Theme Runtime", id, themeClassNames);

                        // If user changed the theme type for current themeDefinition
                        if (id != newId)
                        {
                            Type oldType = themeTypes[id];
                            Type newType = themeTypes[newId];
                            ChangeThemeDefinitionType(index, oldType, newType);
                            return;
                        }

                        // Create Delete button if we have an array of themes
                        // TODO: Troy -> Alright to be empty?
                        if (themeDefinitions.arraySize > 1 && InspectorUIUtility.SmallButton(RemoveThemePropertyContent))
                        {
                            ClearHistoryCache(index);
                            DeleteThemeDefinition((uint)index);

                            serializedObject.Update();
                            EditorUtility.SetDirty(theme);
                            return;
                        }
                    }

                    SerializedProperty customProperties = themeDefinition.FindPropertyRelative("CustomProperties");
                    RenderCustomProperties(customProperties);

                    var themeType = theme.Definitions[index].ThemeType;
                    var themeExample = (InteractableThemeBase)Activator.CreateInstance(themeType);

                    if (themeExample.IsEasingSupported)
                    {
                        RenderEasingProperties(themeDefinition);
                    }

                    if (themeExample.AreShadersSupported)
                    {
                        RenderShaderProperties(themeDefinition);
                    }
                }
            }
        }

        public void RenderThemeStates()
        {
            GUIStyle box = InspectorUIUtility.Box(EditorGUI.indentLevel * ThemeBoxMargin);

            using (new EditorGUILayout.VerticalScope(box))
            {
                for (int n = 0; n < themeStates.Length; n++)
                {
                    InspectorUIUtility.DrawLabel(themeStates[n].Name, (int)(InspectorUIUtility.DefaultFontSize * ThemeStateFontScale), InspectorUIUtility.ColorTint50);

                    for (int j = 0; j < themeDefinitions.arraySize; j++)
                    {
                        SerializedProperty themeDefinition = themeDefinitions.GetArrayElementAtIndex(j);
                        SerializedProperty stateProperties = themeDefinition.FindPropertyRelative("StateProperties");
                        using (new EditorGUI.IndentLevelScope())
                        {
                            for (int i = 0; i < stateProperties.arraySize; i++)
                            {
                                SerializedProperty propertyItem = stateProperties.GetArrayElementAtIndex(i);
                                SerializedProperty values = propertyItem.FindPropertyRelative("Values");

                                if (n >= values.arraySize)
                                {
                                    // This property does not have the correct number of state values*
                                    // TODO: Troy - Auto-populate?
                                    continue;
                                }

                                SerializedProperty name = propertyItem.FindPropertyRelative("Name");
                                SerializedProperty type = propertyItem.FindPropertyRelative("Type");
                                SerializedProperty statePropertyValue = values.GetArrayElementAtIndex(n);

                                // TODO: Change function to stateProperty, int index
                                // TODO: Troy - fix shaderPropName?
                                //RenderValue(item, name.stringValue, shaderPropName, (ThemePropertyTypes)type.intValue);
                                RenderValue(statePropertyValue, name.stringValue, name.stringValue, (ThemePropertyTypes)type.intValue);
                            }
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
            SerializedProperty easing = themeDefinition.FindPropertyRelative("Easing");
            SerializedProperty enabled = easing.FindPropertyRelative("Enabled");

            enabled.boolValue = EditorGUILayout.Toggle(EasingContent, enabled.boolValue);

            if (enabled.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    SerializedProperty time = easing.FindPropertyRelative("LerpTime");
                    SerializedProperty curve = easing.FindPropertyRelative("Curve");

                    EditorGUILayout.PropertyField(time, new GUIContent("Duration", "Animation duration"));
                    EditorGUILayout.PropertyField(curve, new GUIContent("Animation Curve"));
                }
            }
        }

        private static void RenderShaderProperties(SerializedProperty themeDefinition)
        {
            SerializedProperty stateProperties = themeDefinition.FindPropertyRelative("StateProperties");

            for (int i = 0; i < stateProperties.arraySize; i++)
            {
                SerializedProperty stateProperty = stateProperties.GetArrayElementAtIndex(i);
                SerializedProperty type = stateProperty.FindPropertyRelative("Type");

                if (ThemeStateProperty.IsShaderPropertyType((ThemePropertyTypes)type.enumValueIndex))
                {
                    SerializedProperty statePropertyName = stateProperty.FindPropertyRelative("Name");
                    SerializedProperty shader = stateProperty.FindPropertyRelative("TargetShader");
                    SerializedProperty shaderPropertyname = stateProperty.FindPropertyRelative("ShaderPropertyName");

                    // TODO: Troy - move to static function
                    SerializedProperty shaderOptions = stateProperty.FindPropertyRelative("ShaderOptions");
                    if (shaderOptions.arraySize > 0 && shader.objectReferenceValue == null)
                    {
                        var shaderName = stateProperty.FindPropertyRelative("ShaderName");
                        var shaderOptionNames = stateProperty.FindPropertyRelative("ShaderOptionNames");
                        var shaderOptionIndex = stateProperty.FindPropertyRelative("PropId");
                        var shaderOption = shaderOptionNames.GetArrayElementAtIndex(shaderOptionIndex.intValue);

                        // Migrate data over to new model
                        shader.objectReferenceValue = Shader.Find(shaderName.stringValue);
                        shaderPropertyname.stringValue = shaderOption.stringValue;

                        // Wipe old data from trigering this again
                        shaderOptions.ClearArray();

                        stateProperty.serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUILayout.PropertyField(shader, new GUIContent(statePropertyName.stringValue + " Shader"), false);

                    var propertyList = GetShaderPropertyList(shader.objectReferenceValue as Shader, GetShaderPropertyFilter((ThemePropertyTypes)type.enumValueIndex));
                    int selectedIndex = propertyList.IndexOf(shaderPropertyname.stringValue);

                    int newIndex = EditorGUILayout.Popup(statePropertyName.stringValue + " Property", selectedIndex, propertyList.ToArray());
                    if (newIndex != selectedIndex)
                    {
                        shaderPropertyname.stringValue = propertyList[newIndex];
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
                SerializedProperty propType = item.FindPropertyRelative("Type");
                SerializedProperty value = item.FindPropertyRelative("Value");
                ThemePropertyTypes type = (ThemePropertyTypes)propType.intValue;

                RenderValue(value, name.stringValue, name.stringValue, type);
            }
        }

        /// <summary>
        /// Render a single property value
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="propName"></param>
        /// <param name="type"></param>
        public static void RenderValue(SerializedProperty item, string name, string propName, ThemePropertyTypes type)
        {
            SerializedProperty floatValue = item.FindPropertyRelative("Float");
            SerializedProperty vector2Value = item.FindPropertyRelative("Vector2");
            SerializedProperty stringValue = item.FindPropertyRelative("String");

            switch (type)
            {
                case ThemePropertyTypes.Float:
                    floatValue.floatValue = EditorGUILayout.FloatField(name, floatValue.floatValue);
                    break;
                case ThemePropertyTypes.Int:
                    SerializedProperty intValue = item.FindPropertyRelative("Int");
                    intValue.intValue = EditorGUILayout.IntField(name, intValue.intValue);
                    break;
                case ThemePropertyTypes.Color:
                    SerializedProperty colorValue = item.FindPropertyRelative("Color");
                    colorValue.colorValue = EditorGUILayout.ColorField(new GUIContent(propName, propName), colorValue.colorValue);
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    floatValue.floatValue = EditorGUILayout.FloatField(new GUIContent(propName, propName), floatValue.floatValue);
                    break;
                case ThemePropertyTypes.ShaderRange:
                    vector2Value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(propName, propName), vector2Value.vector2Value);
                    break;
                case ThemePropertyTypes.Vector2:
                    vector2Value.vector2Value = EditorGUILayout.Vector2Field(name, vector2Value.vector2Value);
                    break;
                case ThemePropertyTypes.Vector3:
                    SerializedProperty vector3Value = item.FindPropertyRelative("Vector3");
                    vector3Value.vector3Value = EditorGUILayout.Vector3Field(name, vector3Value.vector3Value);
                    break;
                case ThemePropertyTypes.Vector4:
                    SerializedProperty vector4Value = item.FindPropertyRelative("Vector4");
                    vector4Value.vector4Value = EditorGUILayout.Vector4Field(name, vector4Value.vector4Value);
                    break;
                case ThemePropertyTypes.Quaternion:
                    SerializedProperty quaternionValue = item.FindPropertyRelative("Quaternion");
                    Vector4 vect4 = new Vector4(quaternionValue.quaternionValue.x, quaternionValue.quaternionValue.y, quaternionValue.quaternionValue.z, quaternionValue.quaternionValue.w);
                    vect4 = EditorGUILayout.Vector4Field(name, vect4);
                    quaternionValue.quaternionValue = new Quaternion(vect4.x, vect4.y, vect4.z, vect4.w);
                    break;
                case ThemePropertyTypes.Texture:
                    SerializedProperty texture = item.FindPropertyRelative("Texture");
                    EditorGUILayout.PropertyField(texture, new GUIContent(name, ""), false);
                    break;
                case ThemePropertyTypes.Material:
                    SerializedProperty material = item.FindPropertyRelative("Material");
                    EditorGUILayout.PropertyField(material, new GUIContent(name, ""), false);
                    break;
                case ThemePropertyTypes.AudioClip:
                    SerializedProperty audio = item.FindPropertyRelative("AudioClip");
                    EditorGUILayout.PropertyField(audio, new GUIContent(name, ""), false);
                    break;
                case ThemePropertyTypes.Animaiton:
                    SerializedProperty animation = item.FindPropertyRelative("Animation");
                    EditorGUILayout.PropertyField(animation, new GUIContent(name, ""), false);
                    break;
                case ThemePropertyTypes.GameObject:
                    SerializedProperty gameObjectValue = item.FindPropertyRelative("GameObject");
                    EditorGUILayout.PropertyField(gameObjectValue, new GUIContent(name, ""), false);
                    break;
                case ThemePropertyTypes.String:
                    stringValue.stringValue = EditorGUILayout.TextField(name, stringValue.stringValue);
                    break;
                case ThemePropertyTypes.Bool:
                    SerializedProperty boolValue = item.FindPropertyRelative("Bool");
                    boolValue.boolValue = EditorGUILayout.Toggle(name, boolValue.boolValue);
                    break;
                case ThemePropertyTypes.AnimatorTrigger:
                    stringValue.stringValue = EditorGUILayout.TextField(name, stringValue.stringValue);
                    break;
                case ThemePropertyTypes.Shader:
                    SerializedProperty shaderObjectValue = item.FindPropertyRelative("Shader");
                    EditorGUILayout.PropertyField(shaderObjectValue, new GUIContent(name, ""), false);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Theme Definition Management

        protected virtual void AddThemeDefinition()
        {
            // TODO: Troy - Harden this code
            Type defaultType = typeof(InteractableActivateTheme);

            ThemeDefinition newDefinition = ThemeDefinition.GetDefaultThemeDefinition(defaultType).Value;
            ValidateThemeDefinition(ref newDefinition, theme.GetStates());

            theme.Definitions.Add(newDefinition);
            theme.History.Add(new Dictionary<Type, ThemeDefinition>());

            serializedObject.Update();
            EditorUtility.SetDirty(theme);
        }

        protected void DeleteThemeDefinition(uint index)
        {
            if (!(theme != null && theme.Definitions != null && index < theme.Definitions.Count))
            {
                // TOOD: Troy - log errro
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

            ThemeDefinition newDefinition = definition.Value;
            ValidateThemeDefinition(ref newDefinition, theme.GetStates());

            theme.Definitions[index] = newDefinition;

            themeDefinitions.serializedObject.Update();
            EditorUtility.SetDirty(theme);
        }

        protected static void ValidateThemeDefinition(ref ThemeDefinition definition, State[] states)
        {
            // For each theme property with values per possible state
            // ensure the number of values matches the number of states
            foreach (ThemeStateProperty p in definition.StateProperties)
            {
                if (p.Values.Count != states.Length)
                {
                    // Need to fill property with default values to match number of states
                    if (p.Values.Count < states.Length)
                    {
                        for (int i = p.Values.Count - 1; i < states.Length; i++)
                        {
                            p.Values.Add(p.Default.Copy());
                        }
                    }
                    else
                    {
                        // Too many property values, remove to match number of states
                        for (int i = p.Values.Count - 1; i >= states.Length; i--)
                        {
                            p.Values.RemoveAt(i);
                        }
                    }
                }
            }
        }

        #endregion

        #region Theme Definition History

        protected void ClearHistoryCache(int index)
        {
            if (theme == null || theme.History == null || index > theme.History.Count)
            {
                // TOOD: Troy - log errro
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
                    && index < target.Definitions.Count;
        }

        protected void SaveThemeDefinitionHistory(int index, Type definitionClassType)
        {
            if (theme == null || theme.History == null || theme.Definitions == null)
            {
                // TODO: Troy - File warning/error?
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
                // TODO: Troy - File warning/error?
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

        private static ShaderUtil.ShaderPropertyType? GetShaderPropertyFilter(ThemePropertyTypes shaderPropertyType)
        {
            ShaderUtil.ShaderPropertyType? shaderType = null;
            switch (shaderPropertyType)
            {
                case ThemePropertyTypes.Color:
                    shaderType = ShaderUtil.ShaderPropertyType.Color;
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    shaderType = ShaderUtil.ShaderPropertyType.Float;
                    break;
                // TODO: Troy Fill more here
            }
            return shaderType;
        }

        private static List<string> GetShaderPropertyList(Shader shader, ShaderUtil.ShaderPropertyType? filterType = null)
        {
            List<string> results = new List<string>();

            if (shader == null) return results;

            int count = ShaderUtil.GetPropertyCount(shader);
            results.Capacity = count;

            for (int i = 0; i < count; i++)
            {
                bool isHidden = ShaderUtil.IsShaderPropertyHidden(shader, i);
                bool isValidPropertyType = filterType == null || filterType.Value == ShaderUtil.GetPropertyType(shader, i);
                if (!isHidden && isValidPropertyType)
                {
                    results.Add(ShaderUtil.GetPropertyName(shader, i));
                }
            }

            return results;
        }

        // TODO: Troy - What to do with this?
        public static void AddAnimator(SerializedProperty prop)
        {
            SerializedProperty target = prop.FindPropertyRelative("Target");
            SerializedProperty targetStates = prop.FindPropertyRelative("States");

            GameObject host = target.objectReferenceValue as GameObject;
            string path = "Assets/Animations";

            if (host != null)
            {
                string controllerName = host.name + "Controller.controller";

                path = EditorUtility.SaveFilePanelInProject(
                   "Save Animator Controller",
                   controllerName,
                   "controller",
                   "Create a name and select a location for the new Animator Controller");

                if (path.Length != 0)
                {
                    // we have a location
                    UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);
                    AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

                    for (int i = 0; i < targetStates.arraySize; i++)
                    {
                        string name = targetStates.GetArrayElementAtIndex(i).stringValue;

                        controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
                        AnimationClip clip = AnimatorController.AllocateAnimatorClip(name);

                        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
                        settings.loopTime = false;
                        AnimationUtility.SetAnimationClipSettings(clip, settings);

                        AssetDatabase.AddObjectToAsset(clip, controller);
                        AnimatorState newState = controller.AddMotion(clip);

                        //AnimatorState newState = stateMachine.AddState(name);
                        AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(newState);
                        transition.AddCondition(AnimatorConditionMode.If, 0, name);
                        transition.duration = 1;
                    }

                    Animator animator = host.AddComponent<Animator>();
                    animator.runtimeAnimatorController = controller;
                }
            }
        }
    }
#endif
}
