// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Represents a tab section, utilized in Tab View. 
    /// </summary>
    [Serializable]
    public class TabSection
    {
        [SerializeField]
        [Tooltip("The name of the section. ")]
        private string sectionName;

        /// <summary>
        /// The name of the section. 
        /// </summary>
        public string SectionName
        {
            get => sectionName;
            set => sectionName = value;
        }

        [SerializeField]
        [Tooltip("The root of the tab. Tab view will toggle the visibility of this root game object.")]
        private GameObject sectionVisibleRoot;

        /// <summary>
        /// The root of the tab. Tab view will toggle the visibility of this root game object.
        /// </summary>
        public GameObject SectionVisibleRoot
        {
            get => sectionVisibleRoot;
            set => sectionVisibleRoot = value;
        }
    }
}
