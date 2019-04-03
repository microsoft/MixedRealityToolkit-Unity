// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom property drawer to show a foldout help section in the Inspector
    /// </summary>
    /// <example>
    /// <code>
    /// [Help("This is a multiline collapsable help section.\n • Great for providing simple instructions in Inspector.\n • Easy to use.\n • Saves space.")]
    /// </code>
    /// </example>
    [CustomPropertyDrawer(typeof(HelpAttribute))]
    public class HelpDrawer : DecoratorDrawer
    {
        /// <summary>
        /// Unity calls this function to draw the GUI
        /// </summary>
        /// <param name="position">Rectangle to display the GUI in</param>
        public override void OnGUI(Rect position)
        {
            HelpAttribute help = attribute as HelpAttribute;

            HelpFoldOut = EditorGUI.Foldout(position, HelpFoldOut, help.Header);
            if (HelpFoldOut)
            {
                EditorGUI.HelpBox(position, help.Text, MessageType.Info);
            }
        }

        /// <summary>
        /// Gets the height of the decorator
        /// </summary>
        /// <returns></returns>
        public override float GetHeight()
        {
            HelpAttribute help = attribute as HelpAttribute;

            GUIStyle helpStyle = EditorStyles.helpBox;
            Vector2 size = helpStyle.CalcSize(new GUIContent(help.Text));
            float lines = size.y / helpStyle.lineHeight;
            return helpStyle.margin.top + helpStyle.margin.bottom + helpStyle.lineHeight * (HelpFoldOut ? lines : 1.0f);
        }

        #region Private

        /// <summary>
        /// The "help" foldout state
        /// </summary>
        private bool HelpFoldOut = false;

        #endregion
    }
}