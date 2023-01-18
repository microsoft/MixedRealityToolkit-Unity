// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(
        fileName = "MRTKHandsAggregatorConfig.asset",
        menuName = "MRTK/Subsystems/MRTK Hands Aggregator Config")]
    /// <summary>
    /// Configuration for the MRTK-specific hands aggregator.
    /// </summary>
    public class MRTKHandsAggregatorConfig : BaseSubsystemConfig
    {
        [SerializeField]
        [Tooltip("The normalized thumb-forefinger distance at which the pinch is considered 'un-pinched'. " +
                 "Normalized to the length of the user's index finger.")]
        protected float pinchOpenThreshold;

        /// <summary>
        /// The normalized thumb-forefinger distance at which the pinch is considered 'un-pinched'.
        /// Normalized to the length of the user's index finger.
        /// </summary>
        public float PinchOpenThreshold => pinchOpenThreshold;

        [SerializeField]
        [Tooltip("The normalized thumb-forefinger distance at which the pinch is considered 'pinched'. " +
                 "Normalized to the length of the user's index finger.")]
        protected float pinchClosedThreshold;

        /// <summary>
        /// The normalized thumb-forefinger distance at which the pinch is considered 'pinched'.
        /// Normalized to the length of the user's index finger.
        /// </summary>
        public float PinchClosedThreshold => pinchClosedThreshold;

        [SerializeField]
        [Tooltip("The FOV in which the hand must be located to be considered valid for a selection. " +
                 "Measured in degrees off-axis from user's head vector.")]
        protected float handRaiseCameraFov;

        /// <summary>
        /// The FOV in which the hand must be located to be considered valid for a selection.
        /// Measured in degrees off-axis from user's head vector.
        /// </summary>
        public float HandRaiseCameraFov => handRaiseCameraFov;

        [SerializeField, LabelWidth(170)]
        [Tooltip("Degrees of rotation away from the user's head's forward vector for the hand to be considered raised/valid.")]
        protected float handFacingAwayToleranceInDegrees;

        /// <summary>
        /// Degrees of rotation away from the user's head's forward vector for the hand to be considered raised/valid.
        /// </summary>
        public float HandFacingAwayToleranceInDegrees => handFacingAwayToleranceInDegrees;

        /// <summary> Resets config values to a reasonable default. </summary>
        protected virtual void Reset()
        {
            pinchOpenThreshold = 0.75f;
            pinchClosedThreshold = 0.25f;
            handRaiseCameraFov = 60.0f;
            handFacingAwayToleranceInDegrees = 75f;
        }
    }
}
