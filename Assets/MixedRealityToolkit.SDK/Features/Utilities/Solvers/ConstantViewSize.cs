// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// ConstantViewSize solver scales to maintain a constant size relative to the view (currently tied to the Camera)
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ConstantViewSize")]
    public class ConstantViewSize : Solver
    {
        #region ConstantViewSize Parameters

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("The object take up this percent vertically in our view (not technically a percent use 0.5 for 50%)")]
        private float targetViewPercentV = 0.5f;

        /// <summary>
        /// The object take up this percent vertically in our view (not technically a percent use 0.5 for 50%)
        /// </summary>
        public float TargetViewPercentV
        {
            get { return targetViewPercentV; }
            set { targetViewPercentV = value; }
        }

        [SerializeField]
        [Tooltip("If the object is closer than MinDistance, the distance used is clamped here")]
        private float minDistance = 0.5f;

        /// <summary>
        /// If the object is closer than MinDistance, the distance used is clamped here
        /// </summary>
        public float MinDistance
        {
            get { return minDistance; }
            set { minDistance = value; }
        }

        [SerializeField]
        [Tooltip("If the object is farther than MaxDistance, the distance used is clamped here")]
        private float maxDistance = 3.5f;

        /// <summary>
        /// If the object is farther than MaxDistance, the distance used is clamped here
        /// </summary>
        public float MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        [SerializeField]
        [Tooltip("Minimum scale value possible (world space scale)")]
        private float minScale = 0.01f;

        /// <summary>
        /// Minimum scale value possible (world space scale)
        /// </summary>
        public float MinScale
        {
            get { return minScale; }
            set { minScale = value; }
        }

        [SerializeField]
        [Tooltip("Maximum scale value possible (world space scale)")]
        private float maxScale = 100f;

        /// <summary>
        /// Maximum scale value possible (world space scale)
        /// </summary>
        public float MaxScale
        {
            get { return maxScale; }
            set { maxScale = value; }
        }

        [SerializeField]
        [Tooltip("Used for dead zone for scaling")]
        private float scaleBuffer = 0.01f;

        /// <summary>
        /// Used for dead zone for scaling
        /// </summary>
        public float ScaleBuffer
        {
            get { return scaleBuffer; }
            set { scaleBuffer = value; }
        }

        [SerializeField]
        [Tooltip("Overrides auto size calculation with provided manual size. If 0, solver calculates size")]
        private float manualObjectSize = 0;

        /// <summary>
        /// Overrides auto size calculation with provided manual size. If 0, solver calculates size
        /// </summary>
        public float ManualObjectSize
        {
            get { return manualObjectSize; }
            set
            {
                manualObjectSize = value;
                RecalculateBounds();
            }
        }

        public ScaleState ScaleState { get; private set; } = ScaleState.Static;

        /// <summary>
        /// 0 to 1 between MinScale and MaxScale. If current is less than max, then scaling is being applied.
        /// This value is subject to inaccuracies due to smoothing/interpolation/momentum.
        /// </summary>
        public float CurrentScalePercent { get; private set; } = 1f;

        /// <summary>
        /// 0 to 1 between MinDistance and MaxDistance. If current is less than max, object is potentially on a surface [or some other condition like interpolating] (since it may still be on surface, but scale percent may be clamped at max).
        /// This value is subject to inaccuracies due to smoothing/interpolation/momentum.
        /// </summary>
        public float CurrentDistancePercent { get; private set; } = 1f;

        /// <summary>
        /// Returns the scale to be applied based on the FOV. This scale will be multiplied by distance as part of
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

        #endregion

        private float fovScalar = 1f;
        private float objectSize = 1f;

        protected override void Start()
        {
            base.Start();
            RecalculateBounds();
        }

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            float lastScalePct = CurrentScalePercent;

            if (SolverHandler.TransformTarget != null)
            {
                // Get current fov each time instead of trying to cache it.  Can never count on init order these days
                fovScalar = FovScale;

                // Set the linked alt scale ahead of our work. This is an attempt to minimize jittering by having solvers work with an interpolated scale.
                SolverHandler.AltScale.SetGoal(transform.localScale);

                // Calculate scale based on distance from view.  Do not interpolate so we can appear at a constant size if possible.  Borrowed from greybox.
                Vector3 targetPosition = SolverHandler.TransformTarget.position;
                float distance = Mathf.Clamp(Vector3.Distance(transform.position, targetPosition), minDistance, maxDistance);
                float scale = Mathf.Clamp(fovScalar * distance, minScale, maxScale);
                GoalScale = Vector3.one * scale;

                // Save some state information for external use
                CurrentDistancePercent = Mathf.InverseLerp(minDistance, maxDistance, distance);
                CurrentScalePercent = Mathf.InverseLerp(minScale, maxScale, scale);
            }

            float scaleDifference = (CurrentScalePercent - lastScalePct) / SolverHandler.DeltaTime;

            if (scaleDifference > scaleBuffer)
            {
                ScaleState = ScaleState.Growing;
            }
            else if (scaleDifference < -scaleBuffer)
            {
                ScaleState = ScaleState.Shrinking;
            }
            else
            {
                ScaleState = ScaleState.Static;
            }
        }

        /// <summary>
        /// Attempts to calculate the size of the bounds which contains all child renderers for attached GameObject. This information is used in the core solver calculations
        /// </summary>
        public void RecalculateBounds()
        {
            float baseSize;

            // If user set object size override apply, otherwise compute baseSize
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
    }
}
