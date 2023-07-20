// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// All theme elements of a MRTK UX <c>ActionButton</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A MRTK <c>ActionButton</c> object can be added to the an object via the MRTK
    /// game object menu item in Unity.
    /// </para>
    /// <para>
    /// Note, that by default whenever a theme element
    /// is shared by different control types, the same
    /// element is repeated in <c>UX.Common</c> and the default
    /// theme for that element is pointed to <c>UX.Common.xxx</c>
    /// instead of <c>UX.Button.xxx</c>.
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
    public class ButtonTheme
    {
        [Tooltip("Material for highlighting the backplate.")]
        [SerializeField]
        private Material backplateHighlightMaterial;

        /// <summary>
        /// Material for highlighting the backplate.
        /// </summary>
        public Material BackplateHighlightMaterial => backplateHighlightMaterial;

        [Tooltip("Material for highlighting the front plate.")]
        [SerializeField]
        private Material frontplateHighlightMaterial;

        /// <summary>
        /// Material for highlighting the front plate.
        /// </summary>
        public Material FrontplateHighlightMaterial => frontplateHighlightMaterial;

        [Tooltip("Material for 'See It. Say It' backplate.")]
        [SerializeField]
        private Material seeItSayItBackplateQuadMaterial;

        /// <summary>
        /// Material for 'See It. Say It' backplate.
        /// </summary>
        public Material SeeItSayItBackplateQuadMaterial => seeItSayItBackplateQuadMaterial;

        [Tooltip("Material for showing the outer rim geometry.")]
        [SerializeField]
        private Material backplateOuterGeometryMaterial;

        /// <summary>
        /// Material for showing the outer rim geometry.
        /// </summary>
        public Material BackplateOuterGeometryMaterial => backplateOuterGeometryMaterial;

        [Tooltip("Material for showing change of state on the backplate.")]
        [SerializeField]
        private Material backplateQuadMaterial;
        
        /// <summary>
        /// Material for showing change of state on the backplate.
        /// </summary>
        public Material BackplateQuadMaterial => backplateQuadMaterial;

        [Tooltip("Material for showing the toggle true or false state.")]
        [SerializeField]
        private Material backplateToggleQuadMaterial;
        
        /// <summary>
        /// Material for showing the toggle true or false state.
        /// </summary>
        public Material BackplateToggleQuadMaterial => backplateToggleQuadMaterial;

        [Tooltip("Material for highlighting the backplate for circular button.")]
        [SerializeField]
        private Material circularBackplateHighlightMaterial;
        
        /// <summary>
        /// Material for highlighting the backplate for circular button.
        /// </summary>
        public Material CircularBackplateHighlightMaterial => circularBackplateHighlightMaterial;

        [Tooltip("Material for highlighting the front plate for circular button.")]
        [SerializeField]
        private Material circularFrontplateHighlightMaterial;
        
        /// <summary>
        /// Material for highlighting the front plate for circular button.
        /// </summary>
        public Material CircularFrontplateHighlightMaterial => circularFrontplateHighlightMaterial;

        [Tooltip("Material for showing the outer rim geometry for circular button.")]
        [SerializeField]
        private Material circularBackplateOuterGeometryMaterial;
        
        /// <summary>
        /// Material for showing the outer rim geometry for circular button.
        /// </summary>
        public Material CircularBackplateOuterGeometryMaterial => circularBackplateOuterGeometryMaterial;

        [Tooltip("Material for showing change of state on the backplate for circular button.")]
        [SerializeField]
        private Material circularBackplateQuadMaterial;
        
        /// <summary>
        /// Material for showing change of state on the backplate for circular button.
        /// </summary>
        public Material CircularBackplateQuadMaterial => circularBackplateQuadMaterial;

        [Tooltip("Material for showing the toggle true or false state for circular button.")]
        [SerializeField]
        private Material circularBackplateToggleQuadMaterial;
        
        /// <summary>
        /// Material for showing the toggle true or false state for circular button.
        /// </summary>
        public Material CircularBackplateToggleQuadMaterial => circularBackplateToggleQuadMaterial;

        [Tooltip("Name of TextMeshPro style sheet to use for styling text.")]
        [SerializeField]
        private string textStyleSheetName;
        
        /// <summary>
        /// Name of TextMeshPro style sheet to use for styling text
        /// </summary>
        public string TextStyleSheetName => textStyleSheetName;

        [Tooltip("Sprite set for selecting sprites from a key or index.")]
        [SerializeField]
        private SpriteSetTheme spriteSet;
        
        /// <summary>
        /// Sprite set for selecting sprites from a key or index.
        /// </summary>
        public SpriteSetTheme SpriteSet => spriteSet;
    }
}
