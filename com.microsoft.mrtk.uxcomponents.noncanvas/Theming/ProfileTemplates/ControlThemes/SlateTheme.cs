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
