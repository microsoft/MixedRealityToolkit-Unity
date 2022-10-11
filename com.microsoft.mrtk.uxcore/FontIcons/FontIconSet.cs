// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A ScriptableObject for managing a set of character icons for use with MRTK UX via TextMeshPro.
    ///
    /// This is intended to be used with DataConsumerFontIconSet, which can be used to set the desired static
    /// icon for a TextMeshPro component, and can also bind to a data source that can select an icon
    /// by its name.
    ///
    /// When using a stylesheet, it is important to use a style that maps to the desired TMP Font Asset (by its name)
    /// and any other styling desired when rendered, such as size and color.
    /// </summary>
    [CreateAssetMenu(fileName = "MRTK_UX_FontIconSet_New", menuName = "MRTK/UX/Font Icon Set")]
    public class FontIconSet : ScriptableObject
    {
        [SerializeField]
        [Tooltip("A mapping between icon names and the unicode value of the glyph it describes.")]
        private SerializableDictionary<string, uint> glyphIconsByName = new SerializableDictionary<string, uint>();
        public SerializableDictionary<string, uint> GlyphIconsByName => glyphIconsByName;

        [Tooltip("Any TextMeshPro Font Asset that contains the desired icons as glyphs that map to Unicode character values.")]
        [SerializeField]
        private TMP_FontAsset iconFontAsset = null;
        /// <summary>
        /// Any TextMeshPro Font Asset that contains the desired icons as glyphs that map to Unicode character values.
        /// </summary>
        public TMP_FontAsset IconFontAsset => iconFontAsset;

        [Tooltip("Optional material to use for rendering glyphs in editor.")]
        [SerializeField]
        private Material optionalEditorMaterial;
        public Material OptionalEditorMaterial => optionalEditorMaterial;

        /// <summary>
        /// Try to get a glyph icon's unicode value by name.
        /// </summary>
        /// <param name="iconName">The name of the icon to find.</param>
        /// <param name="unicodeValue">The returned unicode value, or 0 if not found.</param>
        /// <returns>True if icon name found, otherwise false.</returns>
        public bool TryGetGlyphIcon(string iconName, out uint unicodeValue)
        {
            unicodeValue = 0;
            return glyphIconsByName.TryGetValue(iconName, out unicodeValue);
        }

        /// <summary>
        /// Add icon to the available set based on a glyph in the TMP_FontAsset.
        /// </summary>
        /// <param name="name">Name for this icon glyph,</param>
        /// <param name="unicodeValue">Unicode value for the glyph.</param>
        /// <returns>Whether it was able to add this icon.</returns>
        public bool AddIcon(string name, uint unicodeValue)
        {
            if (glyphIconsByName.ContainsValue(unicodeValue))
            {
                return false;
            }
            else
            {
                glyphIconsByName[name] = unicodeValue;
                return true;
            }
        }

        /// <summary>
        /// Remove an icon from available set by its name.
        /// </summary>
        /// <param name="iconName">The named icon to remove.</param>
        /// <returns>Whether it was able to find the name and remove it.</returns>
        public bool RemoveIcon(string iconName)
        {
            if (glyphIconsByName.ContainsKey(iconName))
            {
                glyphIconsByName.Remove(iconName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the name of an icon.
        /// </summary>
        /// <remarks>
        /// Note that this will return false if the new name already exists or if the
        /// current name can't be found.
        /// </remarks>
        /// <param name="oldName">The current name of the icon.</param>
        /// <param name="newName">The desired new name of the icon.</param>
        /// <returns>True if it was able to find and update the name.</returns>
        public bool UpdateIconName(string oldName, string newName)
        {
            if (glyphIconsByName.ContainsKey(oldName) && !glyphIconsByName.ContainsKey(newName))
            {
                glyphIconsByName[newName] = glyphIconsByName[oldName];
                glyphIconsByName.Remove(oldName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a unicode string to a uint code (for use with TextMeshPro).
        /// </summary>
        /// <param name="charString">Hex string in the form '\uFFFF'.</param>
        /// <returns>The binary unicode value.</returns>
        public static uint ConvertHexStringToUnicode(string charString)
        {
            uint unicode = 0;

            if (string.IsNullOrEmpty(charString))
                return 0;

            for (int i = 0; i < charString.Length; i++)
            {
                unicode = charString[i];
                // Handle surrogate pairs
                if (i < charString.Length - 1 && char.IsHighSurrogate((char)unicode) && char.IsLowSurrogate(charString[i + 1]))
                {
                    unicode = (uint)char.ConvertToUtf32(charString[i], charString[i + 1]);
                    i += 1;
                }
            }
            return unicode;
        }

        /// <summary>
        /// Converts a uint code to a string (used to convert TextMeshPro codes into a string for text fields).
        /// </summary>
        /// <param name="unicode">Unicode value to be converted to a hex string representation.</param>
        /// <returns>The string version of the unicode value in the form '\uFFFF' where FFFF is ascii hex.</returns>
        public static string ConvertUnicodeToHexString(uint unicode)
        {
            byte[] bytes = System.BitConverter.GetBytes(unicode);
            return Encoding.Unicode.GetString(bytes);
        }
    }
}
