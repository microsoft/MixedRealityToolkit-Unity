// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Tracks the GPU time spent rendering a camera.
    /// For stereo rendering sampling is made from the beginning of the left eye to the end of the right eye.
    /// </summary>
    public class GpuTimingCamera : MonoBehaviour
    {
        public string TimingTag = "Frame";

        private Camera timingCamera;

        private void Start()
        {
            timingCamera = GetComponent<Camera>();
            Debug.Assert(timingCamera, "GpuTimingComponent must be attached to a Camera.");
        }

        protected void OnPreRender()
        {
            if (timingCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left || timingCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
            {
                GpuTiming.BeginSample(TimingTag);
            }
        }

        protected void OnPostRender()
        {
            if (timingCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right || timingCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
            {
                GpuTiming.EndSample();
            }
        }
    }
}