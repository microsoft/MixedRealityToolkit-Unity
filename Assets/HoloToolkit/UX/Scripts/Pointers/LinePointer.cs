// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.UX
{
    [RequireComponent(typeof(DistorterGravity))]
    [UseWith(typeof(LineBase))]
    [UseWith(typeof(LineRendererBase))]
    public class LinePointer : ControllerPointerBase
    {
        [Header("Colors")]
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Blue, GradientDefaultAttribute.ColorEnum.White, 1f, 0.25f)]
        protected Gradient lineColorSelected;
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Blue, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient lineColorValid;
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Gray, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient lineColorNoTarget;
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Red, GradientDefaultAttribute.ColorEnum.Red, 1f, 0.5f)]
        protected Gradient lineColorLockFocus;

        [Range(5, 100)]
        [SerializeField]
        protected int lineCastResolution = 25;
        [SerializeField]
        [DropDownComponent(true, true)]
        protected LineBase lineBase;
        [SerializeField]
        [DropDownComponent(true, true)]
        protected AttachToController input;

        protected LineRendererBase[] lineRenderers;
        protected DistorterGravity distorterGravity;

        protected override void OnEnable()
        {
            base.OnEnable();

            lineBase = GetComponent<LineBase>();
            distorterGravity = GetComponent<DistorterGravity>();
            lineBase.AddDistorter(distorterGravity);
            lineRenderers = lineBase.GetComponentsInChildren<LineRendererBase>();
        }

        public override bool InteractionEnabled
        {
            get
            {
                // Line pointer stays inactive until select is pressed for first time
                return selectPressedOnce & base.InteractionEnabled;
            }
        }

        public override void OnPreRaycast()
        {
            if (lineBase == null)
                return;

            // Set our first and last points
            lineBase.FirstPoint = PointerOrigin;
            lineBase.LastPoint = PointerOrigin + (PointerDirection * PointerExtent);

            // Make sure our array will hold
            if (rays == null || rays.Length != lineCastResolution)
                rays = new RayStep[lineCastResolution];

            // Set up our rays
            if (!FocusLocked)
            {
                // Turn off gravity so we get accurate rays
                distorterGravity.enabled = false;
            }

            float stepSize = 1f / rays.Length;
            Vector3 lastPoint = lineBase.GetUnclampedPoint(0f);
            Vector3 currentPoint = Vector3.zero;

            for (int i = 0; i < rays.Length; i++)
            {
                currentPoint = lineBase.GetUnclampedPoint(stepSize * (i + 1));
                rays[i] = new RayStep(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
        }

        public override void OnPostRaycast()
        {
            // Use the results from the last update to set our HitResult
            float clearWorldLength = 0f;
            distorterGravity.enabled = false;
            Gradient lineColor = lineColorNoTarget;

            if (InteractionEnabled)
            {
                lineBase.enabled = true;

                if (selectPressed)
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
                            clearWorldLength += rays[i].Length;
                        }
                    }

                    // Clamp the end of the parabola to the result hit's point
                    lineBase.LineEndClamp = lineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, lineCastResolution);

                    if (FocusTarget != null)
                    {
                        lineColor = lineColorValid;
                    }

                    if (FocusLocked)
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

            if (FocusLocked)
            {
                lineColor = lineColorLockFocus;
            }

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].LineColor = lineColor;
            }
        }

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(LinePointer))]
        public new class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}