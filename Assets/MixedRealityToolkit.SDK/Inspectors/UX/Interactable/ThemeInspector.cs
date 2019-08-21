// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
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
        protected SerializedProperty settings;
        protected SerializedProperty states;

        protected static InteractableTypesContainer themeOptions;
        protected static string[] shaderOptions;
        protected static State[] themeStates;

        protected GUIStyle boxStyle;
        protected bool layoutComplete = false;
        private const float ThemeStateFontScale = 1.2f;

        private static readonly GUIContent AddThemePropertyLabel = new GUIContent("+ Add Theme Property", "Add Theme Property");
        private static readonly GUIContent RemoveThemePropertyContent = new GUIContent("-", "Remove Theme Property");
        private static readonly GUIContent CreateAnimationsContent = new GUIContent("Create Animations", "Create and add an Animator with AnimationClips");
        private static readonly GUIContent EasingContent = new GUIContent("Easing", "should the theme animate state values");

        protected virtual void OnEnable()
        {
            SetupThemeOptions();
        }

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
            settings = serializedObject.FindProperty("Settings");
            states = serializedObject.FindProperty("States");

            //base.OnInspectorGUI();
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
            if (settings.arraySize < 1 || InspectorUIUtility.FlexButton(AddThemePropertyLabel))
            {
                AddThemeProperty();
            }

            RenderThemeSettings(settings, themeOptions, null, GetStates());

            RenderThemeStates(settings, GetStates(), 0);

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
            themeStates = theme.GetStates();
            return themeStates;
        }

        protected void SetupThemeOptions()
        {
            themeOptions = InteractableProfileItem.GetThemeTypes();
        }

        protected virtual void AddThemeProperty()
        {
            SerializedProperty themeObjSettings = serializedObject.FindProperty("Settings");
            themeObjSettings.InsertArrayElementAtIndex(0);

            AddThemePropertySettings(themeObjSettings);
            ChangeThemeProperty(themeObjSettings.arraySize - 1, themeObjSettings, null, GetStates(), true);
        }

        /// <summary>
        /// set up the theme properties when a theme property is added
        /// </summary>
        /// <param name="themeSettings"></param>
        protected virtual void AddThemePropertySettings(SerializedProperty themeSettings)
        {
            SerializedProperty settingsItem = themeSettings.GetArrayElementAtIndex(themeSettings.arraySize - 1);
            SerializedProperty className = settingsItem.FindPropertyRelative("Name");
            SerializedProperty assemblyQualifiedName = settingsItem.FindPropertyRelative("AssemblyQualifiedName");
            if (themeSettings.arraySize == 1)
            {
                className.stringValue = "ScaleOffsetColorTheme";
                assemblyQualifiedName.stringValue = typeof(ScaleOffsetColorTheme).AssemblyQualifiedName;
            }
            else
            {
                className.stringValue = themeOptions.ClassNames[0];
                assemblyQualifiedName.stringValue = themeOptions.AssemblyQualifiedNames[0];
            }

            SerializedProperty easing = settingsItem.FindPropertyRelative("Easing");

            SerializedProperty time = easing.FindPropertyRelative("LerpTime");
            SerializedProperty curve = easing.FindPropertyRelative("Curve");
            time.floatValue = 0.5f;
            curve.animationCurveValue = AnimationCurve.Linear(0, 1, 1, 1);
        }

        public static SerializedProperty ChangeThemeProperty(int index, SerializedProperty themeSettings, SerializedProperty target, State[] states, bool isNew = false)
        {
            SerializedProperty settingsItem = themeSettings.GetArrayElementAtIndex(index);

            SerializedProperty className = settingsItem.FindPropertyRelative("Name");

            InteractableTypesContainer themeTypes = InteractableProfileItem.GetThemeTypes();

            // get class value types
            if (!string.IsNullOrEmpty(className.stringValue))
            {
                int propIndex = InspectorUIUtility.ReverseLookup(className.stringValue, themeTypes.ClassNames);
                GameObject renderHost = target != null ? (GameObject)target.objectReferenceValue : null;

                InteractableThemeBase themeBase = (InteractableThemeBase)Activator.CreateInstance(themeTypes.Types[propIndex], renderHost);

                // does this object have the right component types
                SerializedProperty isValid = settingsItem.FindPropertyRelative("IsValid");
                SerializedProperty noEasing = settingsItem.FindPropertyRelative("NoEasing");
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
                List<InteractableThemeProperty> properties = themeBase.ThemeProperties;
                List<InteractableCustomSetting> customSettings = themeBase.CustomSettings;

                SerializedProperty sProps = settingsItem.FindPropertyRelative("Properties");
                SerializedProperty history = settingsItem.FindPropertyRelative("History");
                SerializedProperty customHistory = settingsItem.FindPropertyRelative("CustomHistory");

                SerializedProperty custom = settingsItem.FindPropertyRelative("CustomSettings");
                
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

                    InteractableThemeProperty property = properties[propertyIndex];

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
                            if ((InteractableThemePropertyValueTypes)type.intValue == InteractableThemePropertyValueTypes.AnimatorTrigger)
                            {
                                InteractableThemePropertyValue propValue = new InteractableThemePropertyValue();
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
                    if (property.Type == InteractableThemePropertyValueTypes.Color)
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

                    if (property.Type == InteractableThemePropertyValueTypes.ShaderFloat 
                        || property.Type == InteractableThemePropertyValueTypes.ShaderRange)
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

            return themeSettings;
        }

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
        private static SerializedProperty CopyCustomHistory(SerializedProperty oldCustom, List<InteractableCustomSetting> newCustomSettings, SerializedProperty customHistory, out SerializedProperty customHistoryOut)
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
        public static SerializedProperty CopyPropertiesFromHistory(SerializedProperty oldProperties, List<InteractableThemeProperty> newProperties, SerializedProperty history, out SerializedProperty historyOut)
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

            switch ((InteractableThemePropertyValueTypes)type)
            {
                case InteractableThemePropertyValueTypes.Float:
                    floatFrom = copyFrom.FindPropertyRelative("Float");
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = floatFrom.floatValue;
                    break;
                case InteractableThemePropertyValueTypes.Int:
                    SerializedProperty intFrom = copyFrom.FindPropertyRelative("Int");
                    SerializedProperty intTo = copyTo.FindPropertyRelative("Int");
                    intTo.intValue = intFrom.intValue;
                    break;
                case InteractableThemePropertyValueTypes.Color:
                    SerializedProperty colorFrom = copyFrom.FindPropertyRelative("Color");
                    SerializedProperty colorTo = copyTo.FindPropertyRelative("Color");
                    colorTo.colorValue = colorFrom.colorValue;
                    break;
                case InteractableThemePropertyValueTypes.ShaderFloat:
                    floatFrom = copyFrom.FindPropertyRelative("Float");
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = floatFrom.floatValue;
                    break;
                case InteractableThemePropertyValueTypes.ShaderRange:
                    vector2From = copyFrom.FindPropertyRelative("Vector2");
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = vector2From.vector2Value;
                    break;
                case InteractableThemePropertyValueTypes.Vector2:
                    vector2From = copyFrom.FindPropertyRelative("Vector2");
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = vector2From.vector2Value;
                    break;
                case InteractableThemePropertyValueTypes.Vector3:
                    SerializedProperty vector3From = copyFrom.FindPropertyRelative("Vector3");
                    SerializedProperty vector3To = copyTo.FindPropertyRelative("Vector3");
                    vector3To.vector3Value = vector3From.vector3Value;
                    break;
                case InteractableThemePropertyValueTypes.Vector4:
                    SerializedProperty vector4From = copyFrom.FindPropertyRelative("Vector4");
                    SerializedProperty vector4To = copyTo.FindPropertyRelative("Vector4");
                    vector4To.vector4Value = vector4From.vector4Value;
                    break;
                case InteractableThemePropertyValueTypes.Quaternion:
                    SerializedProperty quaternionFrom = copyFrom.FindPropertyRelative("Quaternion");
                    SerializedProperty quaternionTo = copyTo.FindPropertyRelative("Quaternion");
                    quaternionTo.quaternionValue = quaternionFrom.quaternionValue;
                    break;
                case InteractableThemePropertyValueTypes.Texture:
                    SerializedProperty textureFrom = copyFrom.FindPropertyRelative("Texture");
                    SerializedProperty textureTo = copyTo.FindPropertyRelative("Texture");
                    textureTo.objectReferenceValue = textureFrom.objectReferenceValue;
                    break;
                case InteractableThemePropertyValueTypes.Material:
                    SerializedProperty materialFrom = copyFrom.FindPropertyRelative("Material");
                    SerializedProperty materialTo = copyTo.FindPropertyRelative("Material");
                    materialTo.objectReferenceValue = materialFrom.objectReferenceValue;
                    break;
                case InteractableThemePropertyValueTypes.AudioClip:
                    SerializedProperty audioClipFrom = copyFrom.FindPropertyRelative("AudioClip");
                    SerializedProperty audioClipTo = copyTo.FindPropertyRelative("AudioClip");
                    audioClipTo.objectReferenceValue = audioClipFrom.objectReferenceValue;
                    break;
                case InteractableThemePropertyValueTypes.Animaiton:
                    SerializedProperty animationFrom = copyFrom.FindPropertyRelative("Animation");
                    SerializedProperty animationTo = copyTo.FindPropertyRelative("Animation");
                    animationTo.objectReferenceValue = animationFrom.objectReferenceValue;
                    break;
                case InteractableThemePropertyValueTypes.GameObject:
                    SerializedProperty gameObjectFrom = copyFrom.FindPropertyRelative("GameObject");
                    SerializedProperty gameObjectTo = copyTo.FindPropertyRelative("GameObject");
                    gameObjectTo.objectReferenceValue = gameObjectFrom.objectReferenceValue;
                    break;
                case InteractableThemePropertyValueTypes.String:
                    stringFrom = copyFrom.FindPropertyRelative("String");
                    stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = stringFrom.stringValue;
                    break;
                case InteractableThemePropertyValueTypes.Bool:
                    SerializedProperty boolFrom = copyFrom.FindPropertyRelative("Bool");
                    SerializedProperty boolTo = copyTo.FindPropertyRelative("Bool");
                    boolTo.boolValue = boolFrom.boolValue;
                    break;
                case InteractableThemePropertyValueTypes.AnimatorTrigger:
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
        public static SerializedProperty SerializeThemeValues(InteractableThemePropertyValue copyFrom, SerializedProperty copyTo, int type)
        {
            SerializedProperty floatTo;
            SerializedProperty vector2To;
            SerializedProperty stringTo;

            switch ((InteractableThemePropertyValueTypes)type)
            {
                case InteractableThemePropertyValueTypes.Float:
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = copyFrom.Float;
                    break;
                case InteractableThemePropertyValueTypes.Int:
                    SerializedProperty intTo = copyTo.FindPropertyRelative("Int");
                    intTo.intValue = copyFrom.Int;
                    break;
                case InteractableThemePropertyValueTypes.Color:
                    SerializedProperty colorTo = copyTo.FindPropertyRelative("Color");
                    colorTo.colorValue = copyFrom.Color;
                    break;
                case InteractableThemePropertyValueTypes.ShaderFloat:
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = copyFrom.Float;
                    break;
                case InteractableThemePropertyValueTypes.ShaderRange:
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = copyFrom.Vector2;
                    break;
                case InteractableThemePropertyValueTypes.Vector2:
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = copyFrom.Vector2;
                    break;
                case InteractableThemePropertyValueTypes.Vector3:
                    SerializedProperty vector3To = copyTo.FindPropertyRelative("Vector3");
                    vector3To.vector3Value = copyFrom.Vector3;
                    break;
                case InteractableThemePropertyValueTypes.Vector4:
                    SerializedProperty vector4To = copyTo.FindPropertyRelative("Vector4");
                    vector4To.vector4Value = copyFrom.Vector4;
                    break;
                case InteractableThemePropertyValueTypes.Quaternion:
                    SerializedProperty quaternionTo = copyTo.FindPropertyRelative("Quaternion");
                    quaternionTo.quaternionValue = copyFrom.Quaternion;
                    break;
                case InteractableThemePropertyValueTypes.Texture:
                    SerializedProperty textureTo = copyTo.FindPropertyRelative("Texture");
                    textureTo.objectReferenceValue = copyFrom.Texture;
                    break;
                case InteractableThemePropertyValueTypes.Material:
                    SerializedProperty materialTo = copyTo.FindPropertyRelative("Material");
                    materialTo.objectReferenceValue = copyFrom.Material;
                    break;
                case InteractableThemePropertyValueTypes.AudioClip:
                    SerializedProperty audioClipTo = copyTo.FindPropertyRelative("AudioClip");
                    audioClipTo.objectReferenceValue = copyFrom.AudioClip;
                    break;
                case InteractableThemePropertyValueTypes.Animaiton:
                    SerializedProperty animationTo = copyTo.FindPropertyRelative("Animation");
                    animationTo.objectReferenceValue = copyFrom.Animation;
                    break;
                case InteractableThemePropertyValueTypes.GameObject:
                    SerializedProperty gameObjectTo = copyTo.FindPropertyRelative("GameObject");
                    gameObjectTo.objectReferenceValue = copyFrom.GameObject;
                    break;
                case InteractableThemePropertyValueTypes.String:
                    stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = copyFrom.String;
                    break;
                case InteractableThemePropertyValueTypes.Bool:
                    SerializedProperty boolTo = copyTo.FindPropertyRelative("Bool");
                    boolTo.boolValue = copyFrom.Bool;
                    break;
                case InteractableThemePropertyValueTypes.AnimatorTrigger:
                    stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = copyFrom.String;
                    break;
                default:
                    break;
            }

            return copyTo;
        }

        public static void RenderThemeSettings(SerializedProperty themeSettings, 
            InteractableTypesContainer themeOptions, 
            SerializedProperty gameObjectProperty, 
            State[] states, 
            int margin = 0)
        {
            GUIStyle box = InspectorUIUtility.Box(margin);

            // Loop through all InteractableThemePropertySettings of Theme
            for (int settingIndex = 0; settingIndex < themeSettings.arraySize; settingIndex++)
            {
                SerializedProperty settingsItem = themeSettings.GetArrayElementAtIndex(settingIndex);
                SerializedProperty className = settingsItem.FindPropertyRelative("Name");

                using (new EditorGUILayout.VerticalScope(box))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        int id = InspectorUIUtility.ReverseLookup(className.stringValue, themeOptions.ClassNames);
                        int newId = EditorGUILayout.Popup("Theme Property", id, themeOptions.ClassNames);

                        if (themeSettings.arraySize > 1)
                        {
                            if (InspectorUIUtility.SmallButton(RemoveThemePropertyContent))
                            {
                                themeSettings.DeleteArrayElementAtIndex(settingIndex);
                                continue;
                            }
                        }

                        if (id != newId)
                        {
                            SerializedProperty assemblyQualifiedName = settingsItem.FindPropertyRelative("AssemblyQualifiedName");
                            className.stringValue = themeOptions.ClassNames[newId];
                            assemblyQualifiedName.stringValue = themeOptions.AssemblyQualifiedNames[newId];

                            themeSettings = ChangeThemeProperty(settingIndex, themeSettings, gameObjectProperty, states);
                        }
                    }

                    SerializedProperty themeProperties = settingsItem.FindPropertyRelative("Properties");
                    int animatorCount = 0;

                    // Loop through all InteractableThemeProperty of InteractableThemePropertySettings
                    for (int p = 0; p < themeProperties.arraySize; p++)
                    {
                        SerializedProperty propertyItem = themeProperties.GetArrayElementAtIndex(p);

                        SerializedProperty propType = propertyItem.FindPropertyRelative("Type");
                        InteractableThemePropertyValueTypes type = (InteractableThemePropertyValueTypes)propType.intValue;

                        if (type == InteractableThemePropertyValueTypes.AnimatorTrigger)
                        {
                            animatorCount++;
                        }

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
                        }
                    }

                    SerializedProperty customSettings = settingsItem.FindPropertyRelative("CustomSettings");
                    RenderCustomSettings(customSettings);

                    if (animatorCount < themeProperties.arraySize)
                    {
                        RenderEasingProperties(settingsItem);
                    }

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
                            props = themeProperties;
#pragma warning restore 0219    // enable value is never used warning

                            if (InspectorUIUtility.FlexButton(CreateAnimationsContent))
                            {
                                AddAnimator(themeTargetProperty);
                            }
                        }
                    }

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
            InteractableThemePropertyValueTypes type)
        {
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

        private static ShaderPropertyType[] GetShaderPropertyFilters(InteractableThemePropertyValueTypes type)
        {
            ShaderPropertyType[] filter = new ShaderPropertyType[0];
            switch (type)
            {
                case InteractableThemePropertyValueTypes.Color:
                    filter = new ShaderPropertyType[] { ShaderPropertyType.Color };
                    break;
                case InteractableThemePropertyValueTypes.ShaderFloat:
                    filter = new ShaderPropertyType[] { ShaderPropertyType.Float };
                    break;
                case InteractableThemePropertyValueTypes.ShaderRange:
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
        /// <param name="customSettings">SerializedProperty for InteractableThemePropertySettings.CustomSettings</param>
        private static void RenderCustomSettings(SerializedProperty customSettings)
        {
            for (int p = 0; p < customSettings.arraySize; p++)
            {
                SerializedProperty item = customSettings.GetArrayElementAtIndex(p);
                SerializedProperty name = item.FindPropertyRelative("Name");
                SerializedProperty propType = item.FindPropertyRelative("Type");
                SerializedProperty value = item.FindPropertyRelative("Value");
                InteractableThemePropertyValueTypes type = (InteractableThemePropertyValueTypes)propType.intValue;

                RenderValue(value, name.stringValue, name.stringValue, type);
            }
        }

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
        }

        /// <summary>
        /// Render a single property value
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="propName"></param>
        /// <param name="type"></param>
        public static void RenderValue(SerializedProperty item, string name, string propName, InteractableThemePropertyValueTypes type)
        {
            SerializedProperty floatValue = item.FindPropertyRelative("Float");
            SerializedProperty vector2Value = item.FindPropertyRelative("Vector2");
            SerializedProperty stringValue = item.FindPropertyRelative("String");

            switch (type)
            {
                case InteractableThemePropertyValueTypes.Float:
                    floatValue.floatValue = EditorGUILayout.FloatField(name, floatValue.floatValue);
                    break;
                case InteractableThemePropertyValueTypes.Int:
                    SerializedProperty intValue = item.FindPropertyRelative("Int");
                    intValue.intValue = EditorGUILayout.IntField(name, intValue.intValue);
                    break;
                case InteractableThemePropertyValueTypes.Color:
                    SerializedProperty colorValue = item.FindPropertyRelative("Color");
                    colorValue.colorValue = EditorGUILayout.ColorField(new GUIContent(propName, propName), colorValue.colorValue);
                    break;
                case InteractableThemePropertyValueTypes.ShaderFloat:
                    floatValue.floatValue = EditorGUILayout.FloatField(new GUIContent(propName, propName), floatValue.floatValue);
                    break;
                case InteractableThemePropertyValueTypes.ShaderRange:
                    vector2Value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(propName, propName), vector2Value.vector2Value);
                    break;
                case InteractableThemePropertyValueTypes.Vector2:
                    vector2Value.vector2Value = EditorGUILayout.Vector2Field(name, vector2Value.vector2Value);
                    break;
                case InteractableThemePropertyValueTypes.Vector3:
                    SerializedProperty vector3Value = item.FindPropertyRelative("Vector3");
                    vector3Value.vector3Value = EditorGUILayout.Vector3Field(name, vector3Value.vector3Value);
                    break;
                case InteractableThemePropertyValueTypes.Vector4:
                    SerializedProperty vector4Value = item.FindPropertyRelative("Vector4");
                    vector4Value.vector4Value = EditorGUILayout.Vector4Field(name, vector4Value.vector4Value);
                    break;
                case InteractableThemePropertyValueTypes.Quaternion:
                    SerializedProperty quaternionValue = item.FindPropertyRelative("Quaternion");
                    Vector4 vect4 = new Vector4(quaternionValue.quaternionValue.x, quaternionValue.quaternionValue.y, quaternionValue.quaternionValue.z, quaternionValue.quaternionValue.w);
                    vect4 = EditorGUILayout.Vector4Field(name, vect4);
                    quaternionValue.quaternionValue = new Quaternion(vect4.x, vect4.y, vect4.z, vect4.w);
                    break;
                case InteractableThemePropertyValueTypes.Texture:
                    SerializedProperty texture = item.FindPropertyRelative("Texture");
                    EditorGUILayout.PropertyField(texture, new GUIContent(name, ""), false);
                    break;
                case InteractableThemePropertyValueTypes.Material:
                    SerializedProperty material = item.FindPropertyRelative("Material");
                    EditorGUILayout.PropertyField(material, new GUIContent(name, ""), false);
                    break;
                case InteractableThemePropertyValueTypes.AudioClip:
                    SerializedProperty audio = item.FindPropertyRelative("AudioClip");
                    EditorGUILayout.PropertyField(audio, new GUIContent(name, ""), false);
                    break;
                case InteractableThemePropertyValueTypes.Animaiton:
                    SerializedProperty animation = item.FindPropertyRelative("Animation");
                    EditorGUILayout.PropertyField(animation, new GUIContent(name, ""), false);
                    break;
                case InteractableThemePropertyValueTypes.GameObject:
                    SerializedProperty gameObjectValue = item.FindPropertyRelative("GameObject");
                    EditorGUILayout.PropertyField(gameObjectValue, new GUIContent(name, ""), false);
                    break;
                case InteractableThemePropertyValueTypes.String:
                    stringValue.stringValue = EditorGUILayout.TextField(name, stringValue.stringValue);
                    break;
                case InteractableThemePropertyValueTypes.Bool:
                    SerializedProperty boolValue = item.FindPropertyRelative("Bool");
                    boolValue.boolValue = EditorGUILayout.Toggle(name, boolValue.boolValue);
                    break;
                case InteractableThemePropertyValueTypes.AnimatorTrigger:
                    stringValue.stringValue = EditorGUILayout.TextField(name, stringValue.stringValue);
                    break;
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

                        SerializedProperty properties = settingsItem.FindPropertyRelative("Properties");
                        using (new EditorGUI.IndentLevelScope())
                        {
                            for (int i = 0; i < properties.arraySize; i++)
                            {
                                SerializedProperty propertyItem = properties.GetArrayElementAtIndex(i);
                                SerializedProperty name = propertyItem.FindPropertyRelative("Name");
                                SerializedProperty type = propertyItem.FindPropertyRelative("Type");
                                SerializedProperty values = propertyItem.FindPropertyRelative("Values");
                                SerializedProperty shaderNames = propertyItem.FindPropertyRelative("ShaderOptionNames");
                                SerializedProperty propId = propertyItem.FindPropertyRelative("PropId");

                                string shaderPropName = "Shader";

                                if (shaderNames.arraySize > propId.intValue)
                                {
                                    SerializedProperty propName = shaderNames.GetArrayElementAtIndex(propId.intValue);
                                    shaderPropName = propName.stringValue.Substring(1);
                                }

                                if (n >= values.arraySize)
                                {
                                    // the state values for this theme were not created yet
                                    continue;
                                }

                                SerializedProperty item = values.GetArrayElementAtIndex(n);
                                RenderValue(item, name.stringValue, shaderPropName, (InteractableThemePropertyValueTypes)type.intValue);
                            }
                        }
                    }
                }// for loop
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
