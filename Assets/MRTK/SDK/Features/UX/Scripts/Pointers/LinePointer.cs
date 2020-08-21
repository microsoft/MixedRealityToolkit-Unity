// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A simple line pointer for drawing lines from the input source origin to the current pointer position.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/LinePointer")]
    public class LinePointer : BaseControllerPointer
    {
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
        public BaseMixedRealityLineRenderer[] LineRenderers => lineRenderers;

        /// <inheritdoc />
        public override bool IsInteractionEnabled =>
                // If IsTracked is not true, then we don't have position data yet (or have stale data),
                // so remain disabled until we know where to appear (not just at the origin).
                IsFocusLocked || (IsTracked && Controller.IsInPointingPose && base.IsInteractionEnabled);

        private Vector3 lineStartPoint;
        private Vector3 lineEndPoint;

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

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].enabled = true;
            }
        }

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInitialization();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].enabled = false;
            }
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealityPointer Implementation

        private static readonly ProfilerMarker OnPreSceneQueryPerfMarker = new ProfilerMarker("[MRTK] LinePointer.OnPreSceneQuery");

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {
            using (OnPreSceneQueryPerfMarker.Auto())
            {
                PreUpdateLineRenderers();
                UpdateRays();
            }
        }

        private static readonly ProfilerMarker OnPostSceneQueryPerfMarker = new ProfilerMarker("[MRTK] LinePointer.OnPostSceneQuery");

        /// <inheritdoc />
        public override void OnPostSceneQuery()
        {
            using (OnPostSceneQueryPerfMarker.Auto())
            {
                base.OnPostSceneQuery();

                bool isEnabled = IsInteractionEnabled;
                LineBase.enabled = isEnabled;
                if (BaseCursor != null)
                {
                    BaseCursor.SetVisibility(isEnabled);
                }

                PostUpdateLineRenderers();
            }
        }

        private static readonly ProfilerMarker PreUpdateLineRenderersPerfMarker = new ProfilerMarker("[MRTK] LinePointer.PreUpdateLineRenderers");

        protected virtual void PreUpdateLineRenderers()
        {
            using (PreUpdateLineRenderersPerfMarker.Auto())
            {
                Debug.Assert(lineBase != null);

                lineBase.UpdateMatrix();

                // Set our first and last points
                if (IsFocusLocked && IsTargetPositionLockedOnFocusLock && Result != null)
                {
                    // Make the final point 'stick' to the target at the distance of the target
                    SetLinePoints(Position, Result.Details.Point);
                }
                else
                {
                    SetLinePoints(Position, Position + Rotation * Vector3.forward * DefaultPointerExtent);
                }
            }
        }

        private static readonly ProfilerMarker PostUpdateLineRenderersPerfMarker = new ProfilerMarker("[MRTK] LinePointer.PostUpdateLineRenderers");

        protected virtual void PostUpdateLineRenderers()
        {
            using (PostUpdateLineRenderersPerfMarker.Auto())
            {
                if (!IsInteractionEnabled)
                {
                    return;
                }

                // The distance the ray travels through the world before it hits something. Measured in world-units (as opposed to normalized distance).
                float clearWorldLength;
                Gradient lineColor = LineColorNoTarget;

                if (Result?.CurrentPointerTarget != null)
                {
                    // We hit something
                    clearWorldLength = Result.Details.RayDistance;
                    lineColor = IsSelectPressed ? LineColorSelected : LineColorValid;
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

                int maxClampLineSteps = 2;
                for (int i = 0; i < LineRenderers.Length; i++)
                {
                    // Renderers are enabled by default if line is enabled
                    maxClampLineSteps = Mathf.Max(maxClampLineSteps, LineRenderers[i].LineStepCount);
                    LineRenderers[i].LineColor = lineColor;
                }

                // Used to ensure the line doesn't extend beyond the cursor
                float cursorOffsetWorldLength = (BaseCursor != null) ? BaseCursor.SurfaceCursorDistance : 0;

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
                    LineBase.LineEndClamp = clearLocalLength;
                }
            }
        }

        private static readonly ProfilerMarker UpdateRaysPerfMarker = new ProfilerMarker("[MRTK] LinePointer.UpdateRays");

        protected virtual void UpdateRays()
        {
            using (UpdateRaysPerfMarker.Auto())
            {
                const int LineLength = 1;

                // Make sure our array will hold
                if (Rays == null || Rays.Length != LineLength)
                {
                    Rays = new RayStep[LineLength];
                }

                Rays[0].UpdateRayStep(ref lineStartPoint, ref lineEndPoint);
            }
        }

        protected void SetLinePoints(Vector3 startPoint, Vector3 endPoint)
        {
            lineStartPoint = startPoint;
            lineEndPoint = endPoint;

            lineBase.FirstPoint = startPoint;
            lineBase.LastPoint = endPoint;
        }


        #endregion IMixedRealityPointer Implementation
    }
}
