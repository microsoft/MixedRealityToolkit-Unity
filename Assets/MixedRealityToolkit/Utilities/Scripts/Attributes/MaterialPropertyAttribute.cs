// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MixedRealityToolkit.Utilities.Attributes
{
    // Displays a drop-down list of available material properties from the material supplied in a named member
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MaterialPropertyAttribute : DrawOverrideAttribute
    {
        public enum PropertyTypeEnum
        {
            Color,
            Float,
            Range,
            Vector,
        }

        public string Property { get; private set; }
        public PropertyTypeEnum PropertyType { get; private set; }
        public string MaterialMemberName { get; private set; }
        public bool AllowNone { get; private set; }
        public string DefaultProperty { get; private set; }
        public string CustomLabel { get; private set; }

        public MaterialPropertyAttribute(PropertyTypeEnum propertyType, string materialMemberName, bool allowNone = true, string defaultProperty = "_Color", string customLabel = null)
        {
            PropertyType = propertyType;
            MaterialMemberName = materialMemberName;
            AllowNone = allowNone;
            DefaultProperty = defaultProperty;
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Material mat = GetMaterial(target);

            string fieldValue = MaterialPropertyName(
               (string)field.GetValue(target),
               mat,
               PropertyType,
               AllowNone,
               DefaultProperty,
               (string.IsNullOrEmpty(CustomLabel) ? SplitCamelCase(field.Name) : CustomLabel));

            field.SetValue(target, fieldValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            Material mat = GetMaterial(target);

            string propValue = MaterialPropertyName(
               (string)prop.GetValue(target, null),
               mat,
               PropertyType,
               AllowNone,
               DefaultProperty,
               (string.IsNullOrEmpty(CustomLabel) ? SplitCamelCase(prop.Name) : CustomLabel));

            prop.SetValue(target, propValue, null);
        }

        private Material GetMaterial(object target)
        {
            MemberInfo[] members = target.GetType().GetMember(MaterialMemberName);
            if (members.Length == 0)
            {
                Debug.LogError("Couldn't find material member " + MaterialMemberName);
                return null;
            }

            Material mat = null;

            switch (members[0].MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo matField = target.GetType().GetField(MaterialMemberName);
                    mat = matField.GetValue(target) as Material;
                    break;

                case MemberTypes.Property:
                    PropertyInfo matProp = target.GetType().GetProperty(MaterialMemberName);
                    mat = matProp.GetValue(target, null) as Material;
                    break;

                default:
                    Debug.LogError("Couldn't find material member " + MaterialMemberName);
                    break;
            }

            return mat;
        }

        private static string MaterialPropertyName(string property, Material mat, PropertyTypeEnum type, bool allowNone, string defaultProperty, string labelName)
        {
            Color tColor = GUI.color;
            // Create a list of available color and value properties
            List<string> props = new List<string>();

            int selectedPropIndex = 0;

            if (allowNone)
            {
                props.Add("(None)");
            }

            if (mat != null)
            {
                int propertyCount = ShaderUtil.GetPropertyCount(mat.shader);
                string propName = string.Empty;
                for (int i = 0; i < propertyCount; i++)
                {
                    if (ShaderUtil.GetPropertyType(mat.shader, i).ToString() == type.ToString())
                    {
                        propName = ShaderUtil.GetPropertyName(mat.shader, i);
                        if (propName == property)
                        {
                            // We've found our current property
                            selectedPropIndex = props.Count;
                        }
                        props.Add(propName);
                    }
                }

                if (string.IsNullOrEmpty(labelName))
                {
                    labelName = type.ToString();
                }
                int newPropIndex = EditorGUILayout.Popup(labelName, selectedPropIndex, props.ToArray());
                if (allowNone)
                {
                    property = (newPropIndex > 0 ? props[newPropIndex] : string.Empty);
                }
                else
                {
                    if (props.Count > 0)
                    {
                        property = props[newPropIndex];
                    }
                    else
                    {
                        property = defaultProperty;
                    }
                }
                return property;
            }
            else
            {
                GUI.color = Color.Lerp(tColor, Color.gray, 0.5f);
                // Draw an empty property
                EditorGUILayout.Popup(labelName, selectedPropIndex, props.ToArray());
                GUI.color = tColor;
                return string.Empty;
            }
        }
#endif

    }
}