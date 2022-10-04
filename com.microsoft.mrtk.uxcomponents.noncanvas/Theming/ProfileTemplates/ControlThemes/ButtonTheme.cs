// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// All themable elements of a MRTK UX Button.
    ///
    /// Note that by default whenever a themable element
    /// is shared by different control types, the same
    /// element is repeated in UX.Common and the default
    /// theme for that elmeent is pointed to UX.Common.xxx
    /// instead of UX.Button.xxx.
    /// </summary>
    [Serializable]
    public class ButtonTheme
    {
        [Tooltip("Material for highlighting the backplate.")]
        [SerializeField]
        private Material backplateHighlightMaterial;
        public Material BackplateHighlightMaterial => backplateHighlightMaterial;

        [Tooltip("Material for highlighting the frontplate.")]
        [SerializeField]
        private Material frontplateHighlightMaterial;
        public Material FrontplateHighlightMaterial => frontplateHighlightMaterial;

        [Tooltip("Material for 'See It. Say It' backplate.")]
        [SerializeField]
        private Material seeItSayItBackplateQuadMaterial;
        public Material SeeItSayItBackplateQuadMaterial => seeItSayItBackplateQuadMaterial;

        [Tooltip("Material for showing the outer rim geometry.")]
        [SerializeField]
        private Material backplateOuterGeometryMaterial;
        public Material BackplateOuterGeometryMaterial => backplateOuterGeometryMaterial;

        [Tooltip("Material for showing change of state on the backplate.")]
        [SerializeField]
        private Material backplateQuadMaterial;
        public Material BackplateQuadMaterial => backplateQuadMaterial;

        [Tooltip("Material for showing the toggle true or false state.")]
        [SerializeField]
        private Material backplateToggleQuadMaterial;
        public Material BackplateToggleQuadMaterial => backplateToggleQuadMaterial;

        [Tooltip("Material for highlighting the backplate for circular button.")]
        [SerializeField]
        private Material circularBackplateHighlightMaterial;
        public Material CircularBackplateHighlightMaterial => circularBackplateHighlightMaterial;

        [Tooltip("Material for highlighting the frontplate for circular button.")]
        [SerializeField]
        private Material circularFrontplateHighlightMaterial;
        public Material CircularFrontplateHighlightMaterial => circularFrontplateHighlightMaterial;

        [Tooltip("Material for showing the outer rim geometry for circular button.")]
        [SerializeField]
        private Material circularBackplateOuterGeometryMaterial;
        public Material CircularBackplateOuterGeometryMaterial => circularBackplateOuterGeometryMaterial;

        [Tooltip("Material for showing change of state on the backplate for circular button.")]
        [SerializeField]
        private Material circularBackplateQuadMaterial;
        public Material CircularBackplateQuadMaterial => circularBackplateQuadMaterial;

        [Tooltip("Material for showing the toggle true or false state for circular button.")]
        [SerializeField]
        private Material circularBackplateToggleQuadMaterial;
        public Material CircularBackplateToggleQuadMaterial => circularBackplateToggleQuadMaterial;

        [Tooltip("Name of TextMeshPro stylesheet to use for styling text.")]
        [SerializeField]
        private string textStyleSheetName;
        public string TextStyleSheetName => textStyleSheetName;

        [Tooltip("Sprite set for selecting sprites from a key or index.")]
        [SerializeField]
        private SpriteSetTheme spriteSet;
        public SpriteSetTheme SpriteSet => spriteSet;
    }
}
