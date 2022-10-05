// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Classifications for describable objects that may appear in the scene.
    /// </summary>
    [CreateAssetMenu(fileName = "DescribableObjectClassification.asset", menuName = "MRTK/Accessibility/Describable Object Classification")]
    public class DescribableObjectClassification : ScriptableObject
    {
        /// <summary>
        /// Friendly description of the classification (ex: "Places in the world").
        /// </summary>
        [SerializeField]
        [Tooltip("Friendly description of the classification (ex: 'Places in the world').")]
        private string description;

        /// <summary>
        /// Friendly description of the classification (ex: "Locations in the world").
        /// </summary>
        public string Description { get; set; }
    }
}
