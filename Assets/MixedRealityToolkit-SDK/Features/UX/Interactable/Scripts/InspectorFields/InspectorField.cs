// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// A set of Inspector fields for setting up properties in a
    /// component to automatically draw in a custom inspector
    /// </summary>
    public class InspectorField : Attribute
    {
        public enum FieldTypes { Float, Int, String, Bool, Color, DropdownInt, DropdownString, GameObject, ScriptableObject, Object, Material, Texture, Vector2, Vector3, Vector4, Curve, Quaternion, AudioClip, Event }

        public FieldTypes Type { get; set; }
        public string Label { get; set; }
        public string Tooltip { get; set; }
        public string[] Options { get; set; }
        public object Value { get; set; }

        public static PropertySetting FieldToProperty(InspectorField attributes, object fieldValue, string fieldName)
        {
            PropertySetting setting = new PropertySetting();
            setting.Type = attributes.Type;
            setting.Tooltip = attributes.Tooltip;
            setting.Label = attributes.Label;
            setting.Options = attributes.Options;
            setting.Name = fieldName;

            UpdatePropertySetting(setting, fieldValue);

            return setting;
        }
        
        public static PropertySetting UpdatePropertySetting(PropertySetting setting, object update)
        {
            setting.Value = update;
            switch (setting.Type)
            {
                case InspectorField.FieldTypes.Float:
                    setting.FloatValue = (float)update;
                    break;
                case InspectorField.FieldTypes.Int:
                    setting.IntValue = (int)update;
                    break;
                case InspectorField.FieldTypes.String:
                    setting.StringValue = (string)update;
                    break;
                case InspectorField.FieldTypes.Bool:
                    setting.BoolValue = (bool)update;
                    break;
                case InspectorField.FieldTypes.Color:
                    setting.ColorValue = (Color)update;
                    break;
                case InspectorField.FieldTypes.DropdownInt:
                    setting.IntValue = (int)update;
                    break;
                case InspectorField.FieldTypes.DropdownString:
                    setting.StringValue = (string)update;
                    break;
                case InspectorField.FieldTypes.GameObject:
                    setting.GameObjectValue = (GameObject)update;
                    break;
                case InspectorField.FieldTypes.ScriptableObject:
                    setting.ScriptableObjectValue = (ScriptableObject)update;
                    break;
                case InspectorField.FieldTypes.Object:
                    setting.ObjectValue = (UnityEngine.Object)update;
                    break;
                case InspectorField.FieldTypes.Material:
                    setting.MaterialValue = (Material)update;
                    break;
                case InspectorField.FieldTypes.Texture:
                    setting.TextureValue = (Texture)update;
                    break;
                case InspectorField.FieldTypes.Vector2:
                    setting.Vector2Value = (Vector2)update;
                    break;
                case InspectorField.FieldTypes.Vector3:
                    setting.Vector3Value = (Vector3)update;
                    break;
                case InspectorField.FieldTypes.Vector4:
                    setting.Vector4Value = (Vector4)update;
                    break;
                case InspectorField.FieldTypes.Curve:
                    setting.CurveValue = (AnimationCurve)update;
                    break;
                case InspectorField.FieldTypes.Quaternion:
                    setting.QuaternionValue = (Quaternion)update;
                    break;
                case InspectorField.FieldTypes.AudioClip:
                    setting.AudioClipValue = (AudioClip)update;
                    break;
                case InspectorField.FieldTypes.Event:
                    setting.EventValue = (UnityEvent)update;
                    break;
                default:
                    break;
            }

            return setting;
        }
        
        public static object GetSettingValue(List<PropertySetting> settings, string name)
        {
            PropertySetting setting = new PropertySetting();
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Name == name)
                {
                    setting = settings[i];
                    break;
                }
            }
            
            object value = null;

            switch (setting.Type)
            {
                case InspectorField.FieldTypes.Float:
                    value = setting.FloatValue;
                    break;
                case InspectorField.FieldTypes.Int:
                    value = setting.IntValue;
                    break;
                case InspectorField.FieldTypes.String:
                    value = setting.StringValue;
                    break;
                case InspectorField.FieldTypes.Bool:
                    value = setting.BoolValue;
                    break;
                case InspectorField.FieldTypes.Color:
                    value = setting.ColorValue;
                    break;
                case InspectorField.FieldTypes.DropdownInt:
                    value = setting.IntValue;
                    break;
                case InspectorField.FieldTypes.DropdownString:
                    value = setting.StringValue;
                    break;
                case InspectorField.FieldTypes.GameObject:
                    value = setting.GameObjectValue;
                    break;
                case InspectorField.FieldTypes.ScriptableObject:
                    value = setting.ScriptableObjectValue;
                    break;
                case InspectorField.FieldTypes.Object:
                    value = setting.ObjectValue;
                    break;
                case InspectorField.FieldTypes.Material:
                    value = setting.MaterialValue;
                    break;
                case InspectorField.FieldTypes.Texture:
                    value = setting.TextureValue;
                    break;
                case InspectorField.FieldTypes.Vector2:
                    value = setting.Vector2Value;
                    break;
                case InspectorField.FieldTypes.Vector3:
                    value = setting.Vector3Value;
                    break;
                case InspectorField.FieldTypes.Vector4:
                    value = setting.Vector4Value;
                    break;
                case InspectorField.FieldTypes.Curve:
                    value = setting.CurveValue;
                    break;
                case InspectorField.FieldTypes.Quaternion:
                    value = setting.QuaternionValue;
                    break;
                case InspectorField.FieldTypes.AudioClip:
                    value = setting.AudioClipValue;
                    break;
                case InspectorField.FieldTypes.Event:
                    value = setting.EventValue;
                    break;
                default:
                    break;
            }

            setting.Value = value;
            return value;
        }
        
        /// <summary>
        /// Get the index of a list of strings by string comparison
        /// </summary>
        /// <param name="option"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
    }
}
