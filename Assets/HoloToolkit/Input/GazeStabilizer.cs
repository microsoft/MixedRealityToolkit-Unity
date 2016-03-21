// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
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
        [Range(1, 120)]
        public int StoredStabilitySamples = 60;

        [Tooltip("Position based distance away from gravity well.")]
        public float PositionDropOffRadius = 0.02f;

        [Tooltip("Direction based distance away from gravity well.")]
        public float DirectionDropOffRadius = 0.1f;

        [Tooltip("Position lerp interpolation factor.")]
        [Range(0.25f, 0.85f)]
        public float PositionStrength = 0.66f;

        [Tooltip("Direction lerp interpolation factor.")]
        [Range(0.25f, 0.85f)]
        public float DirectionStrength = 0.83f;

        [Tooltip("Stability average weight multiplier factor.")]
        public float StabilityAverageDistanceWeight = 2.0f;

        [Tooltip("Stability variance weight multiplier factor.")]
        public float StabilityVarianceWeight = 1.0f;

        // Access the below public properties from the client class to consume stable values.
        public Vector3 StableHeadPosition { get; private set; }
        public Quaternion StableHeadRotation { get; private set; }
        public Ray StableHeadRay { get; private set; }

        public struct GazeSample
        {
            public Vector3 position;
            public Vector3 direction;
            public float timestamp;
        };

        private Queue<GazeSample> stabilitySamples = new Queue<GazeSample>();

        private Vector3 gazePosition;
        private Vector3 gazeDirection;

        // Most recent calculated instability values.
        private float gazePositionInstability;
        private float gazeDirectionInstability;

        private bool gravityPointExists = false;
        private Vector3 gravityWellPosition;
        private Vector3 gravityWellDirection;

        // Transforms instability value into a modified drop off distance, modify with caution.
        private const float positionDestabilizationFactor = 0.02f;
        private const float directionDestabilizationFactor = 0.3f;

        /// <summary>
        /// Updates the StableHeadPosition and StableHeadRotation based on GazeSample values.
        /// Call this method with Raycasthit parameters to get stable values.
        /// </summary>
        /// <param name="position">Position value from a Raycasthit point.</param>
        /// <param name="rotation">Roration value from a Raycasthit rotation.</param>
        public void UpdateHeadStability(Vector3 position, Quaternion rotation)
        {
            gazePosition = position;
            gazeDirection = rotation * Vector3.forward;

            AddGazeSample(gazePosition, gazeDirection);

            UpdateInstability(out gazePositionInstability, out gazeDirectionInstability);

            // If we don't have a gravity point, just use the gaze position.
            if (!gravityPointExists)
            {
                gravityWellPosition = gazePosition;
                gravityWellDirection = gazeDirection;
                gravityPointExists = true;
            }

            UpdateGravityWellPositionDirection();
        }

        private void AddGazeSample(Vector3 positionSample, Vector3 directionSample)
        {
            // Record and save sample data.
            GazeSample newStabilitySample;
            newStabilitySample.position = positionSample;
            newStabilitySample.direction = directionSample;
            newStabilitySample.timestamp = Time.time;

            if (stabilitySamples != null)
            {
                // Remove from front if we exceed stored samples.
                if (stabilitySamples.Count >= StoredStabilitySamples)
                {
                    stabilitySamples.Dequeue();
                }

                stabilitySamples.Enqueue(newStabilitySample);
            }
        }

        private void UpdateInstability(out float positionInstability, out float directionInstability)
        {
            GazeSample mostRecentSample;

            float positionDeltaMin = 0.0f;
            float positionDeltaMax = 0.0f;
            float positionDeltaMean = 0.0f;

            float directionDeltaMin = 0.0f;
            float directionDeltaMax = 0.0f;
            float directionDeltaMean = 0.0f;

            float positionDelta = 0.0f;
            float directionDelta = 0.0f;

            positionInstability = 0.0f;
            directionInstability = 0.0f;

            // If we have zero or one sample, there is no instability to report.
            if (stabilitySamples.Count < 2)
            {
                return;
            }

            mostRecentSample = stabilitySamples.ElementAt(stabilitySamples.Count - 1);

            // All but most recent.
            for (int i = 0; i < stabilitySamples.Count - 1; ++i)
            {
                // Calculate difference between current sample and most recent sample.
                positionDelta = Vector3.Magnitude(stabilitySamples.ElementAt(i).position - mostRecentSample.position);

                directionDelta = Vector3.Angle(stabilitySamples.ElementAt(i).direction, mostRecentSample.direction) * Mathf.Deg2Rad;

                // Initialize max and min on first sample.
                if (i == 0)
                {
                    positionDeltaMin = positionDelta;
                    positionDeltaMax = positionDelta;
                    directionDeltaMin = directionDelta;
                    directionDeltaMax = directionDelta;
                }
                else
                {
                    // Update maximum, minimum and mean differences from most recent sample.
                    positionDeltaMin = Mathf.Min(positionDelta, positionDeltaMin);
                    positionDeltaMax = Mathf.Max(positionDelta, positionDeltaMax);

                    directionDeltaMin = Mathf.Min(directionDelta, directionDeltaMin);
                    directionDeltaMax = Mathf.Max(directionDelta, directionDeltaMax);
                }

                positionDeltaMean += positionDelta;
                directionDeltaMean += directionDelta;
            }

            positionDeltaMean = positionDeltaMean / (stabilitySamples.Count - 1);
            directionDeltaMean = directionDeltaMean / (stabilitySamples.Count - 1);

            // Calculate stability value for Gaze position and direction.  Note that stability values will be significantly different for position and
            // direction since the position value is based on values in meters while the direction stability is based on data in radians.
            positionInstability = StabilityVarianceWeight * (positionDeltaMax - positionDeltaMin) + StabilityAverageDistanceWeight * positionDeltaMean;
            directionInstability = StabilityVarianceWeight * (directionDeltaMax - directionDeltaMin) + StabilityAverageDistanceWeight * directionDeltaMean;
        }

        private void UpdateGravityWellPositionDirection()
        {
            float stabilityModifiedPositionDropOffDistance;
            float stabilityModifiedDirectionDropOffDistance;
            float normalizedGazeToGravityWellPosition;
            float normalizedGazeToGravityWellDirection;

            // Modify effective size of well based on gaze stability.
            stabilityModifiedPositionDropOffDistance = Mathf.Max(0.0f, PositionDropOffRadius - (gazePositionInstability * positionDestabilizationFactor));
            stabilityModifiedDirectionDropOffDistance = Mathf.Max(0.0f, DirectionDropOffRadius - (gazeDirectionInstability * directionDestabilizationFactor));

            // Determine how far away from the well the gaze is, if that distance is zero push the normalized value above 1.0 to
            // force a gravity well position update.
            normalizedGazeToGravityWellPosition = 2.0f;
            if (stabilityModifiedPositionDropOffDistance > 0.0f)
            {
                normalizedGazeToGravityWellPosition = Vector3.Magnitude(gravityWellPosition - gazePosition) / stabilityModifiedPositionDropOffDistance;
            }

            normalizedGazeToGravityWellDirection = 2.0f;
            if (stabilityModifiedDirectionDropOffDistance > 0.0f)
            {
                normalizedGazeToGravityWellDirection = Mathf.Acos(Vector3.Dot(gravityWellDirection, gazeDirection)) / stabilityModifiedDirectionDropOffDistance;
            }

            // Move gravity well with Gaze if necessary.
            if (normalizedGazeToGravityWellPosition > 1.0f)
            {
                gravityWellPosition = gazePosition - Vector3.Normalize(gazePosition - gravityWellPosition) * stabilityModifiedPositionDropOffDistance;
            }

            if (normalizedGazeToGravityWellDirection > 1.0f)
            {
                gravityWellDirection = Vector3.Normalize(gazeDirection - Vector3.Normalize(gazeDirection - gravityWellDirection) * stabilityModifiedDirectionDropOffDistance);
            }

            // Adjust direction and position towards gravity well based on configurable strengths.
            StableHeadPosition = Vector3.Lerp(gazePosition, gravityWellPosition, PositionStrength);
            StableHeadRotation = Quaternion.LookRotation(Vector3.Lerp(gazeDirection, gravityWellDirection, DirectionStrength));
            StableHeadRay = new Ray(StableHeadPosition, StableHeadRotation * Vector3.forward);
        }
    }
}