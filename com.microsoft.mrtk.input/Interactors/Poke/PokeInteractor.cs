// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using PokePath = Microsoft.MixedReality.Toolkit.IPokeInteractor.PokePath;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An interactor that is used for poking/pressing near interactions.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Poke Interactor")]
    public class PokeInteractor :
        XRBaseControllerInteractor,
        IPokeInteractor,
        IHandedInteractor
    {
        #region PokeInteractor

        [SerializeReference]
        [InterfaceSelector(true)]
        [Tooltip("The pose source representing the poke pose")]
        private IPoseSource pokePoseSource;

        /// <summary>
        /// The pose source representing the poke pose
        /// </summary>
        protected IPoseSource PokePoseSource { get => pokePoseSource; set => pokePoseSource = value; }

        /// <summary>
        /// Called during ProcessInteractor to obtain the poking pose. <see cref="XRBaseInteractor.attachTransform"/> is set to this pose.
        /// Override to customize how poses are calculated.
        /// </summary>
        protected virtual bool TryGetPokePose(out Pose pose)
        {
            pose = Pose.identity;
            return PokePoseSource != null && PokePoseSource.TryGetPose(out pose);
        }

        /// <summary>
        /// Called during ProcessInteractor to obtain the poking radius. All raycasts and other physics detections
        /// are done according to this radius. Override to customize how the radius is calculated.
        /// </summary>
        protected virtual bool TryGetPokeRadius(out float radius)
        {
            HandJointPose jointPose = default;
            if (xrController is ArticulatedHandController handController
                && (XRSubsystemHelpers.HandsAggregator?.TryGetNearInteractionPoint(handController.HandNode, out jointPose) ?? false))
            {
                radius = jointPose.Radius;
                return true;
            }

            radius = default;
            return false;
        }

        #endregion PokeInteractor

        #region IHandedInteractor

        /// <inheritdoc />
        Handedness IHandedInteractor.Handedness => (xrController is ArticulatedHandController handController) ? handController.HandNode.ToHandedness() : Handedness.None;

        #endregion IHandedInteractor

        #region IPokeInteractor

        /// <summary>
        /// The default poke radius returned by <see cref="IPokeInteractor.PokeRadius"/>
        /// if no joint data is obtained.
        /// </summary>
        private const float DefaultPokeRadius = 0.005f;

        /// <inheritdoc />
        public virtual float PokeRadius => pokeRadius > 0 ? pokeRadius : DefaultPokeRadius;
        private float pokeRadius = 0.0f;

        /// <inheritdoc />
        public virtual PokePath PokeTrajectory => pokeTrajectory;
        private PokePath pokeTrajectory;

        #endregion IPokeInteractor

        #region MonoBehaviour

        protected override void Awake()
        {
            base.Awake();
            pokeTrajectory.Start = attachTransform.position;
            pokeTrajectory.End = attachTransform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(pokeTrajectory.Start, PokeRadius);
            Gizmos.DrawLine(pokeTrajectory.Start, pokeTrajectory.End);
            Gizmos.DrawSphere(pokeTrajectory.End, PokeRadius);
        }

        #endregion MonoBehaviour

        #region XRBaseInteractor

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(this.targets);
        }

        // Was our poking point tracked the last time we checked?
        // This will drive isHoverActive.
        private bool pokePointTracked;

        /// <inheritdoc/>
        public override bool isHoverActive
        {
            // Only be available for hovering if the joint or controller is tracked.
            get => base.isHoverActive && (xrController.currentControllerState.inputTrackingState.HasPositionAndRotation() || pokePointTracked);
        }

        /// <inheritdoc/>
        public override bool isSelectActive => true;

        // Scratchpad for GetValidTargets. Spherecast hits and overlaps are recorded here.
        private HashSet<IXRInteractable> targets = new HashSet<IXRInteractable>();

        // Scratchpad for spherecast intersections.
        private RaycastHit[] results = new RaycastHit[8];

        // Scratchpad for collider overlaps.
        private Collider[] overlaps = new Collider[8];

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

                    // pokePointTracked is used to help set isHoverActive.
                    pokePointTracked = TryGetPokePose(out Pose pose) && TryGetPokeRadius(out pokeRadius);
                    if (pokePointTracked)
                    {
                        // If we can get a joint pose, set our transform accordingly.
                        transform.SetPositionAndRotation(pose.position, pose.rotation);
                    }
                    else
                    {
                        // If we don't have a poke pose, reset to whatever our parent XRController's pose is.
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = Quaternion.identity;
                    }

                    // Ensure that the attachTransform tightly follows the interactor's transform
                    attachTransform.SetPositionAndRotation(transform.position, transform.rotation);

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
                }
            }
        }

        #endregion XRBaseInteractor
    }
}
