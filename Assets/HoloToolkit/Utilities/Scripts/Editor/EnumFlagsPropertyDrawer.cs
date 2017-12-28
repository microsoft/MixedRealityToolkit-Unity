using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If we're using MRDL custom editors, let the draw override property handle it
            if (MRTKEditor.ShowCustomEditors)
                return;

            // Otherwise draw a bitmask normally
            base.OnGUI(position, property, label);
        }
    }
#endif
}