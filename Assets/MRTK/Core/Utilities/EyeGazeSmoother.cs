// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Provides some predefined parameters for eye gaze smoothing and saccade detection.
    /// </summary>
    public class EyeGazeSmoother : IMixedRealityEyeSaccadeProvider
    {
        /// <inheritdoc />
        public event Action OnSaccade;

        /// <inheritdoc />
        public event Action OnSaccadeX;

        /// <inheritdoc />
        public event Action OnSaccadeY;

        private readonly float smoothFactorNormalized = 0.96f;
        private readonly float saccadeThreshInDegree = 2.5f; // in degrees (not radians)

        private Ray? oldGaze;
        private int confidenceOfSaccade = 0;
        private int confidenceOfSaccadeThreshold = 6; // TODO(https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/3767): This value should be adjusted based on the FPS of the ET system
        private Ray saccade_initialGazePoint;
        private readonly List<Ray> saccade_newGazeCluster = new List<Ray>();

        private static readonly ProfilerMarker SmoothGazePerfMarker = new ProfilerMarker("[MRTK] EyeGazeSmoother.SmoothGaze");

        /// <summary>
        /// Smooths eye gaze by detecting saccades and tracking gaze clusters.
        /// </summary>
        /// <param name="newGaze">The ray to smooth.</param>
        /// <returns>The smoothed ray.</returns>
        public Ray SmoothGaze(Ray newGaze)
        {
            using (SmoothGazePerfMarker.Auto())
            {
                if (!oldGaze.HasValue)
                {
                    oldGaze = newGaze;
                    return newGaze;
                }

                Ray smoothedGaze = new Ray();
                bool isSaccading = false;

                // Handle saccades vs. outliers: Instead of simply checking that two successive gaze points are sufficiently 
                // apart, we check for clusters of gaze points instead.
                // 1. If the user's gaze points are far enough apart, this may be a saccade, but also could be an outlier.
                //    So, let's mark it as a potential saccade.
                if (IsSaccading(oldGaze.Value, newGaze) && confidenceOfSaccade == 0)
                {
                    confidenceOfSaccade++;
                    saccade_initialGazePoint = oldGaze.Value;
                    saccade_newGazeCluster.Clear();
                    saccade_newGazeCluster.Add(newGaze);
                }
                // 2. If we have a potential saccade marked, let's check if the new points are within the proximity of 
                //    the initial saccade point.
                else if (confidenceOfSaccade > 0 && confidenceOfSaccade < confidenceOfSaccadeThreshold)
                {
                    confidenceOfSaccade++;

                    // First, let's check that we don't just have a bunch of random outliers
                    // The assumption is that after a person saccades, they fixate for a certain 
                    // amount of time resulting in a cluster of gaze points.
                    for (int i = 0; i < saccade_newGazeCluster.Count; i++)
                    {
                        if (IsSaccading(saccade_newGazeCluster[i], newGaze))
                        {
                            confidenceOfSaccade = 0;
                        }

                        // Meanwhile we want to make sure that we are still looking sufficiently far away from our 
                        // original gaze point before saccading.
                        if (!IsSaccading(saccade_initialGazePoint, newGaze))
                        {
                            confidenceOfSaccade = 0;
                        }
                    }
                    saccade_newGazeCluster.Add(newGaze);
                }
                else if (confidenceOfSaccade == confidenceOfSaccadeThreshold)
                {
                    isSaccading = true;
                }

                // Saccade-dependent local smoothing
                if (isSaccading)
                {
                    smoothedGaze.direction = newGaze.direction;
                    smoothedGaze.origin = newGaze.origin;
                    confidenceOfSaccade = 0;
                }
                else
                {
                    smoothedGaze.direction = oldGaze.Value.direction * smoothFactorNormalized + newGaze.direction * (1 - smoothFactorNormalized);
                    smoothedGaze.origin = oldGaze.Value.origin * smoothFactorNormalized + newGaze.origin * (1 - smoothFactorNormalized);
                }

                oldGaze = smoothedGaze;
                return smoothedGaze;
            }
        }

        private static readonly ProfilerMarker IsSaccadingPerfMarker = new ProfilerMarker("[MRTK] EyeGazeSmoother.IsSaccading");

        private bool IsSaccading(Ray rayOld, Ray rayNew)
        {
            using (IsSaccadingPerfMarker.Auto())
            {
                Vector3 v1 = rayOld.origin + rayOld.direction;
                Vector3 v2 = rayNew.origin + rayNew.direction;

                if (Vector3.Angle(v1, v2) > saccadeThreshInDegree)
                {
                    Vector2 hv1 = new Vector2(v1.x, 0);
                    Vector2 hv2 = new Vector2(v2.x, 0);
                    if (OnSaccadeX != null && Vector2.Angle(hv1, hv2) > saccadeThreshInDegree)
                    {
                        OnSaccadeX.Invoke();
                    }

                    Vector2 vv1 = new Vector2(0, v1.y);
                    Vector2 vv2 = new Vector2(0, v2.y);
                    if (OnSaccadeY != null && Vector2.Angle(vv1, vv2) > saccadeThreshInDegree)
                    {
                        OnSaccadeY.Invoke();
                    }

                    PostOnSaccade();

                    return true;
                }
                return false;
            }
        }

        private void PostOnSaccade() => OnSaccade?.Invoke();
    }
}
