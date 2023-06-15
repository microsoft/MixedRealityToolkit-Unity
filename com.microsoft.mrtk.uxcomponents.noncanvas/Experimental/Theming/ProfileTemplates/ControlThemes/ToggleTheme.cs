// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// All themable elements of a MRTK UX Dialog slate.
    ///
    /// Note that by default the UXBindingProfile only maps
    /// to UX.Common.xxx instead of UX.Toggle.xxx for any
    /// element that is common across all UX elements.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven�t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
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
