// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
using System.Collections;
using Windows.Graphics.Holographic;
#endif

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// Helper class to override the projection parameters of the HoloLens frame being presented
    /// so that smaller details may appear more sharp.  The FOV of the HoloLens will be smaller
    /// as a trade-off.
    /// </summary>
    /// <remarks>
    /// Instances of this class are created dynamically by <see cref="MixedRealityCameraSystem"/>.
    /// So there is no need for an AddComponentMenu attribute.
    /// </remarks>
    internal class ProjectionOverride : MonoBehaviour
    {
        /// <summary>
        /// When this is true, projection will be overridden on each frame
        /// </summary>
        public bool ReadingModeEnabled { get; set; } = false;

#if WINDOWS_UWP
        /// <summary>
        /// Coroutine function to set the camera matrices back to their defaults
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator ResetViewMatricesOnFrameEnd()
        {
            yield return new WaitForEndOfFrame();
            CameraCache.Main.ResetStereoViewMatrices();
            CameraCache.Main.ResetStereoProjectionMatrices();
        }

        /// <inheritdoc />
        private void OnPreCull()
        {
            if (!ReadingModeEnabled)
            {
                return;
            }

            const float ResolutionScale = 45.0f / 33.0f;

            StartCoroutine(ResetViewMatricesOnFrameEnd());

            Matrix4x4 leftProj = CameraCache.Main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            Matrix4x4 rightProj = CameraCache.Main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
            leftProj.m00 *= ResolutionScale;
            leftProj.m11 *= ResolutionScale;
            rightProj.m00 *= ResolutionScale;
            rightProj.m11 *= ResolutionScale;
            CameraCache.Main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, leftProj);
            CameraCache.Main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, rightProj);

            HolographicFrame holographicFrame = WindowsMixedRealityUtilities.CurrentWindowsHolographicFrame;
            if (holographicFrame != null)
            {
                HolographicFramePrediction prediction = holographicFrame.CurrentPrediction;

                for (int i = 0; i < prediction.CameraPoses.Count; ++i)
                {
                    HolographicCameraPose cameraPose = prediction.CameraPoses[i];

                    if (cameraPose.HolographicCamera.CanOverrideViewport)
                    {
                        HolographicStereoTransform stereoProjection = cameraPose.ProjectionTransform;

                        stereoProjection.Left.M11 *= ResolutionScale;
                        stereoProjection.Left.M22 *= ResolutionScale;
                        stereoProjection.Right.M11 *= ResolutionScale;
                        stereoProjection.Right.M22 *= ResolutionScale;

                        cameraPose.OverrideProjectionTransform(stereoProjection);
                    }
                }
            }
        }
#endif
    }
}
