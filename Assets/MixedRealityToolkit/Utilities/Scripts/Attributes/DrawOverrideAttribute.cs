// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MixedRealityToolkit.Utilities.Attributes
{
    // Base class for custom drawing without property drawers - prevents the MRTKEditor from drawing a default property, supplies an alternative
    public abstract class DrawOverrideAttribute : Attribute
    {
#if UNITY_EDITOR
        public abstract void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property);
        public abstract void DrawEditor(UnityEngine.Object target, PropertyInfo prop);
#endif

        protected string SplitCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return Regex.Replace(
                Regex.Replace(
                    new string(a),
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }
    }
}