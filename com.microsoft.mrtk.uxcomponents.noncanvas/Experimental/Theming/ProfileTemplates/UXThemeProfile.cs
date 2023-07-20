// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// A theme profile for UX components.
    /// </summary>
    [CreateAssetMenu(fileName = "MRTK_UX_ThemeProfile", menuName = "MRTK/UX/Theme Profile")]
    public class UXThemeProfile : ScriptableObject
    {
        /// <summary>
        /// A theme for UX components.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The conceptual elements in left-to-right dot notation order are:
        /// </para>
        /// <list type="bullet">
        ///     <item>
        ///         <term>Global category (UX)</term>
        ///         <description>All themes in this category are for UX elements.</description>
        ///     </item>
        ///     <item>
        ///         <term>UX Control Type</term>
        ///         <description>The nature of the control. This should generally map one-to-one to a specific <c>prefab.Common</c> can be used to specify fallback properties across all UX controls.</description>
        ///     </item>
        ///     <item>
        ///         <term>UX Control Part</term>
        ///         <description>A specific sub-part of the control; for example, a thumb for a slider. For more complex controls, this could be a compound part.</description>
        ///     </item>
        ///     <item>
        ///         <term>Component category</term>
        ///         <description>The component being themed; for example, text, a material, or a sprint. In general, this should reflect the name of the component <see cref="MonoBehaviour"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>Optional Property</term>
        ///         <description>A specific property of that component; for example, color or style sheet.</description>
        ///     </item>
        /// </list>
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
        public class UXTheme
        {
            [SerializeField, Experimental]
            private CommonTheme common;

            /// <summary>
            /// Button control theme elements.
            /// </summary>
            public CommonTheme Common => common;

            [SerializeField]
            private ButtonTheme button;

            /// <summary>
            /// Button control theme elements.
            /// </summary>
            public ButtonTheme Button => button;

            [SerializeField]
            private SliderTheme slider;

            /// <summary>
            /// Slider control theme elements.
            /// </summary>
            public SliderTheme Slider => slider;

            [SerializeField]
            private DialogTheme dialog;

            /// <summary>
            /// Dialog slate theme elements.
            /// </summary>
            public DialogTheme Dialog => dialog;

            [SerializeField]
            private HandMenuTheme handMenu;

            /// <summary>
            /// Hand menu theme elements.
            /// </summary>
            public HandMenuTheme HandMenu => handMenu;

            [SerializeField]
            private ListMenuTheme listMenu;

            /// <summary>
            /// Hand menu theme elements.
            /// </summary>
            public ListMenuTheme ListMenu => listMenu;

            [SerializeField]
            private ObjectBarTheme objectBar;

            /// <summary>
            /// Object bar theme elements.
            /// </summary>
            public ObjectBarTheme ObjectBar => objectBar;

            [SerializeField]
            private SlateTheme slate;

            /// <summary>
            /// Slate theme elements.
            /// </summary>
            public SlateTheme Slate => slate;

            [SerializeField]
            private ToggleTheme toggle;

            /// <summary>
            /// Toggle theme elements.
            /// </summary>
            public ToggleTheme Toggle => toggle;
        }

        [SerializeField]
        private UXTheme ux;

        /// <summary>
        /// The root of the namespace.
        /// </summary>
        public UXTheme UX => ux;
    }
}
