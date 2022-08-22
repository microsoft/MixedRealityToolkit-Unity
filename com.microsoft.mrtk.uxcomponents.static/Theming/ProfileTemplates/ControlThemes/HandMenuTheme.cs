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
    /// to UX.Common.xxx instead of UX.HandMenu.xxx for any
    /// element that is common across all UX elements.
    /// </summary>
    [Serializable]
    public class HandMenuTheme
    {
        [Tooltip("Backplate material for a hand menu.")]
        [SerializeField]
        private Material backplateMaterial;
        public Material BackplateMaterial => backplateMaterial;
    }
}
