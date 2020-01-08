// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Vector Statistics used in gaze stabilization.
    /// </summary>
    public class VectorRollingStatistics
    {
        /// <summary>
        /// Current standard deviation of the positions of the vectors.
        /// </summary>
        public float CurrentStandardDeviation { get; private set; }

        /// <summary>
        /// Difference to standardDeviation when the latest sample was added.
        /// </summary>
        public float StandardDeviationDeltaAfterLatestSample { get; private set; }

        /// <summary>
        /// How many standard deviations the latest sample was away.
        /// </summary>
        public float StandardDeviationsAwayOfLatestSample { get; private set; }

        /// <summary>
        /// The average position.
        /// </summary>
        public Vector3 Average { get; private set; }

        /// <summary>
        /// The number of samples in the current set (may be 0 - maxSamples)
        /// </summary>
        public float ActualSampleCount { get; private set; }

        /// <summary>
        /// Keeps track of the index into the sample list for the rolling average.
        /// </summary>
        private int currentSampleIndex;

        /// <summary>
        /// An array of samples for calculating standard deviation.
        /// </summary>
        private Vector3[] samples;

        /// <summary>
        /// The sum of all of the samples.
        /// </summary>
        private Vector3 cumulativeFrame;

        /// <summary>
        /// The sum of all of the samples squared.
        /// </summary>
        private Vector3 cumulativeFrameSquared;

        /// <summary>
        /// The total number of samples taken.
        /// </summary>
        private int cumulativeFrameSamples;

        /// <summary>
        /// The maximum number of samples to include in 
        /// the average and standard deviation calculations.
        /// </summary>
        private int maxSamples;

        /// <summary>
        /// Initialize the rolling stats.
        /// </summary>
        public void Init(int sampleCount)
        {
            maxSamples = sampleCount;
            samples = new Vector3[sampleCount];
            Reset();
        }

        /// <summary>
        /// Resets the stats to zero.
        /// </summary>
        public void Reset()
        {
            currentSampleIndex = 0;
            ActualSampleCount = 0;
            cumulativeFrame = Vector3.zero;
            cumulativeFrameSquared = Vector3.zero;
            cumulativeFrameSamples = 0;
            CurrentStandardDeviation = 0.0f;
            StandardDeviationDeltaAfterLatestSample = 0.0f;
            StandardDeviationsAwayOfLatestSample = 0.0f;
            Average = Vector3.zero;

            if (samples != null)
            {
                for (int index = 0; index < samples.Length; index++)
                {
                    samples[index] = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// Adds a new sample to the sample list and updates the stats.
        /// </summary>
        /// <param name="value">The new sample to add</param>
        public void AddSample(Vector3 value)
        {
            if (maxSamples == 0)
            {
                return;
            }

            // remove the old sample from our accumulation
            Vector3 oldSample = samples[currentSampleIndex];

            // -- Below replaces operations:
            // cumulativeFrame -= oldSample;
            // cumulativeFrameSquared -= (oldSample.Mul(oldSample));

            cumulativeFrame.x -= oldSample.x;
            cumulativeFrame.y -= oldSample.y;
            cumulativeFrame.z -= oldSample.z;

            oldSample.x *= oldSample.x;
            oldSample.y *= oldSample.y;
            oldSample.z *= oldSample.z;

            cumulativeFrameSquared.x -= oldSample.x;
            cumulativeFrameSquared.y -= oldSample.y;
            cumulativeFrameSquared.z -= oldSample.z;
            // --

            // Add the new sample to the accumulation
            samples[currentSampleIndex] = value;

            // -- Below replaces operations:
            // cumulativeFrame += value;
            // cumulativeFrameSquared += value.Mul(value);
            cumulativeFrame.x += value.x;
            cumulativeFrame.y += value.y;
            cumulativeFrame.z += value.z;

            Vector3 valueSquared = value;
            valueSquared.x = value.x * value.x;
            valueSquared.y = value.y * value.y;
            valueSquared.z = value.z * value.z;

            cumulativeFrameSquared.x += valueSquared.x;
            cumulativeFrameSquared.y += valueSquared.y;
            cumulativeFrameSquared.z += valueSquared.z;
            // --

            // Keep track of how many samples we have
            cumulativeFrameSamples++;
            ActualSampleCount = Mathf.Min(maxSamples, cumulativeFrameSamples);

            // see how many standard deviations the current sample is from the previous average
            // -- Below replaces operations:
            // Vector3 deltaFromAverage = (Average - value);
            Vector3 deltaFromAverage = Average;
            deltaFromAverage.x -= value.x;
            deltaFromAverage.y -= value.y;
            deltaFromAverage.z -= value.z;
            // --

            float oldStandardDeviation = CurrentStandardDeviation;
            // -- Below replaces operations:
            // StandardDeviationsAwayOfLatestSample = oldStandardDeviation.Equals(0) ? 0 : (deltaFromAverage / oldStandardDeviation).magnitude;
            if (oldStandardDeviation == 0)
            {
                StandardDeviationsAwayOfLatestSample = 0;
            }
            else
            {
                deltaFromAverage.x /= oldStandardDeviation;
                deltaFromAverage.y /= oldStandardDeviation;
                deltaFromAverage.z /= oldStandardDeviation;
                StandardDeviationsAwayOfLatestSample = deltaFromAverage.magnitude;
            }
            // --

            // And calculate new averages and standard deviations
            // (note that calculating a standard deviation of a Vector3 might not 
            // be done properly, but the logic is working for the gaze stabilization scenario)

            // -- Below replaces operations:
            // Average = cumulativeFrame / ActualSampleCount;
            // float newStandardDev = Mathf.Sqrt((cumulativeFrameSquared / ActualSampleCount - Average.Mul(Average)).magnitude);
            Vector3 average = Average;
            average.x = cumulativeFrame.x / ActualSampleCount;
            average.y = cumulativeFrame.y / ActualSampleCount;
            average.z = cumulativeFrame.z / ActualSampleCount;

            Average = average;

            Vector3 frmSqrDivSamples = cumulativeFrameSquared;
            frmSqrDivSamples.x /= ActualSampleCount;
            frmSqrDivSamples.y /= ActualSampleCount;
            frmSqrDivSamples.z /= ActualSampleCount;

            frmSqrDivSamples.x -= (average.x * average.x);
            frmSqrDivSamples.y -= (average.y * average.y);
            frmSqrDivSamples.z -= (average.z * average.z);

            float newStandardDev = Mathf.Sqrt(frmSqrDivSamples.magnitude);
            // --

            StandardDeviationDeltaAfterLatestSample = oldStandardDeviation - newStandardDev;
            CurrentStandardDeviation = newStandardDev;

            // update the next list position
            currentSampleIndex = (currentSampleIndex + 1) % maxSamples;
        }
    }
}
