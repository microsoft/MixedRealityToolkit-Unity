// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// All theme elements of a MRTK UX <c>Toggle</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A MRTK <c>Toggle</c> component can be added to the an object via the MRTK
    /// menu items within Unity's add component menu.
    /// </para>
    /// <para>
    /// Note that by default the <see cref="UXBindingProfileTemplate"/> only maps
    /// to <c>UX.Common.xxx</c> instead of <c>UX.Toggle.xxx</c> for any
    /// element that is common across all UX elements.
    /// </para>
    /// <para>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </para>
    /// </remarks>
    [Serializable]
    public class ToggleTheme
    {
        [Tooltip("Backplate material for a toggle control.")]
        [SerializeField]
        private Material backplateMaterial;

        /// <summary>
        /// Backplate material for a toggle control.
        /// </summary>
        public Material BackplateMaterial => backplateMaterial;

        [Tooltip("Backplate material for a toggle switch control.")]
        [SerializeField]
        private Material thumbMaterial;
        
        /// <summary>
        /// Backplate material for a toggle switch control.
        /// </summary>
        public Material ThumbMaterial => thumbMaterial;

        [Tooltip("A sprite to function as the shadow for the toggle switch control.")]
        [SerializeField]
        private Sprite thumbShadowSprite;
        
        /// <summary>
        /// A sprite to function as the shadow for the toggle switch control.
        /// </summary>
        public Sprite ThumbShadowSprite => thumbShadowSprite;
    }
}
