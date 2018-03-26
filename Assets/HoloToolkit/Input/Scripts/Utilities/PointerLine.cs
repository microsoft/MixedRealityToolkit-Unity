// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.UX;
using UnityEngine;
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    [RequireComponent(typeof(DistorterGravity))]
    [RequireComponent(typeof(LineBase))]
    [RequireComponent(typeof(LineRendererBase))]
    public class PointerLine : AttachToController, IInputHandler
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

        [Range(5, 100)]
        [SerializeField]
        protected int LineCastResolution = 25;

        [SerializeField]
        protected LineBase LineBase;

        [SerializeField]
        [Tooltip("If no line renderers are specified, this array will be auto-populated on startup.")]
        protected LineRendererBase[] LineRenderers;

        protected DistorterGravity DistorterGravity;

        protected override void OnEnable()
        {
            base.OnEnable();

            //if (ControllerInfo != null)
            //{

            //}
        }

        protected void Awake()
        {
            LineBase = GetComponent<LineBase>();
            DistorterGravity = GetComponent<DistorterGravity>();
            LineBase.AddDistorter(DistorterGravity);
            if (LineRenderers == null || LineRenderers.Length == 0)
            {
                LineRenderers = LineBase.GetComponentsInChildren<LineRendererBase>();
            }
        }

        /// <summary>
        /// Line pointer stays inactive until select is pressed for first time
        /// </summary>
        public bool InteractionEnabled { get; set; }

        public void UpdateRenderedLine(RayStep[] lines, PointerResult result)
        {
            if (LineBase == null) { return; }

            // Set our first and last points
            //LineBase.FirstPoint = lines[0].Origin;
            //LineBase.LastPoint = lines[0].Terminus;

            LineBase.LastPoint = result.End.Point;

            //float stepSize = 1f / lines.Length;
            //Vector3 lastPoint = LineBase.GetUnclampedPoint(0f);

            //for (int i = 0; i < lines.Length; i++)
            //{
            //    Vector3 currentPoint = LineBase.GetUnclampedPoint(stepSize * (i + 1));
            //    lines[i] = new RayStep(lastPoint, currentPoint);
            //    lastPoint = currentPoint;
            //}

            // Use the results from the last update to set our NavigationResult
            float clearWorldLength = 0f;
            DistorterGravity.enabled = false;

            Gradient lineColor;
            if (result.End.Object != null)
            {
                lineColor = LineColorSelected;
            }
            else
            {
                lineColor = LineColorNoTarget;
            }

            if (true)
            {
                LineBase.enabled = true;

                //if (selectPressed)
                //{
                //    lineColor = LineColorSelected;
                //}

                //// If we hit something
                //if (Result.CurrentPointerTarget != null)
                //{
                //    // Use the step index to determine the length of the hit
                //    for (int i = 0; i <= Result.RayStepIndex; i++)
                //    {
                //        if (i == Result.RayStepIndex)
                //        {
                //            // Only add the distance between the start point and the hit
                //            clearWorldLength += Vector3.Distance(Result.StartPoint, Result.StartPoint);
                //        }
                //        else if (i < Result.RayStepIndex)
                //        {
                //            // Add the full length of the step to our total distance
                //            clearWorldLength += Rays[i].Length;
                //        }
                //    }

                //    // Clamp the end of the parabola to the result hit's point
                //    LineBase.LineEndClamp = LineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, LineCastResolution);
                //}
                //else
                //{
                //    LineBase.LineEndClamp = 1f;
                //}
            }
            else
            {
                LineBase.enabled = false;
            }

            for (int i = 0; i < LineRenderers.Length; i++)
            {
                LineRenderers[i].LineColor = lineColor;
            }
        }

        protected override void OnAttachToController()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Subscribe to input now that we're parented under the controller
            InteractionManager.InteractionSourceUpdated += InteractionSourceUpdated;
#endif
        }

        protected override void OnDetachFromController()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Unsubscribe from input now that we've detached from the controller
            InteractionManager.InteractionSourceUpdated -= InteractionSourceUpdated;
#endif
        }

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        private void InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
        {
            // Check if the event came from the correct controller
            if (obj.state.source.handedness == Handedness)
            {
                //Select
            }
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
        }
#endif
    }
}
