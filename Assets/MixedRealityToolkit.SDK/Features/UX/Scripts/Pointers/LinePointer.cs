// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.DataProviders;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.Renderers;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Physics.Distorters;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    /// <summary>
    /// A simple line pointer for drawing lines from the input source origin to the current pointer position.
    /// </summary>
    [RequireComponent(typeof(DistorterGravity))]
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

        [SerializeField]
        private DistorterGravity gravityDistorter = null;

        /// <summary>
        /// The Gravity Distorter that is affecting the <see cref="BaseMixedRealityLineDataProvider"/> attached to this pointer.
        /// </summary>
        public DistorterGravity GravityDistorter => gravityDistorter;

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

            if (gravityDistorter == null)
            {
                gravityDistorter = GetComponent<DistorterGravity>();
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

        #endregion MonoBehaviour Implementation

        #region IMixedRealityPointer Implementation

        /// <inheritdoc />
        public override void OnPreRaycast()
        {
            Debug.Assert(lineBase != null);

            Vector3 pointerPosition;
            TryGetPointerPosition(out pointerPosition);

            // Set our first and last points
            lineBase.FirstPoint = pointerPosition;

            if (IsFocusLocked)
            {
                lineBase.LastPoint = pointerPosition + ((Result.Details.Point - pointerPosition).normalized * PointerExtent);
            }
            else
            {
                lineBase.LastPoint = pointerPosition + (PointerDirection * PointerExtent);
            }

            // Make sure our array will hold
            if (Rays == null || Rays.Length != LineCastResolution)
            {
                Rays = new RayStep[LineCastResolution];
            }

            // Set up our rays
            if (!IsFocusLocked)
            {
                // Turn off gravity so we get accurate rays
                gravityDistorter.enabled = false;
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
        public override void OnPostRaycast()
        {
            // Use the results from the last update to set our NavigationResult
            float clearWorldLength = 0f;
            gravityDistorter.enabled = false;
            Gradient lineColor = LineColorNoTarget;

            if (IsInteractionEnabled)
            {
                lineBase.enabled = true;

                if (IsSelectPressed)
                {
                    lineColor = LineColorSelected;
                }

                // If we hit something
                if (Result.CurrentPointerTarget != null)
                {
                    // Use the step index to determine the length of the hit
                    for (int i = 0; i <= Result.RayStepIndex; i++)
                    {
                        if (i == Result.RayStepIndex)
                        {
                            // Only add the distance between the start point and the hit
                            clearWorldLength += Vector3.Distance(Rays[i].Origin, Result.Details.Point);
                        }
                        else if (i < Result.RayStepIndex)
                        {
                            // Add the full length of the step to our total distance
                            clearWorldLength += Rays[i].Length;
                        }
                    }

                    // Clamp the end of the line to the result hit's point
                    lineBase.LineEndClamp = lineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, LineCastResolution);

                    if (FocusTarget != null)
                    {
                        lineColor = LineColorValid;
                    }

                    if (IsFocusLocked)
                    {
                        gravityDistorter.enabled = true;
                        gravityDistorter.WorldCenterOfGravity = Result.Details.Point;
                    }
                }
                else
                {
                    lineBase.LineEndClamp = 1f;
                }
            }
            else
            {
                lineBase.enabled = false;
            }

            if (IsFocusLocked)
            {
                lineColor = LineColorLockFocus;
            }

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].LineColor = lineColor;
            }
        }

        #endregion IMixedRealityPointer Implementation
    }
}