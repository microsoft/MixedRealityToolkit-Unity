// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;
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
        IMRTKInteractorVisuals
    {
        #region HandJointInteractor

        /// <summary>
        /// Concrete implementations should override this function to specify the point
        /// at which the interaction occurs. This would be the tip of the index finger
        /// for a poke interactor, or some other computed position from other data sources.
        /// </summary>
        protected abstract bool TryGetInteractionPoint(out Pose pose);

        #endregion HandJointInteractor

        #region IHandedInteractor

        /// <inheritdoc/>
        Handedness IHandedInteractor.Handedness => (xrController is ArticulatedHandController handController) ? handController.HandNode.ToHandedness() : Handedness.None;

        #endregion IHandedInteractor

        #region IMRTKInteractorVisuals

        [Header("Interactor visuals settings")]

        [SerializeField]
        [Tooltip("The visuals representing the interaction point, such as a cursor, donut, or other marker.")]
        private GameObject touchVisuals;

        /// <summary>
        /// The visuals representing the interaction point, such as a cursor, donut, or other marker.
        /// </summary>
        public GameObject TouchVisuals { get => touchVisuals; set => touchVisuals = value; }

        private static readonly ProfilerMarker SetVisualsPerfMarker =
            new ProfilerMarker("[MRTK] HandJointInteractor.SetVisuals");

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

        #endregion IMRTKInteractorVisuals

        #region MonoBehaviour

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

        #endregion MonoBehaviour

        #region XRBaseInteractor

        /// <summary>
        /// Used to keep track of whether the controller has an interaction point.
        /// </summary>
        private bool interactionPointTracked;

        /// <inheritdoc />
        public override bool isHoverActive
        {
            get
            {
                // Only be available for hovering if the controller is tracked or we have joint data.
                return base.isHoverActive && (xrController.currentControllerState.inputTrackingState.HasPositionAndRotation() || interactionPointTracked);
            }
        }

        #endregion XRBaseInteractor

        #region XRBaseControllerInteractor

        private static readonly ProfilerMarker ProcessInteractorPerfMarker =
            new ProfilerMarker("[MRTK] HandJointInteractor.ProcessInteractor");

        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            using (ProcessInteractorPerfMarker.Auto())
            {
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    // Obtain near interaction point, and set our interactor's
                    // position/rotation to the interaction point's pose.
                    interactionPointTracked = TryGetInteractionPoint(out Pose interactionPose);
                    if (interactionPointTracked)
                    {
                        transform.SetPositionAndRotation(interactionPose.position, interactionPose.rotation);
                    }
                    else
                    {
                        // If we don't have a joint pose, reset to whatever our parent XRController's pose is.
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = Quaternion.identity;
                    }

                    // Ensure that the attachTransform tightly follows the interactor's transform
                    attachTransform.SetPositionAndRotation(transform.position, transform.rotation);

                    SetVisuals(isHoverActive);

                    // UpdateVisuals still needs to be defined, this is a placeholder for future functionality, hence why null is passed in as the argument
                    UpdateVisuals(null);
                }
            }
        }

        #endregion XRBaseControllerInteractor
    }
}
