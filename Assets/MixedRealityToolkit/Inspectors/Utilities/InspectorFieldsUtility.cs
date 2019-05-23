// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A collection of helper functions for adding InspectorFields to a custom Inspector
    /// </summary>

    public static class InspectorFieldsUtility
    {
        /// <summary>
        /// Create a new list of serialized PropertySettings from InspectorFields
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="data"></param>
        public static void PropertySettingsList(SerializedProperty settings, List<InspectorFieldData> data)
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
        /// <param name="prop"></param>
        /// <param name="type"></param>
        /// <param name="update"></param>
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

        /// <summary>
        /// Checks the type a property field and returns if it matches the passed in type
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPropertyType(SerializedProperty prop, InspectorField.FieldTypes type)
        {
            SerializedProperty propType = prop.FindPropertyRelative("Type");
            return (InspectorField.FieldTypes)propType.intValue == type;
        }

        /// <summary>
        /// Render a PropertySettings UI field based on the InspectorField Settings
        /// </summary>
        /// <param name="prop"></param>
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
