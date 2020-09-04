// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// GazeStabilizer iterates over samples of Raycast data and
    /// helps stabilize the user's gaze for precision targeting.
    /// </summary>
    [Serializable]
    public class GazeStabilizer : BaseRayStabilizer
    {
        /// <summary>
        ///Number of samples that you want to iterate on.
        /// </summary>
        public int StoredStabilitySamples => storedStabilitySamples;

        [SerializeField]
        [Range(40, 120)]
        [Tooltip("Number of samples that you want to iterate on.")]
        private int storedStabilitySamples = 60;

        /// <summary>
        /// The stabilized position.
        /// </summary>
        public override Vector3 StablePosition => stablePosition;
        private Vector3 stablePosition;

        /// <summary>
        /// The stabilized rotation.
        /// </summary>
        public override Quaternion StableRotation => stableRotation;
        private Quaternion stableRotation;

        /// <summary>
        /// The stabilized position.
        /// </summary>
        public override Ray StableRay => stableRay;
        private Ray stableRay;

        /// <summary>
        /// Calculates standard deviation and averages for the gaze position.
        /// </summary>
        private readonly VectorRollingStatistics positionRollingStats = new VectorRollingStatistics();

        /// <summary>
        /// Calculates standard deviation and averages for the gaze direction.
        /// </summary>
        private readonly VectorRollingStatistics directionRollingStats = new VectorRollingStatistics();

        /// <summary>
        /// Tunable parameter.
        /// If the standard deviation for the position is above this value, we reset and stop stabilizing.
        /// </summary>
        private const float PositionStandardDeviationReset = 0.2f;

        /// <summary>
        /// Tunable parameter.
        /// If the standard deviation for the direction is above this value, we reset and stop stabilizing.
        /// </summary>
        private const float DirectionStandardDeviationReset = 0.1f;

        /// <summary>
        /// We must have at least this many samples with a standard deviation below the above constants to stabilize
        /// </summary>
        private const int MinimumSamplesRequiredToStabilize = 30;

        /// <summary>
        /// When not stabilizing this is the 'lerp' applied to the position and direction of the gaze to smooth it over time.
        /// </summary>
        private const float UnstabilizedLerpFactor = 0.3f;

        /// <summary>
        /// When stabilizing we will use the standard deviation of the position and direction to create the lerp value.
        /// By default this value will be low and the cursor will be too sluggish, so we 'boost' it by this value.
        /// </summary>
        private const float StabalizedLerpBoost = 10.0f;

        public GazeStabilizer()
        {
            directionRollingStats.Init(storedStabilitySamples);
            positionRollingStats.Init(storedStabilitySamples);
        }

        /// <summary>
        /// Updates the StablePosition and StableRotation based on GazeSample values.
        /// Call this method with RaycastHit parameters to get stable values.
        /// </summary>
        /// <param name="gazePosition">Position value from a RaycastHit point.</param>
        /// <param name="gazeDirection">Direction value from a RaycastHit rotation.</param>
        public override void UpdateStability(Vector3 gazePosition, Vector3 gazeDirection)
        {
            positionRollingStats.AddSample(gazePosition);
            directionRollingStats.AddSample(gazeDirection);

            float lerpPower = UnstabilizedLerpFactor;

            if (positionRollingStats.ActualSampleCount > MinimumSamplesRequiredToStabilize &&      // we have enough samples and...
               (positionRollingStats.CurrentStandardDeviation > PositionStandardDeviationReset ||  // the standard deviation of positions is high or...
                directionRollingStats.CurrentStandardDeviation > DirectionStandardDeviationReset)) // the standard deviation of directions is high
            {
                // We've detected that the user's gaze is no longer fixed, so stop stabilizing so that gaze is responsive.
                // Debug.Log($"Reset {positionRollingStats.CurrentStandardDeviation} {positionRollingStats.StandardDeviationsAwayOfLatestSample} {directionRollingStats.CurrentStandardDeviation} {directionRollingStats.StandardDeviationsAwayOfLatestSample}");
                positionRollingStats.Reset();
                directionRollingStats.Reset();
            }
            else if (positionRollingStats.ActualSampleCount > MinimumSamplesRequiredToStabilize)
            {
                // We've detected that the user's gaze is fairly fixed, so start stabilizing.  The more fixed the gaze the less the cursor will move.
                lerpPower = StabalizedLerpBoost * (positionRollingStats.CurrentStandardDeviation + directionRollingStats.CurrentStandardDeviation);
            }

            stablePosition = Vector3.Lerp(stablePosition, gazePosition, lerpPower);
            stableRotation = Quaternion.LookRotation(Vector3.Lerp(stableRotation * Vector3.forward, gazeDirection, lerpPower));
            stableRay = new Ray(stablePosition, stableRotation * Vector3.forward);
        }
    }
}