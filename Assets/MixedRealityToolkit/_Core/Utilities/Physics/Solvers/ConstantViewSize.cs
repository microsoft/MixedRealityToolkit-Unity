// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Solvers
{
    /// <summary>
    ///   ConstantViewSize solver scales to maintain a constant size relative to the view (currently tied to the Camera)
    /// </summary>
    public class ConstantViewSize : Solver
    {
        private const float ScalePower = 1f;

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("The object take up this percent vertically in our view (not technically a percent use 0.5 for 50%)")]
        private float targetViewPercentV = 0.5f;

        [SerializeField]
        [Tooltip("If the object is closer than MinDistance, the distance used is clamped here")]
        private float minDistance = 0.5f;

        [SerializeField]
        [Tooltip("If the object is farther than MaxDistance, the distance used is clamped here")]
        private float maxDistance = 3.5f;

        [SerializeField]
        [Tooltip("Minimum scale value possible (world space scale)")]
        private float minScale = 0.01f;

        [SerializeField]
        [Tooltip("Maximum scale value possible (world space scale)")]
        private float maxScale = 100f;

        [SerializeField]
        [Tooltip("If you don't trust or don't like the auto size calculation, specify a manual size here. 0 is ignored")]
        private float manualObjectSize = 0;

        /// <summary>
        /// 0 to 1 between MinScale and MaxScale.  If current is less than max, then scaling is being applied.
        /// This value is subject to inaccuracies due to smoothing/interpolation/momentum
        /// </summary>
        public float CurrentScalePercent { get; private set; } = 1f;

        /// <summary>
        /// 0 to 1 between MinDistance and MaxDistance.  If current is less than max, object is potentially on a surface [or some other condition like interpolating] (since it may still be on surface, but scale percent may be clamped at max)
        /// This value is subject to inaccuracies due to smoothing/interpolation/momentum
        /// </summary>
        public float CurrentDistancePercent { get; private set; } = 1f;

        /// <summary>
        /// Returns the scale to be applied based on the FOV.  This scale will be multiplied by distance as part of
        /// the final scale calculation, so this is the ratio of vertical fov to distance.
        /// </summary>
        public float FovScale
        {
            get
            {
                float cameraFovRadians = (CameraCache.Main.aspect * CameraCache.Main.fieldOfView) * Mathf.Deg2Rad;
                float sinFov = Mathf.Sin(cameraFovRadians * 0.5f);
                return 2f * targetViewPercentV * sinFov / objectSize;
            }
        }

        private float fovScalar = 1f;
        private float objectSize = 1f;

        protected virtual void Start()
        {
            float baseSize;

            if (manualObjectSize > 0)
            {
                baseSize = manualObjectSize;
            }
            else
            {
                Vector3 cachedScale = transform.root.localScale;
                transform.root.localScale = Vector3.one;

                var combinedBounds = new Bounds(transform.position, Vector3.zero);
                var renderers = GetComponentsInChildren<Renderer>();

                for (var i = 0; i < renderers.Length; i++)
                {
                    combinedBounds.Encapsulate(renderers[i].bounds);
                }

                baseSize = combinedBounds.extents.magnitude;

                transform.root.localScale = cachedScale;
            }

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

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            if (SolverHandler.TransformTarget != null)
            {
                // Get current fov each time instead of trying to cache it.  Can never count on init order these days
                fovScalar = FovScale;

                // Calculate scale based on distance from view.  Do not interpolate so we can appear at a constant size if possible.  Borrowed from greybox.
                Vector3 targetPosition = SolverHandler.TransformTarget.position;
                float distance = Mathf.Clamp(Vector3.Distance(transform.position, targetPosition), minDistance, maxDistance);
                float scale = Mathf.Clamp(fovScalar * Mathf.Pow(distance, ScalePower), minScale, maxScale);
                GoalScale = Vector3.one * scale;

                // Save some state information for external use
                CurrentDistancePercent = Mathf.InverseLerp(minDistance, maxDistance, distance);
                CurrentScalePercent = Mathf.InverseLerp(minScale, maxScale, scale);

                UpdateWorkingScaleToGoal();
            }
        }
    }
}
