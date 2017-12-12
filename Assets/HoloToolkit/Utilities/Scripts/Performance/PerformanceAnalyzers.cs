using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public abstract class AdaptiveAnalyzer
    {
        public enum AnalyzerType
        {
            FrameRateBased,
            GpuTimeBased
        }

        public enum AnalyzerResult
        {
            BucketUp,
            BucketDown,
            NoChange
        }

        public virtual void OnCreate() { }
        public virtual void OnDestroy() { }

        public abstract AnalyzerResult AdaptPerformance();
    }

    /// <summary>
    /// Performance analyzer using the running frame rate of the title
    /// </summary>
    public class FrameRateAdaptiveAnalyzer : AdaptiveAnalyzer
    {
        /// <summary>
        /// Calculate average FPS/frame time over the given sample interval
        /// </summary>
        private const float sampleInterval = 0.5f; //(s)

        /// <summary>
        /// Perform adaptive analysis at the given interval .
        /// </summary>
        private const float adaptiveAnalysisInterval = 60.0f; // (s)

        /// <summary>
        /// If the amount of samples meeting the target framerate is below this threshold
        /// we switch to a lower quality bucket
        /// </summary>
        private const float switchToLowerQualityThreshold = 0.8f;

        /// <summary>
        /// If the amount of samples meeting the target framerate is above this threshold
        /// for the number of periods given by switchToHigherQualityThreshold we can
        /// move a bucket higher
        /// </summary>
        private const float switchToHigherQualityThreshold = 0.99f;

        /// <summary>
        /// This number shows the amount of periods we need to have consistently high FPS
        /// at the target before we attempt to use a higher quality bucket
        /// </summary>
        private const int analysisPeriodsBeforeBucketUp = 3;

        private LinkedList<float> timeSamples = new LinkedList<float>();

        private float sampleTimeAccumulator = 0;
        private float analysisTimeAccumulator = 0;
        private int numFramesThisSample = 0;

        private bool allowMovingToHigherBucket = true;
        private int numAnalysisPeriodsWithGoodFrameRate = 0;
        private int targetFps = 0;

        public FrameRateAdaptiveAnalyzer()
        {
            targetFps = PerformanceCounters.Instance.TargetFrameRate;
        }

        bool IsPerformanceMeetingThreshold(float threshold, int tolerance)
        {
            int numBelowThreshold = 0;
            int numAtThreshold = 0;
            foreach (var sample in timeSamples)
            {
                var fps = 1 / sample;
                // OK if we're not meeting the bar by a couple of frames
                if (fps < (targetFps - tolerance))
                {
                    numBelowThreshold += 1;
                }
                else
                {
                    numAtThreshold += 1;
                }
            }

            var percentSamplesMeetingFPS = numAtThreshold / (numBelowThreshold + numAtThreshold);
            //Debug.LogFormat("Percent samples with FPS close to target: {0:0.00}", percentSamplesMeetingFPS);

            return percentSamplesMeetingFPS > threshold;
        }

        public override AnalyzerResult AdaptPerformance()
        {
            var result = AnalyzerResult.NoChange;
            var lastFrameTime = PerformanceCounters.Instance.LastFrameTime;
            if (lastFrameTime == 0)
            {
                // the performance counters didn't get to execute. Skip the first frame
                return result;
            }

            sampleTimeAccumulator += lastFrameTime;
            numFramesThisSample += 1;

            if (sampleTimeAccumulator > sampleInterval)
            {
                var avgFrameTime = sampleTimeAccumulator / numFramesThisSample;
                timeSamples.AddLast(avgFrameTime);
                numFramesThisSample = 0;
                sampleTimeAccumulator = 0;
            }

            analysisTimeAccumulator += lastFrameTime;
            if (analysisTimeAccumulator > adaptiveAnalysisInterval)
            {
                // enough time passed and we've collected plenty of samples for analysis
                analysisTimeAccumulator = 0;

                if (!IsPerformanceMeetingThreshold(switchToLowerQualityThreshold, 3))
                {
                    // Too many samples are not meeting the FPS bar we need to move down a perf bucket

                    // If we end up moving a bucket down we do not allow moving to a higher bucket
                    // since we know if won't work
                    allowMovingToHigherBucket = false;
                    result = AnalyzerResult.BucketDown;
                }
                else if (allowMovingToHigherBucket)
                {
                    // Test if we are consistently meeting the FPS target
                    if (IsPerformanceMeetingThreshold(switchToHigherQualityThreshold, 1))
                    {
                        // If we're meeting the FPS target and this is occurring for enough
                        // analysis periods we can try a higher quality bucket
                        numAnalysisPeriodsWithGoodFrameRate += 1;
                        if (numAnalysisPeriodsWithGoodFrameRate == analysisPeriodsBeforeBucketUp)
                        {
                            numAnalysisPeriodsWithGoodFrameRate = 0;
                            result = AnalyzerResult.BucketUp;
                        }
                    }
                    else
                    {
                        numAnalysisPeriodsWithGoodFrameRate = 0;
                    }
                }

                timeSamples.Clear();
            }

            return result;
        }
    }

    /// <summary>
    /// This analyzer uses the GPU render time
    /// </summary>
    public class GpuTimeAdaptiveAnalyzer : AdaptiveAnalyzer
    {
        /// <summary>
        /// The minimum frame time percentage threshold used to increase render quality.
        /// </summary>
        private float MinFrameTimeThreshold = 0.75f;

        /// <summary>
        /// "The maximum frame time percentage threshold used to decrease render quality."
        /// </summary>
        private float MaxFrameTimeThreshold = 0.95f;

        /// <summary>
        /// The number of frames above the frame time threshold before switching to lower bucket.
        /// </summary>
        private int numFramesForLowerBucket = 5;

        /// <summary>
        /// The number of frames below the frame time threshold before switching to higher bucket.
        /// </summary>
        private int numFramesForHigherBucket = 5;

        /// <summary>
        /// The maximum number of frames used to extrapolate a future frame
        /// </summary>
        private int maxLastFrames = 0;
        private LinkedList<float> lastFrames = new LinkedList<float>();

        private const int minFrameCountBeforePerfChange = 5;
        private int frameCountSinceLastLevelUpdate;
        private float maxTimeQuota = 0;
        private float minTimeQuota = 0;
        private bool disableGpuTimingOnDestroy = false;

        private delegate bool FrameTimeCompare(float value, float threshold);

        public GpuTimeAdaptiveAnalyzer()
        {
            // ensure we can store enough last frames in the list
            maxLastFrames = Mathf.Max(numFramesForLowerBucket, numFramesForHigherBucket);
            maxLastFrames = Mathf.Max(maxLastFrames, minFrameCountBeforePerfChange);

            var frameTimeQuota = 1.0f / PerformanceCounters.Instance.TargetFrameRate;
            maxTimeQuota = MaxFrameTimeThreshold * frameTimeQuota;
            minTimeQuota = MinFrameTimeThreshold * frameTimeQuota;

            if (!PerformanceCounters.Instance.GpuTimeEnabled)
            {
                PerformanceCounters.Instance.GpuTimeEnabled = true;
                disableGpuTimingOnDestroy = true;
            }
        }

        public override void OnDestroy()
        {
            if (disableGpuTimingOnDestroy)
            {
                PerformanceCounters.Instance.GpuTimeEnabled = false;
            }
        }

        private bool LastFramesOutsideThreshold(int frameCount, float timeThreshold, FrameTimeCompare compare)
        {
            if (lastFrames.Count < frameCount || frameCountSinceLastLevelUpdate < frameCount)
            {
                return false;
            }

            var lastNode = lastFrames.Last;

            while (frameCount > 0 && lastNode != null)
            {
                if (compare(lastNode.Value, timeThreshold))
                {
                    return false;
                }

                lastNode = lastNode.Previous;
                frameCount--;
            }

            if (frameCount > 0)
            {
                return false;
            }

            return true;
        }


        public override AnalyzerResult AdaptPerformance()
        {
            var result = AnalyzerResult.NoChange;
            var lastFrameTime = PerformanceCounters.Instance.LastFrameGpuTime;
            if (lastFrameTime <= 0)
            {
                return result;
            }

            //Store a list of the frame samples
            lastFrames.AddLast(lastFrameTime);
            if (lastFrames.Count > maxLastFrames)
            {
                lastFrames.RemoveFirst();
            }

            //Wait for a few frames between changes
            frameCountSinceLastLevelUpdate++;
            if (frameCountSinceLastLevelUpdate < minFrameCountBeforePerfChange)
            {
                return result;
            }

            if (lastFrameTime > maxTimeQuota)
            {
                // the last frame GPU rendering time exceeded the quota. Check if this is a consistent
                // behavior and if so move to a lower quality bucket
                if (LastFramesOutsideThreshold(numFramesForLowerBucket, maxTimeQuota, (a, b) => { return a < b; }))
                {
                    result = AnalyzerResult.BucketDown;
                }
            }
            else if (lastFrameTime < minTimeQuota)
            {
                // the last frame GPU rendering time was quick. If this is a consistent behavior we have room
                // to improve performance and we can try moving to a higher quality bucket
                if (LastFramesOutsideThreshold(numFramesForHigherBucket, minTimeQuota, (a, b) => { return a > b; }))
                {
                    result = AnalyzerResult.BucketUp;
                }
            }

            if (result != AnalyzerResult.NoChange)
            {
                frameCountSinceLastLevelUpdate = 0;
            }

            return result;
        }
    }
}
