// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// 
    /// </summary>
    public class ScreenReaderSettings
    {
        [SerializeField]
        [Tooltip("The distance, in meters, at which pseudo spatialized audio will be placed.")]
        private float pseudoSpatializationDistance = 3f;

        public float PseudoSpatializationDistance
        {
            get => pseudoSpatializationDistance;
            set
            {
                if (pseudoSpatializationDistance != value)
                {
                    pseudoSpatializationDistance = value;
                    // todo: notify of the change
                }
            }
        }
    }
}
