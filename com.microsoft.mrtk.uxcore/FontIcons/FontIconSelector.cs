// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A Component that can be used to select a specific icon for display via a TextMeshPro component.
    /// </summary>
    [Serializable]
    public class FontIconSelector : MonoBehaviour
    {
        [Tooltip("The FontIconSet scriptable object that contains the icons available for use.")]
        [SerializeField]
        private FontIconSet fontIcons;
        public FontIconSet FontIcons => fontIcons;

        [Tooltip("The currently selected icon by name.")]
        [SerializeField]
        private string currentIconName;
        public string CurrentIconName
        {
            get
            {
                return currentIconName;
            }

            set
            {
                if (value != currentIconName)
                {
                    SetIcon(value);
                }
            }
        }

        [Tooltip("The TextMeshPro Component to be used to show the icon.")]
        [SerializeField]
        private TMP_Text textMeshProComponent;
        public TMP_Text TextMeshProComponent => textMeshProComponent;

        protected void Awake()
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

        /// <summary>
        /// A TextMeshPro Font Asset that contains the desired icons as glyphs that map to Unicode character values.
        /// </summary>
        public TMP_FontAsset IconFontAsset => iconFontAsset;

        [Tooltip("Any TextMeshPro Font Asset that contains the desired icons as glyphs that map to Unicode character values.")]
        [SerializeField]
        private TMP_FontAsset iconFontAsset = null;


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
