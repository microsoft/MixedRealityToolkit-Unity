// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// The core behavior logic for a button that can be pressed, following the <see cref="StatefulInteractable"/> pattern.
    /// </summary>
    /// <remarks>
    /// This script does not make any assumptions about the visuals associated with this button. Any visuals
    /// script can listen to the <see cref="GetSelectionProgress"/> value, or call <see cref="PushPlaneLocalPosition"/>
    /// to obtain a selection progress value or a local displacement, respectively, to implement a visual layer.
    /// </remarks>
    [AddComponentMenu("MRTK/UX/Pressable Button")]
    public class PressableButton : StatefulInteractable
    {
        [SerializeField]
        [FormerlySerializedAs("smoothSelectedness")]
        [Tooltip("Should this button's selectionProgress be smoothed, or absolute?")]
        private bool smoothSelectionProgress = true;

        /// <summary>
        /// Should this button's selectionProgress be smoothed/animated, or absolute?
        /// </summary>
        public bool SmoothSelectionProgress { get => smoothSelectionProgress; set => smoothSelectionProgress = value; }

        /// <summary>
        /// An enumeration defining the coordinate space of plane distances.
        /// </summary>
        public enum SpaceMode
        {
            /// <summary>
            /// The world coordinate space system should be used.
            /// </summary>
            World,
            
            /// <summary>
            /// The button's local coordinate space system should be used.
            /// </summary>
            Local
        }

        [SerializeField]
        [Tooltip("Describes in which coordinate space the plane distances are stored and calculated")]
        private SpaceMode distanceSpaceMode = SpaceMode.Local;

        /// <summary>
        /// Describes in which coordinate space the plane distances are stored and calculated
        /// </summary>
        public SpaceMode DistanceSpaceMode
        {
            get => distanceSpaceMode;
            set
            {
                // Convert world to local distances and vice versa whenever we switch the mode
                if (value != distanceSpaceMode)
                {
                    distanceSpaceMode = value;
                    float scale = (distanceSpaceMode == SpaceMode.Local) ? WorldToLocalScale : LocalToWorldScale;

                    startPushPlane *= scale;
                    endPushPlane *= scale;
                }
            }
        }

        [SerializeField]
        [Tooltip("The local z-position of the push plane.")]
        private float startPushPlane = 0.0f;

        /// <summary>
        /// The local z-position of the push plane.
        /// </summary>        
        public float StartPushPlane { get => startPushPlane; set => startPushPlane = value; }

        [SerializeField]
        [Tooltip("The local z-position of the plane representing the end of the push displacement.")]
        private float endPushPlane = 0.2f;

        /// <summary>
        /// Maximum push distance. Distance is relative to the pivot of either the moving visuals
        /// if there's any or the button itself.
        /// </summary>
        public float EndPushPlane { get => endPushPlane; set => endPushPlane = value; }

        /// <summary>
        ///  Speed for retracting the moving button visuals on release.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed for retracting the moving button visuals on release.")]
        private float returnSpeed = 0.25f;

        [SerializeField]
        [Tooltip("Ensures that the button can only be pushed from the front. Touching the button from the back or side is prevented.")]
        private bool enforceFrontPush = true;

        /// <summary>
        /// Ensures that the button can only be pushed from the front. Touching the button from the back or side is prevented.
        /// </summary>
        public bool EnforceFrontPush { get => enforceFrontPush; set => enforceFrontPush = value; }

        [SerializeField]
        [FormerlySerializedAs("rejectXYRolloff")]
        [Tooltip("Ensures that the button click event only fires if the push did not roll off the button in the XY plane. Defaults to true.")]
        private bool rejectXYRollOff = true;

        /// <summary>
        /// Ensures that the button click event only fires if the push did not roll off the edge of the button in the XY plane.
        /// Defaults to true.
        /// </summary>
        public bool RejectXYRollOff { get => rejectXYRollOff; set => rejectXYRollOff = value; }

        [SerializeField]
        [FormerlySerializedAs("rolloffXYDepth")]
        [Tooltip("If RejectXYRollOff is enabled, roll-offs will be rejected within this normalized press amount behind the back plane.")]
        private float rollOffXYDepth = 3;

        /// <summary>/
        /// If RejectXYRollOff is enabled, roll-offs will be rejected within this normalized press amount of the back plane.
        /// </summary>
        public float RollOffXYDepth { get => rollOffXYDepth; set => rollOffXYDepth = value; }

        [SerializeField]
        [FormerlySerializedAs("rejectZRolloff")]
        [Tooltip("Ensures that the button click event only fires if the finger exited the button out the back instead of through the front plate. Defaults to false.")]
        private bool rejectZRollOff = false;

        /// <summary>
        /// Ensures that the button click event only fires if the finger exited the button
        /// out the back instead of through the front plate. Defaults to false.
        /// </summary>
        /// <remarks>
        /// People like to push buttons by sticking their finger all the way through, and out the back.
        /// Keep this property false if you want to allow this behavior.
        /// </remarks>
        public bool RejectZRollOff { get => rejectZRollOff; set => rejectZRollOff = value; }

        /// <summary>
        ///  Speed for extending the moving button visuals when selected by a non-touch source.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed for extending the moving button visuals when selected by a non-touch source.")]
        private float extendSpeed = 0.5f;

        #region Private Members

        /// <summary>
        /// Collection of IPokeInteractors that entered the button's press volume in an acceptable trajectory.
        /// This is a subset of the total hovering poke interactors.
        /// </summary>
        private HashSet<IPokeInteractor> validPokeInteractors = new HashSet<IPokeInteractor>();

        /// <summary>
        /// The internal selection progress value; StatefulInteractable.selectionProgress() is overridden
        /// to point to this value.
        /// </summary>
        private float selectionProgress;

        /// <summary>
        /// Transform for local to world space in the world direction of a press
        /// Multiply local scale positions by this value to convert to world space
        /// </summary>
        internal float LocalToWorldScale => (WorldToLocalScale != 0) ? 1.0f / WorldToLocalScale : 0.0f;

        /// <summary>
        /// Transform for world to local space in the world direction of press
        /// Multiply world scale positions by this value to convert to local space
        /// </summary>
        private float WorldToLocalScale => transform.InverseTransformVector(transform.forward).magnitude;

        /// <summary>
        /// If the <see cref="GetSelectionProgress"/> value is smoothed to within this threshold of 0 or 1, the <see cref="GetSelectionProgress"/> will snap to 0 or 1.
        /// </summary>
        private const float selectionProgressEpsilon = 0.00001f;

        #endregion Private Members

        /// <inheritdoc />
        public override float GetSelectionProgress() => selectionProgress;

        /// <inheritdoc />
        protected override bool CanClickOnFirstSelectEntered(SelectEnterEventArgs args)
        {
            return base.CanClickOnFirstSelectEntered(args) && !IsRolledOff();
        }

        /// <inheritdoc />
        protected override bool CanClickOnLastSelectExited(SelectExitEventArgs args)
        {
            return base.CanClickOnLastSelectExited(args) && !IsRolledOff();
        }

        /// <summary>
        /// Returns the position of the front press plane of the button,
        /// using the overall selectionProgress of the button, which includes
        /// all forms of selection enabled on the interactable.
        /// </summary>
        /// <remarks>
        /// If only a normalized 0...1 value for selectionProgress is needed,
        /// query <see cref="GetSelectionProgress" /> instead. Use this to determine the
        /// local position of where the front plate of a moving button would be.
        /// </remarks>
        public Vector3 PushPlaneLocalPosition()
        {
            float distance = MapToDistance(GetSelectionProgress());
            float localPushDistance = distanceSpaceMode == SpaceMode.Local ? distance : WorldToLocalScale * distance;
            return new Vector3(0, 0, localPushDistance);
        }

        /// <summary>
        /// Retrieves the 0..1 amount that the button is currently directly compressed by a finger.
        /// Returns true if at least one finger is actively pressing the button; false otherwise.
        /// </summary>
        /// <remarks>
        /// Excludes any other input mode, such as gaze-pinch, click, or other interactor.
        /// Use <see cref="GetSelectionProgress" /> to query the overall selectionProgress of this button.
        /// </remarks>
        public bool TryGetPressProgress(out float pokeAmount)
        {
            IPokeInteractor farthestInteractor = TryGetFarthestPressDistance(out float touchDistance);

            pokeAmount = Mathf.Clamp01(MapToPressProgress(touchDistance));

            return farthestInteractor != null;
        }

        /// <summary>
        /// Invoked on <see cref="PressableButton"/>, <see cref="Awake"/>, and <see cref="Reset"/> to apply required 
        /// settings to this <see cref="PressableButton"/> instance.
        /// </summary>
        /// <remarks>
        /// This is used to apply default and required configuration to the interactable that are
        /// common across all pressable interactables.
        /// </remarks>
        protected virtual void ApplyRequiredSettings()
        {
            // All buttons are multi-selectable.
            selectMode = InteractableSelectMode.Multiple;

            // You can't grab buttons.
            DisableInteractorType(typeof(IGrabInteractor));
        }

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            ApplyRequiredSettings();
        }

        /// <inheritdoc/>
        protected override void OnDisable()
        {
            // Reset the selectionProgress to 0 when the button is disabled.
            selectionProgress = 0.0f;
            base.OnDisable();
        }

        /// <summary>
        /// A Unity Editor only event function that is called when the script is loaded or a value changes in the Unity Inspector.
        /// </summary>
        private void OnValidate()
        {
            ApplyRequiredSettings();
        }

        /// <summary>
        /// A Unity event function that is called when the script should reset it's default values
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            ApplyRequiredSettings();
        }

        #region XRI methods

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            bool baseIsSelectable = base.IsSelectableBy(interactor);

            if (interactor is IPokeInteractor pokeInteractor)
            {
                float scaledFingerOffset = distanceSpaceMode == SpaceMode.Local ? pokeInteractor.PokeRadius * WorldToLocalScale : pokeInteractor.PokeRadius;
                float pressDistanceThisFrame = GetDistanceAlongPushDirection(pokeInteractor.PokeTrajectory.End) + scaledFingerOffset;
                float pressAmountThisFrame = MapToPressProgress(pressDistanceThisFrame);

                if (IsPokeSelected)
                {
                    // same as "b in BC", see heuristic below
                    bool pressedEnough = pressAmountThisFrame > DeselectThreshold && !IsOutsideFootprint(pokeInteractor.PokeTrajectory.End, 0.0001f);
                    return baseIsSelectable && pressedEnough;
                }
                else
                {
                    // If the trajectory's magnitude is zero (or unreported) we can skip
                    // the more complicated press heuristic and perform only a simple check.
                    if (Mathf.Approximately((pokeInteractor.PokeTrajectory.End - pokeInteractor.PokeTrajectory.Start).sqrMagnitude, 0))
                    {
                        // same as "b in BC", see heuristic below
                        bool pressedEnough = pressAmountThisFrame > SelectThreshold && !IsOutsideFootprint(pokeInteractor.PokeTrajectory.End, 0.0001f);
                        return baseIsSelectable && pressedEnough;
                    }

                    // Otherwise, we compute a ray-intersection heuristic to catch fast-moving presses reliably.
                    // We check the entry/exit intersections of the ray from last frame's poke point to this frame's
                    // poke point, and, based on a set of rules, compute whether a selection is possible.

                    //   a (last frame)
                    //    \
                    // ------------- A (front plane)
                    //     \
                    // ------------- B (select/deselect threshold)
                    //      \
                    // ------------- C (back plane)
                    //       \
                    //        b (this frame)

                    // Heuristic: Allow select iff (a in AB || ab intersects A) && (b in BC || ab intersects C)

                    // ab intersect A
                    float worldStartPlane = distanceSpaceMode == SpaceMode.Local ? startPushPlane * LocalToWorldScale : startPushPlane;
                    float intersectA = IntersectQuad(pokeInteractor.PokeTrajectory.Start, pokeInteractor.PokeTrajectory.End, worldStartPlane - pokeInteractor.PokeRadius);

                    // ab intersect C
                    float worldEndPlane = distanceSpaceMode == SpaceMode.Local ? endPushPlane * LocalToWorldScale : endPushPlane;
                    float intersectC = IntersectQuad(pokeInteractor.PokeTrajectory.Start, pokeInteractor.PokeTrajectory.End, worldEndPlane - pokeInteractor.PokeRadius);

                    bool abIntersectA = intersectA >= 0.0f;
                    bool abIntersectC = intersectC >= 0.0f;

                    // If both intersections occurred, the closest intersection wins.
                    if (abIntersectA && abIntersectC)
                    {
                        if (intersectA < intersectC)
                        {
                            abIntersectC = false;
                        }
                        else
                        {
                            abIntersectA = false;
                        }
                    }

                    // Do an immediate calculation here in the case that a PokeInteractor is already sufficiently pressing us before
                    // we've calculated selectionProgress() for this frame.
                    float pressDistanceLastFrame = GetDistanceAlongPushDirection(pokeInteractor.PokeTrajectory.Start) + scaledFingerOffset;
                    float pressAmountLastFrame = MapToPressProgress(pressDistanceLastFrame);

                    bool aInAB = pressAmountLastFrame > 0 && pressAmountLastFrame <= SelectThreshold && !IsOutsideFootprint(pokeInteractor.PokeTrajectory.End, 0.0001f);
                    bool bInBC = pressAmountThisFrame > SelectThreshold;

                    bool startCheck = aInAB || abIntersectA;
                    bool endCheck = bInBC || abIntersectC;

                    // Only allow poke selection from poke interactors that we deemed valid when they began their hovers.
                    // This ensures that back-press rejection, side-press rejection, etc are respected by the ray intersection heuristic.
                    if (!validPokeInteractors.Contains(pokeInteractor) && !(intersectA >= 0.0f)) { return false; }

                    return baseIsSelectable && startCheck && endCheck && pokeInteractor.isHoverActive;
                }
            }
            else
            {
                return baseIsSelectable;
            }
        }

        private static readonly ProfilerMarker ProcessInteractablePerfMarker =
            new ProfilerMarker("[MRTK] PressableButton.ProcessInteractor");

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            using (ProcessInteractablePerfMarker.Auto())
            {
                // We have the option to do time-based lerp'ing/smoothing/animating of button value,
                // so we do that here in a single-call-per-frame function. (The selectionProgress method
                // may be called any number of times per frame.)
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    bool isPoked = TryGetPressProgress(out float pokeAmount);

                    float totalPressProgress = pokeAmount;
                    bool isVariablySelected = false;

                    foreach (var interactor in interactorsHovering)
                    {
                        if (interactor is IVariableSelectInteractor variableSelectInteractor)
                        {
                            totalPressProgress = Mathf.Max(totalPressProgress, variableSelectInteractor.SelectProgress);
                            isVariablySelected = true;
                        }
                    }

                    foreach (var interactor in interactorsSelecting)
                    {
                        if (interactor is IVariableSelectInteractor variableSelectInteractor)
                        {
                            totalPressProgress = Mathf.Max(totalPressProgress, variableSelectInteractor.SelectProgress);
                            isVariablySelected = true;
                        }
                        else if (!(interactor is IPokeInteractor)) // Exclude PokeInteractors because we've already counted them.
                        {
                            totalPressProgress = 1.0f;
                        }
                    }

                    // If we aren't poking or pinching, we do a lerp to animate the pressProgress.
                    if (!isPoked && !isVariablySelected && smoothSelectionProgress)
                    {
                        // If total pressProgress is 1, we are extending. If zero, we are retracting.
                        // Todo: make framerate independent, but *equally snappy*. Smoothing.SmoothTo is
                        // unacceptably slow/smooth for this. We may end up exposing a custom curve here.
                        selectionProgress = Mathf.Lerp(selectionProgress, totalPressProgress, totalPressProgress == 1 ? extendSpeed : returnSpeed);

                        // Snap selectionProgress to ends of range, plus/minus selectionProgressEpsilon.
                        if (selectionProgress < selectionProgressEpsilon)
                        {
                            selectionProgress = 0;
                        }

                        if (selectionProgress > 1 - selectionProgressEpsilon)
                        {
                            selectionProgress = 1;
                        }
                    }
                    else
                    {
                        // Otherwise, we set the value to the actual pressProgress for maximum responsiveness.
                        selectionProgress = totalPressProgress;
                    }
                }
            }

            base.ProcessInteractable(updatePhase);
        }

        /// <inheritdoc/>
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);

            // If we decide this interactor has begun its hover in a well-behaved way,
            // we add it to our hash set of valid pokes.
            if (args.interactorObject is IPokeInteractor pokeInteractor && !IsOutsideFootprint(pokeInteractor.PokeTrajectory.End, 0.0001f))
            {
                float pressDistanceLastFrame = GetDistanceAlongPushDirection(pokeInteractor.PokeTrajectory.Start);
                float pressDistanceThisFrame = GetDistanceAlongPushDirection(pokeInteractor.PokeTrajectory.End);

                // Reject if we enforce front pushes and we're already pushing somewhere along the button!
                if (enforceFrontPush && pressDistanceThisFrame > startPushPlane && pressDistanceLastFrame > pressDistanceThisFrame)
                {
                    return;
                }

                validPokeInteractors.Add(pokeInteractor);
            }
        }

        /// <inheritdoc />
        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);

            if (args.interactorObject is IPokeInteractor pokeInteractor)
            {
                // Remove from our valid poke hash set if it was registered there.
                validPokeInteractors.Remove(pokeInteractor);
            }
        }

        #endregion XRI events

        #region Public Transform Utilities

        /// <summary>
        /// Returns local position along the push direction for the given local/global distance
        /// </summary>
        public Vector3 GetLocalPositionAlongPushDirection(float distance)
        {
            float localDistance = distanceSpaceMode == SpaceMode.Local ? distance : WorldToLocalScale * distance;
            return new Vector3(0, 0, localDistance);
        }

        /// <summary>
        /// Returns world space position along the push direction for the given local distance
        /// </summary>
        /// 
        internal Vector3 GetWorldPositionAlongPushDirection(float localDistance)
        {
            float distance = (distanceSpaceMode == SpaceMode.Local) ? localDistance * LocalToWorldScale : localDistance;
            return transform.position + transform.forward * distance;
        }

        /// <summary>
        /// Returns the local/global distance along the push direction for the passed in world position
        /// </summary>
        public float GetDistanceAlongPushDirection(Vector3 positionWorldSpace)
        {
            Vector3 localPosition = transform.InverseTransformPoint(positionWorldSpace);
            float localDistance = localPosition.z;
            return distanceSpaceMode == SpaceMode.Local ? localDistance : LocalToWorldScale * localDistance;
        }

        #endregion Public Transform Utilities

        #region Private Methods

        private static float InverseLerpUnclamped(float min, float max, float value) => (value - min) / (max - min);

        // Maps a 0...1 pressProgress amount to a distance between startPushPlane and endPushPlane.
        private float MapToDistance(float pressProgress) => Mathf.Lerp(startPushPlane, endPushPlane, pressProgress);

        // Maps a distance between startPushPlane and endPushPlane to a normalized press progress amount.
        private float MapToPressProgress(float distance) => InverseLerpUnclamped(startPushPlane, endPushPlane, distance);

        // This function projects the current touch positions onto the 1D press direction of the button.
        // It will output the farthest pushed distance from the button's initial position.
        // Returns the farthest-pressing interactor. If no interactors are touching the button, returns null.
        private IPokeInteractor TryGetFarthestPressDistance(out float distance)
        {
            // If we're not being touched/poked, we aren't pressed at all.
            if (!IsPokeHovered.Active)
            {
                distance = startPushPlane;
                return null;
            }

            float farthestDistance = Mathf.NegativeInfinity;
            IPokeInteractor farthestInteractor = null;

            foreach (var interactor in validPokeInteractors)
            {
                float scaledFingerOffset = distanceSpaceMode == SpaceMode.Local ? interactor.PokeRadius * WorldToLocalScale : interactor.PokeRadius;
                float testDistance = GetDistanceAlongPushDirection(interactor.PokeTrajectory.End) + scaledFingerOffset;

                if (testDistance > farthestDistance)
                {
                    farthestDistance = testDistance;
                    farthestInteractor = interactor;
                }
            }

            distance = Mathf.Clamp(farthestDistance, startPushPlane, Mathf.Infinity);
            return farthestInteractor;
        }

        // Returns true if the current farthest-pressing IPokeInteractor has "fallen off" the edge of the button.
        // Configured by the rejectXYRollOff and rejectZRollOff fields. 
        // If RejectXYRollOff is true, this method checks if the interactor has fallen off the 2D footprint of the button
        // in the X-Y plane.
        // If RejectZRollOff is true, this method checks if the finger has gone through the button and out the back.
        private bool IsRolledOff()
        {
            // Early-out if neither type of roll-off rejection is desired.
            if (!rejectXYRollOff && !rejectZRollOff)
            {
                return false;
            }

            // Get the interactor that is responsible for the current release/interaction.
            IPokeInteractor farthestInteractor = TryGetFarthestPressDistance(out float pressDistance);
            float pressAmount = MapToPressProgress(pressDistance);

            if (farthestInteractor != null)
            {
                // If we are configured to reject XY roll-off,
                // check whether we are outside the 2D footprint of the button.
                if (rejectXYRollOff &&
                    IsOutsideFootprint(farthestInteractor.PokeTrajectory.End, 0.0001f) &&
                    pressAmount < RollOffXYDepth)
                {
                    return true;
                }

                // If we are configured to reject Z roll-off,
                // check whether the touch point is beyond the far-push-plane.
                if (rejectZRollOff && pressDistance >= EndPushPlane)
                {
                    return true;
                }

            }

            return false;
        }

        // Returns true if the world-space touch point is outside the 2D X-Y local footprint of the button.
        private bool IsOutsideFootprint(Vector3 globalTouchPoint, float tolerance)
        {
            Vector3 closestPointOnCollider = colliders[0].ClosestPoint(globalTouchPoint);

            // Use InverseTransformDirection so we are scale-invariant.
            Vector3 localRollOffVector = colliders[0].transform.InverseTransformDirection(closestPointOnCollider - globalTouchPoint);

            // If the local roll-off vector's X or Y components are greater than the tolerance,
            // the touch point is outside the 2D footprint of the button.
            return Mathf.Abs(localRollOffVector.x) > tolerance ||
                   Mathf.Abs(localRollOffVector.y) > tolerance;
        }

        // Depth in world units
        private float IntersectQuad(Vector3 start, Vector3 end, float depth)
        {
            Plane plane = new Plane(transform.forward, transform.position + transform.forward * depth);
            bool hit = plane.Raycast(new Ray(start, end - start), out float enter);

            if (hit && enter <= (start - end).magnitude && !IsOutsideFootprint(start + (end - start) * enter, 0.0001f))
            {
                return enter;
            }
            else
            {
                return -1;
            }
        }

        #endregion Private Methods
    }
}
