// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A set of Inspector fields for setting up properties in a
    /// component that can be automatically rendered in a custom inspector
    /// </summary>
    public class InspectorGenericFields<T>
    {
        /// <summary>
        /// Copies values from Inspector PropertySettings to an instantiated class on start,
        /// helps overcome polymorphism limitations of serialization
        /// </summary>
        public static void LoadSettings(T target, List<InspectorPropertySetting> settings)
        {
            Type myType = target.GetType();

            List<PropertyInfo> propInfoList = new List<PropertyInfo>(myType.GetProperties());
            for (int i = 0; i < propInfoList.Count; i++)
            {
                PropertyInfo propInfo = propInfoList[i];
                var attrs = (InspectorField[])propInfo.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    object value = InspectorField.GetSettingValue(settings, propInfo.Name);
                    if (value != null)
                    {
                        propInfo.SetValue(target, value);
                    }
                }
            }

            List<FieldInfo> fieldInfoList = new List<FieldInfo>(myType.GetFields());
            for (int i = 0; i < fieldInfoList.Count; i++)
            {
                FieldInfo fieldInfo = fieldInfoList[i];
                var attrs = (InspectorField[])fieldInfo.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    object value = InspectorField.GetSettingValue(settings, fieldInfo.Name);
                    if (value != null)
                    {
                        fieldInfo.SetValue(target, value);
                    }
                }
            }
        }

        /// <summary>
        /// Searches through a class for InspectorField tags creates properties that can be serialized and
        /// automatically rendered in a custom inspector
        /// </summary>
        public static List<InspectorPropertySetting> GetSettings(T source)
        {
            Type myType = source.GetType();
            List<InspectorPropertySetting> settings = new List<InspectorPropertySetting>();

            List<PropertyInfo> propInfoList = new List<PropertyInfo>(myType.GetProperties());
            for (int i = 0; i < propInfoList.Count; i++)
            {
                PropertyInfo propInfo = propInfoList[i];
                var attrs = (InspectorField[])propInfo.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    settings.Add(InspectorField.FieldToProperty(attr, propInfo.GetValue(source, null), propInfo.Name));
                }
            }

            List<FieldInfo> fieldInfoList = new List<FieldInfo>(myType.GetFields());
            for (int i = 0; i < fieldInfoList.Count; i++)
            {
                FieldInfo fieldInfo = fieldInfoList[i];
                var attrs = (InspectorField[])fieldInfo.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    settings.Add(InspectorField.FieldToProperty(attr, fieldInfo.GetValue(source), fieldInfo.Name));
                }
            }

            return settings;
        }
    }
}
