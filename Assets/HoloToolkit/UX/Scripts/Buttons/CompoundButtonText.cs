// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonText : ProfileButtonBase<ButtonTextProfile>
    {
        [DropDownComponent]
        public TextMesh TextMesh;

        /// <summary>
        /// Turn off text entirely
        /// </summary>
        [EditableProp]
        public bool DisableText {
            get {
                return disableText;
            }
            set {
                if (disableText != value) {
                    disableText = value;
                    UpdateStyle();
                }
            }
        }

        [ShowIfBoolValue("DisableText", false)]
        [TextAreaProp(30)]
        public string Text {
            get {
                if (TextMesh == null) {
                    return string.Empty;
                }
                return TextMesh.text;
            }
            set {
                TextMesh.text = value;
            }
        }

        [ShowIfBoolValue("DisableText", false)]
        [RangeProp(0f, 1f)]
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

        [ShowIfBoolValue("DisableText", false)]
        [Tooltip("Disregard the text style in the profile")]
        public bool OverrideFontStyle = false;

        [ShowIfBoolValue("OverrideFontStyle")]
        [ShowIfBoolValue("DisableText", false)]
        [Tooltip("Style to use for override.")]
        public FontStyle Style;

        [ShowIfBoolValue("DisableText", false)]
        [Tooltip("Disregard the anchor in the profile.")]
        public bool OverrideAnchor = false;

        [ShowIfBoolValue("OverrideAnchor")]
        [ShowIfBoolValue("DisableText", false)]
        [Tooltip("Anchor to use for override.")]
        public TextAnchor Anchor;

        [ShowIfBoolValue("DisableText", false)]
        [Tooltip("Disregard the size in the profile.")]
        public bool OverrideSize = false;
        
        [ShowIfBoolValue("OverrideSize")]
        [ShowIfBoolValue("DisableText", false)]
        [Tooltip("Size to use for override.")]
        public int Size = 72;

        [ShowIfBoolValue("DisableText", false)]
        [Tooltip("When true, no offset is applied to the text object.")]
        public bool OverrideOffset = false;

        [SerializeField]
        [HideInMRTKInspector]
        private float alpha = 1f;

        [SerializeField]
        [HideInMRTKInspector]
        private bool disableText = false;

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

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(CompoundButtonText))]
        public class CustomEditor : MRTKEditor { }
#endif
    }
}