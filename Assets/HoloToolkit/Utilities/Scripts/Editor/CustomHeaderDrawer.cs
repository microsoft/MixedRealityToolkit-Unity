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
    [CustomPropertyDrawer(typeof(HeaderAttribute))]
    public class CustomHeaderDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return MRTKEditor.ShowCustomEditors ? 0f : 24f;
        }

        public override void OnGUI(Rect position)
        {
            // If we're using MRDL custom editors, don't show the header
            if (MRTKEditor.ShowCustomEditors)
                return;

            // Otherwise draw it normally
            GUI.Label(position, (base.attribute as HeaderAttribute).header, EditorStyles.boldLabel);
        }
    }
#endif

}