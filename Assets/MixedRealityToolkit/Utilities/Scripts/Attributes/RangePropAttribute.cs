// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MixedRealityToolkit.Utilities.Attributes
{
    // Displays an int or float property as a range
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RangePropAttribute : DrawOverrideAttribute
    {

        public enum TypeEnum
        {
            Float,
            Int,
        }

        public float MinFloat { get; private set; }
        public float MaxFloat { get; private set; }
        public int MinInt { get; private set; }
        public int MaxInt { get; private set; }
        public TypeEnum Type { get; private set; }

        public RangePropAttribute(float min, float max)
        {
            MinFloat = min;
            MaxFloat = max;
            Type = TypeEnum.Float;
        }

        public RangePropAttribute(int min, int max)
        {
            MinInt = min;
            MaxInt = max;
            Type = TypeEnum.Int;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            // (safe since this is property-only attribute)
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(int))
            {
                int propIntValue = (int)prop.GetValue(target, null);
                propIntValue = EditorGUILayout.IntSlider(SplitCamelCase(prop.Name), propIntValue, MinInt, MaxInt);
                prop.SetValue(target, propIntValue, null);
            }
            else if (prop.PropertyType == typeof(float))
            {
                float propFloatValue = (float)prop.GetValue(target, null);
                propFloatValue = EditorGUILayout.Slider(SplitCamelCase(prop.Name), propFloatValue, MinFloat, MaxFloat);
                prop.SetValue(target, propFloatValue, null);
            }
        }
#endif

    }
}