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
    /// A set of field/proptery tags used to define how a property should render in a custom inspector
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
        /// The type of field or propterty value type
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
        /// A string list of options for a popup list
        /// </summary>
        public string[] Options { get; set; }
        
        /// <summary>
        /// An object to hold the actual value
        /// </summary>
        public UnityEngine.Object Value { get; set; }

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

        /// <summary>
        /// Set the value of the propertySetting
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static PropertySetting UpdatePropertySetting(PropertySetting setting, object update)
        {
            setting.Value = update;
            return setting;
        }
        
        /// <summary>
        /// Get the propertySettings value
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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

            return setting.Value;
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
