// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Draws a customer decorator drawer that displays a help box with rich text tagging implementation as experimental.
    /// </summary>
    [CustomPropertyDrawer(typeof(ExperimentalAttribute))]
    public class ExperimentalDrawer : DecoratorDrawer
    {
        /// <summary>
        /// Unity calls this function to draw the GUI.
        /// </summary>
        /// <param name="position">Rectangle to display the GUI in</param>
        public override void OnGUI(Rect position)
        {
            var experimental = attribute as ExperimentalAttribute;

            if (experimental != null)
            {
                var defaultValue = EditorStyles.helpBox.richText;
                EditorStyles.helpBox.richText = true;
                EditorGUI.HelpBox(position, experimental.Text, MessageType.Warning);
                EditorStyles.helpBox.richText = defaultValue;
            }
        }

        /// <summary>
        /// Returns the height required to display UI elements drawn by OnGUI.
        /// </summary>
        /// <returns>The height required by OnGUI.</returns>
        public override float GetHeight()
        {
            var experimental = attribute as ExperimentalAttribute;

            if (experimental != null)
            {
                return EditorStyles.helpBox.CalcHeight(new GUIContent(experimental.Text), EditorGUIUtility.currentViewWidth);
            }

            return base.GetHeight();
        }
    }
}