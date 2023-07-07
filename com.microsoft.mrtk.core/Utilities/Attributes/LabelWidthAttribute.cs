// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Indicates that the serializable property's label should be of a particular width when rendered in the Unity inspector window.
    /// </summary>
    /// <remarks>
    /// This attribute does not indicate a font size change, and is only applicable to applications running in the Unity Editor.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class LabelWidthAttribute : PropertyAttribute
    {
        /// <summary>
        /// Get the width to apply to the serializable property's label when the label is rendered in the Unity inspector window.
        /// </summary>
        public float Width { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelWidthAttribute"/> class.
        /// </summary>
        /// <param name="width">The width to apply to the serializable property's label when the label is rendered in the Unity inspector window.</param>
        public LabelWidthAttribute(float width)
        {
            Width = width;
        }
    }
}
