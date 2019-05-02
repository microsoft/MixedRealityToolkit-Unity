// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Inspector for themes, and used by Interactable
    /// </summary>
    
#if UNITY_EDITOR
    [CustomEditor(typeof(Theme))]
    public class ThemeInspector : UnityEditor.Editor
    {
        protected SerializedProperty settings;

        protected static InteractableTypesContainer themeOptions;
        protected static string[] shaderOptions;
        protected static State[] themeStates;

        // indent tracker
        protected static int indentOnSectionStart = 0;

        protected GUIStyle boxStyle;
        protected bool layoutComplete = false;

        protected virtual void OnEnable()
        {
            settings = serializedObject.FindProperty("Settings");
            SetupThemeOptions();
        }

        public override void OnInspectorGUI()
        {
            //RenderBaseInspector()
            RenderCustomInspector();
        }

        protected virtual void RenderBaseInspector()
        {
            base.OnInspectorGUI();
        }

        public virtual void RenderCustomInspector()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            bool hasStates = false;
            if (layoutComplete || Event.current.type == EventType.Layout)
            {
                boxStyle = InspectorUIUtility.Box(0);

                GUILayout.BeginVertical(boxStyle);
                hasStates = RenderStates();

                GUILayout.EndVertical();
            }

            if (!hasStates)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }


            if (GetStates().Length < 1)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (settings.arraySize < 1)
            {
                AddThemeProperty(new int[] { 0 });
            }

            if (layoutComplete || Event.current.type == EventType.Layout)
            {

                RenderThemeSettings(settings, null, themeOptions, null, new int[] { 0, -1, 0 }, GetStates());

                InspectorUIUtility.FlexButton(new GUIContent("+", "Add Theme Property"), new int[] { 0 }, AddThemeProperty);

                // render a list of all the properties from the theme based on state
                RenderThemeStates(settings, GetStates(), 0);

                layoutComplete = true;
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// draw the states property field for assigning states
        /// Set the default state if one does not exist
        /// </summary>
        /// <returns></returns>
        protected bool RenderStates()
        {
            // States
            bool showStates = false;
            bool drawerStarted = false;
            SerializedProperty states = serializedObject.FindProperty("States");
            if (states.objectReferenceValue != null)
            {
                string statesPrefKey = target.name + "Settings_States";
                bool prefsShowStates = EditorPrefs.GetBool(statesPrefKey);
                EditorGUI.indentLevel = indentOnSectionStart + 1;
                showStates = InspectorUIUtility.DrawSectionStart(states.objectReferenceValue.name + " (Click to edit)", indentOnSectionStart, prefsShowStates, FontStyle.Normal, false);
                drawerStarted = true;

                if (showStates != prefsShowStates)
                {
                    EditorPrefs.SetBool(statesPrefKey, showStates);
                }
            }
            else
            {
                string[] stateLocations = AssetDatabase.FindAssets("DefaultInteractableStates");
                if (stateLocations.Length > 0)
                {
                    for (int k = 0; k < stateLocations.Length; k++)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(stateLocations[0]);
                        States defaultStates = (States)AssetDatabase.LoadAssetAtPath(path, typeof(States));
                        if (defaultStates != null)
                        {
                            states.objectReferenceValue = defaultStates;
                            break;
                        }
                    }
                }
                else
                {
                    showStates = true;
                }
            }

            if (showStates)
            {
                EditorGUILayout.PropertyField(states, new GUIContent("States", "The States this Interactable is based on"), true);
            }

            if (drawerStarted)
            {
                InspectorUIUtility.DrawSectionEnd(indentOnSectionStart);
            }

            if (states.objectReferenceValue == null)
            {
                InspectorUIUtility.DrawError("Please assign a States object! Ex: DefaultInteractableStates");
                serializedObject.ApplyModifiedProperties();
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

        protected virtual void AddThemeProperty(int[] arr, SerializedProperty prop = null)
        {
            int index = arr[0];

            SerializedProperty themeObjSettings = serializedObject.FindProperty("Settings");
            themeObjSettings.InsertArrayElementAtIndex(index);

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

        protected static void RemoveThemeProperty(int[] arr, SerializedProperty prop = null)
        {
            int index = arr[0];

            RemoveThemePropertySettings(prop, index);
        }

        protected static void RemoveThemePropertySettings(SerializedProperty themeSettings, int index)
        {
            themeSettings.DeleteArrayElementAtIndex(index);
        }

        public static SerializedProperty ChangeThemeProperty(int index, SerializedProperty themeSettings, SerializedProperty target, State[] states, bool isNew = false)
        {
            SerializedProperty settingsItem = themeSettings.GetArrayElementAtIndex(index);

            SerializedProperty className = settingsItem.FindPropertyRelative("Name");

            InteractableTypesContainer themeTypes = InteractableProfileItem.GetThemeTypes();

            // get class value types
            if (!String.IsNullOrEmpty(className.stringValue))
            {
                int propIndex = InspectorUIUtility.ReverseLookup(className.stringValue, themeTypes.ClassNames);
                GameObject renderHost = null;
                if (target != null)
                {
                    renderHost = (GameObject)target.objectReferenceValue;
                }

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
                    for (int i = 0; i < themeBase.Types.Length; i++)
                    {
                        Type type = themeBase.Types[i];
                        if (renderHost.gameObject.GetComponent(type))
                        {
                            if (type == typeof(TextMesh) || type == typeof(Text))
                            {
                                hasText = true;
                            }

                            if (type == typeof(Renderer))
                            {
                                hasRenderer = true;
                            }

                            valid = true;
                        }
                    }
                }

                isValid.boolValue = valid;

                // setup the values
                // get the state names

                List<InteractableThemeProperty> properties = themeBase.ThemeProperties;

                SerializedProperty sProps = settingsItem.FindPropertyRelative("Properties");
                SerializedProperty history = settingsItem.FindPropertyRelative("History");

                if (isNew)
                {
                    sProps.ClearArray();
                }
                else
                {
                    // stick the copy in the new format into sProps.
                    sProps = CopyPropertiesFromHistory(sProps, properties, history, out history);
                }

                for (int i = 0; i < properties.Count; i++)
                {
                    bool newItem = false;
                    if (isNew)
                    {
                        sProps.InsertArrayElementAtIndex(sProps.arraySize);
                        newItem = true;
                    }

                    SerializedProperty item = sProps.GetArrayElementAtIndex(i);
                    SerializedProperty name = item.FindPropertyRelative("Name");
                    SerializedProperty type = item.FindPropertyRelative("Type");
                    SerializedProperty values = item.FindPropertyRelative("Values");

                    name.stringValue = properties[i].Name;
                    type.intValue = (int)properties[i].Type;

                    int valueCount = states.Length;

                    for (int j = 0; j < valueCount; j++)
                    {
                        if (values.arraySize <= j)
                        {
                            values.InsertArrayElementAtIndex(values.arraySize);
                            newItem = true;
                        }

                        SerializedProperty valueItem = values.GetArrayElementAtIndex(j);
                        SerializedProperty valueName = valueItem.FindPropertyRelative("Name");
                        valueName.stringValue = states[j].Name;

                        if (newItem && properties[i].Default != null)
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
                                SerializeThemeValues(properties[i].Default, valueItem, type.intValue);
                            }
                        }
                    }
                    
                    List<ShaderPropertyType> shaderPropFilter = new List<ShaderPropertyType>();
                    // do we need a propId?
                    if (properties[i].Type == InteractableThemePropertyValueTypes.Color)
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

                    if (properties[i].Type == InteractableThemePropertyValueTypes.ShaderFloat || properties[i].Type == InteractableThemePropertyValueTypes.shaderRange)
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
                        Debug.Log(shaderNames.arraySize + " / " + propId.intValue);
                    }
                }

                if (!valid)
                {
                    isValid.boolValue = false;
                }
            }

            return themeSettings;
        }

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

            for (int h = 0; h < oldValues.arraySize; h++)
            {
                newValues.InsertArrayElementAtIndex(newValues.arraySize);
                SerializedProperty newValue = newValues.GetArrayElementAtIndex(newValues.arraySize - 1);
                SerializedProperty valueItem = oldValues.GetArrayElementAtIndex(h);
                newValue = CopyThemeValues(valueItem, newValue, newType.intValue);
            }

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
                case InteractableThemePropertyValueTypes.shaderRange:
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
                case InteractableThemePropertyValueTypes.shaderRange:
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

        public static void RenderThemeSettings(SerializedProperty themeSettings, SerializedObject themeObj, InteractableTypesContainer themeOptions, SerializedProperty gameObject, int[] listIndex, State[] states)
        {
            GUIStyle box = InspectorUIUtility.Box(0);
            if (themeObj != null)
            {
                box = InspectorUIUtility.Box(34);
                themeObj.Update();
            }

            for (int n = 0; n < themeSettings.arraySize; n++)
            {
                SerializedProperty settingsItem = themeSettings.GetArrayElementAtIndex(n);
                SerializedProperty className = settingsItem.FindPropertyRelative("Name");

                EditorGUI.indentLevel = indentOnSectionStart;

                EditorGUILayout.BeginVertical(box);
                // a dropdown for the type of theme, they should make sense
                // show theme dropdown
                int id = InspectorUIUtility.ReverseLookup(className.stringValue, themeOptions.ClassNames);

                EditorGUILayout.BeginHorizontal();
                int newId = EditorGUILayout.Popup("Theme Property", id, themeOptions.ClassNames);

                if (n > 0)
                {
                    // standalone or inside a profile? if(listIndex[1] < 1)
                    if (listIndex[1] < 0)
                    {
                        listIndex[2] = n;
                    }

                    bool removed = InspectorUIUtility.SmallButton(new GUIContent(InspectorUIUtility.Minus, "Remove Theme Property"), listIndex, RemoveThemeProperty, themeSettings);

                    if (removed)
                    {
                        continue;
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (id != newId)
                {
                    SerializedProperty assemblyQualifiedName = settingsItem.FindPropertyRelative("AssemblyQualifiedName");
                    className.stringValue = themeOptions.ClassNames[newId];
                    assemblyQualifiedName.stringValue = themeOptions.AssemblyQualifiedNames[newId];

                    // add the themeOjects if in a profile?
                    //themeObj = ChangeThemeProperty(n, themeObj, gameObject);
                    themeSettings = ChangeThemeProperty(n, themeSettings, gameObject, states);
                }

                SerializedProperty sProps = settingsItem.FindPropertyRelative("Properties");
                EditorGUI.indentLevel = indentOnSectionStart + 1;

                int animatorCount = 0;
                int idCount = 0;
                for (int p = 0; p < sProps.arraySize; p++)
                {

                    SerializedProperty item = sProps.GetArrayElementAtIndex(p);
                    SerializedProperty propId = item.FindPropertyRelative("PropId");
                    SerializedProperty name = item.FindPropertyRelative("Name");

                    SerializedProperty shaderNames = item.FindPropertyRelative("ShaderOptionNames");
                    SerializedProperty shaderName = item.FindPropertyRelative("ShaderName");
                    SerializedProperty propType = item.FindPropertyRelative("Type");

                    InteractableThemePropertyValueTypes type = (InteractableThemePropertyValueTypes)propType.intValue;

                    if (type == InteractableThemePropertyValueTypes.AnimatorTrigger)
                    {
                        animatorCount++;
                    }

                    bool hasTextComp = false;
                    if (shaderNames.arraySize > 0)
                    {
                        // show shader property dropdown
                        if (idCount < 1)
                        {
                            GUILayout.Space(5);
                        }

                        string[] shaderOptionNames = InspectorUIUtility.GetOptions(shaderNames);
                        string propName = shaderOptionNames[propId.intValue];
                        bool hasShaderProperty = true;

                        if (gameObject == null)
                        {
                            EditorGUILayout.LabelField(new GUIContent("Shader: " + shaderName.stringValue));
                        }
                        else
                        {
                            GameObject renderHost = gameObject.objectReferenceValue as GameObject;
                            if (renderHost != null)
                            {
                                Renderer renderer = renderHost.GetComponent<Renderer>();
                                TextMesh mesh = renderHost.GetComponent<TextMesh>();
                                Text text = renderHost.GetComponent<Text>();
                                hasTextComp = text != null || mesh != null;
                                if (renderer != null && !hasTextComp)
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
                                        case InteractableThemePropertyValueTypes.shaderRange:
                                            filter = new ShaderPropertyType[] { ShaderPropertyType.Float };
                                            break;
                                        default:
                                            break;
                                    }

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
                            idCount++;

                            if (!hasShaderProperty)
                            {
                                InspectorUIUtility.DrawError(propName + " is not available on the currently assigned Material.");
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField(new GUIContent("Text Property: " + (InteractableThemePropertyValueTypes)propId.intValue));
                        }

                        // Handle issue where the material color id renders on objects it shouldn't!!!!!!!!!!!!!!
                        // theme is save for a game object with a renderer, but when put on a textmesh, rendering prop values show up.
                        // when changing the theme type on a TextMesh, everything works, but the rendering prop is removed from the theme on the renderer object.
                        // make this passive, only show up when needed.
                    }
                }

                EditorGUI.indentLevel = indentOnSectionStart;
                GUILayout.Space(5);

                if (animatorCount < sProps.arraySize)
                {
                    // show theme properties
                    SerializedProperty easing = settingsItem.FindPropertyRelative("Easing");
                    SerializedProperty enabled = easing.FindPropertyRelative("Enabled");

                    SerializedProperty noEasing = settingsItem.FindPropertyRelative("NoEasing");
                    if (!noEasing.boolValue)
                    {
                        InspectorUIUtility.DrawDivider();
                        enabled.boolValue = EditorGUILayout.Toggle(new GUIContent("Easing", "should the theme animate state values"), enabled.boolValue);

                        if (enabled.boolValue)
                        {
                            EditorGUI.indentLevel = indentOnSectionStart + 1;
                            SerializedProperty time = easing.FindPropertyRelative("LerpTime");
                            SerializedProperty curve = easing.FindPropertyRelative("Curve");

                            time.floatValue = EditorGUILayout.FloatField(new GUIContent("Duration", "animation duration"), time.floatValue);
                            EditorGUILayout.PropertyField(curve, new GUIContent("Animation Curve"));

                            EditorGUI.indentLevel = indentOnSectionStart;
                        }
                    }
                    else
                    {
                        enabled.boolValue = false;
                    }
                }

                // check to see if an animatorController exists
                if (animatorCount > 0 && gameObject != null)
                {
                    GameObject host = gameObject.objectReferenceValue as GameObject;
                    Animator animator = null;

                    if (host != null)
                    {
                        animator = host.GetComponent<Animator>();
                    }

                    if (animator == null && host != null)
                    {
                        SerializedProperty targetInfo = settingsItem.FindPropertyRelative("ThemeTarget");
                        SerializedProperty targetStates = targetInfo.FindPropertyRelative("States");
                        targetStates = GetSerializedStates(targetStates, states);

#pragma warning disable 0219    // disable value is never used warning
                        //assigning values to be passed to the FlexButton
                        SerializedProperty target = targetInfo.FindPropertyRelative("Target");
                        SerializedProperty props = targetInfo.FindPropertyRelative("Properties");
                        target = gameObject;
                        props = sProps;
#pragma warning restore 0219    // enable value is never used warning

                        InspectorUIUtility.FlexButton(new GUIContent("Create Animations", "Create and add an Animator with AnimationClips"), listIndex, AddAnimator, targetInfo);
                    }
                }

                EditorGUILayout.EndVertical();

                if (themeObj != null)
                {
                    themeObj.ApplyModifiedProperties();
                }
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

        public static void RenderThemeStates(SerializedProperty settings, State[] states, int margin)
        {
            GUIStyle box = InspectorUIUtility.Box(margin);

            EditorGUILayout.BeginVertical(box);
            for (int n = 0; n < states.Length; n++)
            {
                InspectorUIUtility.DrawLabel(states[n].Name, 12, InspectorUIUtility.ColorTint50);

                EditorGUI.indentLevel = indentOnSectionStart + 1;

                for (int j = 0; j < settings.arraySize; j++)
                {
                    SerializedProperty settingsItem = settings.GetArrayElementAtIndex(j);

                    SerializedProperty properties = settingsItem.FindPropertyRelative("Properties");

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
                        SerializedProperty floatValue = item.FindPropertyRelative("Float");
                        SerializedProperty vector2Value = item.FindPropertyRelative("Vector2");
                        SerializedProperty stringValue = item.FindPropertyRelative("String");

                        switch ((InteractableThemePropertyValueTypes)type.intValue)
                        {
                            case InteractableThemePropertyValueTypes.Float:
                                floatValue.floatValue = EditorGUILayout.FloatField(new GUIContent(name.stringValue, ""), floatValue.floatValue);
                                break;
                            case InteractableThemePropertyValueTypes.Int:
                                SerializedProperty intValue = item.FindPropertyRelative("Int");
                                intValue.intValue = EditorGUILayout.IntField(new GUIContent(name.stringValue, ""), intValue.intValue);
                                break;
                            case InteractableThemePropertyValueTypes.Color:
                                SerializedProperty colorValue = item.FindPropertyRelative("Color");
                                colorValue.colorValue = EditorGUILayout.ColorField(new GUIContent(shaderPropName, shaderPropName), colorValue.colorValue);
                                break;
                            case InteractableThemePropertyValueTypes.ShaderFloat:
                                floatValue.floatValue = EditorGUILayout.FloatField(new GUIContent(shaderPropName, shaderPropName), floatValue.floatValue);
                                break;
                            case InteractableThemePropertyValueTypes.shaderRange:
                                vector2Value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(shaderPropName, shaderPropName), vector2Value.vector2Value);
                                break;
                            case InteractableThemePropertyValueTypes.Vector2:
                                vector2Value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(name.stringValue, ""), vector2Value.vector2Value);
                                break;
                            case InteractableThemePropertyValueTypes.Vector3:
                                SerializedProperty vector3Value = item.FindPropertyRelative("Vector3");
                                vector3Value.vector3Value = EditorGUILayout.Vector3Field(new GUIContent(name.stringValue, ""), vector3Value.vector3Value);
                                break;
                            case InteractableThemePropertyValueTypes.Vector4:
                                SerializedProperty vector4Value = item.FindPropertyRelative("Vector4");
                                vector4Value.vector4Value = EditorGUILayout.Vector4Field(new GUIContent(name.stringValue, ""), vector4Value.vector4Value);
                                break;
                            case InteractableThemePropertyValueTypes.Quaternion:
                                SerializedProperty quaternionValue = item.FindPropertyRelative("Quaternion");
                                Vector4 vect4 = new Vector4(quaternionValue.quaternionValue.x, quaternionValue.quaternionValue.y, quaternionValue.quaternionValue.z, quaternionValue.quaternionValue.w);
                                vect4 = EditorGUILayout.Vector4Field(new GUIContent(name.stringValue, ""), vect4);
                                quaternionValue.quaternionValue = new Quaternion(vect4.x, vect4.y, vect4.z, vect4.w);
                                break;
                            case InteractableThemePropertyValueTypes.Texture:
                                SerializedProperty texture = item.FindPropertyRelative("Texture");
                                EditorGUILayout.PropertyField(texture, new GUIContent(name.stringValue, ""), false);
                                break;
                            case InteractableThemePropertyValueTypes.Material:
                                SerializedProperty material = item.FindPropertyRelative("Material");
                                EditorGUILayout.PropertyField(material, new GUIContent(name.stringValue, ""), false);
                                break;
                            case InteractableThemePropertyValueTypes.AudioClip:
                                SerializedProperty audio = item.FindPropertyRelative("AudioClip");
                                EditorGUILayout.PropertyField(audio, new GUIContent(name.stringValue, ""), false);
                                break;
                            case InteractableThemePropertyValueTypes.Animaiton:
                                SerializedProperty animation = item.FindPropertyRelative("Animation");
                                EditorGUILayout.PropertyField(animation, new GUIContent(name.stringValue, ""), false);
                                break;
                            case InteractableThemePropertyValueTypes.GameObject:
                                SerializedProperty gameObjectValue = item.FindPropertyRelative("GameObject");
                                EditorGUILayout.PropertyField(gameObjectValue, new GUIContent(name.stringValue, ""), false);
                                break;
                            case InteractableThemePropertyValueTypes.String:
                                stringValue.stringValue = EditorGUILayout.TextField(new GUIContent(name.stringValue, ""), stringValue.stringValue);
                                break;
                            case InteractableThemePropertyValueTypes.Bool:
                                SerializedProperty boolValue = item.FindPropertyRelative("Bool");
                                boolValue.boolValue = EditorGUILayout.Toggle(new GUIContent(name.stringValue, ""), boolValue.boolValue);
                                break;
                            case InteractableThemePropertyValueTypes.AnimatorTrigger:
                                stringValue.stringValue = EditorGUILayout.TextField(new GUIContent(name.stringValue, ""), stringValue.stringValue);
                                break;
                            default:
                                break;
                        }
                    }
                }

                EditorGUI.indentLevel = indentOnSectionStart;
            }
            GUILayout.Space(5);

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        public static void AddAnimator(int[] arr, SerializedProperty prop = null)
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
                material = new Material(Shader.Find("MixedRealityToolkit/Standard"));
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
