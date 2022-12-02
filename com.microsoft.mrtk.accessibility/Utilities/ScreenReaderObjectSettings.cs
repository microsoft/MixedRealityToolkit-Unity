// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Settings that configure how the screen reader discovers accessible objects and renders the audio.
    /// </summary>
    public class ScreenReaderObjectSettings
    {
        [SerializeField]
        [Tooltip("The maximum distance from the user for which objects will be read.")]
        private float maxObjectDistance = float.PositiveInfinity;

        /// <summary>
        /// The maximum distance from the user for which objects will be read.
        /// </summary>
        public float MaxObjectDistance
        {
            get => maxObjectDistance;
            set => maxObjectDistance = value;
        }


        [SerializeField]
        [Tooltip("How the screen reader should spatialize it's output for objects.")]
        private ScreenReaderSpatializationOption spatializationOption = ScreenReaderSpatializationOption.Full;

        /// <summary>
        /// How the screen reader should spatialize it's output for objects.
        /// </summary>
        public ScreenReaderSpatializationOption SpatializationOption
        {
            get => spatializationOption;
            set => spatializationOption = value;
        }
    }
}
