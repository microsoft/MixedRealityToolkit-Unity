// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// The core behavior logic for a pressable button, following the <see cref="StatefulInteractable"/> pattern.
    /// </summary>
    /// <remarks>
    /// This script does not make any assumptions about the visuals associated with this button; any visuals
    /// script can listen to the <see cref="Selectedness"/> value, or call <see cref="PushPlaneLocalPosition">
    /// to obtain a 0...1 selectedness value or a local displacement, respectively, to implement a visual layer.
    /// </remarks>
    [AddComponentMenu("MRTK/UX/Pressable Button")]
    public class PressableButton : StatefulInteractable
    {
        [SerializeField]
        [Tooltip("Should this button's selectedness be smoothed, or absolute?")]
        private bool smoothSelectedness = true;

        /// <summary>
        /// Should this button's selectedness be smoothed/animated, or absolute?
        /// </summary>
        public bool SmoothSelectedness { get => smoothSelectedness; set => smoothSelectedness = value; }

        /// <summary>
        /// An enumeration defining the coordinate space of plane distances.
        /// </summary>
        public enum SpaceMode
        {
            /// <summary>
            /// The world coordinate space system should be used.
            // </summary>
            World,
            
            /// <summary>
            /// The button's local coordinate space system should be used.
            // </summary>
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
        protected float startPushPlane = 0.0f;

        /// <summary>
        /// The local z-position of the push plane.
        /// TODO: should be renamed to start push plane distance?
        /// </summary>
        public float StartPushPlane { get => startPushPlane; set => startPushPlane = value; }

        [SerializeField]
        [Tooltip("The local z-position of the plane representing the end of the push displacement.")]
        private float endPushPlane = 0.2f;

        /// <summary>
        /// Maximum push distance. Distance is relative to the pivot of either the moving visuals
        /// if there's any or the button itself.
        /// TODO: should be renamed to end push plane distance?
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
        [Tooltip("Ensures that the button click event only fires if the push did not roll off the button in the XY plane. Defaults to true.")]
        private bool rejectXYRolloff = true;

        /// <summary>
        /// Ensures that the button click event only fires if the push did not roll off the edge of the button in the XY plane.
        /// Defaults to true.
        /// </summary>
        public bool RejectXYRolloff { get => rejectXYRolloff; set => rejectXYRolloff = value; }

        [SerializeField]
        [Tooltip("If RejectXYRolloff is enabled, rolloffs will be rejected within this normalized press amount behind the backplane.")]
        private float rolloffXYDepth = 3;

        /// <summary>/
        /// If RejectXYRolloff is enabled, rolloffs will be rejected within this normalized press amount of the backplane.
        /// </summary>
        public float RolloffXYDepth { get => rolloffXYDepth; set => rolloffXYDepth = value; }

        [SerializeField]
        [Tooltip("Ensures that the button click event only fires if the finger exited the button out the back instead of through the frontplate. Defaults to false.")]
        private bool rejectZRolloff = false;

        /// <summary>
        /// Ensures that the button click event only fires if the finger exited the button
        /// out the back instead of through the frontplate. Defaults to false.
        /// </summary>
        /// <remarks>
        /// People like to push buttons by sticking their finger all the way through, and out the back.
        /// Keep this property false if you want to allow this behavior.
        /// </remarks>
        public bool RejectZRolloff { get => rejectZRolloff; set => rejectZRolloff = value; }

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
        /// The internal selectedness value; StatefulInteractable.Selectedness() is overridden
        /// to point to this value.
        /// </summary>
        private float selectedness;

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
        /// If the selectedness lerps to within this threshold of 0 or 1, the selectedness will snap to 0 or 1.
        /// </summary>
        private const float selectednessEpsilon = 0.00001f;

        #endregion Private Members

        /// <inheritdoc />
        public override float Selectedness() => selectedness;

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
        /// using the overall Selectedness of the button, which includes
        /// all forms of selection enabled on the interactable.
        /// </summary>
        /// <remarks>
        /// If only a normalized 0...1 value for selectedness is needed,
        /// query <see cref="Selectedness"> instead. Use this to determine the
        /// local position of where the front plate of a moving button would be.
        /// </remarks>
        public Vector3 PushPlaneLocalPosition()
        {
            float distance = MapToDistance(Selectedness());
            float localPushDistance = distanceSpaceMode == SpaceMode.Local ? distance : WorldToLocalScale * distance;
            return new Vector3(0, 0, localPushDistance);
        }

        /// <summary>
        /// Retrieves the 0..1 amount that the button is currently directly compressed by a finger.
        /// Returns true if at least one finger is actively pressing the button; false otherwise.
        /// </summary>
        /// <remarks>
        /// Excludes any other input mode, such as gaze-pinch, click, or other interactor.
        /// Use <see cref="Selectedness"> to query the overall selectedness of this button.
        /// </remarks>
        public bool TryGetPressedness(out float pokeAmount)
        {
            IPokeInteractor farthestInteractor = TryGetFarthestPressDistance(out float touchDistance);

            pokeAmount = Mathf.Clamp01(MapToPressedness(touchDistance));

            return farthestInteractor != null;
        }

        /// <summary>
        /// Applies default/required configuration to the interactable that are
        /// common across all pressable interactables.
        /// </summary>
        protected virtual void ApplyRequiredSettings()
        {
            // All buttons are multi-selectable.
            selectMode = InteractableSelectMode.Multiple;

            // You can't grab buttons.
            DisableInteractorType(typeof(IGrabInteractor));
        }

        protected override void Awake()
        {
            base.Awake();
            ApplyRequiredSettings();
        }

        protected override void OnDisable()
        {
            // Reset the selectedness to 0 when the button is disabled.
            selectedness = 0.0f;
            base.OnDisable();
        }

        private void OnValidate()
        {
            ApplyRequiredSettings();
        }

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
                float pressAmountThisFrame = MapToPressedness(pressDistanceThisFrame);

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
                    // we've calculated Selectedness() for this frame.
                    float pressDistanceLastFrame = GetDistanceAlongPushDirection(pokeInteractor.PokeTrajectory.Start) + scaledFingerOffset;
                    float pressAmountLastFrame = MapToPressedness(pressDistanceLastFrame);

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
                // We have the option to do time-based lerping/smoothing/animating of button value,
                // so we do that here in a single-call-per-frame function. (The Selectedness method
                // may be called any number of times per frame.)
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    bool isPoked = TryGetPressedness(out float pokeAmount);

                    float totalPressedness = pokeAmount;
                    bool isVariablySelected = false;

                    foreach (var interactor in interactorsHovering)
                    {
                        if (interactor is IVariableSelectInteractor variableSelectInteractor)
                        {
                            totalPressedness = Mathf.Max(totalPressedness, variableSelectInteractor.SelectProgress);
                            isVariablySelected = true;
                        }
                    }

                    foreach (var interactor in interactorsSelecting)
                    {
                        if (interactor is IVariableSelectInteractor variableSelectInteractor)
                        {
                            totalPressedness = Mathf.Max(totalPressedness, variableSelectInteractor.SelectProgress);
                            isVariablySelected = true;
                        }
                        else if (!(interactor is IPokeInteractor)) // Exclude PokeInteractors because we've already counted them.
                        {
                            totalPressedness = 1.0f;
                        }
                    }

                    // If we aren't poking or pinching, we do a lerp to animate the pressedness.
                    if (!isPoked && !isVariablySelected && smoothSelectedness)
                    {
                        // If total pressedness is 1, we are extending. If zero, we are retracting.
                        // Todo: make framerate independent, but *equally snappy*. Smoothing.SmoothTo is
                        // unacceptably slow/smooth for this. We may end up exposing a custom curve here.
                        selectedness = Mathf.Lerp(selectedness, totalPressedness, totalPressedness == 1 ? extendSpeed : returnSpeed);

                        // Snap selectedness to ends of range, plus/minus selectednessEpsilon.
                        if (selectedness < selectednessEpsilon)
                        {
                            selectedness = 0;
                        }

                        if (selectedness > 1 - selectednessEpsilon)
                        {
                            selectedness = 1;
                        }
                    }
                    else
                    {
                        // Otherwise, we set the value to the actual pressedness for maximum responsiveness.
                        selectedness = totalPressedness;
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
            // we add it to our hashset of valid pokes.
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
                // Remove from our valid poke hashset if it was registered there.
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

        // Maps a 0...1 pressedness amount to a distance between startPushPlane and endPushPlane.
        private float MapToDistance(float pressedness) => Mathf.Lerp(startPushPlane, endPushPlane, pressedness);

        // Maps a distance between startPushPlane and endPushPlane to a normalized pressedness amount.
        private float MapToPressedness(float distance) => InverseLerpUnclamped(startPushPlane, endPushPlane, distance);

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
        // Configured by the rejectXYRolloff and rejectZRolloff fields. 
        // If RejectXYRolloff is true, this method checks if the interactor has fallen off the 2D footprint of the button
        // in the X-Y plane.
        // If RejectZRolloff is true, this method checks if the finger has gone through the button and out the back.
        private bool IsRolledOff()
        {
            // Early-out if neither type of rolloff rejection is desired.
            if (!rejectXYRolloff && !rejectZRolloff)
            {
                return false;
            }

            // Get the interactor that is responsible for the current release/interaction.
            IPokeInteractor farthestInteractor = TryGetFarthestPressDistance(out float pressDistance);
            float pressAmount = MapToPressedness(pressDistance);

            if (farthestInteractor != null)
            {
                // If we are configured to reject XY rolloff,
                // check whether we are outside the 2D footprint of the button.
                if (rejectXYRolloff &&
                    IsOutsideFootprint(farthestInteractor.PokeTrajectory.End, 0.0001f) &&
                    pressAmount < RolloffXYDepth)
                {
                    return true;
                }

                // If we are configured to reject Z rolloff,
                // check whether the touch point is beyond the far-push-plane.
                if (rejectZRolloff && pressDistance >= EndPushPlane)
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
            Vector3 localRolloffVector = colliders[0].transform.InverseTransformDirection(closestPointOnCollider - globalTouchPoint);

            // If the local rolloff vector's X or Y components are greater than the tolerance,
            // the touch point is outside the 2D footprint of the button.
            return Mathf.Abs(localRolloffVector.x) > tolerance ||
                   Mathf.Abs(localRolloffVector.y) > tolerance;
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
