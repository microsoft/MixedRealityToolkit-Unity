// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilties.Attributes;
using MixedRealityToolkit.UtiltiesInspectors;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Utilties
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