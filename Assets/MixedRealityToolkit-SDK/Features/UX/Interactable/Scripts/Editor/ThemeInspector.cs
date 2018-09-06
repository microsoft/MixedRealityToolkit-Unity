using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Theme))]
    public class ThemeInspector : InspectorBase
    {
        private SerializedProperty settings;

        protected virtual void OnEnable()
        {
            settings = serializedObject.FindProperty("Settings");
            AdjustListSettings(settings.arraySize);
            SetupThemeOptions();
        }

        /*

        protected override State[] GetStates()
        {
            // TODO: make sure we are getting current states that were saved and not overwrite states
            InteractableStates states = new InteractableStates(InteractableStates.Default);
            return states.GetStates();
        }

        protected override void AddThemeProperty(int[] arr)
        {
            //base.AddThemeProperty(arr);
            int index = arr[0];
            
            SerializedProperty themeObjSettings = serializedObject.FindProperty("Settings");
            themeObjSettings.InsertArrayElementAtIndex(index);

            SerializedProperty settingsItem = themeObjSettings.GetArrayElementAtIndex(themeObjSettings.arraySize - 1);
            SerializedProperty className = settingsItem.FindPropertyRelative("Name");
            if (themeObjSettings.arraySize == 1)
            {

                className.stringValue = "ScaleOffsetColorTheme";
            }
            else
            {
                className.stringValue = themeOptions[0];
            }

            SerializedProperty easing = settingsItem.FindPropertyRelative("Easing");

            SerializedProperty time = easing.FindPropertyRelative("LerpTime");
            SerializedProperty curve = easing.FindPropertyRelative("Curve");
            time.floatValue = 0.5f;
            curve.animationCurveValue = AnimationCurve.Linear(0, 1, 1, 1);
        }

        protected override void RemoveThemeProperty(int[] arr)
        {
            int index = arr[2];
            
            SerializedProperty themeObjSettings = serializedObject.FindProperty("Settings");
            themeObjSettings.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        */

        protected bool enabled = false;

        protected string[] themeOptions;
        protected Type[] themeTypes;
        protected string[] shaderOptions;

        /*
        protected virtual void OnEnable()
        {
            instance = (Interactable)target;
            eventList = instance.Events;

            listSettings = new List<ListSettings>();

            profileList = serializedObject.FindProperty("Profiles");
            AdjustListSettings(profileList.arraySize);
            showProfiles = EditorPrefs.GetBool(prefKey, showProfiles);

            SetupEventOptions();
            SetupThemeOptions();

            enabled = true;
        }*/

        public virtual void RenderCustomInspector()
        {
            // TODO: !!!!! need access to a game object to get shader info
            // TODO: !!!!! need access to states to get state info
            // TODO: !!!!! need to make sure we refresh the shader list when the target changes
            // TODO: !!!!! neet to get shader props, use default if one has not been set.

            // FIX : when adding a new setting, the rendered values is a dupe of the previous values in the list, but the dropdown is default.

            
            //base.OnInspectorGUI();
            serializedObject.Update();

            boxStyle = Box(0);

            GUILayout.BeginVertical(boxStyle);
            bool hasStates = RenderStates();

            GUILayout.EndVertical();

            if (!hasStates)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (settings.arraySize < 1)
            {
                AddThemeProperty(new int[] { 0 });
            }
            
            RenderThemeSettings(settings, themeOptions, null, new int[] { 0, -1, 0 });

            RemoveButton(new GUIContent("+", "Add Theme Property"), new int[] { 0 }, AddThemeProperty);
            // get list of all the properties from the themes

            RenderThemeStates(settings, GetStates(), 0);

            serializedObject.ApplyModifiedProperties();
        }

        protected bool RenderStates()
        {
            // States
            bool showStates = false;
            bool sectionStarted = false;
            SerializedProperty states = serializedObject.FindProperty("States");
            if (states.objectReferenceValue != null)
            {
                string statesPrefKey = target.name + "Settings_States";
                bool prefsShowStates = EditorPrefs.GetBool(statesPrefKey);
                EditorGUI.indentLevel = indentOnSectionStart + 1;
                showStates = DrawSectionStart(states.objectReferenceValue.name + " (Click to edit)", indentOnSectionStart, prefsShowStates, FontStyle.Normal, false);
                sectionStarted = true;
                if (showStates != prefsShowStates)
                {
                    EditorPrefs.SetBool(statesPrefKey, showStates);
                }
            }
            else
            {
                string[] stateLocations = AssetDatabase.FindAssets("DefaultInteractableStates t:States");
                if (stateLocations.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(stateLocations[0]);
                    States defaultStates = (States)AssetDatabase.LoadAssetAtPath(path, typeof(States));
                    states.objectReferenceValue = defaultStates;
                }
                else
                {
                    showStates = true;
                }
            }

            if (showStates)
            {
                EditorGUILayout.PropertyField(states, new GUIContent("States", "The States this Interactable is based on"));
            }

            if (sectionStarted)
            {
                DrawSectionEnd(indentOnSectionStart);
            }

            if (states.objectReferenceValue == null)
            {
                DrawError("Please assign a States object!");
                serializedObject.ApplyModifiedProperties();
                return false;
            }

            return true;
        }

        protected virtual State[] GetStates()
        {
            Theme theme = (Theme)target;
            return theme.GetStates();
        }

        protected virtual void RenderBaseInspector()
        {
            base.OnInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            //RenderBaseInspector()
            RenderCustomInspector();
        }

       
        protected virtual void AddThemeProperty(int[] arr)
        {
            int index = arr[0];

            SerializedProperty themeObjSettings = serializedObject.FindProperty("Settings");
            themeObjSettings.InsertArrayElementAtIndex(index);

            AddThemePropertySettings(themeObjSettings);
            ChangeThemeProperty(themeObjSettings.arraySize - 1, themeObjSettings, null, true);
        }

        protected virtual void AddThemePropertySettings(SerializedProperty themeSettings)
        {
            SerializedProperty settingsItem = themeSettings.GetArrayElementAtIndex(themeSettings.arraySize - 1);
            SerializedProperty className = settingsItem.FindPropertyRelative("Name");
            if (themeSettings.arraySize == 1)
            {
                className.stringValue = "ScaleOffsetColorTheme";
            }
            else
            {
                className.stringValue = themeOptions[0];
            }

            SerializedProperty easing = settingsItem.FindPropertyRelative("Easing");

            SerializedProperty time = easing.FindPropertyRelative("LerpTime");
            SerializedProperty curve = easing.FindPropertyRelative("Curve");
            time.floatValue = 0.5f;
            curve.animationCurveValue = AnimationCurve.Linear(0, 1, 1, 1);
        }

        protected virtual void RemoveThemeProperty(int[] arr)
        {
            int index = arr[0];

            SerializedProperty themeObjSettings = serializedObject.FindProperty("Settings");
            RemoveThemePropertySettings(themeObjSettings, index);
        }

        protected virtual void RemoveThemePropertySettings(SerializedProperty themeSettings, int index)
        {
            themeSettings.DeleteArrayElementAtIndex(index);
        }

        protected virtual SerializedProperty ChangeThemeProperty(int index, SerializedProperty themeSettings, SerializedProperty target, bool isNew = false)
        {
            SerializedProperty settingsItem = themeSettings.GetArrayElementAtIndex(index);

            SerializedProperty className = settingsItem.FindPropertyRelative("Name");

            // get class value types
            if (!String.IsNullOrEmpty(className.stringValue))
            {
                int propIndex = ReverseLookup(className.stringValue, themeOptions);
                GameObject renderHost = null;
                if (target != null)
                {
                    renderHost = (GameObject)target.objectReferenceValue;
                }

                ThemeBase themeBase = (ThemeBase)Activator.CreateInstance(themeTypes[propIndex], renderHost);

                // does this object have the right component types
                SerializedProperty isValid = settingsItem.FindPropertyRelative("IsValid");
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

                List<ThemeProperty> properties = themeBase.ThemeProperties;

                SerializedProperty sProps = settingsItem.FindPropertyRelative("Properties");
                SerializedProperty history = settingsItem.FindPropertyRelative("History");

                if (isNew)
                {
                    sProps.ClearArray();
                }
                else
                {
                    //sProps.ClearArray();
                    //isNew = true;
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

                    State[] states = GetStates();
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
                            // assign default values if new item
                            SerializeThemeValues(properties[i].Default, valueItem, type.intValue);
                        }
                    }

                    //TODO: make sure shader has currently selected property

                    List<ShaderPropertyType> shaderPropFilter = new List<ShaderPropertyType>();
                    // do we need a propId?
                    if (properties[i].Type == ThemePropertyValueTypes.Color)
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

                    if (properties[i].Type == ThemePropertyValueTypes.ShaderFloat || properties[i].Type == ThemePropertyValueTypes.shaderRange)
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

                    }
                }

                if (!valid)
                {
                    isValid.boolValue = false;
                }
            }

            return themeSettings;
        }

        protected SerializedProperty CopyPropertiesFromHistory(SerializedProperty oldProperties, List<ThemeProperty> newProperties, SerializedProperty history, out SerializedProperty historyOut)
        {
            int oldCount = oldProperties.arraySize;

            for (int i = oldCount - 1; i > -1; i--)
            {
                if (history != null)
                {
                    SerializedProperty item = oldProperties.GetArrayElementAtIndex(i);
                    SerializedProperty name = item.FindPropertyRelative("Name");
                    SerializedProperty type = item.FindPropertyRelative("Type");
                    SerializedProperty values = item.FindPropertyRelative("Values");

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

        public static SerializedProperty CopyThemeValues(SerializedProperty copyFrom, SerializedProperty copyTo, int type)
        {
            SerializedProperty floatFrom;
            SerializedProperty floatTo;
            SerializedProperty vector2From;
            SerializedProperty vector2To;

            switch ((ThemePropertyValueTypes)type)
            {
                case ThemePropertyValueTypes.Float:
                    floatFrom = copyFrom.FindPropertyRelative("Float");
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = floatFrom.floatValue;
                    break;
                case ThemePropertyValueTypes.Int:
                    SerializedProperty intFrom = copyFrom.FindPropertyRelative("Int");
                    SerializedProperty intTo = copyTo.FindPropertyRelative("Int");
                    intTo.intValue = intFrom.intValue;
                    break;
                case ThemePropertyValueTypes.Color:
                    SerializedProperty colorFrom = copyFrom.FindPropertyRelative("Color");
                    SerializedProperty colorTo = copyTo.FindPropertyRelative("Color");
                    colorTo.colorValue = colorFrom.colorValue;
                    break;
                case ThemePropertyValueTypes.ShaderFloat:
                    floatFrom = copyFrom.FindPropertyRelative("Float");
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = floatFrom.floatValue;
                    break;
                case ThemePropertyValueTypes.shaderRange:
                    vector2From = copyFrom.FindPropertyRelative("Vector2");
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = vector2From.vector2Value;
                    break;
                case ThemePropertyValueTypes.Vector2:
                    vector2From = copyFrom.FindPropertyRelative("Vector2");
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = vector2From.vector2Value;
                    break;
                case ThemePropertyValueTypes.Vector3:
                    SerializedProperty vector3From = copyFrom.FindPropertyRelative("Vector3");
                    SerializedProperty vector3To = copyTo.FindPropertyRelative("Vector3");
                    vector3To.vector3Value = vector3From.vector3Value;
                    break;
                case ThemePropertyValueTypes.Vector4:
                    SerializedProperty vector4From = copyFrom.FindPropertyRelative("Vector4");
                    SerializedProperty vector4To = copyTo.FindPropertyRelative("Vector4");
                    vector4To.vector4Value = vector4From.vector4Value;
                    break;
                case ThemePropertyValueTypes.Quaternion:
                    SerializedProperty quaternionFrom = copyFrom.FindPropertyRelative("Quaternion");
                    SerializedProperty quaternionTo = copyTo.FindPropertyRelative("Quaternion");
                    quaternionTo.quaternionValue = quaternionFrom.quaternionValue;
                    break;
                case ThemePropertyValueTypes.Texture:
                    SerializedProperty textureFrom = copyFrom.FindPropertyRelative("Texture");
                    SerializedProperty textureTo = copyTo.FindPropertyRelative("Texture");
                    textureTo.objectReferenceValue = textureFrom.objectReferenceValue;
                    break;
                case ThemePropertyValueTypes.Material:
                    SerializedProperty materialFrom = copyFrom.FindPropertyRelative("Material");
                    SerializedProperty materialTo = copyTo.FindPropertyRelative("Material");
                    materialTo.objectReferenceValue = materialFrom.objectReferenceValue;
                    break;
                case ThemePropertyValueTypes.AudioClip:
                    SerializedProperty audioClipFrom = copyFrom.FindPropertyRelative("AudioClip");
                    SerializedProperty audioClipTo = copyTo.FindPropertyRelative("AudioClip");
                    audioClipTo.objectReferenceValue = audioClipFrom.objectReferenceValue;
                    break;
                case ThemePropertyValueTypes.Animaiton:
                    SerializedProperty animationFrom = copyFrom.FindPropertyRelative("Animation");
                    SerializedProperty animationTo = copyTo.FindPropertyRelative("Animation");
                    animationTo.objectReferenceValue = animationFrom.objectReferenceValue;
                    break;
                case ThemePropertyValueTypes.GameObject:
                    SerializedProperty gameObjectFrom = copyFrom.FindPropertyRelative("GameObject");
                    SerializedProperty gameObjectTo = copyTo.FindPropertyRelative("GameObject");
                    gameObjectTo.objectReferenceValue = gameObjectFrom.objectReferenceValue;
                    break;
                case ThemePropertyValueTypes.String:
                    SerializedProperty stringFrom = copyFrom.FindPropertyRelative("String");
                    SerializedProperty stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = stringFrom.stringValue;
                    break;
                case ThemePropertyValueTypes.Bool:
                    SerializedProperty boolFrom = copyFrom.FindPropertyRelative("Bool");
                    SerializedProperty boolTo = copyTo.FindPropertyRelative("Bool");
                    boolTo.boolValue = boolFrom.boolValue;
                    break;
                default:
                    break;
            }

            return copyTo;
        }

        public static SerializedProperty SerializeThemeValues(ThemePropertyValue copyFrom, SerializedProperty copyTo, int type)
        {
            SerializedProperty floatTo;
            SerializedProperty vector2To;

            switch ((ThemePropertyValueTypes)type)
            {
                case ThemePropertyValueTypes.Float:
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = copyFrom.Float;
                    break;
                case ThemePropertyValueTypes.Int:
                    SerializedProperty intTo = copyTo.FindPropertyRelative("Int");
                    intTo.intValue = copyFrom.Int;
                    break;
                case ThemePropertyValueTypes.Color:
                    SerializedProperty colorTo = copyTo.FindPropertyRelative("Color");
                    colorTo.colorValue = copyFrom.Color;
                    break;
                case ThemePropertyValueTypes.ShaderFloat:
                    floatTo = copyTo.FindPropertyRelative("Float");
                    floatTo.floatValue = copyFrom.Float;
                    break;
                case ThemePropertyValueTypes.shaderRange:
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = copyFrom.Vector2;
                    break;
                case ThemePropertyValueTypes.Vector2:
                    vector2To = copyTo.FindPropertyRelative("Vector2");
                    vector2To.vector2Value = copyFrom.Vector2;
                    break;
                case ThemePropertyValueTypes.Vector3:
                    SerializedProperty vector3To = copyTo.FindPropertyRelative("Vector3");
                    vector3To.vector3Value = copyFrom.Vector3;
                    break;
                case ThemePropertyValueTypes.Vector4:
                    SerializedProperty vector4To = copyTo.FindPropertyRelative("Vector4");
                    vector4To.vector4Value = copyFrom.Vector4;
                    break;
                case ThemePropertyValueTypes.Quaternion:
                    SerializedProperty quaternionTo = copyTo.FindPropertyRelative("Quaternion");
                    quaternionTo.quaternionValue = copyFrom.Quaternion;
                    break;
                case ThemePropertyValueTypes.Texture:
                    SerializedProperty textureTo = copyTo.FindPropertyRelative("Texture");
                    textureTo.objectReferenceValue = copyFrom.Texture;
                    break;
                case ThemePropertyValueTypes.Material:
                    SerializedProperty materialTo = copyTo.FindPropertyRelative("Material");
                    materialTo.objectReferenceValue = copyFrom.Material;
                    break;
                case ThemePropertyValueTypes.AudioClip:
                    SerializedProperty audioClipTo = copyTo.FindPropertyRelative("AudioClip");
                    audioClipTo.objectReferenceValue = copyFrom.AudioClip;
                    break;
                case ThemePropertyValueTypes.Animaiton:
                    SerializedProperty animationTo = copyTo.FindPropertyRelative("Animation");
                    animationTo.objectReferenceValue = copyFrom.Animation;
                    break;
                case ThemePropertyValueTypes.GameObject:
                    SerializedProperty gameObjectTo = copyTo.FindPropertyRelative("GameObject");
                    gameObjectTo.objectReferenceValue = copyFrom.GameObject;
                    break;
                case ThemePropertyValueTypes.String:
                    SerializedProperty stringTo = copyTo.FindPropertyRelative("String");
                    stringTo.stringValue = copyFrom.String;
                    break;
                case ThemePropertyValueTypes.Bool:
                    SerializedProperty boolTo = copyTo.FindPropertyRelative("Bool");
                    boolTo.boolValue = copyFrom.Bool;
                    break;
                default:
                    break;
            }

            return copyTo;
        }

        protected void RenderThemeSettings(SerializedProperty themeSettings, string[] themeOptions, SerializedProperty gameObject, int[] listIndex)
        {
            for (int n = 0; n < themeSettings.arraySize; n++)
            {
                SerializedProperty settingsItem = themeSettings.GetArrayElementAtIndex(n);
                SerializedProperty className = settingsItem.FindPropertyRelative("Name");

                EditorGUI.indentLevel = indentOnSectionStart;

                EditorGUILayout.BeginVertical(boxStyle);
                // a dropdown for the type of theme, they should make sense
                // show event dropdown
                int id = ReverseLookup(className.stringValue, themeOptions);

                EditorGUILayout.BeginHorizontal();
                int newId = EditorGUILayout.Popup("Theme Property", id, themeOptions);

                if (n > 0)
                {
                    if (listIndex[1] < 0)
                    {
                        listIndex[2] = n;
                    }

                    bool removed = SmallButton(new GUIContent("\u2212", "Remove Theme Property"), listIndex, RemoveThemeProperty);

                    if (removed)
                    {
                        continue;
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (id != newId)
                {
                    className.stringValue = themeOptions[newId];

                    themeSettings = ChangeThemeProperty(n, themeSettings, gameObject);
                }

                SerializedProperty sProps = settingsItem.FindPropertyRelative("Properties");
                EditorGUI.indentLevel = indentOnSectionStart + 1;
                int idCount = 0;
                for (int p = 0; p < sProps.arraySize; p++)
                {

                    SerializedProperty item = sProps.GetArrayElementAtIndex(p);
                    SerializedProperty propId = item.FindPropertyRelative("PropId");
                    SerializedProperty name = item.FindPropertyRelative("Name");

                    SerializedProperty shaderList = item.FindPropertyRelative("ShaderOptions");
                    SerializedProperty shaderNames = item.FindPropertyRelative("ShaderOptionNames");
                    SerializedProperty shaderName = item.FindPropertyRelative("ShaderName");
                    SerializedProperty propType = item.FindPropertyRelative("Type");

                    bool hasTextComp = false;
                    if (shaderNames.arraySize > 0)
                    {
                        // show shader property dropdown
                        if (idCount < 1)
                        {
                            GUILayout.Space(5);
                        }

                        string[] shaderOptionNames = SerializedPropertyToOptions(shaderNames);
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
                                    ThemePropertyValueTypes type = (ThemePropertyValueTypes)propType.intValue;
                                    ShaderPropertyType[] filter = new ShaderPropertyType[0];
                                    switch (type)
                                    {
                                        case ThemePropertyValueTypes.Color:
                                            filter = new ShaderPropertyType[] { ShaderPropertyType.Color };
                                            break;
                                        case ThemePropertyValueTypes.ShaderFloat:
                                            filter = new ShaderPropertyType[] { ShaderPropertyType.Float };
                                            break;
                                        case ThemePropertyValueTypes.shaderRange:
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
                            popupStyle.margin.right = Mathf.RoundToInt(Screen.width * 0.25f);
                            propId.intValue = EditorGUILayout.Popup("Material " + name.stringValue + "Id", propId.intValue, shaderOptionNames, popupStyle);
                            idCount++;

                            if (!hasShaderProperty)
                            {
                                DrawError(propName + " is not available on the currently assigned Material.");
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField(new GUIContent("Text Property: " + (ThemePropertyValueTypes)propId.intValue));
                        }

                        // Handle isse where the material color id renders on objects it shouldn't!!!!!!!!!!!!!!
                        // theme is save for a game object with a renderer, but when put on a textmesh, rendering prop values show up.
                        // when changing the theme type on a TextMesh, everything works, but the rendering prop is removed from the theme on the renderer object.
                        // make this passive, only show up when needed.
                    }
                }

                EditorGUI.indentLevel = indentOnSectionStart;
                GUILayout.Space(5);
                DrawDivider();

                // show theme properties
                SerializedProperty easing = settingsItem.FindPropertyRelative("Easing");
                SerializedProperty ease = easing.FindPropertyRelative("EaseValues");

                ease.boolValue = EditorGUILayout.Toggle(new GUIContent("Easing", "should the theme animate state values"), ease.boolValue);
                if (ease.boolValue)
                {
                    EditorGUI.indentLevel = indentOnSectionStart + 1;
                    SerializedProperty time = easing.FindPropertyRelative("LerpTime");
                    //time.floatValue = 0.5f;
                    SerializedProperty curve = easing.FindPropertyRelative("Curve");
                    //curve.animationCurveValue = AnimationCurve.Linear(0, 1, 1, 1);

                    time.floatValue = EditorGUILayout.FloatField(new GUIContent("Duration", "animation duration"), time.floatValue);
                    EditorGUILayout.PropertyField(curve, new GUIContent("Animation Curve"));

                    EditorGUI.indentLevel = indentOnSectionStart;
                }

                if (n > 0)
                {
                    //RemoveButton("Remove Property", new int[] {i,t,n}, RemoveThemeProperty);
                }
                EditorGUILayout.EndVertical();
            }
        }

        public static void RenderThemeStates(SerializedProperty settings, State[] states, int margin)
        {
            GUIStyle box = Box(margin);

            EditorGUILayout.BeginVertical(box);
            for (int n = 0; n < states.Length; n++)
            {
                DrawLabel(states[n].Name, 12, titleColor);

                EditorGUI.indentLevel = indentOnSectionStart + 1;

                for (int j = 0; j < settings.arraySize; j++)
                {
                    SerializedProperty settingsItem = settings.GetArrayElementAtIndex(j);
                    SerializedProperty className = settingsItem.FindPropertyRelative("Name");

                    SerializedProperty properties = settingsItem.FindPropertyRelative("Properties");

                    for (int i = 0; i < properties.arraySize; i++)
                    {
                        SerializedProperty propertyItem = properties.GetArrayElementAtIndex(i);
                        SerializedProperty name = propertyItem.FindPropertyRelative("Name");
                        SerializedProperty type = propertyItem.FindPropertyRelative("Type");
                        SerializedProperty values = propertyItem.FindPropertyRelative("Values");
                        SerializedProperty propId = propertyItem.FindPropertyRelative("PropId");
                        SerializedProperty options = propertyItem.FindPropertyRelative("ShaderOptions");
                        SerializedProperty names = propertyItem.FindPropertyRelative("ShaderOptionNames");
                        SerializedProperty shaderName = propertyItem.FindPropertyRelative("ShaderName");

                        if (n >= values.arraySize)
                        {
                            continue;
                        }

                        SerializedProperty item = values.GetArrayElementAtIndex(n);
                        SerializedProperty floatValue = item.FindPropertyRelative("Float");
                        SerializedProperty vector2Value = item.FindPropertyRelative("Vector2");

                        switch ((ThemePropertyValueTypes)type.intValue)
                        {
                            case ThemePropertyValueTypes.Float:
                                floatValue.floatValue = EditorGUILayout.FloatField(new GUIContent(name.stringValue, ""), floatValue.floatValue);
                                break;
                            case ThemePropertyValueTypes.Int:
                                SerializedProperty intValue = item.FindPropertyRelative("Int");
                                intValue.intValue = EditorGUILayout.IntField(new GUIContent(name.stringValue, ""), intValue.intValue);
                                break;
                            case ThemePropertyValueTypes.Color:
                                SerializedProperty colorValue = item.FindPropertyRelative("Color");
                                colorValue.colorValue = EditorGUILayout.ColorField(new GUIContent(name.stringValue, ""), colorValue.colorValue);
                                break;
                            case ThemePropertyValueTypes.ShaderFloat:
                                floatValue.floatValue = EditorGUILayout.FloatField(new GUIContent(name.stringValue, ""), floatValue.floatValue);
                                break;
                            case ThemePropertyValueTypes.shaderRange:
                                vector2Value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(name.stringValue, ""), vector2Value.vector2Value);
                                break;
                            case ThemePropertyValueTypes.Vector2:
                                vector2Value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(name.stringValue, ""), vector2Value.vector2Value);
                                break;
                            case ThemePropertyValueTypes.Vector3:
                                SerializedProperty vector3Value = item.FindPropertyRelative("Vector3");
                                vector3Value.vector3Value = EditorGUILayout.Vector3Field(new GUIContent(name.stringValue, ""), vector3Value.vector3Value);
                                break;
                            case ThemePropertyValueTypes.Vector4:
                                SerializedProperty vector4Value = item.FindPropertyRelative("Vector4");
                                vector4Value.vector4Value = EditorGUILayout.Vector4Field(new GUIContent(name.stringValue, ""), vector4Value.vector4Value);
                                break;
                            case ThemePropertyValueTypes.Quaternion:
                                SerializedProperty quaternionValue = item.FindPropertyRelative("Quaternion");
                                Vector4 vect4 = new Vector4(quaternionValue.quaternionValue.x, quaternionValue.quaternionValue.y, quaternionValue.quaternionValue.z, quaternionValue.quaternionValue.w);
                                vect4 = EditorGUILayout.Vector4Field(new GUIContent(name.stringValue, ""), vect4);
                                quaternionValue.quaternionValue = new Quaternion(vect4.x, vect4.y, vect4.z, vect4.w);
                                break;
                            case ThemePropertyValueTypes.Texture:
                                SerializedProperty texture = item.FindPropertyRelative("Texture");
                                EditorGUILayout.PropertyField(texture, new GUIContent(name.stringValue, ""), false);
                                break;
                            case ThemePropertyValueTypes.Material:
                                SerializedProperty material = item.FindPropertyRelative("Material");
                                EditorGUILayout.PropertyField(material, new GUIContent(name.stringValue, ""), false);
                                break;
                            case ThemePropertyValueTypes.AudioClip:
                                SerializedProperty audio = item.FindPropertyRelative("AudioClip");
                                EditorGUILayout.PropertyField(audio, new GUIContent(name.stringValue, ""), false);
                                break;
                            case ThemePropertyValueTypes.Animaiton:
                                SerializedProperty animation = item.FindPropertyRelative("Animation");
                                EditorGUILayout.PropertyField(animation, new GUIContent(name.stringValue, ""), false);
                                break;
                            case ThemePropertyValueTypes.GameObject:
                                SerializedProperty gameObjectValue = item.FindPropertyRelative("GameObject");
                                EditorGUILayout.PropertyField(gameObjectValue, new GUIContent(name.stringValue, ""), false);
                                break;
                            case ThemePropertyValueTypes.String:
                                SerializedProperty stringValue = item.FindPropertyRelative("String");
                                stringValue.stringValue = EditorGUILayout.TextField(new GUIContent(name.stringValue, ""), stringValue.stringValue);
                                break;
                            case ThemePropertyValueTypes.Bool:
                                SerializedProperty boolValue = item.FindPropertyRelative("Bool");
                                boolValue.boolValue = EditorGUILayout.Toggle(new GUIContent(name.stringValue, ""), boolValue.boolValue);
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

        protected static void PropertySettingsList(SerializedProperty settings, List<InteractableEvent.FieldData> data)
        {
            settings.ClearArray();

            for (int i = 0; i < data.Count; i++)
            {
                settings.InsertArrayElementAtIndex(settings.arraySize);
                SerializedProperty settingItem = settings.GetArrayElementAtIndex(settings.arraySize - 1);

                UpdatePropertySettings(settingItem, (int)data[i].Attributes.Type, data[i].Value);

                SerializedProperty type = settingItem.FindPropertyRelative("Type");
                SerializedProperty tooltip = settingItem.FindPropertyRelative("Tooltip");
                SerializedProperty label = settingItem.FindPropertyRelative("Label");
                SerializedProperty options = settingItem.FindPropertyRelative("Options");

                type.enumValueIndex = (int)data[i].Attributes.Type;
                tooltip.stringValue = data[i].Attributes.Tooltip;
                label.stringValue = data[i].Attributes.Label;
                options.ClearArray();

                if (data[i].Attributes.Options != null)
                {
                    for (int j = 0; j < data[i].Attributes.Options.Length; j++)
                    {
                        options.InsertArrayElementAtIndex(j);
                        SerializedProperty item = options.GetArrayElementAtIndex(j);
                        item.stringValue = data[i].Attributes.Options[j];
                    }
                }
            }
        }

        protected static void UpdatePropertySettings(SerializedProperty prop, int type, object update)
        {
            SerializedProperty intValue = prop.FindPropertyRelative("IntValue");
            SerializedProperty stringValue = prop.FindPropertyRelative("StringValue");

            switch ((InspectorField.FieldTypes)type)
            {
                case InspectorField.FieldTypes.Float:
                    SerializedProperty floatValue = prop.FindPropertyRelative("FloatValue");
                    floatValue.floatValue = (float)update;
                    break;
                case InspectorField.FieldTypes.Int:
                    intValue.intValue = (int)update;
                    break;
                case InspectorField.FieldTypes.String:

                    stringValue.stringValue = (string)update;
                    break;
                case InspectorField.FieldTypes.Bool:
                    SerializedProperty boolValue = prop.FindPropertyRelative("BoolValue");
                    boolValue.boolValue = (bool)update;
                    break;
                case InspectorField.FieldTypes.Color:
                    SerializedProperty colorValue = prop.FindPropertyRelative("ColorValue");
                    colorValue.colorValue = (Color)update;
                    break;
                case InspectorField.FieldTypes.DropdownInt:
                    intValue.intValue = (int)update;
                    break;
                case InspectorField.FieldTypes.DropdownString:
                    stringValue.stringValue = (string)update;
                    break;
                case InspectorField.FieldTypes.GameObject:
                    SerializedProperty gameObjectValue = prop.FindPropertyRelative("GameObjectValue");
                    gameObjectValue.objectReferenceValue = (GameObject)update;
                    break;
                case InspectorField.FieldTypes.ScriptableObject:
                    SerializedProperty scriptableObjectValue = prop.FindPropertyRelative("ScriptableObjectValue");
                    scriptableObjectValue.objectReferenceValue = (ScriptableObject)update;
                    break;
                case InspectorField.FieldTypes.Object:
                    SerializedProperty objectValue = prop.FindPropertyRelative("ObjectValue");
                    objectValue.objectReferenceValue = (UnityEngine.Object)update;
                    break;
                case InspectorField.FieldTypes.Material:
                    SerializedProperty materialValue = prop.FindPropertyRelative("MaterialValue");
                    materialValue.objectReferenceValue = (Material)update;
                    break;
                case InspectorField.FieldTypes.Texture:
                    SerializedProperty textureValue = prop.FindPropertyRelative("TextureValue");
                    textureValue.objectReferenceValue = (Texture)update;
                    break;
                case InspectorField.FieldTypes.Vector2:
                    SerializedProperty vector2Value = prop.FindPropertyRelative("Vector2Value");
                    vector2Value.vector2Value = (Vector2)update;
                    break;
                case InspectorField.FieldTypes.Vector3:
                    SerializedProperty vector3Value = prop.FindPropertyRelative("Vector3Value");
                    vector3Value.vector3Value = (Vector3)update;
                    break;
                case InspectorField.FieldTypes.Vector4:
                    SerializedProperty vector4Value = prop.FindPropertyRelative("Vector4Value");
                    vector4Value.vector4Value = (Vector4)update;
                    break;
                case InspectorField.FieldTypes.Curve:
                    SerializedProperty curveValue = prop.FindPropertyRelative("CurveValue");
                    curveValue.animationCurveValue = (AnimationCurve)update;
                    break;
                case InspectorField.FieldTypes.Quaternion:
                    SerializedProperty quaternionValue = prop.FindPropertyRelative("QuaternionValue");
                    quaternionValue.quaternionValue = (Quaternion)update;
                    break;
                case InspectorField.FieldTypes.AudioClip:
                    SerializedProperty audioClip = prop.FindPropertyRelative("AudioClipValue");
                    audioClip.objectReferenceValue = (AudioClip)update;
                    break;
                default:
                    break;
            }
        }

        protected static string[] GetOptions(SerializedProperty options)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < options.arraySize; i++)
            {
                list.Add(options.GetArrayElementAtIndex(i).stringValue);
            }

            return list.ToArray();
        }

        protected static int GetOptionsIndex(SerializedProperty options, string selection)
        {
            for (int i = 0; i < options.arraySize; i++)
            {
                if (options.GetArrayElementAtIndex(i).stringValue == selection)
                {
                    return i;
                }
            }

            return 0;
        }

        public static void DisplayPropertyField(SerializedProperty prop)
        {
            SerializedProperty type = prop.FindPropertyRelative("Type");
            SerializedProperty label = prop.FindPropertyRelative("Label");
            SerializedProperty tooltip = prop.FindPropertyRelative("Tooltip");
            SerializedProperty options = prop.FindPropertyRelative("Options");

            SerializedProperty intValue = prop.FindPropertyRelative("IntValue");
            SerializedProperty stringValue = prop.FindPropertyRelative("StringValue");

            switch ((InspectorField.FieldTypes)type.intValue)
            {
                case InspectorField.FieldTypes.Float:
                    SerializedProperty floatValue = prop.FindPropertyRelative("FloatValue");
                    floatValue.floatValue = EditorGUILayout.FloatField(new GUIContent(label.stringValue, tooltip.stringValue), floatValue.floatValue);
                    break;
                case InspectorField.FieldTypes.Int:
                    intValue.intValue = EditorGUILayout.IntField(new GUIContent(label.stringValue, tooltip.stringValue), intValue.intValue);
                    break;
                case InspectorField.FieldTypes.String:
                    stringValue.stringValue = EditorGUILayout.TextField(new GUIContent(label.stringValue, tooltip.stringValue), stringValue.stringValue);
                    break;
                case InspectorField.FieldTypes.Bool:
                    SerializedProperty boolValue = prop.FindPropertyRelative("BoolValue");
                    boolValue.boolValue = EditorGUILayout.Toggle(new GUIContent(label.stringValue, tooltip.stringValue), boolValue.boolValue);
                    break;
                case InspectorField.FieldTypes.Color:
                    SerializedProperty colorValue = prop.FindPropertyRelative("ColorValue");
                    colorValue.colorValue = EditorGUILayout.ColorField(new GUIContent(label.stringValue, tooltip.stringValue), colorValue.colorValue);
                    break;
                case InspectorField.FieldTypes.DropdownInt:
                    intValue.intValue = EditorGUILayout.Popup(label.stringValue, intValue.intValue, GetOptions(options));
                    break;
                case InspectorField.FieldTypes.DropdownString:
                    string[] stringOptions = GetOptions(options);
                    int selection = GetOptionsIndex(options, stringValue.stringValue);
                    int newIndex = EditorGUILayout.Popup(label.stringValue, intValue.intValue, stringOptions);
                    if (selection != newIndex)
                    {
                        stringValue.stringValue = stringOptions[newIndex];
                    }
                    break;
                case InspectorField.FieldTypes.GameObject:
                    SerializedProperty gameObjectValue = prop.FindPropertyRelative("GameObjectValue");
                    EditorGUILayout.PropertyField(gameObjectValue, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.ScriptableObject:
                    SerializedProperty scriptableObjectValue = prop.FindPropertyRelative("ScriptableObjectValue");
                    EditorGUILayout.PropertyField(scriptableObjectValue, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.Object:
                    SerializedProperty objectValue = prop.FindPropertyRelative("ObjectValue");
                    EditorGUILayout.PropertyField(objectValue, new GUIContent(label.stringValue, tooltip.stringValue), true);
                    break;
                case InspectorField.FieldTypes.Material:
                    SerializedProperty materialValue = prop.FindPropertyRelative("MaterialValue");
                    EditorGUILayout.PropertyField(materialValue, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.Texture:
                    SerializedProperty textureValue = prop.FindPropertyRelative("TextureValue");
                    EditorGUILayout.PropertyField(textureValue, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.Vector2:
                    SerializedProperty vector2Value = prop.FindPropertyRelative("Vector2Value");
                    vector2Value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(label.stringValue, tooltip.stringValue), vector2Value.vector2Value);
                    break;
                case InspectorField.FieldTypes.Vector3:
                    SerializedProperty vector3Value = prop.FindPropertyRelative("Vector3Value");
                    vector3Value.vector3Value = EditorGUILayout.Vector3Field(new GUIContent(label.stringValue, tooltip.stringValue), vector3Value.vector3Value);
                    break;
                case InspectorField.FieldTypes.Vector4:
                    SerializedProperty vector4Value = prop.FindPropertyRelative("Vector4Value");
                    vector4Value.vector4Value = EditorGUILayout.Vector4Field(new GUIContent(label.stringValue, tooltip.stringValue), vector4Value.vector4Value);
                    break;
                case InspectorField.FieldTypes.Curve:
                    SerializedProperty curveValue = prop.FindPropertyRelative("CurveValue");
                    curveValue.animationCurveValue = EditorGUILayout.CurveField(new GUIContent(label.stringValue, tooltip.stringValue), curveValue.animationCurveValue);
                    break;
                case InspectorField.FieldTypes.Quaternion:
                    SerializedProperty quaternionValue = prop.FindPropertyRelative("QuaternionValue");
                    Vector4 vect4 = new Vector4(quaternionValue.quaternionValue.x, quaternionValue.quaternionValue.y, quaternionValue.quaternionValue.z, quaternionValue.quaternionValue.w);
                    vect4 = EditorGUILayout.Vector4Field(new GUIContent(label.stringValue, tooltip.stringValue), vect4);
                    quaternionValue.quaternionValue = new Quaternion(vect4.x, vect4.y, vect4.z, vect4.w);
                    break;
                case InspectorField.FieldTypes.AudioClip:
                    SerializedProperty audioClip = prop.FindPropertyRelative("AudioClipValue");
                    EditorGUILayout.PropertyField(audioClip, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                default:
                    break;
            }
        }
        
        protected void SetupThemeOptions()
        {
            ProfileItem.ThemeLists lists = ProfileItem.GetThemeTypes();
            themeOptions = lists.Names.ToArray();
            themeTypes = lists.Types.ToArray();
        }

        // redundant method, put in a utils with static methods!!!
        public static int ReverseLookup(string option, string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == option)
                {
                    return i;
                }
            }

            return 0;
        }
        
        public static ShaderInfo GetShaderProperties(Renderer renderer, ShaderPropertyType[] filter)
        {
            ShaderInfo info = new ShaderInfo();
            List<ShaderProperties> properties = new List<ShaderProperties>();
            Material material = null;

            if (renderer != null)
            {
                material = ThemeBase.GetValidMaterial(renderer);
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

        public static string[] SerializedPropertyToOptions(SerializedProperty arr)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < arr.arraySize; i++)
            {
                list.Add(arr.GetArrayElementAtIndex(i).stringValue);
            }
            return list.ToArray();
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
