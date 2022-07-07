// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Allows the user to select a specific icon for display via a TextMeshPro component.
    /// See <see cref="FontIconSelectorInspector"/> for the majority of the user-facing editor code.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Font Icon Selector")]
    public class FontIconSelector : MonoBehaviour
    {
        [Tooltip("The FontIconSet scriptable object that contains the icons available for use.")]
        [SerializeField]
        private FontIconSet fontIcons;

        /// <summary>
        /// The FontIconSet ScriptableObject that contains the icons
        /// available for use, and their human-readable names.
        /// </summary>
        public FontIconSet FontIcons => fontIcons;

        [Tooltip("The currently selected icon's name, as defined by the FontIconSet ScriptableObject.")]
        [SerializeField]
        private string currentIconName;

        /// <summary>
        /// The currently selected icon's name, as defined by the FontIconSet ScriptableObject.
        /// </summary>
        public string CurrentIconName
        {
            get => currentIconName;

            set
            {
                if (value != currentIconName)
                {
                    SetIcon(value);
                }
            }
        }

        [Tooltip("The TextMeshPro used to show the icon.")]
        [SerializeField]
        private TMP_Text textMeshProComponent;

        /// <summary>
        /// The TextMeshPro used to show the icon.
        /// </summary>
        public TMP_Text TextMeshProComponent => textMeshProComponent;

        private void Awake()
        {
            if (textMeshProComponent == null)
            {
                textMeshProComponent = GetComponent<TMP_Text>();
            }
            SetIcon(currentIconName);
        }

        private void OnValidate()
        {
            SetIcon(currentIconName);
        }

        [Tooltip("Any TextMeshPro Font Asset that contains the desired icons as glyphs that map to Unicode character values.")]
        [SerializeField]
        private TMP_FontAsset iconFontAsset = null;

        /// <summary>
        /// A TextMeshPro Font Asset that contains the desired icons as glyphs that map to Unicode character values.
        /// </summary>
        public TMP_FontAsset IconFontAsset => iconFontAsset;

        private void SetIcon(string newIconName)
        {
            if (fontIcons != null && textMeshProComponent != null)
            {
                if (fontIcons.TryGetGlyphIcon(newIconName, out uint unicodeValue))
                {
                    currentIconName = newIconName;
                    textMeshProComponent.text = FontIconSet.ConvertUnicodeToHexString(unicodeValue);
                }
            }
        }
    }
}
