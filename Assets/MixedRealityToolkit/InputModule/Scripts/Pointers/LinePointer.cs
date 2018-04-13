// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.Utilities.Attributes;
using MixedRealityToolkit.Utilities.Distorters;
using MixedRealityToolkit.UX.Lines;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Pointers
{
    [RequireComponent(typeof(DistorterGravity))]
    [UseWith(typeof(LineBase))]
    [UseWith(typeof(LineRendererBase))]
    public class LinePointer : BaseControllerPointer
    {
        [Header("Colors")]
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Blue, GradientDefaultAttribute.ColorEnum.White, 1f, 0.25f)]
        protected Gradient LineColorSelected;

        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Blue, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient LineColorValid;

        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Gray, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient LineColorNoTarget;

        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Red, GradientDefaultAttribute.ColorEnum.Red, 1f, 0.5f)]
        protected Gradient LineColorLockFocus;

        [Range(5, 100)]
        [SerializeField]
        protected int LineCastResolution = 25;

        [SerializeField]
        [DropDownComponent(true, true)]
        protected LineBase LineBase;

        [SerializeField]
        [Tooltip("If no line renderers are specified, this array will be auto-populated on startup.")]
        protected LineRendererBase[] LineRenderers;

        protected DistorterGravity DistorterGravity;

        protected override void OnEnable()
        {
            base.OnEnable();

            LineBase = GetComponent<LineBase>();
            DistorterGravity = GetComponent<DistorterGravity>();
            LineBase.AddDistorter(DistorterGravity);
            if (LineRenderers == null || LineRenderers.Length == 0) {
                LineRenderers = LineBase.GetComponentsInChildren<LineRendererBase>();
            }
        }

        /// <summary>
        /// Line pointer stays inactive until select is pressed for first time
        /// </summary>
        public override bool InteractionEnabled
        {
            get
            {
                return SelectPressedOnce & base.InteractionEnabled;
            }
        }

        public override void OnPreRaycast()
        {
            if (LineBase == null) { return; }

            Vector3 pointerPosition;
            TryGetPointerPosition(out pointerPosition);

            // Set our first and last points
            LineBase.FirstPoint = pointerPosition;
            LineBase.LastPoint = pointerPosition + (PointerDirection * (PointerExtent ?? FocusManager.GlobalPointingExtent));

            // Make sure our array will hold
            if (Rays == null || Rays.Length != LineCastResolution)
            {
                Rays = new RayStep[LineCastResolution];
            }

            // Set up our rays
            if (!FocusLocked)
            {
                // Turn off gravity so we get accurate rays
                DistorterGravity.enabled = false;
            }

            float stepSize = 1f / Rays.Length;
            Vector3 lastPoint = LineBase.GetUnclampedPoint(0f);

            for (int i = 0; i < Rays.Length; i++)
            {
                Vector3 currentPoint = LineBase.GetUnclampedPoint(stepSize * (i + 1));
                Rays[i] = new RayStep(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
        }

        public override void OnPostRaycast()
        {
            // Use the results from the last update to set our NavigationResult
            float clearWorldLength = 0f;
            DistorterGravity.enabled = false;
            Gradient lineColor = LineColorNoTarget;

            if (InteractionEnabled)
            {
                LineBase.enabled = true;

                if (SelectPressed)
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
                            clearWorldLength += Vector3.Distance(Result.StartPoint, Result.StartPoint);
                        }
                        else if (i < Result.RayStepIndex)
                        {
                            // Add the full length of the step to our total distance
                            clearWorldLength += Rays[i].Length;
                        }
                    }

                    // Clamp the end of the parabola to the result hit's point
                    LineBase.LineEndClamp = LineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, LineCastResolution);

                    if (FocusTarget != null)
                    {
                        lineColor = LineColorValid;
                    }

                    if (FocusLocked)
                    {
                        DistorterGravity.enabled = true;
                        DistorterGravity.WorldCenterOfGravity = Result.CurrentPointerTarget.transform.position;
                    }
                }
                else
                {
                    LineBase.LineEndClamp = 1f;
                }
            }
            else
            {
                LineBase.enabled = false;
            }

            if (FocusLocked)
            {
                lineColor = LineColorLockFocus;
            }

            for (int i = 0; i < LineRenderers.Length; i++)
            {
                LineRenderers[i].LineColor = lineColor;
            }
        }
    }
}
