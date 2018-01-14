// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Inspectors.EditorScript;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Utilities.EditorScript
{
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
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.LowerLeft
                };
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
}