// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// All themable elements of a MRTK UX Slider.
    ///
    /// Note that by default the UXBindingProfile only maps
    /// to UX.Common.xxx instead of UX.Button.xxx for any
    /// element that is common across all UX elements.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
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
