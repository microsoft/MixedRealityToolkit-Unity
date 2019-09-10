// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A collection of helper functions for adding InspectorFields to a custom Inspector
    /// </summary>
    public static class InspectorFieldsUtility
    {
        public static bool AreFieldsSame(SerializedProperty settings, List<InspectorFieldData> fieldList)
        {
            // If number of fields don't match, automaticaly not the same
            if (settings.arraySize != fieldList.Count)
            {
                return false;
            }

            // If same number of fields, ensure union of two lists is a perfect match
            for (int idx = 0; idx < settings.arraySize - 1; idx++)
            {
                SerializedProperty name = settings.GetArrayElementAtIndex(idx).FindPropertyRelative("Name");

                if (fieldList.FindIndex(s => s.Name == name.stringValue) == -1)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Update list of serialized PropertySettings from new or removed InspectorFields
        /// </summary>
        public static void UpdateSettingsList(SerializedProperty settings, List<InspectorFieldData> fieldList)
        {
            // Delete existing settings that now have missing field
            // Remove data entries for existing setting matches
            for (int idx = settings.arraySize - 1; idx >= 0; idx--)
            {
                SerializedProperty settingItem = settings.GetArrayElementAtIndex(idx);
                SerializedProperty name = settingItem.FindPropertyRelative("Name");

                int index = fieldList.FindIndex(s => s.Name == name.stringValue);
                if (index != -1)
                {
                    fieldList.RemoveAt(index);
                }
                else
                {
                    settings.DeleteArrayElementAtIndex(idx);
                }
            }

            AddFieldsToSettingsList(settings, fieldList);
        }

        /// <summary>
        /// Create a new list of serialized PropertySettings from InspectorFields
        /// </summary>
        public static void ClearSettingsList(SerializedProperty settings, List<InspectorFieldData> data)
        {
            settings.ClearArray();

            AddFieldsToSettingsList(settings, data);
        }

        /// <summary>
        /// Adds InspectorFields to list of serialized PropertySettings
        /// </summary>
        public static void AddFieldsToSettingsList(SerializedProperty settings, List<InspectorFieldData> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                settings.InsertArrayElementAtIndex(settings.arraySize);
                SerializedProperty settingItem = settings.GetArrayElementAtIndex(settings.arraySize - 1);

                UpdatePropertySettings(settingItem, (int)data[i].Attributes.Type, data[i].Value);

                SerializedProperty type = settingItem.FindPropertyRelative("Type");
                SerializedProperty tooltip = settingItem.FindPropertyRelative("Tooltip");
                SerializedProperty label = settingItem.FindPropertyRelative("Label");
                SerializedProperty options = settingItem.FindPropertyRelative("Options");
                SerializedProperty name = settingItem.FindPropertyRelative("Name");

                type.enumValueIndex = (int)data[i].Attributes.Type;
                tooltip.stringValue = data[i].Attributes.Tooltip;
                label.stringValue = data[i].Attributes.Label;
                name.stringValue = data[i].Name;

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
        
        /// <summary>
        /// Update a property value in a serialized PropertySettings
        /// </summary>
        public static void UpdatePropertySettings(SerializedProperty prop, int type, object update)
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
                case InspectorField.FieldTypes.Event:
                    // read only, do not update here or a new instance of the event will be created
                    break;
                default:
                    break;
            }
        }

        public static List<InspectorFieldData> GetInspectorFields(System.Object target)
        {
            List<InspectorFieldData> fields = new List<InspectorFieldData>();
            Type myType = target.GetType();

            foreach (PropertyInfo prop in myType.GetProperties())
            {
                var attrs = (InspectorField[])prop.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    fields.Add(new InspectorFieldData() { Name = prop.Name, Attributes = attr, Value = prop.GetValue(target, null) });
                }
            }

            foreach (FieldInfo field in myType.GetFields())
            {
                var attrs = (InspectorField[])field.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    fields.Add(new InspectorFieldData() { Name = field.Name, Attributes = attr, Value = field.GetValue(target) });
                }
            }

            return fields;
        }

        /// <summary>
        /// Checks the type a property field and returns if it matches the passed in type
        /// </summary>
        public static bool IsPropertyType(SerializedProperty prop, InspectorField.FieldTypes type)
        {
            SerializedProperty propType = prop.FindPropertyRelative("Type");
            return (InspectorField.FieldTypes)propType.intValue == type;
        }

        /// <summary>
        /// Render a PropertySettings UI field based on the InspectorField Settings
        /// </summary>
        public static void DisplayPropertyField(SerializedProperty prop)
        {
            SerializedProperty type = prop.FindPropertyRelative("Type");
            SerializedProperty label = prop.FindPropertyRelative("Label");
            SerializedProperty tooltip = prop.FindPropertyRelative("Tooltip");
            SerializedProperty options = prop.FindPropertyRelative("Options");

            SerializedProperty intValue = prop.FindPropertyRelative("IntValue");
            SerializedProperty stringValue = prop.FindPropertyRelative("StringValue");

            Rect position;
            GUIContent propLabel = new GUIContent(label.stringValue, tooltip.stringValue);
            switch ((InspectorField.FieldTypes)type.intValue)
            {
                case InspectorField.FieldTypes.Float:
                    SerializedProperty floatValue = prop.FindPropertyRelative("FloatValue");
                    EditorGUILayout.PropertyField(floatValue, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.Int:
                    EditorGUILayout.PropertyField(intValue, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.String:
                    EditorGUILayout.PropertyField(stringValue, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.Bool:
                    SerializedProperty boolValue = prop.FindPropertyRelative("BoolValue");
                    EditorGUILayout.PropertyField(boolValue, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.Color:
                    SerializedProperty colorValue = prop.FindPropertyRelative("ColorValue");
                    EditorGUILayout.PropertyField(colorValue, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.DropdownInt:
                    position = EditorGUILayout.GetControlRect();
                    EditorGUI.BeginProperty(position, propLabel, intValue);
                    {
                        intValue.intValue = EditorGUI.Popup(position, label.stringValue, intValue.intValue, InspectorUIUtility.GetOptions(options));
                    }
                    EditorGUI.EndProperty();
                    break;
                case InspectorField.FieldTypes.DropdownString:
                    string[] stringOptions = InspectorUIUtility.GetOptions(options);
                    int selection = InspectorUIUtility.GetOptionsIndex(options, stringValue.stringValue);
                    position = EditorGUILayout.GetControlRect();
                    EditorGUI.BeginProperty(position, propLabel, intValue);
                    {
                        int newIndex = EditorGUI.Popup(position, label.stringValue, selection, stringOptions);
                        if (selection != newIndex)
                        {
                            stringValue.stringValue = stringOptions[newIndex];
                            intValue.intValue = newIndex;
                        }
                    }
                    EditorGUI.EndProperty();
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
                    EditorGUILayout.PropertyField(vector2Value, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.Vector3:
                    SerializedProperty vector3Value = prop.FindPropertyRelative("Vector3Value");
                    EditorGUILayout.PropertyField(vector3Value, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.Vector4:
                    SerializedProperty vector4Value = prop.FindPropertyRelative("Vector4Value");
                    EditorGUILayout.PropertyField(vector4Value, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.Curve:
                    SerializedProperty curveValue = prop.FindPropertyRelative("CurveValue");
                    EditorGUILayout.PropertyField(curveValue, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                case InspectorField.FieldTypes.Quaternion:
                    SerializedProperty quaternionValue = prop.FindPropertyRelative("QuaternionValue");
                    Vector4 vect4 = new Vector4(quaternionValue.quaternionValue.x, quaternionValue.quaternionValue.y, quaternionValue.quaternionValue.z, quaternionValue.quaternionValue.w);
                    position = EditorGUILayout.GetControlRect();
                    EditorGUI.BeginProperty(position, propLabel, quaternionValue);
                    {
                        vect4 = EditorGUI.Vector4Field(position, propLabel, vect4);
                        quaternionValue.quaternionValue = new Quaternion(vect4.x, vect4.y, vect4.z, vect4.w);
                    }
                    EditorGUI.EndProperty();
                    break;
                case InspectorField.FieldTypes.AudioClip:
                    SerializedProperty audioClip = prop.FindPropertyRelative("AudioClipValue");
                    EditorGUILayout.PropertyField(audioClip, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.Event:
                    SerializedProperty uEvent = prop.FindPropertyRelative("EventValue");
                    EditorGUILayout.PropertyField(uEvent, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                default:
                    break;
            }
        }
    }
}
