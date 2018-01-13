// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Utilities
{
    /// <summary>
    /// A helper class for making applications more usable across different devices.
    /// </summary>
    public static class UsabilityUtilities
    {
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
                const float HololensV1PixelHeight = 720f;
                const float HololensV1FieldOfView = 17.15f;
                const float HololensV1PixelsPerDegree = (HololensV1PixelHeight / HololensV1FieldOfView);

                float pixelsPerDegree = (camera.pixelHeight / camera.fieldOfView);

                // This scaling equation was derived by having a number of people look at a piece of UI with text
                // on a HoloLens V1 and a few other HMDs. Each person scaled the content up or down until they
                // reached an "optimal usability" scale for that HMD. Then, the chosen "optimal usability" scales
                // were plotted against the HMDs' pixels per degree, and an equation was chosen that approximated
                // the chosen scales across people and HMDs decently well.
                //
                // The equation currently places HoloLensV1 at 1x scale, which means content previously designed
                // for HoloLens will work as expected. Also, it's asymptotic, so HMDs with extremely high pixels
                // per degree won't shrink content down too quickly.
                //
                // All that said, keep in mind that as new HMDs are created with different pixels per degree and
                // different visual characteristics, the equation may need to be adjusted or reworked to include
                // those new characteristics as input to make sure it best targets the broad range of devices.

                float unclampedScaleFactor = Mathf.Sqrt(HololensV1PixelsPerDegree / pixelsPerDegree);

                const float MinimumScaleFactor = 0.1f;
                const float MaximumScaleFactor = 10f;

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
