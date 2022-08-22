// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// All themable elements of a MRTK UX Dialog slate.
    ///
    /// Note that by default the UXBindingProfile only maps
    /// to UX.Common.xxx instead of UX.Toggle.xxx for any
    /// element that is common across all UX elements.
    /// </summary>
    [Serializable]
    public class ToggleTheme
    {
        [Tooltip("Backplate material for a toggleable control.")]
        [SerializeField]
        private Material backplateMaterial;
        public Material BackplateMaterial => backplateMaterial;

        [Tooltip("Backplate material for a toggleable switch control.")]
        [SerializeField]
        private Material thumbMaterial;
        public Material ThumbMaterial => thumbMaterial;

        [Tooltip("A sprite to function as the shadow for the toggleable switch control.")]
        [SerializeField]
        private Sprite thumbShadowSprite;
        public Sprite ThumbShadowSprite => thumbShadowSprite;
    }
}
