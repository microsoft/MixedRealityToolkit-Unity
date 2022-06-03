// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PokePath = Microsoft.MixedReality.Toolkit.IPokeInteractor.PokePath;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An interactor that is used for poking/pressing near interactions.
    /// </summary>
    [AddComponentMenu("Scripts/Microsoft/MRTK/Input/Poke Interactor")]
    public class PokeInteractor : XRBaseControllerInteractor, IPokeInteractor, IHandedInteractor, IMRTKInteractorVisuals
    {
        #region IHandedInteractor

        /// <inheritdoc />
        public Handedness Handedness => HandNode.ToHandedness();

        #endregion IHandedInteractor

        #region XRBaseInteractor

        /// <inheritdoc />
        public override bool isSelectActive => true;

        /// <inheritdoc />
        public override bool isHoverActive
        {
            get
            {
                // Only be available for hovering if the joint or controller is tracked.
                return base.isHoverActive && (xrController.currentControllerState.inputTrackingState.HasPositionAndRotation() || pokePointTracked);
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

        #endregion

        [SerializeField]
        [Tooltip("The XRNode on which this hand is located.")]
        private XRNode handNode;

        [SerializeField]
        [Tooltip("Which specific hand joint does this interactor track?")]
        private TrackedHandJoint joint;

        /// <summary>
        /// The XRNode on which this hand is located.
        /// </summary>
        protected XRNode HandNode => handNode;

        // Aggregator reference cache.
        private HandsAggregatorSubsystem handsAggregator;

        /// <summary>
        /// The default poke radius returned by <see cref="IPokeInteractor.PokeRadius"/>
        /// if no joint data is obtained.
        /// </summary>
        private const float defaultPokeRadius = 0.005f;

        #region IPokeInteractor

        /// <inheritdoc />
        public virtual float PokeRadius => defaultPokeRadius;

        private PokePath pokeTrajectory;

        /// <inheritdoc />
        public virtual PokePath PokeTrajectory => pokeTrajectory;

        #endregion

        private XROrigin origin;

        // Was our poking point tracked the last time we checked?
        // Ths will drive isHoverActive.
        private bool pokePointTracked;

        protected override void OnDisable()
        {
            base.OnDisable();

            // Hiding interactor visuals
            SetVisuals(false);
        }

        protected override void Awake()
        {
            base.Awake();
            handsAggregator = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
            origin = GetComponentInParent<XROrigin>();
            pokeTrajectory.Start = attachTransform.position;
            pokeTrajectory.End = attachTransform.position;
        }

        // Scratchpad for GetValidTargets. Spherecast hits and overlaps are recorded here.
        private HashSet<IXRInteractable> targets = new HashSet<IXRInteractable>();

        // Scratchpad for spherecast intersections.
        private RaycastHit[] results = new RaycastHit[8];

        // Scratchpad for collider overlaps.
        private Collider[] overlaps = new Collider[8];

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(this.targets);
        }

        /// <summary>
        /// Called during ProcessInteractor to obtain the poking pose. AttachTransform is set to this pose.
        /// Override to customize how poses are calculated.
        /// </summary>
        protected virtual bool TryGetPokePose(out HandJointPose jointPose)
        {
            if (handsAggregator != null && handsAggregator.TryGetJoint(joint, handNode, out jointPose))
            {
                return true;
            }
            else
            {
                jointPose = new HandJointPose();
                return false;
            }
        }

        private static readonly ProfilerMarker ProcessInteractorPerfMarker =
            new ProfilerMarker("[MRTK] PokeInteractor.ProcessInteractor");

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            using (ProcessInteractorPerfMarker.Auto())
            {
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    // The start of our new trajectory is the end of the last frame's trajectory.
                    pokeTrajectory.Start = pokeTrajectory.End;

                    // If we can get a joint pose, set our attachtransform accordingly.
                    if (TryGetPokePose(out HandJointPose jointPose))
                    {
                        // pokePointTracked sets isHoverActive.
                        pokePointTracked = true;
                        attachTransform.SetPositionAndRotation(jointPose.Position, jointPose.Rotation);
                    }
                    else
                    {
                        // If we don't have a joint pose, just reset the attachTransform back to neutral.
                        pokePointTracked = false;
                        attachTransform.localPosition = Vector3.zero;
                        attachTransform.localRotation = Quaternion.identity;
                    }

                    // The endpoint of our trajectory is the current attachTransform, regardless
                    // if this interactor set the attachTransform or whether we are just on a motion controller.
                    pokeTrajectory.End = attachTransform.position;

                    targets.Clear();

                    // If the trajectory is essentially stationary, we'll do a sphere overlap instead.
                    // SphereCasts return nothing if the start/end are the same.
                    if ((pokeTrajectory.End - pokeTrajectory.Start).sqrMagnitude <= 0.0001f)
                    {
                        int numOverlaps = UnityEngine.Physics.OverlapSphereNonAlloc(pokeTrajectory.End, PokeRadius, overlaps, UnityEngine.Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

                        for (int i = 0; i < numOverlaps; i++)
                        {
                            // Add intersections to target list.
                            if (interactionManager.TryGetInteractableForCollider(overlaps[i], out IXRInteractable interactable))
                            {
                                targets.Add(interactable);
                            }
                        }
                    }
                    else
                    {
                        // Otherwise, we perform a spherecast (essentially, a thick raycast)
                        // from the start to the end of the recorded trajectory, with a radius/thickness
                        // corresponding to the joint radius.
                        int numHits = UnityEngine.Physics.SphereCastNonAlloc(
                            pokeTrajectory.Start,
                            PokeRadius,
                            (pokeTrajectory.End - pokeTrajectory.Start).normalized,
                            results,
                            (pokeTrajectory.End - pokeTrajectory.Start).magnitude,
                            UnityEngine.Physics.DefaultRaycastLayers,
                            QueryTriggerInteraction.Ignore);

                        for (int i = 0; i < numHits; i++)
                        {
                            // Add intersections to target list.
                            if (interactionManager.TryGetInteractableForCollider(results[i].collider, out IXRInteractable interactable))
                            {
                                targets.Add(interactable);
                            }
                        }
                    }

                    // Update visuals (cursor)
                    SetVisuals(isHoverActive);
                    UpdateVisuals(interactablesHovered.Count > 0 ? interactablesHovered[0] as XRBaseInteractable : null);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(pokeTrajectory.Start, PokeRadius);
            Gizmos.DrawLine(pokeTrajectory.Start, pokeTrajectory.End);
            Gizmos.DrawSphere(pokeTrajectory.End, PokeRadius);
        }

        #region IMRTKInteractorVisuals

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
                TouchVisuals.transform.SetPositionAndRotation(pokeTrajectory.End, attachTransform.rotation);
            }
        }

        #endregion
    }
}
