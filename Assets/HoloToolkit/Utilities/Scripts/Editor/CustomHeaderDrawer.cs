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
            return (MRTKEditor.ShowCustomEditors && MRTKEditor.CustomEditorActive) ? 0f : 24f;
        }

        public override void OnGUI(Rect position)
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel);
                headerStyle.alignment = TextAnchor.LowerLeft;
            }

            // If we're using MRDL custom editors, don't show the header
            if (MRTKEditor.ShowCustomEditors && MRTKEditor.CustomEditorActive)
            {
                return;
            }

            // Otherwise draw it normally
            GUI.Label(position, (base.attribute as HeaderAttribute).header, headerStyle);
        }

        private static GUIStyle headerStyle = null;
    }
#endif

}