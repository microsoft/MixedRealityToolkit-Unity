// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A wrapper for the XRRayInteractor which stores extra information for MRTK management/services
    /// </summary>
    [AddComponentMenu("Scripts/Microsoft/MRTK/Input/MRTK Ray Interactor")]
    public class MRTKRayInteractor : XRRayInteractor, IRayInteractor, IHandedInteractor, IVariableSelectInteractor
    {
        /// <summary>
        /// Is this ray currently hovering a UnityUI/Canvas element?
        /// </summary>
        public bool HasUIHover => TryGetUIModel(out TrackedDeviceModel model) && model.currentRaycast.isValid;

        /// <summary>
        /// Is this ray currently selecting a UnityUI/Canvas element?
        /// </summary>
        public bool HasUISelection => HasUIHover && isUISelectActive;

        public Handedness Handedness => (xrController is ArticulatedHandController handController) ? handController.HandNode.ToHandedness() : Handedness.None;

        /// <summary>
        /// Cached reference to hands aggregator for efficient per-frame use.
        /// </summary>
        private HandsAggregatorSubsystem handsAggregator;

        /// <summary>
        /// Used to check if the parent controller is tracked or not
        /// Hopefully this becomes part of the base Unity XRI API.
        /// </summary>
        private bool IsTracked => xrController.currentControllerState.inputTrackingState.HasPositionAndRotation();

        /// <summary>
        /// How unselected the interactor must be to initiate a new hover or selection on a new target.
        /// Separate from the hand controller's threshold for pinchedness, so that we can tune
        /// overall pinchedness separately from the roll-off prevention.
        /// Should be [0,1].
        /// </summary>
        /// <remarks>
        /// May be made serialized + exposed in the future.
        /// Larger than the relaxation threshold on <see cref="GazePinchInteractor"/>, as fewer
        /// accidental activations will occur with rays.
        /// <remarks>
        protected internal float relaxationThreshold = 0.5f;

        // Whether the hand has relaxed (i.e. fully unselected) pre-selection.
        // Used to prevent accidental activations by requiring a full selection motion to complete.
        private bool isRelaxedBeforeSelect = false;

        private float refDistance = 0;

        private Pose initialLocalAttach = Pose.identity;

        #region IVariableSelectInteractor

        /// <inheritdoc />
        public float SelectProgress => xrController.selectInteractionState.value;

        #endregion

        #region XRBaseControllerInteractor

        /// <inheritdoc />
        public override bool CanHover(IXRHoverInteractable interactable)
        {
            // We stay hovering if we have selected anything.
            bool stickyHover = !hasSelection || IsSelecting(interactable);

            // We are ready to pinch if we are in the PinchReady position,
            // or if we are already selecting something.
            bool ready = isHoverActive || isSelectActive;

            // Is this a new interactable that we aren't already hovering?
            bool isNew = !IsHovering(interactable);

            // If so, should we be allowed to initiate a new hover on it?
            // This prevents us from "rolling off" one target and immediately
            // semi-pressing another.
            bool canHoverNew = !isNew || SelectProgress < relaxationThreshold;

            return base.CanHover(interactable) && stickyHover && ready && canHoverNew;
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            var selectActivated = base.CanSelect(interactable);

            return base.CanSelect(interactable) && (!hasSelection || IsSelecting(interactable)) && isRelaxedBeforeSelect;
        }

        /// <inheritdoc />
        public override bool isHoverActive
        {
            get
            {
                // When the gaze pinch interactor is already selecting an object, use the default interactor behavior
                if (hasSelection)
                {
                    return base.isHoverActive;
                }
                // Otherwise, this selector is only allowed to hover if we can tell that the palm for the corresponding hand/controller is facing away from the user.
                else
                {
                    bool hoverActive = base.isHoverActive;
                    if (hoverActive)
                    {
                        if (xrController is ArticulatedHandController handController)
                        {
                            // Get hands aggregator subsystem reference, if still null.
                            // Should avoid per-frame allocs by only acquiring aggregator reference once.
                            if (handsAggregator == null)
                            {
                                handsAggregator = HandsUtils.GetSubsystem();
                            }

                            bool isPalmFacingAway = false;
                            if (handsAggregator?.TryGetPalmFacingAway(handController.HandNode, out isPalmFacingAway) ?? true)
                            {
                                hoverActive &= isPalmFacingAway;
                            }
                        }
                    }

                    return hoverActive && IsTracked;
                }
            }
        }

        private static readonly ProfilerMarker ProcessInteractorPerfMarker =
            new ProfilerMarker("[MRTK] MRTKRayInteractor.ProcessInteractor");

        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            using (ProcessInteractorPerfMarker.Auto())
            {
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    // If we've fully relaxed, we can begin hovering/selecting a new target.
                    if (SelectProgress < relaxationThreshold)
                    {
                        isRelaxedBeforeSelect = true;
                    }
                    // If we're not relaxed, and we aren't currently hovering or selecting anything,
                    // we can't initiate new hovers or selections.
                    else if (!hasHover && !hasSelection)
                    {
                        isRelaxedBeforeSelect = false;
                    }

                    if (xrController is ActionBasedController abController &&
                            abController.rotationAction.action?.activeControl?.device is TrackedDevice device)
                    {
                        attachTransform.rotation = PlayspaceUtilities.ReferenceTransform.rotation * device.deviceRotation.ReadValue();
                    }

                    if (hasSelection)
                    {
                        float distanceRatio = PoseUtilities.GetDistanceToBody(new Pose(transform.position, transform.rotation)) / refDistance;
                        attachTransform.localPosition = new Vector3(initialLocalAttach.position.x, initialLocalAttach.position.y, initialLocalAttach.position.z * distanceRatio);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            initialLocalAttach = new Pose(attachTransform.localPosition, attachTransform.localRotation);
            refDistance = PoseUtilities.GetDistanceToBody(new Pose(transform.position, transform.rotation));
        }

        #endregion
    }
}
