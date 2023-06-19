// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    [CreateAssetMenu(fileName = "MRTK_UX_ThemeProfile", menuName = "MRTK/UX/Theme Profile")]
    public class UXThemeProfile : ScriptableObject
    {
        /// <summary>
        /// The conceptual elements in left-to-right dot notation order are:
        /// - Global category(UX)� All themes in this category are for UX elements
        /// - UX Control Type � The nature of the control(eg.Button, Slider, Checkbox, Slate, Common). This should generally map 1-to-1 to a specific prefab.Common can be used to specify fallback properties across all UX controls.
        /// - UX Control Part � A specific sub-part of the control (eg.Thumb for a slider). For more complex controls, this could be a compound part such as ScaleTickMarks
        /// - Component category � The component being themed(eg.Text, Material, Sprite). In general, this should reflect the name of the component MonoBehavior.
        /// - Optional Property � A specific property of that component (eg.Color, StyleSheet

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
        public class UXTheme
        {
            /// <summary>
            /// Button control themable elements
            /// </summary>
            [SerializeField, Experimental]
            private CommonTheme common;
            public CommonTheme Common => common;
            /// <summary>
            /// Button control themable elements
            /// </summary>
            [SerializeField]
            private ButtonTheme button;
            public ButtonTheme Button => button;

            /// <summary>
            /// Slider control themable elements
            /// </summary>
            [SerializeField]
            private SliderTheme slider;
            public SliderTheme Slider => slider;

            /// <summary>
            /// Dialog slate themable elements
            /// </summary>
            [SerializeField]
            private DialogTheme dialog;
            public DialogTheme Dialog => dialog;

            /// <summary>
            /// Handmenu themable elements
            /// </summary>
            [SerializeField]
            private HandMenuTheme handMenu;
            public HandMenuTheme HandMenu => handMenu;

            /// <summary>
            /// Handmenu themable elements
            /// </summary>
            [SerializeField]
            private ListMenuTheme listMenu;
            public ListMenuTheme ListMenu => listMenu;

            [SerializeField]
            private ObjectBarTheme objectBar;
            public ObjectBarTheme ObjectBar => objectBar;

            [SerializeField]
            private SlateTheme slate;
            public SlateTheme Slate => slate;

            [SerializeField]
            private ToggleTheme toggle;
            public ToggleTheme Toggle => toggle;
        }

        [SerializeField]
        private UXTheme ux;          // Establishes the root of the namespace.
        public UXTheme UX => ux;
    }
}
