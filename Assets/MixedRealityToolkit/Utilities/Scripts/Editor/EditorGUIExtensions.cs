// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Utilities.EditorScript
{
    /// <summary>
    /// Extensions for the UnityEnditor.EditorGUI class.
    /// </summary>
    public class EditorGUIExtensions : MonoBehaviour
    {
        public static float Indent
        {
            get
            {
                return EditorGUI.IndentedRect(new Rect()).x;
            }
        }

        /// <summary>
        /// Space to place between adjacent horizontal elements.
        /// Matches value used in Unity source for EditorGUILayout.Space().
        /// </summary>
        public const float HorizontalSpacing = 6.0f;

        public static bool Button(Rect position, string text)
        {
            return Button(position, new GUIContent(text));
        }

        public static bool Button(Rect position, GUIContent content)
        {
            float indent = Indent;
            position.x += indent;
            position.width -= indent;
            return GUI.Button(position, content);
        }

        public static void Label(Rect position, string text)
        {
            float indent = Indent;
            position.x += indent;
            position.width -= indent;
            GUI.Label(position, text);
        }

        // Based on logic in Unity source function EditorGUI.GetSinglePropertyHeight().
        // Doesn't handle serialized types with nested serialized children.
        public static float GetTypeHeight(bool hasLabel, Type valueType)
        {
            if (valueType == typeof(Vector3) || valueType == typeof(Vector2))
            {
                return (!hasLabel || EditorGUIUtility.wideMode ? 0f : EditorGUIUtility.singleLineHeight) + EditorGUIUtility.singleLineHeight;
            }

            if (valueType == typeof(Rect))
            {
                return (!hasLabel || EditorGUIUtility.wideMode ? 0f : EditorGUIUtility.singleLineHeight) + EditorGUIUtility.singleLineHeight * 2;
            }

            if (valueType == typeof(Bounds))
            {
                return (!hasLabel ? 0f : EditorGUIUtility.singleLineHeight) + EditorGUIUtility.singleLineHeight * 2;
            }

            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Shows an object field editor for object types that do no derive from UnityEngine.Object.
        /// </summary>
        /// <typeparam name="T">Type of the object to modify.</typeparam>
        /// <param name="position">The region to show the UI.</param>
        /// <param name="label">Label to show.</param>
        /// <param name="value">Current value to show.</param>
        /// <param name="allowSceneObjects">Whether scene objects should be allowed in the set of field choices.</param>
        /// <returns>The new value.</returns>
        public static T ObjectField<T>(Rect position, GUIContent label, T value, bool allowSceneObjects)
        {
            object objValue = value;

            Type valueType = objValue.GetType();
            if (valueType == typeof(Bounds))
            {
                objValue = EditorGUI.BoundsField(position, label, (Bounds)objValue);
            }
            else if (valueType == typeof(Color))
            {
                objValue = EditorGUI.ColorField(position, label, (Color)objValue);
            }
            else if (valueType == typeof(Material))
            {
                objValue = EditorGUI.ObjectField(position, (Material)objValue, typeof(Material), allowSceneObjects);
            }
            else if (valueType == typeof(AnimationCurve))
            {
                objValue = EditorGUI.CurveField(position, label, (AnimationCurve)objValue);
            }
            else if (valueType == typeof(float))
            {
                objValue = EditorGUI.FloatField(position, label, (float)objValue);
            }
            else if (valueType == typeof(int))
            {
                objValue = EditorGUI.IntField(position, label, (int)objValue);
            }
            else if (valueType == typeof(LayerMask))
            {
                objValue = EditorGUI.MaskField(position, label, (LayerMask)objValue, LayerMaskExtensions.LayerMaskNames);
            }
            else if (valueType.IsEnum)
            {
                if (valueType.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0)
                {
                    objValue = EditorGUI.EnumMaskField(position, label, (Enum)objValue);
                }
                else
                {
                    objValue = EditorGUI.EnumPopup(position, label, (Enum)objValue);
                }
            }
            else if (valueType == typeof(Rect))
            {
                objValue = EditorGUI.RectField(position, label, (Rect)objValue);
            }
            else if (valueType == typeof(string))
            {
                objValue = EditorGUI.TextField(position, label, (string)objValue);
            }
            else if (valueType == typeof(Vector2))
            {
                objValue = EditorGUI.Vector2Field(position, new GUIContent(), (Vector2)objValue);
            }
            else if (valueType == typeof(Vector3))
            {
                objValue = EditorGUI.Vector3Field(position, new GUIContent(), (Vector3)objValue);
            }
            else if (valueType == typeof(Vector4))
            {
                if (label.image != null)
                {
                    throw new ArgumentException("Images not supported for labels of Vector4 fields.", "label");
                }

                if (!string.IsNullOrEmpty(label.tooltip))
                {
                    throw new ArgumentException("Tool-tips not supported for labels of Vector4 fields.", "label");
                }

                objValue = EditorGUI.Vector4Field(position, label.text, (Vector4)objValue);
            }
            else if (Equals(objValue, typeof(SceneAsset)))
            {
                objValue = EditorGUI.ObjectField(position, (SceneAsset)objValue, typeof(SceneAsset), allowSceneObjects);
            }
            else if (objValue is UnityEngine.Object)
            {
                objValue = EditorGUI.ObjectField(position, label, (UnityEngine.Object)objValue, valueType, allowSceneObjects);
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unimplemented value type: {0}.",
                        valueType),
                    "value");
            }

            return (T)objValue;
        }
    }
}