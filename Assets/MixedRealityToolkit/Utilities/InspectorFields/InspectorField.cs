// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.InspectorFields
{
    /// <summary>
    /// A set of field/property tags used to define how a property should render in a custom inspector
    /// </summary>
    public class InspectorField : Attribute
    {
        /// <summary>
        /// Property types used for casting and defining property fields in the inspector
        /// </summary>
        public enum FieldTypes
        {
            Float,
            Int,
            String,
            Bool,
            Color,
            DropdownInt,
            DropdownString,
            GameObject,
            ScriptableObject,
            Object,
            Material,
            Texture,
            Vector2,
            Vector3,
            Vector4,
            Curve,
            Quaternion,
            AudioClip,
            Event
        }

        /// <summary>
        /// The type of field or property value type
        /// </summary>
        public FieldTypes Type { get; set; }

        /// <summary>
        /// The label that will be rendered with the property field in the custom inspector
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// A tooltip for the property field
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// A string list of options for a pop-up list
        /// </summary>
        public string[] Options { get; set; }
        
        /// <summary>
        /// An object to hold the actual value
        /// </summary>
        public UnityEngine.Object Value { get; set; }

        public static InspectorPropertySetting FieldToProperty(InspectorField attributes, object fieldValue, string fieldName)
        {
            InspectorPropertySetting setting = new InspectorPropertySetting();
            setting.Type = attributes.Type;
            setting.Tooltip = attributes.Tooltip;
            setting.Label = attributes.Label;
            setting.Options = attributes.Options;
            setting.Name = fieldName;
            setting = UpdatePropertySetting(setting, fieldValue);
            return setting;
        }

        /// <summary>
        /// Set the value of the propertySetting
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static InspectorPropertySetting UpdatePropertySetting(InspectorPropertySetting setting, object update)
        {
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
        
        /// <summary>
        /// Get the propertySettings value
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetSettingValue(List<InspectorPropertySetting> settings, string name)
        {
            InspectorPropertySetting setting = new InspectorPropertySetting();
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

            return value;
        }
        
        /// <summary>
        /// Get the index from a list of strings using string comparison
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
