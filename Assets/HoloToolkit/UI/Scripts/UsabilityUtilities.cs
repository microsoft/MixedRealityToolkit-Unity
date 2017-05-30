// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A helper class for making applications more usable across different devices.
    /// </summary>
    public static class UsabilityUtilities
    {
        private const float HololensV1PixelHeight = 720f;
        private const float HololensV1FieldOfView = 17.15f;
        private const float HololensV1PixelsPerDegree = (HololensV1PixelHeight / HololensV1FieldOfView);

        private const float OtherHmdPixelHeight = 1200f;
        private const float OtherHmdFieldOfView = 110f;
        private const float OtherHmdPixelsPerDegree = (OtherHmdPixelHeight / OtherHmdFieldOfView);

        // This scale factor was measured by having a number of humans choose an "optimal usability" size for
        // a piece of UI with text on both a HoloLens V1 device and a different HMD and comparing their
        // choices from one HMD to the other. Keep in mind that as new HMDs come on the market, this scale
        // factor may be adjusted or the whole equation may be reworked to better target all devices.
        private const float HumanMeasuredScaleFactorFromHololensV1ToOtherHmd = 2.18f;

        private const float Slope = ((HumanMeasuredScaleFactorFromHololensV1ToOtherHmd - 1f) / (OtherHmdPixelsPerDegree - HololensV1PixelsPerDegree));
        private const float YAxisIntercept = (1f - (HololensV1PixelsPerDegree * Slope));

        private const float MinimumScaleFactor = 0.1f;
        private const float MaximumScaleFactor = 10f;

        /// <summary>
        /// Gets a factor useful for scaling visual and interactable objects based on a camera's characteristics (resolution, field of view, etc).
        /// </summary>
        /// <param name="camera">The camera whose characteristics to consider.</param>
        /// <returns>The scale factor.</returns>
        public static float GetUsabilityScaleFactor(Camera camera)
        {
            float scaleFactor;

            if (camera == null)
            {
                Debug.LogError("camera is required.");
                scaleFactor = 1f;
            }
            else
            {
                float pixelsPerDegree = (camera.pixelHeight / camera.fieldOfView);
                float unclampedScaleFactor = ((Slope * pixelsPerDegree) + YAxisIntercept);

                scaleFactor = Mathf.Clamp(unclampedScaleFactor, MinimumScaleFactor, MaximumScaleFactor);

#if !UNITY_EDITOR
                Debug.AssertFormat(unclampedScaleFactor == scaleFactor,
                    "Usability scale factor got clamped from {0} to {1}. Are we calculating HMD characteristics correctly?",
                    unclampedScaleFactor,
                    scaleFactor
                    );
#endif
            }

            return scaleFactor;
        }
    }
}
