// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Utilities which make creating property drawers easier.
    /// </summary>
    public static class PropertyDrawerUtilities
    {
        public const int VerticalPadding = 2;
        public const int Height = 16;
        public const int VerticalSpacing = Height + VerticalPadding;

        /// <summary>
        /// Determines the height required for drawing the complete property.
        /// </summary>
        /// <param name="lines">The number of lines (vertical fields) that comprise the property.</param>
        /// <returns>
        /// The height required to draw the property contents.
        /// </returns>
        public static float CalculatePropertyHeight(int lines)
        {
            return EditorGUIUtility.singleLineHeight * lines + ((lines - 1) * PropertyDrawerUtilities.VerticalPadding);
        }

        /// <summary>
        /// Determines the appropriate position for a control in the custom inspector.
        /// </summary>
        /// <param name="rootPosition">The root position for the custom inspector.</param>
        /// <param name="verticalSpacing">The desired spacing between rows.</param>
        /// <param name="rowMultiplier">The "row number" for the control.</param>
        /// <param name="height">The height of the row.</param>
        /// <returns>
        /// Rect providing the position at which to place the control.
        /// </returns>
        public static Rect GetPosition(
            Rect rootPosition,
            int verticalSpacing,
            int rowMultiplier,
            int height)
        {
            return new Rect(
                rootPosition.x,
                rootPosition.y + (verticalSpacing * rowMultiplier) + (VerticalPadding * rowMultiplier),
                rootPosition.width,
                height);
        }
    }
}
