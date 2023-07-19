// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Draws a customer decorator drawer that displays a help box with rich text tagging implementation as experimental.
    /// </summary>
    [CustomPropertyDrawer(typeof(ExperimentalAttribute))]
    public class ExperimentalDrawer : DecoratorDrawer
    {
        /// <summary>
        /// A function called by Unity to render and handle GUI events.
        /// </summary>
        /// <param name="position">Rectangle to display the GUI in</param>
        public override void OnGUI(Rect position)
        {
            if (attribute is ExperimentalAttribute experimental)
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
            if (attribute is ExperimentalAttribute experimental)
            {
                return EditorStyles.helpBox.CalcHeight(new GUIContent(experimental.Text), EditorGUIUtility.currentViewWidth);
            }

            return base.GetHeight();
        }
    }
}
