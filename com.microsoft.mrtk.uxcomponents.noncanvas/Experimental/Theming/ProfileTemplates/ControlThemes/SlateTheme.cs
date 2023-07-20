// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// All theme elements of a MRTK UX <c>Slate</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that by default the <see cref="UXBindingProfileTemplate"/> only maps
    /// to <c>UX.Common.xxx</c> instead of <c>UX.Slate.xxx</c> for any
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
    public class SlateTheme
    {
        [Tooltip("Backplate material for the title section of slate.")]
        [SerializeField]
        private Material titleBackplateMaterial;

        /// <summary>
        /// Backplate material for the title section of slate.
        /// </summary>
        public Material TitleBackplateMaterial => titleBackplateMaterial;

        [Tooltip("Backplate material for the content section of a slate.")]
        [SerializeField]
        private Material contentBackplateMaterial;
        
        /// <summary>
        /// Backplate material for the content section of a slate.
        /// </summary>
        public Material ContentBackplateMaterial => contentBackplateMaterial;

        [Tooltip("Backplate material for simple slates.")]
        [SerializeField]
        private Material innerQuadBackplateMaterial;

        /// <summary>
        /// Backplate material for simple slates.
        /// </summary>
        public Material InnerQuadBackplateMaterial => innerQuadBackplateMaterial;
    }
}
