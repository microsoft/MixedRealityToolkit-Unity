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
    public class GenericFields<T>
    {
        public static void LoadSettings(T target, List<PropertySetting> settings)
        {
            Type myType = target.GetType();

            foreach (PropertyInfo prop in myType.GetProperties())
            {
                var attrs = (InspectorField[])prop.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    object value = InspectorField.GetSettingValue(settings, prop.Name);
                    prop.SetValue(target, value);
                }
            }

            foreach (FieldInfo field in myType.GetFields())
            {
                var attrs = (InspectorField[])field.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    object value = InspectorField.GetSettingValue(settings, field.Name);
                    field.SetValue(target, value);
                }
            }
        }
        
        public static List<PropertySetting> GetSettings(T source)
        {
            Type myType = source.GetType();
            List<PropertySetting> settings = new List<PropertySetting>();

            foreach (PropertyInfo prop in myType.GetProperties())
            {
                var attrs = (InspectorField[])prop.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    settings.Add(InspectorField.FieldToProperty(attr, prop.GetValue(source, null), prop.Name));
                }
            }

            foreach (FieldInfo field in myType.GetFields())
            {
                var attrs = (InspectorField[])field.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    settings.Add(InspectorField.FieldToProperty(attr, field.GetValue(source), field.Name));
                }
            }

            return settings;
        }
    }
}
