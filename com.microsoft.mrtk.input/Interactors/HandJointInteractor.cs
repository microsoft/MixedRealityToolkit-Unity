// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An abstract XRDirectInteractor that represents a near interaction
    /// driven by articulated hand data. Implementers should define GetInteractionPoint
    /// to specify the desired interaction point, using the cached reference to
    /// the hands aggregator subsystem.
    /// </summary>
    public abstract class HandJointInteractor :
        XRDirectInteractor,
        IHandedInteractor,
        IMRTKInteractorVisuals,
        IColliderDisabledReceiver
    {
        [SerializeField]
        [Tooltip("The XRNode on which this hand is located.")]
        private XRNode handNode;

        /// <summary>
        /// The XRNode on which this hand is located.
        /// </summary>
        protected XRNode HandNode => handNode;

        private HandsAggregatorSubsystem handsAggregator;

        /// <summary>
        /// Cached reference to hands aggregator for efficient per-frame use.
        /// </summary>
        protected HandsAggregatorSubsystem HandsAggregator
        {
            get
            {
                if (handsAggregator == null)
                {
                    handsAggregator = HandsUtils.GetSubsystem();
                }
                return handsAggregator;
            }
        }

        public Handedness Handedness => HandNode.ToHandedness();

        /// <summary>
        /// Used to keep track of whether the controller has an interaction point.
        /// </summary>
        private bool interactionPointTracked;

        #region MonoBehaviour Implementation

        protected override void Start()
        {
            base.Start();

            // caching HandsAggregator Subsystem
            handsAggregator = HandsUtils.GetSubsystem();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Showing interactor visuals
            SetVisuals(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            // Hiding interactor visuals
            SetVisuals(false);
        }

        #endregion

        #region XRBaseControllerInteractor

        /// <inheritdoc />
        public override bool isHoverActive
        {
            get
            {
                // Only be available for hovering if the controller is tracked or we have joint data.
                return base.isHoverActive && (xrController.currentControllerState.inputTrackingState.HasPositionAndRotation() || interactionPointTracked);
            }
        }

        /// <summary>
        /// Concrete implementations should override this function to specify the point
        /// at which the interaction occurs. This would be the tip of the index finger
        /// for a poke interactor, or some other computed position from other data sources.
        /// </summary>
        protected abstract bool TryGetInteractionPoint(out HandJointPose jointPose);

        private static readonly ProfilerMarker ProcessInteractorPerfMarker =
            new ProfilerMarker("[MRTK] ArticulatedTouchInteractor.ProcessInteractor");

        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            using (ProcessInteractorPerfMarker.Auto())
            {
                if (handsAggregator == null)
                {
                    return;
                }

                // TODO, profile. Is this too expensive for OnBeforeRender?
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
                {
                    // Obtain near interaction point, and set our interactor's
                    // position/rotation to the interaction point's pose.
                    interactionPointTracked = TryGetInteractionPoint(out HandJointPose interactionPoint);
                    if (interactionPointTracked)
                    {
                        attachTransform.SetPositionAndRotation(interactionPoint.Position, interactionPoint.Rotation);
                    }
                    else
                    {
                        // If we have no valid tracked interaction point, reset to whatever our parent XRController's pose is.
                        attachTransform.localPosition = Vector3.zero;
                        attachTransform.localRotation = Quaternion.identity;
                    }

                    SetVisuals(isHoverActive);

                    // UpdateVisuals still needs to be defined, this is a placeholder for future functionality, hence why null is passed in as the argument
                    UpdateVisuals(null);
                }
            }
        }

        #endregion

        #region IMRTKInteractorVisuals

        [SerializeField]
        [Tooltip("The visuals representing the interaction point, such as a cursor, donut, or other marker")]
        private GameObject touchVisuals;

        /// <summary>
        /// The visuals representing the interaction point, such as a cursor, donut, or other marker.
        /// </summary>
        public GameObject TouchVisuals { get => touchVisuals; set => touchVisuals = value; }

        private static readonly ProfilerMarker SetVisualsPerfMarker =
            new ProfilerMarker("[MRTK] ArticulatedTouchInteractor.SetVisuals");

        /// <inheritdoc/>
        public virtual void SetVisuals(bool isVisible)
        {
            using (SetVisualsPerfMarker.Auto())
            {
                if (TouchVisuals == null) { return; }

                TouchVisuals.SetActive(isVisible);
            }
        }

        /// <inheritdoc/>
        public virtual void UpdateVisuals(XRBaseInteractable interactable)
        {
            if (TouchVisuals != null)
            {
                TouchVisuals.transform.SetPositionAndRotation(attachTransform.position, attachTransform.rotation);
            }
        }

        #endregion

        /// <inheritdoc/>
        void IColliderDisabledReceiver.NotifyColliderDisabled(Collider collider)
        {
            OnTriggerExit(collider);
        }
    }
}
