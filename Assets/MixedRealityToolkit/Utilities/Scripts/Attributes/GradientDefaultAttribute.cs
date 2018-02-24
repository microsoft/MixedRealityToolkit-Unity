// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MixedRealityToolkit.Utilities.Attributes
{
    // Adds a 'default' button to a color gradient that will supply default color values
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GradientDefaultAttribute : DrawOverrideAttribute
    {
        // Used because you can't pass colors as attribute vars :/
        public enum ColorEnum
        {
            Black,
            Blue,
            Clear,
            Cyan,
            Gray,
            Green,
            Magenta,
            Red,
            White,
            Yellow
        }

        public GradientDefaultAttribute(ColorEnum startColor, ColorEnum endColor, float startAlpha = 1f, float endAlpha = 1f)
        {
            this.startColor = GetColor(startColor);
            this.endColor = GetColor(endColor);
            this.startColor.a = startAlpha;
            this.endColor.a = endAlpha;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Gradient gradientValue = field.GetValue(target) as Gradient;

            if (gradientValue == null || gradientValue.colorKeys == null || gradientValue.colorKeys.Length == 0)
                gradientValue = GetDefault();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property);
            if (GUILayout.Button("Default"))
            {
                gradientValue = GetDefault();
            }
            EditorGUILayout.EndHorizontal();

            field.SetValue(target, gradientValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            throw new NotImplementedException();
        }
#endif

        private Gradient GetDefault()
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2] {
                    new GradientColorKey(startColor, 0f),
                    new GradientColorKey(endColor, 1f)
                };
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2]
            {
                    new GradientAlphaKey(startColor.a, 0f),
                    new GradientAlphaKey(endColor.a, 0f),
            };
            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private Color startColor;
        private Color endColor;

        private static Color GetColor(ColorEnum color)
        {
            switch (color)
            {
                case ColorEnum.Black:
                    return Color.black;
                case ColorEnum.Blue:
                    return Color.blue;
                case ColorEnum.Clear:
                default:
                    return Color.clear;
                case ColorEnum.Cyan:
                    return Color.cyan;
                case ColorEnum.Gray:
                    return Color.gray;
                case ColorEnum.Green:
                    return Color.green;
                case ColorEnum.Magenta:
                    return Color.magenta;
                case ColorEnum.Red:
                    return Color.red;
                case ColorEnum.White:
                    return Color.white;
                case ColorEnum.Yellow:
                    return Color.yellow;
            }
        }
    }
}