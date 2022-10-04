// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// All themable elements of a MRTK UX Slider.
    ///
    /// Note that by default the UXBindingProfile only maps
    /// to UX.Common.xxx instead of UX.Button.xxx for any
    /// element that is common across all UX elements.
    /// </summary>
    [Serializable]
    public class SliderTheme
    {
        [Tooltip("Material for the entire base of a slider.")]
        [SerializeField]
        private Material trackBaseVisualMaterial;
        public Material TrackBaseVisualMaterial => trackBaseVisualMaterial;

        [Tooltip("Material for highlighting the filled portion of the slider.")]
        [SerializeField]
        private Material trackFilledVisualMaterial;
        public Material TrackFilledVisualMaterial => trackFilledVisualMaterial;

        [Tooltip("Material for the visualization of a slider thumb.")]
        [SerializeField]
        private Material thumbVisualMaterial;
        public Material ThumbVisualMaterial => thumbVisualMaterial;

        [Tooltip("Backplate material for the entire slider.")]
        [SerializeField]
        private Material backplateMaterial;
        public Material BackplateMaterial => backplateMaterial;

    }
}
