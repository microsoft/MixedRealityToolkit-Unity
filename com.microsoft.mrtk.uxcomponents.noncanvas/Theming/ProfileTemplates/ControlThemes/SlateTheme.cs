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
    /// to UX.Common.xxx instead of UX.Slate.xxx for any
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
    public class SlateTheme
    {
        [Tooltip("Backplate material for the title section of slate.")]
        [SerializeField]
        private Material titleBackplateMaterial;
        public Material TitleBackplateMaterial => titleBackplateMaterial;

        [Tooltip("Backplate material for the content section of a slate.")]
        [SerializeField]
        private Material contentBackplateMaterial;
        public Material ContentBackplateMaterial => contentBackplateMaterial;

        [Tooltip("Backplate material for simple slates.")]
        [SerializeField]
        private Material innerQuadBackplateMaterial;
        public Material InnerQuadBackplateMaterial => innerQuadBackplateMaterial;
    }
}
