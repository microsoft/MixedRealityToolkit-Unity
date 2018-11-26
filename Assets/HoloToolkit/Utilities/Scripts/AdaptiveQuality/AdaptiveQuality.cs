// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Main components for controlling the quality of the system to maintain a steady frame rate.
    /// Calculates a QualityLevel based on the reported frame rate and the refresh rate of the device inside the provided thresholds.
    /// A QualityChangedEvent is triggered whenever the quality level changes.
    /// Uses the GpuTimingCamera component to measure GPU time of the frame, if the Camera doesn't already have this component, it is automatically added.
    /// </summary>

    public class AdaptiveQuality : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The minimum frame time percentage threshold used to increase render quality.")]
        private float MinFrameTimeThreshold = 0.75f;

        [SerializeField]
        [Tooltip("The maximum frame time percentage threshold used to decrease render quality.")]
        private float MaxFrameTimeThreshold = 0.95f;

        [SerializeField]
        private int MinQualityLevel = -5;
        [SerializeField]
        private int MaxQualityLevel = 5;
        [SerializeField]
        private int StartQualityLevel = 5;

        public delegate void QualityChangedEvent(int newQuality, int previousQuality);
        public event QualityChangedEvent QualityChanged;

        public int QualityLevel { get; private set; }
        public int RefreshRate { get; private set; }

        private float frameTimeQuota;

        /// <summary>
        /// The maximum number of frames used to extrapolate a future frame
        /// </summary>
        private const int maxLastFrames = 7;
        private Queue<float> lastFrames = new Queue<float>();

        private const int minFrameCountBeforeQualityChange = 5;
        private int frameCountSinceLastLevelUpdate;

        [SerializeField]
        private Camera adaptiveCamera;

        public const string TimingTag = "Frame";

        private void OnEnable()
        {
            QualityLevel = StartQualityLevel;

            // Store our refresh rate

#if UNITY_2017_2_OR_NEWER
            RefreshRate = (int)UnityEngine.XR.XRDevice.refreshRate;
#else
            RefreshRate = (int)UnityEngine.VR.VRDevice.refreshRate;
#endif
            if (RefreshRate == 0)
            {
                RefreshRate = 60;
                if (!Application.isEditor)
                {
                    Debug.LogWarning("Could not retrieve the HMD's native refresh rate. Assuming " + RefreshRate + " Hz.");
                }
            }
            frameTimeQuota = 1.0f / RefreshRate;

            // Assume main camera if no camera was setup
            if (adaptiveCamera == null)
            {
                adaptiveCamera = Camera.main;
            }

            // Make sure we have the GpuTimingCamera component attached to our camera with the correct timing tag
            GpuTimingCamera gpuCamera = adaptiveCamera.GetComponent<GpuTimingCamera>();
            if (gpuCamera == null || gpuCamera.TimingTag.CompareTo(TimingTag) != 0)
            {
                adaptiveCamera.gameObject.AddComponent<GpuTimingCamera>();
            }
        }

        protected void Update()
        {
            UpdateAdaptiveQuality();
        }

        private bool LastFramesBelowThreshold(int frameCount)
        {
            // Make sure we have enough new frames since last change
            if (lastFrames.Count < frameCount || frameCountSinceLastLevelUpdate < frameCount)
            {
                return false;
            }

            float maxTime = frameTimeQuota * MinFrameTimeThreshold;
            // See if all our frames are below the threshold
            foreach (var frameTime in lastFrames)
            {
                if (frameTime >= maxTime)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateQualityLevel(int delta)
        {
            // Change and clamp the new quality level
            int prevQualityLevel = QualityLevel;
            QualityLevel = Mathf.Clamp(QualityLevel + delta, MinQualityLevel, MaxQualityLevel);

            //Trigger the event if we changed quality
            if (QualityLevel != prevQualityLevel)
            {
                if (QualityChanged != null)
                {
                    QualityChanged(QualityLevel, prevQualityLevel);
                }
                frameCountSinceLastLevelUpdate = 0;
            }
        }

        private void UpdateAdaptiveQuality()
        {
            float lastAppFrameTime = (float)GpuTiming.GetTime("Frame");

            if (lastAppFrameTime <= 0)
            {
                return;
            }

            //Store a list of the frame samples
            lastFrames.Enqueue(lastAppFrameTime);
            if (lastFrames.Count > maxLastFrames)
            {
                lastFrames.Dequeue();
            }

            //Wait for a few frames between changes
            frameCountSinceLastLevelUpdate++;
            if (frameCountSinceLastLevelUpdate < minFrameCountBeforeQualityChange)
            {
                return;
            }

            // If the last frame is over budget, decrease quality level by 2 slots.
            if (lastAppFrameTime > MaxFrameTimeThreshold * frameTimeQuota)
            {
                UpdateQualityLevel(-2);
            }
            else if (lastAppFrameTime < MinFrameTimeThreshold * frameTimeQuota)
            {
                // If the last 5 frames are below the GPU usage threshold, increase quality level by one.
                if (LastFramesBelowThreshold(maxLastFrames))
                {
                    UpdateQualityLevel(1);
                }
            }
        }
    }
}
