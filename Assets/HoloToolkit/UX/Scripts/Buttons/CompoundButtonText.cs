//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonText : ProfileButtonBase<ButtonTextProfile>
    {
        public TextMesh TextMesh;
        
        /// <summary>
        /// Turn off text entirely
        /// </summary>
        public bool DisableText
        {
            get
            {
                return disableText;
            }
            set
            {
                if (disableText != value)
                {
                    disableText = value;
                    UpdateStyle();
                }
            }
        }

        /// <summary>
        /// Disregard the text style in the profile
        /// </summary>
        public bool OverrideFontStyle = false;

        /// <summary>
        /// Style to use for override
        /// </summary>
        public FontStyle Style;

        /// <summary>
        /// Disregard the anchor in the profile
        /// </summary>
        public bool OverrideAnchor = false;

        /// <summary>
        /// Anchor to use for override
        /// </summary>
        public TextAnchor Anchor;

        /// <summary>
        /// Disregard the size in the profile
        /// </summary>
        public bool OverrideSize = false;

        /// <summary>
        /// Size to use for override
        /// </summary>
        public int Size = 72;

        /// <summary>
        /// Disregard the offset in the profile.
        /// When this is selected, no offset is applied to the text object.
        /// </summary>
        public bool OverrideOffset = false;
        
        /// <summary>
        /// The text value of the button
        /// </summary>
        public string Text
        {
            get
            {
                if (TextMesh == null)
                {
                    return string.Empty;
                }
                return TextMesh.text;
            }
            set
            {
                TextMesh.text = value;
            }
        }

        public float Alpha {
            get {
                return alpha;
            }
            set {
                if (value != alpha) {
                    alpha = value;
                    UpdateStyle();
                }
            }
        }

        private void OnEnable()
        {
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            if (TextMesh == null)
            {
                Debug.LogWarning("Text mesh was null in CompoundButtonText " + name);
                return;
            }

            if (DisableText)
            {
                TextMesh.gameObject.SetActive(false);
            }
            else
            {
                // Update text based on profile
                if (Profile != null)
                {
                    TextMesh.font = Profile.Font;
                    TextMesh.fontStyle = Profile.Style;
                    TextMesh.fontSize = OverrideSize ? Size : Profile.Size;
                    TextMesh.fontStyle = OverrideFontStyle ? Style : Profile.Style;
                    TextMesh.anchor = OverrideAnchor ? Anchor : Profile.Anchor;
                    TextMesh.alignment = Profile.Alignment;
                    Color c = Profile.Color;
                    c.a = alpha;
                    TextMesh.color = c;

                    // Apply offset
                    if (!OverrideOffset)
                    {
                        TextMesh.transform.localPosition = Profile.GetOffset(TextMesh.anchor);
                    }

                    TextMesh.gameObject.SetActive(true);
                }
            }
        }

        private void OnDrawGizmos ()
        {
            UpdateStyle();
        }

        [SerializeField]
        private float alpha = 1f;

        [SerializeField]
        private bool disableText = false;
    }
}