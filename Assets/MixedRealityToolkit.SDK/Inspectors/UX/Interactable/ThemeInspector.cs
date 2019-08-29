// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    /// <summary>
    /// Inspector for themes, and used by Interactable
    /// </summary>

#if UNITY_EDITOR
    [CustomEditor(typeof(Theme))]
    public class ThemeInspector : UnityEditor.Editor
    {
        protected SerializedProperty themeDefinitions;
        protected SerializedProperty states;
        protected Theme theme;

        protected static string[] shaderOptions;
        protected static State[] themeStates;

        protected GUIStyle boxStyle;
        protected bool layoutComplete = false;
        private const float ThemeStateFontScale = 1.2f;

        private static readonly GUIContent AddThemePropertyLabel = new GUIContent("+ Add Theme Property", "Add Theme Property");
        private static readonly GUIContent RemoveThemePropertyContent = new GUIContent("-", "Remove Theme Property");
        private static readonly GUIContent CreateAnimationsContent = new GUIContent("Create Animations", "Create and add an Animator with AnimationClips");
        private static readonly GUIContent EasingContent = new GUIContent("Easing", "should the theme animate state values");

        public override void OnInspectorGUI()
        {
            RenderCustomInspector();
        }

        protected virtual void RenderBaseInspector()
        {
            base.OnInspectorGUI();
        }

        public virtual void RenderCustomInspector()
        {
            theme = target as Theme;


            // TODO: Troy Make this all a static function that can be called*

            themeDefinitions = serializedObject.FindProperty("Definitions");
            states = serializedObject.FindProperty("States");

            serializedObject.Update();

            boxStyle = InspectorUIUtility.Box(0);

            using (new EditorGUILayout.VerticalScope(boxStyle))
            {
                if (!RenderStates())
                {
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }

            // If no theme properties assigned, add a default one
            if (themeDefinitions.arraySize < 1 || InspectorUIUtility.FlexButton(AddThemePropertyLabel))
            {
                AddThemeDefinition();
                return;
            }

            RenderThemeSettings(theme, themeDefinitions, GetStates());

            RenderThemeStates(themeDefinitions, GetStates(), 0);

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// draw the states property field for assigning states
        /// Set the default state if one does not exist
        /// </summary>
        /// <returns></returns>
        protected bool RenderStates()
        {
            // If states value is not provided, try to use Default states type
            if (states.objectReferenceValue == null)
            {
                states.objectReferenceValue = GetDefaultInteractableStates();
            }

            GUI.enabled = !(EditorApplication.isPlaying || EditorApplication.isPaused);
            EditorGUILayout.PropertyField(states, new GUIContent("States", "The States this Interactable is based on"));
            GUI.enabled = true;

            if (states.objectReferenceValue == null || GetStates().Length < 1)
            {
                InspectorUIUtility.DrawError("Please assign a valid States object!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the list of states from the theme
        /// </summary>
        /// <returns></returns>
        protected virtual State[] GetStates()
        {
            Theme theme = (Theme)target;

            // TODO: Troy Move this function out to here?
            themeStates = theme.GetStates();
            return themeStates;
        }

        protected virtual void AddThemeDefinition()
        {
            // TODO: Troy - Harden this code
            var themeOptions = InteractableProfileItem.GetThemeTypes();
            var defaultType = themeOptions.Types[0];

            theme.Definitions.Add(CreateThemeDefinition(theme.GetStates(), defaultType));

            serializedObject.Update();
        }

        private static string BuildPreferenceKey(Theme target, int index, Type definitionClassType)
        {
            return target.name + "_" + index + "_" + definitionClassType.Name;
        }

        protected static void ClearThemeDefinitions(Theme target, int index)
        {
            var themeOptions = InteractableProfileItem.GetThemeTypes();

            foreach(var type in themeOptions.Types)
            {
                string prefKey = BuildPreferenceKey(target, index, type);
                SessionState.EraseString(prefKey);
            }
        }

        protected static void SaveThemeDefinition(Theme target, int index, Type definitionClassType)
        {
            if (target == null || target.Definitions == null 
                || index < 0 || target.Definitions.Count < index)
            {
                // TODO: Troy - File warning/error?
                return;
            }

            string prefKey = BuildPreferenceKey(target, index, definitionClassType);

            var definition = target.Definitions[index];
            string jsonDefinition = JsonUtility.ToJson(definition);

            SessionState.SetString(prefKey, jsonDefinition);

            Debug.Log(jsonDefinition);
        }

        protected static void LoadThemeDefinition(Theme target, int index, Type newDefinitionClassType)
        {
            string prefKey = BuildPreferenceKey(target, index, newDefinitionClassType);
            var historyJSON = SessionState.GetString(prefKey, string.Empty);

            if (!string.IsNullOrEmpty(historyJSON))
            {
                ThemeDefinition savedDefinition = JsonUtility.FromJson<ThemeDefinition>(historyJSON);
                target.Definitions[index] = savedDefinition;
            }
            else
            {
                // TODO: Troy - Add comments, 
                target.Definitions[index] = CreateThemeDefinition(target.GetStates(), newDefinitionClassType);
            }
        }

        private static ThemeDefinition CreateThemeDefinition(State[] states, Type newDefinitionClassType)
        {
            InteractableThemeBase themeBase = (InteractableThemeBase)Activator.CreateInstance(newDefinitionClassType);

            ThemeDefinition newDefinition = new ThemeDefinition()
            {
                ClassName = newDefinitionClassType.Name,
                AssemblyQualifiedName = newDefinitionClassType.AssemblyQualifiedName, // TODO: Troy - SystemType.GetReference(type);
                Type = newDefinitionClassType,
                NoEasing = themeBase.NoEasing,
                StateProperties = themeBase.GetDefaultStateProperties(),
                CustomProperties = themeBase.GetDefaultThemeProperties(),
            };

            // TODO: Troy - Create comment here
            foreach (ThemeStateProperty p in newDefinition.StateProperties)
            {
                foreach (State s in states)
                {
                    p.Values.Add(p.Default.Copy());
                }
            }

            return newDefinition;
        }

        /*
        public static SerializedProperty ChangeThemeDefinition(int index, SerializedProperty themeDefinitionsList, SerializedProperty target, State[] states, bool isNew = false)
        {
            SerializedProperty themeDefinitionItem = themeDefinitionsList.GetArrayElementAtIndex(index);

            SerializedProperty className = themeDefinitionItem.FindPropertyRelative("ClassName");

            InteractableTypesContainer themeTypes = InteractableProfileItem.GetThemeTypes();

            // get class value types
            if (!string.IsNullOrEmpty(className.stringValue))
            {
                int propIndex = InspectorUIUtility.ReverseLookup(className.stringValue, themeTypes.ClassNames);
                GameObject renderHost = target != null ? (GameObject)target.objectReferenceValue : null;

                InteractableThemeBase themeBase = (InteractableThemeBase)Activator.CreateInstance(themeTypes.Types[propIndex], renderHost);

                // does this object have the right component types
                SerializedProperty isValid = themeDefinitionItem.FindPropertyRelative("IsValid");
                SerializedProperty noEasing = themeDefinitionItem.FindPropertyRelative("NoEasing");
                noEasing.boolValue = themeBase.NoEasing;

                bool valid = false;
                bool hasText = false;
                bool hasRenderer = false;

                if (renderHost != null)
                {
                    foreach(Type type in themeBase.Types)
                    {
                        if (renderHost.gameObject.GetComponent(type))
                        {
                            hasText = hasText || type == typeof(TextMesh) || type == typeof(Text);
                            hasRenderer = hasRenderer || type == typeof(Renderer);
                            valid = true;
                        }
                    }
                }

                isValid.boolValue = valid;

                // setup the values
                // get the state names
                List<ThemeStateProperty> properties = themeBase.StateProperties;
                List<ThemeProperty> customSettings = themeBase.Properties;

                SerializedProperty sProps = themeDefinitionItem.FindPropertyRelative("StateProperties");
                //SerializedProperty history = settingsItem.FindPropertyRelative("History");
                //SerializedProperty customHistory = settingsItem.FindPropertyRelative("CustomHistory");

                SerializedProperty custom = themeDefinitionItem.FindPropertyRelative("CustomProperties");
                
                if (isNew)
                {
                    sProps.ClearArray();
                    custom.ClearArray();
                }
                else
                {
                    // stick the copy in the new format into sProps.
                    sProps = CopyPropertiesFromHistory(sProps, properties, history, out history);
                    custom = CopyCustomHistory(custom, customSettings, customHistory, out customHistory);
                }

                for (int propertyIndex = 0; propertyIndex < properties.Count; propertyIndex++)
                {
                    bool newItem = isNew;
                    if (isNew)
                    {
                        sProps.InsertArrayElementAtIndex(sProps.arraySize);
                    }

                    ThemeStateProperty property = properties[propertyIndex];

                    SerializedProperty item = sProps.GetArrayElementAtIndex(propertyIndex);
                    SerializedProperty name = item.FindPropertyRelative("Name");
                    SerializedProperty type = item.FindPropertyRelative("Type");
                    SerializedProperty values = item.FindPropertyRelative("Values");

                    name.stringValue = property.Name;
                    type.intValue = (int)property.Type;

                    int numOfValues = states.Length;
                    for (int j = 0; j < numOfValues; j++)
                    {
                        if (values.arraySize <= j)
                        {
                            values.InsertArrayElementAtIndex(values.arraySize);
                            newItem = true;
                        }

                        SerializedProperty valueItem = values.GetArrayElementAtIndex(j);
                        SerializedProperty valueName = valueItem.FindPropertyRelative("Name");
                        valueName.stringValue = states[j].Name;

                        if (newItem && property.Default != null)
                        {
                            if ((ThemePropertyTypes)type.intValue == ThemePropertyTypes.AnimatorTrigger)
                            {
                                ThemePropertyValue propValue = new ThemePropertyValue();
                                propValue.Name = valueName.stringValue;
                                propValue.String = states[j].Name;

                                SerializeThemeValues(propValue, valueItem, type.intValue);
                            }
                            else
                            {
                                // assign default values if new item
                                SerializeThemeValues(property.Default, valueItem, type.intValue);
                            }
                        }
                    }
                    
                    List<ShaderPropertyType> shaderPropFilter = new List<ShaderPropertyType>();
                    // do we need a propId?
                    if (property.Type == ThemePropertyTypes.Color)
                    {
                        if ((!hasText && hasRenderer) || (!hasText && target == null))
                        {
                            shaderPropFilter.Add(ShaderPropertyType.Color);
                        }
                        else if (!hasText && !hasRenderer)
                        {
                            valid = false;
                        }
                    }

                    if (property.Type == ThemePropertyTypes.ShaderFloat 
                        || property.Type == ThemePropertyTypes.ShaderRange)
                    {
                        if (hasRenderer || target == null)
                        {
                            shaderPropFilter.Add(ShaderPropertyType.Float);
                            shaderPropFilter.Add(ShaderPropertyType.Range);
                        }
                        else
                        {
                            valid = false;
                        }
                    }

                    SerializedProperty propId = item.FindPropertyRelative("PropId");
                    if (newItem)
                    {
                        propId.intValue = 0;
                    }

                    SerializedProperty shaderList = item.FindPropertyRelative("ShaderOptions");
                    SerializedProperty shaderNames = item.FindPropertyRelative("ShaderOptionNames");
                    SerializedProperty shaderName = item.FindPropertyRelative("ShaderName");

                    shaderList.ClearArray();
                    shaderNames.ClearArray();

                    if (valid && shaderPropFilter.Count > 0)
                    {
                        Renderer renderer = null;
                        if (renderHost != null)
                        {
                            renderer = renderHost.gameObject.GetComponent<Renderer>();
                        }

                        ShaderInfo info = GetShaderProperties(renderer, shaderPropFilter.ToArray());
                        PopulateShaderNames(shaderList, shaderNames, shaderName, info);
                    }
                }

                if (!valid)
                {
                    isValid.boolValue = false;
                }
            }

            return themeDefinitionsList;
        }*/

        private static void PopulateShaderNames(SerializedProperty shaderList, SerializedProperty shaderNames, SerializedProperty shaderName, ShaderInfo info)
        {
            ShaderProperties[] shaderProps = info.ShaderOptions;
            shaderName.stringValue = info.Name;
            for (int n = 0; n < shaderProps.Length; n++)
            {
                shaderList.InsertArrayElementAtIndex(shaderList.arraySize);
                SerializedProperty shaderListItem = shaderList.GetArrayElementAtIndex(shaderList.arraySize - 1);
                SerializedProperty shaderListName = shaderListItem.FindPropertyRelative("Name");
                SerializedProperty shaderListType = shaderListItem.FindPropertyRelative("Type");
                SerializedProperty shaderListRange = shaderListItem.FindPropertyRelative("Range");

                shaderListName.stringValue = shaderProps[n].Name;
                shaderListType.intValue = (int)shaderProps[n].Type;
                shaderListRange.vector2Value = shaderProps[n].Range;

                shaderNames.InsertArrayElementAtIndex(shaderNames.arraySize);
                SerializedProperty names = shaderNames.GetArrayElementAtIndex(shaderNames.arraySize - 1);
                names.stringValue = shaderProps[n].Name;
            }
        }

        /// <summary>
        /// copy custom settings from history
        /// </summary>
        /// <param name="oldCustom"></param>
        /// <param name="newCustomSettings"></param>
        /// <param name="customHistory"></param>
        /// <param name="customHistoryOut"></param>
        /// <returns></returns>
        private static SerializedProperty CopyCustomHistory(SerializedProperty oldCustom, List<ThemeProperty> newCustomSettings, SerializedProperty customHistory, out SerializedProperty customHistoryOut)
        {
            int oldCount = oldCustom.arraySize;

            for (int i = oldCount - 1; i > -1; i--)
            {
                if (customHistory != null)
                {
                    SerializedProperty item = oldCustom.GetArrayElementAtIndex(i);
                    SerializedProperty name = item.FindPropertyRelative("Name");
                    SerializedProperty type = item.FindPropertyRelative("Type");

                    bool hasProperty = false;
                    for (int j = 0; j < customHistory.arraySize; j++)
                    {
                        SerializedProperty historyItem = customHistory.GetArrayElementAtIndex(j);
                        SerializedProperty historyName = historyItem.FindPropertyRelative("Name");
                        SerializedProperty historyType = historyItem.FindPropertyRelative("Type");

                        if (name.stringValue == historyName.stringValue && type.intValue == historyType.intValue)
                        {
                            hasProperty = true;

                            // update history
                            historyItem = CopyCustomSettings(item, historyItem);
                            break;
                        }
                    }

                    if (!hasProperty)
                    {
                        // add new item to history
                        customHistory.InsertArrayElementAtIndex(customHistory.arraySize);
                        SerializedProperty historyItem = customHistory.GetArrayElementAtIndex(customHistory.arraySize - 1);
                        historyItem = CopyCustomSettings(item, historyItem);
                    }
                }

                oldCustom.DeleteArrayElementAtIndex(i);
            }

            customHistoryOut = customHistory;

            for (int i = 0; i < newCustomSettings.Count; i++)
            {
                oldCustom.InsertArrayElementAtIndex(oldCustom.arraySize);
                SerializedProperty newProp = oldCustom.GetArrayElementAtIndex(oldCustom.arraySize - 1);

                SerializedProperty newName = newProp.FindPropertyRelative("Name");
                SerializedProperty newType = newProp.FindPropertyRelative("Type");
                SerializedProperty newValue = newProp.FindPropertyRelative("Value");
                newName.stringValue = newCustomSettings[i].Name;
                newType.intValue = (int)newCustomSettings[i].Type;

                if (customHistory != null)
                {
                    for (int j = 0; j < customHistory.arraySize; j++)
                    {
                        SerializedProperty item = customHistory.GetArrayElementAtIndex(j);
                        SerializedProperty name = item.FindPropertyRelative("Name");
                        SerializedProperty type = item.FindPropertyRelative("Type");
                        SerializedProperty value = item.FindPropertyRelative("Value");

                        if (name.stringValue == newName.stringValue && type.intValue == newType.intValue)
                        {
                            newValue = CopyThemeValues(value, newValue, newType.intValue);
                        }
                    }
                }
            }

            return oldCustom;
        }

        /// <summary>
        /// copy history values to current theme property
        /// </summary>
        /// <param name="oldProperties"></param>
        /// <param name="newProperties"></param>
        /// <param name="history"></param>
        /// <param name="historyOut"></param>
        /// <returns></returns>
        public static SerializedProperty CopyPropertiesFromHistory(SerializedProperty oldProperties, List<ThemeStateProperty> newProperties, SerializedProperty history, out SerializedProperty historyOut)
        {
            int oldCount = oldProperties.arraySize;

            for (int i = oldCount - 1; i > -1; i--)
            {
                if (history != null)
                {
                    SerializedProperty item = oldProperties.GetArrayElementAtIndex(i);
                    SerializedProperty name = item.FindPropertyRelative("Name");
                    SerializedProperty type = item.FindPropertyRelative("Type");

                    bool hasProperty = false;
                    for (int j = 0; j < history.arraySize; j++)
                    {
                        SerializedProperty historyItem = history.GetArrayElementAtIndex(j);
                        SerializedProperty historyName = historyItem.FindPropertyRelative("Name");
                        SerializedProperty historyType = historyItem.FindPropertyRelative("Type");

                        if (name.stringValue == historyName.stringValue && type.intValue == historyType.intValue)
                        {
                            hasProperty = true;

                            // update history
                            historyItem = CopyThemeProperties(item, historyItem);
                            break;
                        }
                    }

                    if (!hasProperty)
                    {
                        // add new item to history
                        history.InsertArrayElementAtIndex(history.arraySize);
                        SerializedProperty historyItem = history.GetArrayElementAtIndex(history.arraySize - 1);
                        historyItem = CopyThemeProperties(item, historyItem);
                    }
                }

                oldProperties.DeleteArrayElementAtIndex(i);
            }

            historyOut = history;

            for (int i = 0; i < newProperties.Count; i++)
            {
                oldProperties.InsertArrayElementAtIndex(oldProperties.arraySize);
                SerializedProperty newProp = oldProperties.GetArrayElementAtIndex(oldProperties.arraySize - 1);

                SerializedProperty newName = newProp.FindPropertyRelative("Name");
                SerializedProperty newType = newProp.FindPropertyRelative("Type");
                SerializedProperty newValues = newProp.FindPropertyRelative("Values");
                SerializedProperty newPropId = newProp.FindPropertyRelative("PropId");
                newName.stringValue = newProperties[i].Name;
                newType.intValue = (int)newProperties[i].Type;

                if (history != null)
                {
                    for (int j = 0; j < history.arraySize; j++)
                    {
                        SerializedProperty item = history.GetArrayElementAtIndex(j);
                        SerializedProperty name = item.FindPropertyRelative("Name");
                        SerializedProperty type = item.FindPropertyRelative("Type");
                        SerializedProperty values = item.FindPropertyRelative("Values");
                        SerializedProperty propId = item.FindPropertyRelative("PropId");

                        if (name.stringValue == newName.stringValue && type.intValue == newType.intValue)
                        {
                            newPropId.intValue = propId.intValue;

                            for (int h = 0; h < values.arraySize; h++)
                            {
                                if (h >= newValues.arraySize)
                                {
                                    newValues.InsertArrayElementAtIndex(newValues.arraySize);
                                }

                                SerializedProperty newValue = newValues.GetArrayElementAtIndex(h);
                                SerializedProperty valueItem = values.GetArrayElementAtIndex(h);
                                newValue = CopyThemeValues(valueItem, newValue, newType.intValue);
                            }
                        }
                    }
                }
            }

            return oldProperties;
        }

        /// <summary>
        /// copy some theme property values from serialized properties
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        /// <returns></returns>
        public static SerializedProperty CopyThemeProperties(SerializedProperty copyFrom, SerializedProperty copyTo)
        {
            SerializedProperty newName = copyTo.FindPropertyRelative("Name");
            SerializedProperty newType = copyTo.FindPropertyRelative("Type");
            SerializedProperty newValues = copyTo.FindPropertyRelative("Values");
            SerializedProperty newPropId = copyTo.FindPropertyRelative("PropId");

            SerializedProperty oldName = copyFrom.FindPropertyRelative("Name");
            SerializedProperty oldType = copyFrom.FindPropertyRelative("Type");
            SerializedProperty oldValues = copyFrom.FindPropertyRelative("Values");
            SerializedProperty oldPropId = copyFrom.FindPropertyRelative("PropId");

            newName.stringValue = oldName.stringValue;
            newType.intValue = oldType.intValue;
            newPropId.intValue = oldPropId.intValue;

            newValues.ClearArray();

            for (int index = 0; index < oldValues.arraySize; index++)
            {
                newValues.InsertArrayElementAtIndex(newValues.arraySize);
                SerializedProperty newValue = newValues.GetArrayElementAtIndex(newValues.arraySize - 1);
                SerializedProperty valueItem = oldValues.GetArrayElementAtIndex(index);
                newValue = CopyThemeValues(valueItem, newValue, newType.intValue);
            }

            return copyTo;
        }

        /// <summary>
        /// copy some theme property values from serialized properties
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        /// <returns></returns>
        public static SerializedProperty CopyCustomSettings(SerializedProperty copyFrom, SerializedProperty copyTo)
        {
            SerializedProperty newName = copyTo.FindPropertyRelative("Name");
            SerializedProperty newType = copyTo.FindPropertyRelative("Type");
            SerializedProperty newValue = copyTo.FindPropertyRelative("Value");

            SerializedProperty oldName = copyFrom.FindPropertyRelative("Name");
            SerializedProperty oldType = copyFrom.FindPropertyRelative("Type");
            SerializedProperty oldValue = copyFrom.FindPropertyRelative("Value");

            newName.stringValue = oldName.stringValue;
            newType.intValue = oldType.intValue;

            newValue = CopyThemeValues(oldValue, newValue, newType.intValue);

            return copyTo;
        }

        /// <summary>
        /// copy theme values from serialized properties
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SerializedProperty CopyThemeValues(SerializedProperty copyFrom, SerializedProperty copyTo, int type)
        {
            SerializedProperty floatFrom;
            SerializedProperty floatTo;
            SerializedProperty vector2From;
            SerializedProperty vector2To;
            SerializedProperty stringFrom;
            SerializedProperty stringTo;

            switch ((ThemePropertyTypes)type)
            {
                case ThemePropertyTypes.Float:
                    floatFrom = copyFrom.FindPropertyRelative("Float");
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = floatFrom.floatValue;
                    break;
                case ThemePropertyTypes.Int:
                    SerializedProperty intFrom = copyFrom.FindPropertyRelative("Int");
                    SerializedProperty intTo = copyTo.FindPropertyRelative("Int");
                    intTo.intValue = intFrom.intValue;
                    break;
                case ThemePropertyTypes.Color:
                    SerializedProperty colorFrom = copyFrom.FindPropertyRelative("Color");
                    SerializedProperty colorTo = copyTo.FindPropertyRelative("Color");
                    colorTo.colorValue = colorFrom.colorValue;
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    floatFrom = copyFrom.FindPropertyRelative("Float");
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = floatFrom.floatValue;
                    break;
                case ThemePropertyTypes.ShaderRange:
                    vector2From = copyFrom.FindPropertyRelative("Vector2");
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = vector2From.vector2Value;
                    break;
                case ThemePropertyTypes.Vector2:
                    vector2From = copyFrom.FindPropertyRelative("Vector2");
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = vector2From.vector2Value;
                    break;
                case ThemePropertyTypes.Vector3:
                    SerializedProperty vector3From = copyFrom.FindPropertyRelative("Vector3");
                    SerializedProperty vector3To = copyTo.FindPropertyRelative("Vector3");
                    vector3To.vector3Value = vector3From.vector3Value;
                    break;
                case ThemePropertyTypes.Vector4:
                    SerializedProperty vector4From = copyFrom.FindPropertyRelative("Vector4");
                    SerializedProperty vector4To = copyTo.FindPropertyRelative("Vector4");
                    vector4To.vector4Value = vector4From.vector4Value;
                    break;
                case ThemePropertyTypes.Quaternion:
                    SerializedProperty quaternionFrom = copyFrom.FindPropertyRelative("Quaternion");
                    SerializedProperty quaternionTo = copyTo.FindPropertyRelative("Quaternion");
                    quaternionTo.quaternionValue = quaternionFrom.quaternionValue;
                    break;
                case ThemePropertyTypes.Texture:
                    SerializedProperty textureFrom = copyFrom.FindPropertyRelative("Texture");
                    SerializedProperty textureTo = copyTo.FindPropertyRelative("Texture");
                    textureTo.objectReferenceValue = textureFrom.objectReferenceValue;
                    break;
                case ThemePropertyTypes.Material:
                    SerializedProperty materialFrom = copyFrom.FindPropertyRelative("Material");
                    SerializedProperty materialTo = copyTo.FindPropertyRelative("Material");
                    materialTo.objectReferenceValue = materialFrom.objectReferenceValue;
                    break;
                case ThemePropertyTypes.AudioClip:
                    SerializedProperty audioClipFrom = copyFrom.FindPropertyRelative("AudioClip");
                    SerializedProperty audioClipTo = copyTo.FindPropertyRelative("AudioClip");
                    audioClipTo.objectReferenceValue = audioClipFrom.objectReferenceValue;
                    break;
                case ThemePropertyTypes.Animaiton:
                    SerializedProperty animationFrom = copyFrom.FindPropertyRelative("Animation");
                    SerializedProperty animationTo = copyTo.FindPropertyRelative("Animation");
                    animationTo.objectReferenceValue = animationFrom.objectReferenceValue;
                    break;
                case ThemePropertyTypes.GameObject:
                    SerializedProperty gameObjectFrom = copyFrom.FindPropertyRelative("GameObject");
                    SerializedProperty gameObjectTo = copyTo.FindPropertyRelative("GameObject");
                    gameObjectTo.objectReferenceValue = gameObjectFrom.objectReferenceValue;
                    break;
                case ThemePropertyTypes.String:
                    stringFrom = copyFrom.FindPropertyRelative("String");
                    stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = stringFrom.stringValue;
                    break;
                case ThemePropertyTypes.Bool:
                    SerializedProperty boolFrom = copyFrom.FindPropertyRelative("Bool");
                    SerializedProperty boolTo = copyTo.FindPropertyRelative("Bool");
                    boolTo.boolValue = boolFrom.boolValue;
                    break;
                case ThemePropertyTypes.AnimatorTrigger:
                    stringFrom = copyFrom.FindPropertyRelative("String");
                    stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = stringFrom.stringValue;
                    break;
                default:
                    break;
            }

            return copyTo;
        }

        /// <summary>
        /// load theme property values into a serialized property
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SerializedProperty SerializeThemeValues(ThemePropertyValue copyFrom, SerializedProperty copyTo, int type)
        {
            SerializedProperty floatTo;
            SerializedProperty vector2To;
            SerializedProperty stringTo;

            switch ((ThemePropertyTypes)type)
            {
                case ThemePropertyTypes.Float:
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = copyFrom.Float;
                    break;
                case ThemePropertyTypes.Int:
                    SerializedProperty intTo = copyTo.FindPropertyRelative("Int");
                    intTo.intValue = copyFrom.Int;
                    break;
                case ThemePropertyTypes.Color:
                    SerializedProperty colorTo = copyTo.FindPropertyRelative("Color");
                    colorTo.colorValue = copyFrom.Color;
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = copyFrom.Float;
                    break;
                case ThemePropertyTypes.ShaderRange:
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = copyFrom.Vector2;
                    break;
                case ThemePropertyTypes.Vector2:
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = copyFrom.Vector2;
                    break;
                case ThemePropertyTypes.Vector3:
                    SerializedProperty vector3To = copyTo.FindPropertyRelative("Vector3");
                    vector3To.vector3Value = copyFrom.Vector3;
                    break;
                case ThemePropertyTypes.Vector4:
                    SerializedProperty vector4To = copyTo.FindPropertyRelative("Vector4");
                    vector4To.vector4Value = copyFrom.Vector4;
                    break;
                case ThemePropertyTypes.Quaternion:
                    SerializedProperty quaternionTo = copyTo.FindPropertyRelative("Quaternion");
                    quaternionTo.quaternionValue = copyFrom.Quaternion;
                    break;
                case ThemePropertyTypes.Texture:
                    SerializedProperty textureTo = copyTo.FindPropertyRelative("Texture");
                    textureTo.objectReferenceValue = copyFrom.Texture;
                    break;
                case ThemePropertyTypes.Material:
                    SerializedProperty materialTo = copyTo.FindPropertyRelative("Material");
                    materialTo.objectReferenceValue = copyFrom.Material;
                    break;
                case ThemePropertyTypes.AudioClip:
                    SerializedProperty audioClipTo = copyTo.FindPropertyRelative("AudioClip");
                    audioClipTo.objectReferenceValue = copyFrom.AudioClip;
                    break;
                case ThemePropertyTypes.Animaiton:
                    SerializedProperty animationTo = copyTo.FindPropertyRelative("Animation");
                    animationTo.objectReferenceValue = copyFrom.Animation;
                    break;
                case ThemePropertyTypes.GameObject:
                    SerializedProperty gameObjectTo = copyTo.FindPropertyRelative("GameObject");
                    gameObjectTo.objectReferenceValue = copyFrom.GameObject;
                    break;
                case ThemePropertyTypes.String:
                    stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = copyFrom.String;
                    break;
                case ThemePropertyTypes.Bool:
                    SerializedProperty boolTo = copyTo.FindPropertyRelative("Bool");
                    boolTo.boolValue = copyFrom.Bool;
                    break;
                case ThemePropertyTypes.AnimatorTrigger:
                    stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = copyFrom.String;
                    break;
                default:
                    break;
            }

            return copyTo;
        }

        public static void RenderThemeSettings(Theme target,
            SerializedProperty themeDefinitions, 
            State[] states,
            int margin = 0)
        {
            GUIStyle box = InspectorUIUtility.Box(margin);

            // Loop through all InteractableThemePropertySettings of Theme
            for (int index = 0; index < themeDefinitions.arraySize; index++)
            {
                SerializedProperty themeDefinition = themeDefinitions.GetArrayElementAtIndex(index);

                SerializedProperty className = themeDefinition.FindPropertyRelative("ClassName");

                using (new EditorGUILayout.VerticalScope(box))
                {
                    // TODO: Troy -> Important commentshere
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var themeOptions = InteractableProfileItem.GetThemeTypes();
                        int id = InspectorUIUtility.ReverseLookup(className.stringValue, themeOptions.ClassNames);
                        int newId = EditorGUILayout.Popup("Theme Runtime", id, themeOptions.ClassNames);

                        // Create Delete button if we have an array of themes
                        // TODO: Troy -> Alright to be empty?
                        if (themeDefinitions.arraySize > 1)
                        {
                            if (InspectorUIUtility.SmallButton(RemoveThemePropertyContent))
                            {
                                //ClearThemeDefinitions(target, index);

                                target.Definitions.RemoveAt(index);
                                //target.IDs.RemoveAt(index);

                                themeDefinitions.serializedObject.Update();
                                // TODO: Troy - Need to call delete themedefinition button
                                //themeDefinitions.DeleteArrayElementAtIndex(index);
                                return;
                            }
                        }

                        // If user changed the theme type for current themeDefinition, 
                        if (id != newId)
                        {
                            Type oldType = themeOptions.Types[id];
                            Type newType = themeOptions.Types[newId];

                            // Save theme definition to cache
                            SaveThemeDefinition(target, index, oldType);

                            // Load new theme definition or grab last state from cache
                            LoadThemeDefinition(target, index, newType);

                            themeDefinitions.serializedObject.Update();
                            return;

                            // TODO: Troy 
                            // Save serializedproperty/serializedobject.update()?

                            // TODO: Troy
                            /*
                            SerializedProperty assemblyQualifiedName = themeDefinition.FindPropertyRelative("AssemblyQualifiedName");
                            className.stringValue = themeOptions.ClassNames[newId];
                            assemblyQualifiedName.stringValue = themeOptions.AssemblyQualifiedNames[newId];
                            themeSettings = ChangeThemeDefinition(settingIndex, themeSettings, gameObjectProperty, states);
                            */
                        }
                    }

                    SerializedProperty stateProperties = themeDefinition.FindPropertyRelative("StateProperties");
                    int animatorCount = 0;

                    // TODO: Troy Update comment
                    // Loop through all InteractableThemeProperty of InteractableThemePropertySettings
                    for (int p = 0; p < stateProperties.arraySize; p++)
                    {
                        SerializedProperty propertyItem = stateProperties.GetArrayElementAtIndex(p);

                        SerializedProperty propType = propertyItem.FindPropertyRelative("Type");
                        ThemePropertyTypes type = (ThemePropertyTypes)propType.intValue;

                        if (type == ThemePropertyTypes.AnimatorTrigger)
                        {
                            animatorCount++;
                        }

                        // TODO: Troy Delete code?
                        /*
                        SerializedProperty shaderNames = propertyItem.FindPropertyRelative("ShaderOptionNames");
                        if (shaderNames.arraySize > 0)
                        {
                            RenderShaderProperties(propertyItem, gameObjectProperty, type);
                        }
                        else
                        {
                            // If there are no shader options available
                            SerializedProperty shaderList = propertyItem.FindPropertyRelative("ShaderOptions");
                            SerializedProperty shaderName = propertyItem.FindPropertyRelative("ShaderName");

                            ShaderPropertyType[] filter = GetShaderPropertyFilters(type);
                            ShaderInfo info = GetShaderProperties(null, filter);
                            PopulateShaderNames(shaderList, shaderNames, shaderName, info);
                        }*/
                    }

                    SerializedProperty customProperties = themeDefinition.FindPropertyRelative("CustomProperties");
                    RenderCustomProperties(customProperties);

                    // TODO: Troy - Just check no-easing????
                    if (animatorCount < stateProperties.arraySize)
                    {
                        RenderEasingProperties(themeDefinition);
                    }

                    // TODO: Troy -> Re-add Create animation button*
                    /*
                    // check to see if an animatorController exists
                    if (animatorCount > 0 && gameObjectProperty != null)
                    {
                        GameObject host = gameObjectProperty.objectReferenceValue as GameObject;
                        Animator animator = null;

                        if (host != null)
                        {
                            animator = host.GetComponent<Animator>();
                        }

                        if (animator == null && host != null)
                        {
                            SerializedProperty themeTargetProperty = settingsItem.FindPropertyRelative("ThemeTarget");
                            SerializedProperty targetStates = themeTargetProperty.FindPropertyRelative("States");
                            targetStates = GetSerializedStates(targetStates, states);

#pragma warning disable 0219
                            // disable value is never used warning
                            //assigning values to be passed to the FlexButton
                            SerializedProperty targetProperty = themeTargetProperty.FindPropertyRelative("Target");
                            SerializedProperty props = themeTargetProperty.FindPropertyRelative("Properties");
                            targetProperty = gameObjectProperty;
                            props = stateProperties;
#pragma warning restore 0219    // enable value is never used warning

                            if (InspectorUIUtility.FlexButton(CreateAnimationsContent))
                            {
                                AddAnimator(themeTargetProperty);
                            }
                        }
                    }*/

                }
            }
        }

        /// <summary>
        /// Render shader name options for InteractableThemeProperty in a InteractableThemePropertySettings
        /// </summary>
        /// <param name="themePropertyItem">SerializedProperty of InteractableThemeProperty</param>
        /// <param name="gameObjectProperty">SerializedProperty for gameobject associated with Theme and consequently the InteractableThemeProperty</param>
        /// <param name="type">Theme property type</param>
        private static void RenderShaderProperties(SerializedProperty themePropertyItem,
            SerializedProperty gameObjectProperty, 
            ThemePropertyTypes type)
        {
            // TODO: Troy -> Delete this entirely -> Should use RenderValue 
            bool hasTextComp = false;
            SerializedProperty propId = themePropertyItem.FindPropertyRelative("PropId");
            SerializedProperty name = themePropertyItem.FindPropertyRelative("Name");

            SerializedProperty shaderNames = themePropertyItem.FindPropertyRelative("ShaderOptionNames");
            SerializedProperty shaderName = themePropertyItem.FindPropertyRelative("ShaderName");

            string[] shaderOptionNames = InspectorUIUtility.GetOptions(shaderNames);
            string propName = shaderOptionNames[propId.intValue];
            bool hasShaderProperty = true;

            if (gameObjectProperty == null)
            {
                EditorGUILayout.LabelField(new GUIContent("Shader: " + shaderName.stringValue));
            }
            else
            {
                GameObject renderHost = gameObjectProperty.objectReferenceValue as GameObject;
                if (renderHost != null)
                {
                    Renderer renderer = renderHost.GetComponent<Renderer>();
                    hasTextComp = InteractableColorTheme.HasTextComponentOnObject(renderHost);
                    if (renderer != null && !hasTextComp)
                    {
                        ShaderPropertyType[] filter = GetShaderPropertyFilters(type);
                        ShaderInfo info = GetShaderProperties(renderer, filter);

                        if (info.Name != shaderName.stringValue)
                        {
                            hasShaderProperty = false;

                            for (int i = 0; i < info.ShaderOptions.Length; i++)
                            {
                                if (info.ShaderOptions[i].Name == propName)
                                {
                                    hasShaderProperty = true;
                                    break;
                                }
                            }
                        }

                    }
                }

            }

            if (!hasTextComp)
            {
                GUIStyle popupStyle = new GUIStyle(EditorStyles.popup);
                popupStyle.margin.right = Mathf.RoundToInt(Screen.width - (Screen.width - 40));
                propId.intValue = EditorGUILayout.Popup("Material " + name.stringValue + "Id", propId.intValue, shaderOptionNames, popupStyle);

                if (!hasShaderProperty)
                {
                    InspectorUIUtility.DrawError(propName + " is not available on the currently assigned Material.");
                }
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("Text Property: Color"));
            }

            GUILayout.Space(5);

            // Handle issue where the material color id renders on objects it shouldn't
            // theme is save for a game object with a renderer, but when put on a textmesh, rendering prop values show up.
            // when changing the theme type on a TextMesh, everything works, but the rendering prop is removed from the theme on the renderer object.
            // make this passive, only show up when needed.
        }

        private static ShaderPropertyType[] GetShaderPropertyFilters(ThemePropertyTypes type)
        {
            ShaderPropertyType[] filter = new ShaderPropertyType[0];
            switch (type)
            {
                case ThemePropertyTypes.Color:
                    filter = new ShaderPropertyType[] { ShaderPropertyType.Color };
                    break;
                case ThemePropertyTypes.ShaderFloat:
                    filter = new ShaderPropertyType[] { ShaderPropertyType.Float };
                    break;
                case ThemePropertyTypes.ShaderRange:
                    filter = new ShaderPropertyType[] { ShaderPropertyType.Float };
                    break;
                default:
                    break;
            }

            return filter;
        }

        /// <summary>
        /// Renders easing & related time properties for InteractableThemePropertySettings
        /// </summary>
        /// <param name="settingsItem">Serialized property of a InteractableThemePropertySettings object</param>
        private static void RenderEasingProperties(SerializedProperty settingsItem)
        {
            SerializedProperty easing = settingsItem.FindPropertyRelative("Easing");
            SerializedProperty enabled = easing.FindPropertyRelative("Enabled");

            SerializedProperty noEasing = settingsItem.FindPropertyRelative("NoEasing");
            if (!noEasing.boolValue)
            {
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
            else
            {
                enabled.boolValue = false;
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

        // TODO: Troy - Delete? Related to animator button code but not used?
        /*
        public static SerializedProperty GetSerializedStates(SerializedProperty serialized, State[] states)
        {
            serialized.ClearArray();
            for (int i = 0; i < states.Length; i++)
            {
                serialized.InsertArrayElementAtIndex(serialized.arraySize);
                SerializedProperty state = serialized.GetArrayElementAtIndex(serialized.arraySize - 1);
                SerializedProperty activeIndex = state.FindPropertyRelative("ActiveIndex");
                SerializedProperty bit = state.FindPropertyRelative("Bit");
                SerializedProperty index = state.FindPropertyRelative("Index");
                SerializedProperty name = state.FindPropertyRelative("Name");
                SerializedProperty value = state.FindPropertyRelative("Value");

                activeIndex.intValue = states[i].ActiveIndex;
                bit.intValue = states[i].Bit;
                index.intValue = states[i].Index;
                name.stringValue = states[i].Name;
                value.intValue = states[i].Value;
            }

            return serialized;
        }*/

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
                // TODO: Troy - Render ShaderProperty here
                default:
                    break;
            }
        }

        public static void RenderThemeStates(SerializedProperty settings, State[] states, int margin = 0)
        {
            GUIStyle box = InspectorUIUtility.Box(margin);

            EditorGUILayout.BeginVertical(box);

                for (int n = 0; n < states.Length; n++)
                {
                    InspectorUIUtility.DrawLabel(states[n].Name, (int)(InspectorUIUtility.DefaultFontSize * ThemeStateFontScale), InspectorUIUtility.ColorTint50);

                    for (int j = 0; j < settings.arraySize; j++)
                    {
                        SerializedProperty settingsItem = settings.GetArrayElementAtIndex(j);

                        SerializedProperty properties = settingsItem.FindPropertyRelative("StateProperties");
                        using (new EditorGUI.IndentLevelScope())
                        {
                            for (int i = 0; i < properties.arraySize; i++)
                            {
                                SerializedProperty propertyItem = properties.GetArrayElementAtIndex(i);
                                SerializedProperty name = propertyItem.FindPropertyRelative("Name");
                                SerializedProperty type = propertyItem.FindPropertyRelative("Type");
                                SerializedProperty values = propertyItem.FindPropertyRelative("Values");
                                //SerializedProperty shaderNames = propertyItem.FindPropertyRelative("ShaderOptionNames");
                                //SerializedProperty propId = propertyItem.FindPropertyRelative("PropId");

                                // TODO: Troy - Delete
                                /*
                                string shaderPropName = "Shader";

                                if (shaderNames.arraySize > propId.intValue)
                                {
                                    SerializedProperty propName = shaderNames.GetArrayElementAtIndex(propId.intValue);
                                    shaderPropName = propName.stringValue.Substring(1);
                                }*/

                                if (n >= values.arraySize)
                                {
                                    // the state values for this theme were not created yet
                                    continue;
                                }

                                SerializedProperty item = values.GetArrayElementAtIndex(n);
                                // TODO: Troy - Fix shaderPorpName
                                //RenderValue(item, name.stringValue, shaderPropName, (ThemePropertyTypes)type.intValue);
                                RenderValue(item, name.stringValue, name.stringValue, (ThemePropertyTypes)type.intValue);
                            }
                        }
                    }
                }
                GUILayout.Space(5);

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

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

        public static ShaderInfo GetShaderProperties(Renderer renderer, ShaderPropertyType[] filter)
        {
            ShaderInfo info = new ShaderInfo();
            List<ShaderProperties> properties = new List<ShaderProperties>();
            Material material = null;

            if (renderer != null)
            {
                material = InteractableThemeShaderUtils.GetValidMaterial(renderer);
            }
            else
            {
                material = new Material(Shader.Find("Mixed Reality Toolkit/Standard"));
            }

            if (material != null)
            {
                info.Name = material.shader.name;
                int count = ShaderUtil.GetPropertyCount(material.shader);

                for (int i = 0; i < count; i++)
                {
                    string name = ShaderUtil.GetPropertyName(material.shader, i);
                    ShaderPropertyType type = ShaderUtilConvert(ShaderUtil.GetPropertyType(material.shader, i));
                    bool isHidden = ShaderUtil.IsShaderPropertyHidden(material.shader, i);
                    Vector2 range = new Vector2(ShaderUtil.GetRangeLimits(material.shader, i, 1), ShaderUtil.GetRangeLimits(material.shader, i, 2));

                    if (!isHidden && HasShaderPropertyType(filter, type))
                    {
                        properties.Add(new ShaderProperties() { Name = name, Type = type, Range = range });
                    }
                }
            }

            info.ShaderOptions = properties.ToArray();
            return info;
        }

        public static States GetDefaultInteractableStates()
        {
            AssetDatabase.Refresh();
            string[] stateLocations = AssetDatabase.FindAssets("DefaultInteractableStates");
            if (stateLocations.Length > 0)
            {
                for (int i = 0; i < stateLocations.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(stateLocations[i]);
                    States defaultStates = (States)AssetDatabase.LoadAssetAtPath(path, typeof(States));
                    if (defaultStates != null)
                    {
                        return defaultStates;
                        //states.objectReferenceValue = defaultStates;
                    }
                }
            }

            return null;
        }

        public static ShaderPropertyType ShaderUtilConvert(ShaderUtil.ShaderPropertyType type)
        {
            ShaderPropertyType shaderType;
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    shaderType = ShaderPropertyType.Color;
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    shaderType = ShaderPropertyType.Vector;
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    shaderType = ShaderPropertyType.Float;
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    shaderType = ShaderPropertyType.Range;
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    shaderType = ShaderPropertyType.TexEnv;
                    break;
                default:
                    shaderType = ShaderPropertyType.None;
                    break;
            }
            return shaderType;
        }

        public static bool HasShaderPropertyType(ShaderPropertyType[] filter, ShaderPropertyType type)
        {
            for (int i = 0; i < filter.Length; i++)
            {
                if (filter[i] == type)
                {
                    return true;
                }
            }

            return false;
        }
    }
#endif
}
