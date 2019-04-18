// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A simple line pointer for drawing lines from the input source origin to the current pointer position.
    /// </summary>
    public class LinePointer : BaseControllerPointer
    {
        [Range(1, 50)]
        [SerializeField]
        [Tooltip("This setting has a high performance cost. Values above 20 are not recommended.")]
        protected int LineCastResolution = 10;

        [SerializeField]
        protected Gradient LineColorSelected = new Gradient();

        [SerializeField]
        protected Gradient LineColorValid = new Gradient();

        [SerializeField]
        protected Gradient LineColorInvalid = new Gradient();

        [SerializeField]
        protected Gradient LineColorNoTarget = new Gradient();

        [SerializeField]
        protected Gradient LineColorLockFocus = new Gradient();

        [SerializeField]
        private BaseMixedRealityLineDataProvider lineBase;

        /// <summary>
        /// The Line Data Provider driving this pointer.
        /// </summary>
        public BaseMixedRealityLineDataProvider LineBase => lineBase;

        [SerializeField]
        [Tooltip("If no line renderers are specified, this array will be auto-populated on startup.")]
        private BaseMixedRealityLineRenderer[] lineRenderers;

        /// <summary>
        /// The current line renderers that this pointer is utilizing.
        /// </summary>
        /// <remarks>
        /// If no line renderers are specified, this array will be auto-populated on startup.
        /// </remarks>
        public BaseMixedRealityLineRenderer[] LineRenderers
        {
            get { return lineRenderers; }
            set { lineRenderers = value; }
        }

        private void CheckInitialization()
        {
            if (lineBase == null)
            {
                lineBase = GetComponent<BaseMixedRealityLineDataProvider>();
            }

            if (lineBase == null)
            {
                Debug.LogError($"No Mixed Reality Line Data Provider found on {gameObject.name}. Did you forget to add a Line Data provider?");
            }

            if (lineBase != null && (lineRenderers == null || lineRenderers.Length == 0))
            {
                lineRenderers = lineBase.GetComponentsInChildren<BaseMixedRealityLineRenderer>();
            }

            if (lineRenderers == null || lineRenderers.Length == 0)
            {
                Debug.LogError($"No Mixed Reality Line Renderers found on {gameObject.name}. Did you forget to add a Mixed Reality Line Renderer?");
            }
        }

        #region MonoBehaviour Implementation

        protected virtual void OnValidate()
        {
            CheckInitialization();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInitialization();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (BaseMixedRealityLineRenderer lineRenderer in lineRenderers)
            {
                lineRenderer.enabled = false;
            }
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealityPointer Implementation

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            Debug.Assert(lineBase != null);

            lineBase.UpdateMatrix();

            // Set our first and last points
            if (IsFocusLocked && IsTargetPositionLockedOnFocusLock && Result != null)
            {
                // Make the final point 'stick' to the target at the distance of the target
                SetLinePoints(Position, Result.Details.Point, Result.Details.RayDistance);
            }
            else
            {
                SetLinePoints(Position, Position + Rotation * Vector3.forward * DefaultPointerExtent, DefaultPointerExtent);
            }

            // Make sure our array will hold
            if (Rays == null || Rays.Length != LineCastResolution)
            {
                Rays = new RayStep[LineCastResolution];
            }

            float stepSize = 1f / Rays.Length;
            Vector3 lastPoint = lineBase.GetUnClampedPoint(0f);
            for (int i = 0; i < Rays.Length; i++)
            {
                Vector3 currentPoint = lineBase.GetUnClampedPoint(stepSize * (i + 1));
                Rays[i].UpdateRayStep(ref lastPoint, ref currentPoint);
                lastPoint = currentPoint;
            }
        }

        /// <inheritdoc />
        public override void OnPostSceneQuery()
        {
            base.OnPostSceneQuery();

            Gradient lineColor = LineColorNoTarget;

            if (!IsActive)
            {
                lineBase.enabled = false;
                BaseCursor?.SetVisibility(false);
                return;
            }

            lineBase.enabled = true;
            BaseCursor?.SetVisibility(true);

            // The distance the ray travels through the world before it hits something. Measured in world-units (as opposed to normalized distance).
            float clearWorldLength;
            // Used to ensure the line doesn't extend beyond the cursor
            float cursorOffsetWorldLength = (BaseCursor != null) ? BaseCursor.SurfaceCursorDistance : 0;

            // If we hit something
            if (Result?.CurrentPointerTarget != null)
            {
                clearWorldLength = Result.Details.RayDistance;

                lineColor = LineColorValid;
            }
            else
            {
                clearWorldLength = DefaultPointerExtent;

                lineColor = IsSelectPressed ? LineColorSelected : LineColorNoTarget;
            }

            if (IsFocusLocked)
            {
                lineColor = LineColorLockFocus;
            }

            int maxClampLineSteps = LineCastResolution;

            foreach (BaseMixedRealityLineRenderer lineRenderer in lineRenderers)
            {
                // Renderers are enabled by default if line is enabled
                lineRenderer.enabled = true;
                maxClampLineSteps = Mathf.Max(maxClampLineSteps, lineRenderer.LineStepCount);
                lineRenderer.LineColor = lineColor;
            }

            // If focus is locked, we're sticking to the target
            // So don't clamp the world length
            if (IsFocusLocked && IsTargetPositionLockedOnFocusLock)
            {
                float cursorOffsetLocalLength = LineBase.GetNormalizedLengthFromWorldLength(cursorOffsetWorldLength);
                LineBase.LineEndClamp = 1 - cursorOffsetLocalLength;
            }
            else
            {
                // Otherwise clamp the line end by the clear distance
                float clearLocalLength = lineBase.GetNormalizedLengthFromWorldLength(clearWorldLength - cursorOffsetWorldLength, maxClampLineSteps);
                lineBase.LineEndClamp = clearLocalLength;
            }
        }

        protected virtual void SetLinePoints(Vector3 startPoint, Vector3 endPoint, float distance)
        {
            lineBase.FirstPoint = startPoint;
            lineBase.LastPoint = endPoint;
        }

        public override bool IsInteractionEnabled =>
                // If IsTracked is not true, then we don't have position data yet (or have stale data),
                // so remain disabled until we know where to appear (not just at the origin).
                IsFocusLocked || (IsTracked && Controller.IsInPointingPose && base.IsInteractionEnabled);

        #endregion IMixedRealityPointer Implementation
    }
}
