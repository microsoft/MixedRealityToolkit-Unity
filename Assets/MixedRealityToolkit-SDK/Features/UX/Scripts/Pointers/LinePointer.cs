// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.Renderers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Distorters;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    [RequireComponent(typeof(DistorterGravity))]
    public class LinePointer : BaseControllerPointer
    {
        [SerializeField]
        private Gradient lineColorSelected = new Gradient();

        [SerializeField]
        private Gradient lineColorValid = new Gradient();

        [SerializeField]
        private Gradient lineColorNoTarget = new Gradient();

        [SerializeField]
        private Gradient lineColorLockFocus = new Gradient();

        [Range(5, 100)]
        [SerializeField]
        private int lineCastResolution = 25;

        [SerializeField]
        private BaseMixedRealityLineDataProvider lineBase;

        [SerializeField]
        [Tooltip("If no line renderers are specified, this array will be auto-populated on startup.")]
        private BaseMixedRealityLineRenderer[] lineRenderers;

        [SerializeField]
        private DistorterGravity distorterGravity = null;

        private void OnValidate()
        {
            CheckInitialization();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckInitialization();
        }

        private void CheckInitialization()
        {
            if (lineBase == null)
            {
                lineBase = GetComponent<BaseMixedRealityLineDataProvider>();
            }

            if (lineBase == null)
            {
                Debug.LogError($"No Mixed Reality Line Data Provider found on {gameObject.name}.");
            }

            if (distorterGravity == null)
            {
                distorterGravity = GetComponent<DistorterGravity>();
            }

            if (lineBase != null && (lineRenderers == null || lineRenderers.Length == 0))
            {
                lineRenderers = lineBase.GetComponentsInChildren<BaseMixedRealityLineRenderer>();
            }

            if (lineRenderers == null || lineRenderers.Length == 0)
            {
                Debug.LogError($"No Mixed Reality Line Renderers found on {gameObject.name}.");
            }
        }

        /// <summary>
        /// Line pointer stays inactive until select is pressed for first time
        /// </summary>
        public override bool IsInteractionEnabled => HasSelectPressedOnce & base.IsInteractionEnabled;

        /// <inheritdoc />
        public override void OnPreRaycast()
        {
            Debug.Assert(lineBase != null);

            Vector3 pointerPosition;
            TryGetPointerPosition(out pointerPosition);

            // Set our first and last points
            lineBase.FirstPoint = pointerPosition;
            lineBase.LastPoint = pointerPosition + (PointerDirection * (PointerExtent ?? InputSystem.FocusProvider.GlobalPointingExtent));

            // Make sure our array will hold
            if (Rays == null || Rays.Length != lineCastResolution)
            {
                Rays = new RayStep[lineCastResolution];
            }

            // Set up our rays
            if (!IsFocusLocked)
            {
                // Turn off gravity so we get accurate rays
                distorterGravity.enabled = false;
            }

            float stepSize = 1f / Rays.Length;
            Vector3 lastPoint = lineBase.GetUnClampedPoint(0f);

            for (int i = 0; i < Rays.Length; i++)
            {
                Vector3 currentPoint = lineBase.GetUnClampedPoint(stepSize * (i + 1));
                Rays[i] = new RayStep(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
        }

        /// <inheritdoc />
        public override void OnPostRaycast()
        {
            // Use the results from the last update to set our NavigationResult
            float clearWorldLength = 0f;
            distorterGravity.enabled = false;
            Gradient lineColor = lineColorNoTarget;

            if (IsInteractionEnabled)
            {
                lineBase.enabled = true;

                if (IsSelectPressed)
                {
                    lineColor = lineColorSelected;
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
                            clearWorldLength += Vector3.Distance(Result.StartPoint, Result.StartPoint);
                        }
                        else if (i < Result.RayStepIndex)
                        {
                            // Add the full length of the step to our total distance
                            clearWorldLength += Rays[i].Length;
                        }
                    }

                    // Clamp the end of the parabola to the result hit's point
                    lineBase.LineEndClamp = lineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, lineCastResolution);

                    if (FocusTarget != null)
                    {
                        lineColor = lineColorValid;
                    }

                    if (IsFocusLocked)
                    {
                        distorterGravity.enabled = true;
                        distorterGravity.WorldCenterOfGravity = Result.CurrentPointerTarget.transform.position;
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
                lineColor = lineColorLockFocus;
            }

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].LineColor = lineColor;
            }
        }
    }
}