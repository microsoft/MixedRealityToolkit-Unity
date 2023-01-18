// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Visual driver for "squeezable" bounding box visuals. This is powered by
    /// the Shell_Rounded_Bound shader, which offers visually distinctive rounded
    /// corners and a 3D occlusion effect. It also "squeezes" with variable selection input,
    /// giving physical feedback to users as they select/grab/pinch the object.
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Squeezable Box Visuals")]
    internal class SqueezableBoxVisuals : MonoBehaviour
    {
        // The box will snap to this thickness whenever IsFlat = true.
        // Useful if you're using a BoundsControl on FlattenMode.Always,
        // and you want bounds to be flat even when the object isn't.
        private const float flattenThickness = 0.002f;

        // Reference to the interactable in charge of the bounded object.
        private StatefulInteractable interactable;

        // Property block used to control the box renderer.
        private MaterialPropertyBlock propertyBlock;

        [SerializeField]
        [Tooltip("The renderer of the squeezable bounds box.")]
        private MeshRenderer boundsRenderer;

        /// <summary>
        /// The renderer of the squeezable bounds box.
        /// </summary>
        public MeshRenderer BoundsRenderer
        {
            get => boundsRenderer;
            set => boundsRenderer = value;
        }

        [SerializeField]
        [Tooltip("The parent transform of the affordances/handles.")]
        private Transform handlesContainer;

        /// <summary>
        /// The parent transform of the affordances/handles.
        /// </summary>
        public Transform HandlesContainer
        {
            get => handlesContainer;
            set => handlesContainer = value;
        }

        [SerializeField]
        [Tooltip("If true, all handles will be shown. If false, only the handles along the edge of the box will be shown.")]
        private bool showInternalHandles = false;

        /// <summary>
        /// Should internal handles be shown?
        /// </summary>
        public bool ShowInternalHandles
        {
            get => showInternalHandles;
            set => showInternalHandles = value;
        }

        [SerializeField]
        [Tooltip("Lerp time for smoothing the active focus parameter (fades in the box to indicate intent/focus)")]
        private float focusLerpTime = 0.01f;

        /// <summary>
        /// Lerp time for smoothing the active focus parameter (fades in the box to indicate intent/focus)
        /// </summary>
        public float FocusLerpTime
        {
            get => focusLerpTime;
            set => focusLerpTime = value;
        }

        [SerializeField]
        [Tooltip("Lerp time for smoothing the shrink/squeeze parameter (shrinks/squeezes the box based on variable select input)")]
        private float shrinkLerpTime = 0.0000000001f;

        /// <summary>
        /// Lerp time for smoothing the shrink/squeeze parameter (shrinks/squeezes the box based on variable select input)
        /// </summary>
        public float ShrinkLerpTime
        {
            get => shrinkLerpTime;
            set => shrinkLerpTime = value;
        }

        [SerializeField]
        [Tooltip("Lerp time for smoothing the activation parameter (changes color of box to indicate activation)")]
        private float activationLerpTime = 0.001f;

        /// <summary>
        /// Lerp time for smoothing the activation parameter (changes color of box to indicate activation)
        /// </summary>
        public float ActivationLerpTime
        {
            get => activationLerpTime;
            set => activationLerpTime = value;
        }

        // Reference to a BoundsControl component that
        // will inform this visual of flattening, manipulation events,
        // and active handle types.
        private BoundsControl boundsControl;

        // Gaze with intention, ray hover, etc
        private float activeFocus;

        // Squeeze/shrinking factor on the box.
        private float shrink;

        // Glow/color change of the box.
        private float activation;

        private List<BoundsHandleInteractable> handles;

        // Scratchpad for convex hull calculations.
        private List<HandlePoint> projectedHandles = new List<HandlePoint>();

        // Scratchpad for convex hull calculations.
        private Stack<HandlePoint> stack = new Stack<HandlePoint>();

        // The offset from the overall scale that the handle container should have
        // when un-shrunk. Used to match the handles to the shader effect.
        private Vector3 unpinchScaleOffset;

        // The offset from the overall scale that the handle container should have
        // when shrunk. Used to match the handles to the shader effect.
        private Vector3 pinchScaleOffset;

        #region Convex Hull Internals

        // Container for tracking both a handle's interactable and its screenspace-projected position.
        private struct HandlePoint
        {
            public BoundsHandleInteractable Handle;
            public Vector2 Position;
        }

        // Local positions of handles are extruded from the center of the cube by this amount to help avoid singularities.
        private static float convexPadding = 0.001f;

        // A static variable to store the anchors for our comparisons, as the comparison function is static
        // for performance reasons.
        private static Vector2 comparisonAnchor;

        // The vector along which this bounds is flattened.
        private Vector3 flattenVector;

        // Static Comparison<T> to enhance performance when sorting based on turn direction/polar angle.
        private static Comparison<HandlePoint> CompareHandlePoints = (a, b) => TurnDirection(comparisonAnchor, a.Position, b.Position);

        #endregion Convex Hull Private Fields

        private void Awake()
        {
            boundsControl = GetComponentInParent<BoundsControl>();

            // If we are attached to a BoundsControl,, use the BoundsControl's interactable.
            if (boundsControl != null)
            {
                interactable = boundsControl.Interactable;
            }

            propertyBlock = new MaterialPropertyBlock();
            handles = new List<BoundsHandleInteractable>(GetComponentsInChildren<BoundsHandleInteractable>(true));

            foreach (BoundsHandleInteractable handle in handles)
            {
                projectedHandles.Add(new HandlePoint() { Handle = handle, Position = Vector3.zero });
                handle.BoundsControlRoot = boundsControl;
            }

            // Read the box properties out of the material. This will inform our (un)pinchScaleOffsets to keep
            // the handles aligned.
            if (boundsRenderer != null)
            {
                GetBoxProperties(boundsRenderer, out float padding, out float shrinkFraction);
                unpinchScaleOffset = (Vector3.one * (2.0f * (padding)));
                pinchScaleOffset = (Vector3.one * (2.0f * (padding * shrinkFraction)));
            }
            

            // Compute flatten vector at startup.
            flattenVector = BoundsCalculator.CalculateFlattenVector(transform.lossyScale);
        }

        private void Update()
        {
            // Read state off of the BoundsControl component.
            bool handlesActive = boundsControl != null && boundsControl.HandlesActive;
            bool isManipulated = boundsControl != null && boundsControl.IsManipulated;

            // Compute smoothed focus, activation, and shrink values.
            float targetActiveFocus = 0;
            float targetActivation = 0;
            float targetShrink = 0;

            if (interactable != null)
            {
                targetActiveFocus = (interactable.IsActiveHovered || interactable.isSelected || handlesActive) ? 1 : 0;
                targetActivation = interactable.Selectedness();
                targetShrink = interactable.Selectedness();

                if (handlesActive)
                {
                    targetShrink = Mathf.Clamp(targetShrink, 0.8f, 1);
                }
            }
            else
            {
                targetActiveFocus = handlesActive ? 1 : 0;
                targetShrink = handlesActive ? 1 : 0;
            }

            targetActivation = Mathf.Clamp01(targetActivation + (isManipulated ? 1 : 0));

            // Smooth/animate our values.
            activeFocus = Smoothing.SmoothTo(activeFocus, targetActiveFocus, focusLerpTime, Time.deltaTime);
            activation = Smoothing.SmoothTo(activation, targetActivation, activationLerpTime, Time.deltaTime);
            shrink = Smoothing.SmoothTo(shrink, targetShrink, shrinkLerpTime, Time.deltaTime);

            // Write the values into the bounds renderer.
            if (boundsRenderer != null)
            {
                WritePropertyValues(boundsRenderer, propertyBlock, activeFocus, activation, shrink);
            }

            // If we are flat, we squeeze our local scale to the const flatten thickness.
            // Useful when flattening bounds on an object that isn't actually flat.
            if (boundsControl != null && boundsControl.IsFlat)
            {
                if (Mathf.Abs(flattenVector.x) > 0)
                {
                    transform.localScale = new Vector3(flattenThickness / transform.parent.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else if (Mathf.Abs(flattenVector.y) > 0)
                {
                    transform.localScale = new Vector3(transform.localScale.x, flattenThickness / transform.parent.localScale.y, transform.localScale.z);
                }
                else if (Mathf.Abs(flattenVector.z) > 0)
                {
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, flattenThickness / transform.parent.localScale.z);
                }
            }

            // If we have handles, update them.
            if (handlesContainer != null)
            {
                UpdateHandles(handlesActive,
                              boundsControl != null ? boundsControl.EnabledHandles : HandleType.None,
                              boundsControl != null && boundsControl.IsFlat);
            }
        }

        /// <summary>
        /// Writes the property values into the renderer. Override this if you have custom box visuals that use different material property bindings.
        /// </summary>
        /// <param name="renderer">The renderer to write to.</param>
        /// <param name="propertyBlock">The property block to use. Will be overwritten by this method.</param>
        /// <param name="focus">In a range [0,1], this controls the fade-in/out of the box.</param>
        /// <param name="activation">In a range [0,1], this changes the color of the box to the "active" color.</param>
        /// <param name="shrink">In a range [0,1], this shrinks the box from (size + padding * 2) to (size + (padding * 2 * shrinkFraction)</param>
        protected virtual void WritePropertyValues(MeshRenderer renderer, MaterialPropertyBlock propertyBlock, float focus, float activation, float shrink)
        {
            renderer.GetPropertyBlock(propertyBlock);
            // Fades the box in.
            propertyBlock.SetFloat("_Gaze_Focus_", focus);
            // Changes the color.
            propertyBlock.SetFloat("_Pinched_", activation);
            // Shrinks the box.
            propertyBlock.SetFloat("_Extra_Input_Progress_", shrink);
            renderer.SetPropertyBlock(propertyBlock);
        }

        /// <summary>
        /// Calculates the squeezable-box-specific values from the current material setup.
        /// </summary>
        /// <param name="renderer">The renderer to write to.</param>
        /// <param name="propertyBlock">The property block to use. Will be overwritten by this method.</param>
        /// <param name="padding">The original padding from the edge of the box geometry to the rendered box visual edge. World units.</param>
        /// <param name="shrinkFraction">The box will shrink to this fraction of the original padding when the shrink parameter = 1.</param>
        protected virtual void GetBoxProperties(MeshRenderer renderer, out float padding, out float shrinkFraction)
        {
            padding = renderer.material.GetFloat("_Bevel_Radius_");
            shrinkFraction = renderer.material.GetFloat("_Shrunk_Radius_Fraction_");

        }

        private static readonly ProfilerMarker UpdateHandlesPerfMarker =
            new ProfilerMarker("[MRTK] SqueezableBoxVisuals.UpdateHandles");

        private void UpdateHandles(bool handlesActive, HandleType activeHandleTypes, bool isFlat)
        {
            using (UpdateHandlesPerfMarker.Auto())
            {
                // Adjust the local scale of the handles container such that the handles match the edge of the shrinkable box.
                Vector3 scaleReciprocal = new Vector3(1.0f / handlesContainer.lossyScale.x, 1.0f / handlesContainer.lossyScale.y, 1.0f / handlesContainer.lossyScale.z);
                handlesContainer.localScale = Vector3.one + Vector3.Scale(Vector3.Lerp(unpinchScaleOffset, pinchScaleOffset, shrink), scaleReciprocal);

                foreach (var handle in handles)
                {
                    // Only show the handle if the type of the handle is one we want to show.
                    handle.enabled = handlesActive && ((activeHandleTypes & handle.HandleType) == handle.HandleType);

                    if (showInternalHandles)
                    {
                        handle.IsOccluded = false;
                        handle.IsFlattened = false;
                    }
                }

                // Depending on flattening, we either compute the flat-hiding or we compute the convex hull.
                if (handlesActive)
                {
                    if (isFlat)
                    {
                        HideFlattenedHandles();
                    }
                    else
                    {
                        // If we don't want to show the internal handles, we run the convex hull algorithm.
                        if (!showInternalHandles)
                        {
                            ConvexHull();
                        }
                    }
                }
            }
        }

        private static readonly ProfilerMarker ConvexHullPerfMarker =
            new ProfilerMarker("[MRTK] SqueezableBoxVisuals.ConvexHull");

        // Compute a convex hull of all handle points in screenspace.
        // Handles that are a member of the convex hull set (i.e., along the edge of the hull)
        // are considered visible and valid. Handles on the inside of the convex hull are invalid
        // and should be hidden.
        // Uses a Graham scan to compute the convex hull.
        // Read more at https://en.wikipedia.org/wiki/Graham_scan
        private void ConvexHull()
        {
            using (ConvexHullPerfMarker.Auto())
            {
                for (int i = 0; i < projectedHandles.Count; i++)
                {
                    projectedHandles[i] = new HandlePoint()
                    {
                        Handle = projectedHandles[i].Handle,
                        Position = Camera.main.WorldToScreenPoint(projectedHandles[i].Handle.transform.position + (projectedHandles[i].Handle.transform.position - transform.position).normalized * convexPadding),
                    };
                }

                comparisonAnchor = new Vector2(Mathf.Infinity, Mathf.Infinity);

                for (int i = 0; i < projectedHandles.Count; i++)
                {
                    if (projectedHandles[i].Position.y < comparisonAnchor.y)
                    {
                        comparisonAnchor = projectedHandles[i].Position;
                    }
                    else if (projectedHandles[i].Position.y == comparisonAnchor.y && projectedHandles[i].Position.x < comparisonAnchor.x)
                    {
                        comparisonAnchor = projectedHandles[i].Position;
                    }
                }

                projectedHandles.Sort(CompareHandlePoints);

                stack.Clear();
                foreach (HandlePoint handlePoint in projectedHandles)
                {
                    while (stack.Count > 1 && TurnDirection(PeekSecond(stack).Position, stack.Peek().Position, handlePoint.Position) > 0)
                    {
                        var popped = stack.Pop();
                        popped.Handle.IsOccluded = true;
                        popped.Handle.IsFlattened = false;
                    }
                    stack.Push(handlePoint);
                }

                foreach (var handle in stack)
                {
                    handle.Handle.IsOccluded = false;
                    handle.Handle.IsFlattened = false;
                }

                foreach (var handle in handles)
                {
                    handle.IsFlattened = false;
                }
            }
        }

        private static readonly ProfilerMarker HideFlattenedHandlesPerfMarker =
            new ProfilerMarker("[MRTK] SqueezableBoxVisuals.HideFlattenedHandles");

        // If the box is flattened, we compute which handles should be visible or not.
        // This is the alternative occlusion calculation to the convex hull.
        private void HideFlattenedHandles()
        {
            using (HideFlattenedHandlesPerfMarker.Auto())
            {
                // Determine whether the flatten vector points away from the user, or towards the user.
                // If the vector points away, we need to correct it to point towards the user.
                // Handles use this vector to determine their orientation for flattened visuals.
                bool flattenPointsAway = Vector3.Dot(handlesContainer.TransformVector(flattenVector),
                                                    handlesContainer.position - Camera.main.transform.position) > 0;
                if (flattenPointsAway)
                {
                    flattenVector = -flattenVector;
                }

                foreach (var handle in handles)
                {
                    // Inform each handle of their occlusion/flattening status.
                    Vector3 projection = Vector3.Project(handle.transform.localPosition, flattenVector);
                    handle.IsOccluded = !(Vector3.Dot(handle.transform.localPosition, flattenVector) > 0);
                    handle.IsFlattened = true;
                    handle.FlattenVector = flattenVector;
                }
            }
        }

        // Computes whether it is a clockwise or counterclockwise wind to go from point a to point b around the anchor.
        // Used to sort the points at the beginning of the Graham scan hull computation.
        private static int TurnDirection(Vector2 anchor, Vector2 a, Vector2 b)
        {
            float det = a.x * (b.y - anchor.y) + b.x * (anchor.y - a.y) + anchor.x * (a.y - b.y);

            if (det < -Mathf.Epsilon)
            {
                return 1;
            }
            else if (det > Mathf.Epsilon)
            {
                return -1;
            }
            else
            {
                return Vector2.SqrMagnitude(a - anchor) < Vector2.SqrMagnitude(b - anchor) ? -1 : 1;
            }
        }

        // Peek the second-from-top item on the stack.
        private T PeekSecond<T>(Stack<T> stack)
        {
            T temp = stack.Pop();
            T second = stack.Peek();
            stack.Push(temp);
            return second;
        }
    }
}
