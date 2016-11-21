// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GazeStabilizer iterates over samples of Raycast data and
    /// helps stabilize the user's gaze for precision targeting.
    /// </summary>
    public class GazeStabilizer : MonoBehaviour
    {
        [Tooltip("Number of samples that you want to iterate on.")]
        [Range(40, 120)]
        public int StoredStabilitySamples = 60;

        // Access the below public properties from the client class to consume stable values.
        public Vector3 StableHeadPosition { get; private set; }
        public Quaternion StableHeadRotation { get; private set; }
        public Ray StableHeadRay { get; private set; }

        /// <summary>
        /// These classes do the work of calculating standard deviation and averages for the gaze
        /// position and direction.
        /// </summary>
        private VectorRollingStatistics positionRollingStats = new VectorRollingStatistics();
        private VectorRollingStatistics directionRollingStats = new VectorRollingStatistics();

        /// <summary>
        /// These are the tunable parameters.
        /// </summary>
        // If the standard deviation is above these values we reset and stop stabalizing
        private const float positionStandardDeviationReset = 0.2f;
        private const float directionStandardDeviationReset = 0.1f;

        // We must have at least this many samples with a standard deviation below the above constants to stabalize
        private const int minimumSamplesRequiredToStabalize = 30;

        // When not stabalizing this is the 'lerp' applied to the position and direction of the gaze to smooth it over time.
        private const float unstabalizedLerpFactor = 0.3f;

        // When stabalizing we will use the standard deviation of the position and direction to create the lerp value.  
        // By default this value will be low and the cursor will be too sluggish, so we 'boost' it by this value.
        private const float stabalizedLerpBoost = 10.0f;

        private void Awake()
        {
            directionRollingStats.Init(StoredStabilitySamples);
            positionRollingStats.Init(StoredStabilitySamples);
        }

        /// <summary>
        /// Updates the StableHeadPosition and StableHeadRotation based on GazeSample values.
        /// Call this method with Raycasthit parameters to get stable values.
        /// </summary>
        /// <param name="position">Position value from a RaycastHit point.</param>
        /// <param name="rotation">Rotation value from a RaycastHit rotation.</param>
        public void UpdateHeadStability(Vector3 position, Quaternion rotation)
        {
            Vector3 gazePosition = position;
            Vector3 gazeDirection = rotation * Vector3.forward;

            positionRollingStats.AddSample(gazePosition);
            directionRollingStats.AddSample(gazeDirection);

            float lerpPower = unstabalizedLerpFactor;
            if (positionRollingStats.ActualSampleCount > minimumSamplesRequiredToStabalize && // we have enough samples and...
                (positionRollingStats.CurrentStandardDeviation > positionStandardDeviationReset || // the standard deviation of positions is high or...
                 directionRollingStats.CurrentStandardDeviation > directionStandardDeviationReset)) // the standard deviation of directions is high
            { 
                // We've detected that the user's gaze is no longer fixed, so stop stabalizing so that gaze is responsive.
                //Debug.LogFormat("Reset {0} {1} {2} {3}", positionRollingStats.standardDeviation, positionRollingStats.standardDeviationsAway, directionRollignStats.standardDeviation, directionRollignStats.standardDeviationsAway);
                positionRollingStats.Reset();
                directionRollingStats.Reset();
            }
            else if (positionRollingStats.ActualSampleCount > minimumSamplesRequiredToStabalize)
            {
                // We've detected that the user's gaze is fairly fixed, so start stabalizing.  The more fixed the gaze the less the cursor will move.
                lerpPower = stabalizedLerpBoost * (positionRollingStats.CurrentStandardDeviation + directionRollingStats.CurrentStandardDeviation);
            }

            StableHeadPosition = Vector3.Lerp(StableHeadPosition, gazePosition, lerpPower);
            StableHeadRotation = Quaternion.LookRotation(Vector3.Lerp(StableHeadRotation * Vector3.forward, gazeDirection, lerpPower));
            StableHeadRay = new Ray(StableHeadPosition, StableHeadRotation * Vector3.forward);
        }
    }
}