

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Render style for MRTK buttons.
    /// </summary>
    public enum ButtonIconStyle
    {
        Quad,   // Renders using a material on a quad. Icon texture is set using a MaterialPropertyBlock.
        Sprite, // Renders using a sprite renderer. Icon texture is set by changing the sprite.
        Char,   // Renders using TextMeshPro. Icon texture is set by changing the character.
        None,   // Hides the icons.
    }
}