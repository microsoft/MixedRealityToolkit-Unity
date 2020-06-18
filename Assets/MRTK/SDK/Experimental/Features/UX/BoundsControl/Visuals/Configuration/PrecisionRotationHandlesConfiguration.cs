// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEngine;
using UnityEngine.Events;


namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    [CreateAssetMenu(fileName = "PrecisionRotationHandlesConfiguration", menuName = "Mixed Reality Toolkit/Experimental/Bounds Control/Precision Rotation Handles Configuration")]
    public class PrecisionRotationHandlesConfiguration : RotationHandlesConfiguration
    {
        [SerializeField]
        [Tooltip("Prefab used to display this type of bounds control handle. If not set, default shape will be used (scale default: boxes, rotation default: spheres)")]
        GameObject precisionWidgetPrefab = null;

        /// <summary>
        /// An additional widget for enhancing manipulation precision.
        /// </summary>
        public GameObject PrecisionWidgetPrefab
        {
            get { return precisionWidgetPrefab; }
            set
            {
                if (precisionWidgetPrefab != value)
                {
                    precisionWidgetPrefab = value;
                }
            }
        }

        /// <summary>
        /// Fabricates an instance of PrecisionRotationHandles, applying
        /// this config to it whilst creating it.
        /// </summary>
        /// <returns>New PrecisionRotationHandles</returns>
        internal override RotationHandles ConstructInstance()
        {
            // Return a new PrecisionRotationHandles, using this config as the active config.
            return new PrecisionRotationHandles(this);
        }
    }
}


