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
    /// A set of Inspector fields for setting up properties in a
    /// component that can be automatically rendered in a custom inspector
    /// </summary>
    public class InspectorGenericFields<T>
    {
        /// <summary>
        /// Copies values from Inspector PropertySettings to an instantiated class on start,
        /// helps overcome polymorphism limitations of serialization
        /// </summary>
        /// <param name="target"></param>
        /// <param name="settings"></param>
        public static void LoadSettings(T target, List<InspectorPropertySetting> settings)
        {
            Type myType = target.GetType();

            PropertyInfo[] propInfoList = myType.GetProperties();
            for (int i = 0; i < propInfoList.Length; i++)
            {
                PropertyInfo propInfo = propInfoList[i];
                var attrs = (InspectorField[])propInfo.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    object value = InspectorField.GetSettingValue(settings, propInfo.Name);
                    propInfo.SetValue(target, value);
                }
            }

            FieldInfo[] fieldInfoList = myType.GetFields();
            for (int i = 0; i < fieldInfoList.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfoList[i];
                var attrs = (InspectorField[])fieldInfo.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    object value = InspectorField.GetSettingValue(settings, fieldInfo.Name);
                    fieldInfo.SetValue(target, value);
                }
            }
        }
        
        /// <summary>
        /// Searches through a class for InspectorField tags creates properties that can be serialized and
        /// automatically rendered in a custom inspector
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<InspectorPropertySetting> GetSettings(T source)
        {
            Type myType = source.GetType();
            List<InspectorPropertySetting> settings = new List<InspectorPropertySetting>();

            PropertyInfo[] propInfoList = myType.GetProperties();
            for (int i = 0; i < propInfoList.Length; i++)
            {
                PropertyInfo propInfo = propInfoList[i];
                var attrs = (InspectorField[])propInfo.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    settings.Add(InspectorField.FieldToProperty(attr, propInfo.GetValue(source, null), propInfo.Name));
                }
            }

            FieldInfo[] fieldInfoList = myType.GetFields();
            for (int i = 0; i < fieldInfoList.Length; i++)
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
