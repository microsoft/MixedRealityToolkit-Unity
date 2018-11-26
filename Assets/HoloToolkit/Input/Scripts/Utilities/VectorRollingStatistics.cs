// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Vector Statistics used in gaze stabilization.
    /// </summary>
    public class VectorRollingStatistics
    {
        /// <summary>
        /// Current standard deviation of the positions of the vectors
        /// </summary>
        public float CurrentStandardDeviation;

        /// <summary>
        /// Difference to standardDeviation when the latest sample was added.
        /// </summary>
        public float StandardDeviationDeltaAfterLatestSample;

        /// <summary>
        /// How many standard deviations the latest sample was away.
        /// </summary>
        public float StandardDeviationsAwayOfLatestSample;

        /// <summary>
        /// The average position.
        /// </summary>
        public Vector3 Average;

        /// <summary>
        /// The number of samples in the current set (may be 0 - maxSamples)
        /// </summary>
        public float ActualSampleCount;

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
        /// <param name="sampleCount"></param>
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
            cumulativeFrame -= oldSample;
            cumulativeFrameSquared -= (oldSample.Mul(oldSample));

            // Add the new sample to the accumulation
            samples[currentSampleIndex] = value;
            cumulativeFrame += value;
            cumulativeFrameSquared += value.Mul(value);

            // Keep track of how many samples we have
            cumulativeFrameSamples++;
            ActualSampleCount = Mathf.Min(maxSamples, cumulativeFrameSamples);

            // see how many standard deviations the current sample is from the previous average
            Vector3 deltaFromAverage = (Average - value);
            float oldStandardDeviation = CurrentStandardDeviation;
            StandardDeviationsAwayOfLatestSample = oldStandardDeviation == 0 ? 0 : (deltaFromAverage / oldStandardDeviation).magnitude;

            // And calculate new averages and standard deviations
            // (note that calculating a standard deviation of a Vector3 might not 
            // be done properly, but the logic is working for the gaze stabilization scenario)
            Average = cumulativeFrame / ActualSampleCount;
            float newStandardDev = Mathf.Sqrt((cumulativeFrameSquared / ActualSampleCount - Average.Mul(Average)).magnitude);
            StandardDeviationDeltaAfterLatestSample = oldStandardDeviation - newStandardDev;
            CurrentStandardDeviation = newStandardDev;

            // update the next list position
            currentSampleIndex = (currentSampleIndex + 1) % maxSamples;
        }
    }
}
