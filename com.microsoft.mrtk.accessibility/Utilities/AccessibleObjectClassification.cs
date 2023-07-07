// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Classification for accessible objects that may appear in the scene.
    /// </summary>
    [CreateAssetMenu(
        fileName = "AccessibleObjectClassification.asset",
        menuName = "MRTK/Accessibility/Accessible Object Classification")]
    public class AccessibleObjectClassification : ScriptableObject
    {
        [SerializeField, Experimental]
        [Tooltip("Friendly description of the classification.")]
        private string description;

        /// <summary>
        /// Friendly description of the classification (ex: "Locations in the world").
        /// </summary>
        public string Description { get; set; }
    }
}
