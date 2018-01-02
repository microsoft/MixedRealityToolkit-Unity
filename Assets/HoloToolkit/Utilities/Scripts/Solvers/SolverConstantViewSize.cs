//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity
{
    /// <summary>
    ///   ConstantViewSize solver scales to maintain a constant size relative to the view (currently tied to the Camera)
    /// </summary>
    public class SolverConstantViewSize : Solver
    {
        #region public members
        [Tooltip("The object take up this percent vertically in our view (not technically a percent use 0.5 for 50%)")]
        public float TargetViewPercentV = 0.5f;

        [Tooltip("If the object is closer than MinDistance, the distance used is clamped here")]
        public float MinDistance = 0.5f;

        [Tooltip("If the object is farther than MaxDistance, the distance used is clamped here")]
        public float MaxDistance = 3.5f;

        [Tooltip("Minimum scale value possible (world space scale)")]
        public float MinScale = 0.01f;

        [Tooltip("Maximum scale value possible (world space scale)")]
        public float MaxScale = 100f;

        [Tooltip("Used for dead zone for scaling")]
        public float ScaleBuffer = 0.01f;

        [Tooltip("If you don't trust or don't like the auto size calculation, specify a manual size here. 0 is ignored")]
        public float ManualObjectSize = 0;
        public enum ScaleStateEnum
        {
            Static,
            Shrinking,
            Growing
        }

        [HideInInspector]
        public ScaleStateEnum ScaleState = ScaleStateEnum.Static;

        // 0 to 1 between MinScale and MaxScale.  If currently < max, then scaling is being applied.
        // This value is subject to inaccuracies due to smoothing/interpolation/momentum
        public float CurrentScalePercent
        {
            get
            {
                return objectScalePercent;
            }
        }

        // 0 to 1 between MinDistance and MaxDistance.  If current < max, object is potentially on a surface [or some other condition like interpolating] (since it may still be on surface, but scale percent may be clamped at max)
        // This value is subject to inaccuracies due to smoothing/interpolation/momentum
        public float CurrentDistancePercent
        {
            get
            {
                return objectDistancePercent;
            }
        }
        #endregion

        #region private members
        private float fovScalar = 1f;
        private float objectSize = 1f;
        private float objectScalePercent = 1f;
        private float objectDistancePercent = 1f;
        #endregion

        protected override void Start()
        {
            base.Start();

            float baseSize = CalculateObjectSize();

            if (baseSize > 0)
            {
                objectSize = baseSize;
            }
            else
            {
                Debug.LogWarning("ConstantViewSize: Object base size calculate was 0, defaulting to 1");
                objectSize = 1f;
            }
        }

        /// <summary>
        ///   Attempts to calculate the size of the bounds which contains all child renderers.
        ///   This may be tricky to use, as this happens during initialization, while the app may
        ///   be undergoing scaling by other solvers/components.  Thus, the size calculation might
        ///   be inaccurate.  It's probably a better idea to use ManualObjectSize just to be sure.
        /// </summary>
        /// <returns> Object diameter </returns>
        private float CalculateObjectSize()
        {
            if (ManualObjectSize > 0)
            {
                return ManualObjectSize;
            }

            Vector3 rootScale = transform.root.localScale;
            transform.root.localScale = Vector3.one;

            float maxSize = 1f;

            Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);
            Renderer[] renderers = this.GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renderers)
            {
                combinedBounds.Encapsulate(render.bounds);
            }

            maxSize = combinedBounds.extents.magnitude;

            transform.root.localScale = rootScale;

            return maxSize;
        }


        public override void SolverUpdate()
        {
            float lastScalePct = objectScalePercent;
            AdjustSizeForView(solverHandler.TransformTarget);
            float scaleDiff = (objectScalePercent - lastScalePct) / solverHandler.DeltaTime;

            if (scaleDiff > ScaleBuffer)
            {
                ScaleState = ScaleStateEnum.Growing;
            }
            else if (scaleDiff < -ScaleBuffer)
            {
                ScaleState = ScaleStateEnum.Shrinking;
            }
            else
            {
                ScaleState = ScaleStateEnum.Static;
            }
        }

        /// <summary>
        ///   Returns the scale to be applied based on the FOV.  This scale will be multiplied by distance as part of
        ///   the final scale calculation, so this is the ratio of vertical fov to distance.
        /// </summary>
        /// <returns> Scale of vFOV </returns>
        public float GetFOVScalar()
        {
            float camFOVrad = (CameraCache.Main.aspect * CameraCache.Main.fieldOfView) * Mathf.Deg2Rad;

            float sinfov = Mathf.Sin(camFOVrad * 0.5f);
            float scalar = 2f * TargetViewPercentV * sinfov / objectSize;

            return scalar;
        }

        private void AdjustSizeForView(Transform targTransform)
        {
            if (targTransform != null)
            {
                // Get current fov each time instead of trying to cache it.  Can never count on init order these days
                fovScalar = GetFOVScalar();

                // Set the linked alt scale ahead of our work.  This is an attempt to minimize jittering by having solvers work with an interpolated scale.
                solverHandler.AltScale.SetGoal(this.transform.localScale);

                // Calculate scale based on distance from view.  Do not interpolate so we can appear at a constant size if possible.  Borrowed from greybox.
                //bool ignoreParent = false;
                float scalePower = 1f;

                Vector3 targetPosition = targTransform.position;
                float distance = Mathf.Clamp(Vector3.Distance(transform.position, targetPosition), MinDistance, MaxDistance);
                float scale = Mathf.Clamp(fovScalar * Mathf.Pow(distance, scalePower), MinScale, MaxScale);
                GoalScale = Vector3.one * scale;

                // Save some state information for external use
                objectDistancePercent = Mathf.InverseLerp(MinDistance, MaxDistance, distance);
                objectScalePercent = Mathf.InverseLerp(MinScale, MaxScale, scale);

                UpdateWorkingScaleToGoal();
            }
        }
    }

}
