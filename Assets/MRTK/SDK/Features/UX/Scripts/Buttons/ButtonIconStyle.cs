
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Render style for MRTK buttons.
    /// </summary>
    public enum ButtonIconStyle
    {
        /// <summary>
        /// Renders using a material on a quad. Icon texture is set using a MaterialPropertyBlock.
        /// </summary>
        Quad,
        /// <summary>
        /// Renders using a sprite renderer. Icon texture is set by changing the sprite.
        /// </summary>
        Sprite,
        /// <summary>
        /// Renders using TextMeshPro. Icon texture is set by changing the character.
        /// </summary>
        Char,
        /// <summary>
        /// Hides the icons.
        /// </summary>
        None,
    }
}