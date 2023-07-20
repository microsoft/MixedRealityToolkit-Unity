// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// Common MRTK control theme elements.
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    [Serializable]
    public class CommonTheme
    {
        [SerializeField]
        private TMP_StyleSheet textStyleSheet;

        /// <summary>
        /// The text style sheet.
        /// </summary>
        public TMP_StyleSheet TextStyleSheet => textStyleSheet;

        [SerializeField]
        private SpriteSetTheme spriteSet;

        /// <summary>
        /// The sprint set theme.
        /// </summary>
        public SpriteSetTheme SpriteSet => spriteSet;
    }
}
