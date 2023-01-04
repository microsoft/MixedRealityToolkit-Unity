// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// ObjectManipulator allows for the manipulation (move, rotate, scale)
    /// of an object by any interactor with a valid attach transform.
    /// Multi-handed interactions and physics-enabled objects are also supported.
    /// </summary>
    /// <remarks>
    /// ObjectManipulator works with both rigidbody and non-rigidbody objects,
    /// and allows for throwing and catching interactions. Any interactor
    /// with a well-formed attach transform can interact with and manipulate
    /// an ObjectManipulator. This is a drop-in replacement for the built-in
    /// XRI XRGrabInteractable, that allows for flexible multi-handed interactions.
    /// ObjectManipulator doesn't track controller velocity, so for precise fast-paced
    /// throwing interactions that only need one hand, XRGrabInteractable may
    /// give better results.
    /// </remarks>
    [RequireComponent(typeof(PlacementHub))]
    [AddComponentMenu("MRTK/Spatial Manipulation/New Object Manipulator")]
    public class NewObjectManipulator : StatefulInteractable
    {
        #region Public Enums

        /// <summary>
        /// Describes what pivot the manipulated object will rotate about when
        /// you rotate your hand. This is not a description of any limits or
        /// additional rotation logic. If no other factors (such as constraints)
        /// are involved, rotating your hand by an amount should rotate the object
        /// by the same amount.
        /// For example a possible future value here is RotateAboutUserDefinedPoint
        /// where the user could specify a pivot that the object is to rotate
        /// around.
        /// An example of a value that should not be found here is MaintainRotationToUser
        /// as this restricts rotation of the object when we rotate the hand.
        /// </summary>
        public enum RotateAnchorType
        {
            RotateAboutObjectCenter,
            RotateAboutGrabPoint
        };

        [System.Flags]
        public enum ReleaseBehaviorType
        {
            KeepVelocity = 1 << 0,
            KeepAngularVelocity = 1 << 1
        }

        #endregion Public Enums

        #region Serialized Fields

        [SerializeField]
        [EnumFlags]
        [Tooltip("What kinds of manipulation should be allowed?")]
        private TransformFlags allowedManipulations = TransformFlags.Move | TransformFlags.Rotate | TransformFlags.Scale;

        /// <summary>
        /// What kinds of manipulation should be allowed?
        /// </summary>
        public TransformFlags AllowedManipulations
        {
            get => allowedManipulations;
            set => allowedManipulations = value;
        }

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand near")]
        private RotateAnchorType rotationAnchorNear = RotateAnchorType.RotateAboutGrabPoint;

        /// <summary>
        /// Rotation behavior of object when using one hand near
        /// </summary>
        public RotateAnchorType RotationAnchorNear
        {
            get => rotationAnchorNear;
            set => rotationAnchorNear = value;
        }

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand at distance")]
        private RotateAnchorType rotationAnchorFar = RotateAnchorType.RotateAboutGrabPoint;

        /// <summary>
        /// Rotation behavior of object when using one hand at distance
        /// </summary>
        public RotateAnchorType RotationAnchorFar
        {
            get => rotationAnchorFar;
            set => rotationAnchorFar = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("Rigid body behavior of the dragged object when releasing it.")]
        private ReleaseBehaviorType releaseBehavior = ReleaseBehaviorType.KeepVelocity | ReleaseBehaviorType.KeepAngularVelocity;

        /// <summary>
        /// Rigid body behavior of the dragged object when releasing it.
        /// </summary>
        public ReleaseBehaviorType ReleaseBehavior
        {
            get => releaseBehavior;
            set => releaseBehavior = value;
        }

        [FormerlySerializedAs("smoothingActive")]
        [SerializeField]
        [Tooltip("Frame-rate independent smoothing for far interactions. Far smoothing is enabled by default.")]
        private bool smoothingFar = true;

        /// <summary>
        /// Whether to enable frame-rate independent smoothing for far interactions.
        /// </summary>
        /// <remarks>
        /// Far smoothing is enabled by default.
        /// </remarks>
        public bool SmoothingFar
        {
            get => smoothingFar;
            set => smoothingFar = value;
        }

        [SerializeField]
        [Tooltip("Frame-rate independent smoothing for near interactions. Note that enabling near smoothing may be perceived as being 'disconnected' from the hand.")]
        private bool smoothingNear = true;

        /// <summary>
        /// Whether to enable frame-rate independent smoothing for near interactions.
        /// </summary>
        /// <remarks>
        /// Note that enabling near smoothing may be perceived as being 'disconnected' from the hand.
        /// </remarks>
        public bool SmoothingNear
        {
            get => smoothingNear;
            set => smoothingNear = value;
        }

        [Serializable]
        /// <summary>
        /// The SystemTypes for the desired type of manipulation logic for move, rotate, and scale.
        /// </summary>
        public struct LogicType
        {
            [SerializeField]
            [Tooltip("The concrete type of ManipulationLogic<Vector3> to use for moving.")]
            [Extends(typeof(ManipulationLogic<Vector3>), TypeGrouping.ByNamespaceFlat)]
            /// <summary>
            /// The concrete type of <see cref="ManipulationLogic"/> to use for moving.
            /// </summary>
            public SystemType moveLogicType;

            [SerializeField]
            [Tooltip("The concrete type of ManipulationLogic<Quaternion> to use for rotating.")]
            [Extends(typeof(ManipulationLogic<Quaternion>), TypeGrouping.ByNamespaceFlat)]
            /// <summary>
            /// The concrete type of <see cref="ManipulationLogic"/> to use for rotating.
            /// </summary>
            public SystemType rotateLogicType;

            [SerializeField]
            [Tooltip("The concrete type of ManipulationLogic<Vector3> to use for scaling.")]
            [Extends(typeof(ManipulationLogic<Vector3>), TypeGrouping.ByNamespaceFlat)]
            /// <summary>
            /// The concrete type of <see cref="ManipulationLogic"/> to use for scaling.
            /// </summary>
            public SystemType scaleLogicType;
        }

        [SerializeField]
        [Tooltip("The concrete types of ManipulationLogic<T> to use for manipulations.")]
        private LogicType manipulationLogicTypes = new LogicType
        {
            moveLogicType = typeof(MoveLogic),
            rotateLogicType = typeof(RotateLogic),
            scaleLogicType = typeof(ScaleLogic)
        };

        /// <summary>
        /// The concrete types of <see cref="ManipulationLogic<T>"/> to use for manipulations.
        /// </summary>
        /// <remarks>
        /// Setting this field at runtime can be expensive (reflection) and interrupt/break
        /// currently occurring manipulations. Use with caution. Best used at startup or when
        /// instantiating ObjectManipulators from code.
        /// </remarks>
        public LogicType ManipulationLogicTypes
        {
            get => manipulationLogicTypes;
            set
            {
                // Re-instantiating manip logics is expensive and can interrupt ongoing interactions.
                manipulationLogicTypes = value;
                InstantiateManipulationTransformations();
            }
        }

        #endregion Serialized Fields

        #region Protected Properties

        /// <summary>
        /// The current <see cref="InteractionFlags"/> for the current interaction.
        /// </summary>
        /// <remarks>
        /// Prioritizes near grab over ray selection, and ray selection over gaze selection.
        /// Will return a one-hot <see cref="InteractionFlags"/>.
        /// </remarks>
        protected virtual InteractionFlags CurrentInteractionType
        {
            get
            {
                if (IsGrabSelected)
                {
                    return InteractionFlags.Near;
                }
                else if (IsRaySelected)
                {
                    return InteractionFlags.Ray;
                }
                else if (IsGazePinchSelected)
                {
                    return InteractionFlags.Gaze;
                }
                else
                {
                    return InteractionFlags.Generic;
                }
            }
        }

        protected MoveTransformation moveManipulation;

        protected RotateTransformation rotateManipulation;

        protected ScaleTransformation scaleManipulation;

        #endregion Protected Properties

        #region Private Properties

        private PlacementHub placementHub;

        private bool ShouldSmooth => (IsGrabSelected && SmoothingNear) || (!IsGrabSelected && SmoothingFar);

        private bool wasSmoothed;

        private Rigidbody rigidBody;

        private bool wasGravity = false;

        private bool wasKinematic = false;

        // Reusable list for fetching interactionPoints from interactors.
        private List<Pose> interactionPoints = new List<Pose>();

        // Reusable list for fetching attachPoints from interactors.
        private List<Pose> attachPoints = new List<Pose>();

        // Reusable list for fetching grabPoints from interactors.
        private List<Pose> grabPoints = new List<Pose>();

        #endregion Private Properties

        #region MonoBehaviour Functions

        protected virtual void ApplyRequiredSettings()
        {
            // ObjectManipulator is never selected by poking.
            DisableInteractorType(typeof(IPokeInteractor));
        }

        protected override void Reset()
        {
            base.Reset();
            ApplyRequiredSettings();
            selectMode = InteractableSelectMode.Multiple;
        }

        private void OnValidate()
        {
            ApplyRequiredSettings();
        }

        protected override void Awake()
        {
            base.Awake();

            placementHub = transform.GetComponent<PlacementHub>();

            ApplyRequiredSettings();

            rigidBody = GetComponent<Rigidbody>();

            InstantiateManipulationTransformations();
        }

        #endregion

        private void InstantiateManipulationTransformations()
        {
            moveManipulation = new MoveTransformation(this);
            rotateManipulation = new RotateTransformation(this);
            scaleManipulation = new ScaleTransformation(this);

            moveManipulation.logic = Activator.CreateInstance(ManipulationLogicTypes.moveLogicType) as ManipulationLogic<Vector3>;
            rotateManipulation.logic = Activator.CreateInstance(ManipulationLogicTypes.rotateLogicType) as ManipulationLogic<Quaternion>;
            scaleManipulation.logic = Activator.CreateInstance(ManipulationLogicTypes.scaleLogicType) as ManipulationLogic<Vector3>;
        }

        /// <summary>
        /// Override this class to provide the transform of the reference frame (e.g. the camera) against which to compute the damping.
        ///
        /// This intended for the situation of FPS-style controllers moving forward at constant speed while holding an object,
        /// to prevent damping from pushing the body towards the player.
        /// </summary>
        /// <param name="args">Arguments of the OnSelectEntered event that called this function</param>
        /// <returns>The Transform that should be used to define the reference frame or null to use the global reference frame</returns>
        protected virtual Transform GetReferenceFrameTransform(SelectEnterEventArgs args) => null;

        private static readonly ProfilerMarker OnSelectEnteredPerfMarker =
            new ProfilerMarker("[MRTK] ObjectManipulator.OnSelectEntered");

        /// <inheritdoc />
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            using (OnSelectEnteredPerfMarker.Auto())
            {
                base.OnSelectEntered(args);

                wasSmoothed = placementHub.UseSmoothing;
                placementHub.UseSmoothing = (IsGrabSelected && SmoothingNear) || (!IsGrabSelected && SmoothingFar);

                // Only record rigidbody settings if this is the *first*
                // selection event! Otherwise, we'll record the during-interaction
                // rigidbody information, which we've already dirtied.
                if (rigidBody != null && interactorsSelecting.Count == 1)
                {
                    wasGravity = rigidBody.useGravity;
                    wasKinematic = rigidBody.isKinematic;

                    rigidBody.useGravity = false;
                    rigidBody.isKinematic = false;
                }

                // ideally, the reference frame should be that of the camera. Here the interactorObject transform is the best available alternative.
                placementHub.SetReferenceFrameTransform(GetReferenceFrameTransform(args));

                var initialTransform = new MixedRealityTransform(transform.position, transform.rotation, transform.localScale);

                moveManipulation.logic.Setup(interactorsSelecting, this, initialTransform);
                rotateManipulation.logic.Setup(interactorsSelecting, this, initialTransform);
                scaleManipulation.logic.Setup(interactorsSelecting, this, initialTransform);

                placementHub.Transformations.Add(scaleManipulation);
                placementHub.Transformations.Add(rotateManipulation);
                placementHub.Transformations.Add(moveManipulation);
            }
        }

        private static readonly ProfilerMarker OnSelectExitedPerfMarker =
            new ProfilerMarker("[MRTK] ObjectManipulator.OnSelectExited");

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            using (OnSelectExitedPerfMarker.Auto())
            {
                base.OnSelectExited(args);

                placementHub.UseSmoothing = wasSmoothed;

                // Only release the rigidbody (restore rigidbody settings/configuration)
                // if this is the last select event!
                if (rigidBody != null && interactorsSelecting.Count == 0)
                {
                    ReleaseRigidBody(rigidBody.velocity, rigidBody.angularVelocity);
                }

                placementHub.Transformations.Remove(moveManipulation);
                placementHub.Transformations.Remove(rotateManipulation);
                placementHub.Transformations.Remove(scaleManipulation);
            }
        }

        private static readonly ProfilerMarker ScaleLogicMarker = new ProfilerMarker("[MRTK] ScaleLogic.Update");
        private static readonly ProfilerMarker RotateLogicMarker = new ProfilerMarker("[MRTK] RotateLogic.Update");
        private static readonly ProfilerMarker MoveLogicMarker = new ProfilerMarker("[MRTK] MoveLogic.Update");

        private static readonly ProfilerMarker ObjectManipulatorProcessInteractableMarker =
            new ProfilerMarker("[MRTK] ObjectManipulator.ProcessInteractable");

        ///<inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            using (ObjectManipulatorProcessInteractableMarker.Auto())
            {
                base.ProcessInteractable(updatePhase);

                if(!isSelected)
                {
                    return;
                }

                // Evaluate user input in the UI Update() function.
                // If we are using physics, targetTransform is not applied directly but instead deferred
                // to the ApplyForcesToRigidbody() function called from FixedUpdate()
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    RotateAnchorType rotateType = CurrentInteractionType == InteractionFlags.Near ? RotationAnchorNear : RotationAnchorFar;
                    bool useCenteredAnchor = rotateType == RotateAnchorType.RotateAboutObjectCenter;
                    bool isOneHanded = interactorsSelecting.Count == 1;

                    using (ScaleLogicMarker.Auto())
                    {
                        if (allowedManipulations.IsMaskSet(TransformFlags.Scale))
                        {
                            scaleManipulation.interactorsSelecting = interactorsSelecting;
                            scaleManipulation.useCenteredAnchor = useCenteredAnchor;
                        }
                    }

                    using (RotateLogicMarker.Auto())
                    {
                        if (allowedManipulations.IsMaskSet(TransformFlags.Rotate))
                        {
                            rotateManipulation.interactorsSelecting = interactorsSelecting;
                            rotateManipulation.useCenteredAnchor = useCenteredAnchor;
                        }
                    }

                    using (MoveLogicMarker.Auto())
                    {
                        if (allowedManipulations.IsMaskSet(TransformFlags.Move))
                        {
                            moveManipulation.interactorsSelecting = interactorsSelecting;
                            moveManipulation.useCenteredAnchor = useCenteredAnchor;
                        }
                    }
                }
            }
        }

        private void ReleaseRigidBody(Vector3 velocity, Vector3 angularVelocity)
        {
            if (rigidBody != null)
            {
                rigidBody.useGravity = wasGravity;
                rigidBody.isKinematic = wasKinematic;

                // Match the object's velocity to the controller for near interactions
                // Otherwise keep the objects current velocity so that it's not dampened unnaturally
                if (IsGrabSelected)
                {
                    if ((releaseBehavior & ReleaseBehaviorType.KeepVelocity) == ReleaseBehaviorType.KeepVelocity)
                    {
                        rigidBody.velocity = velocity;
                    }

                    if ((releaseBehavior & ReleaseBehaviorType.KeepAngularVelocity) == ReleaseBehaviorType.KeepAngularVelocity)
                    {
                        rigidBody.angularVelocity = angularVelocity;
                    }
                }
            }
        }

        // TODO, may want to move this
        // into an extension method on the controller, or into some utility box.
        /// <summary>
        /// Gets the absolute device (grip) rotation associated with the specified interactor.
        /// Used to query actual grabbing rotation, vs a ray rotation.
        /// </summary>
        private bool TryGetGripRotation(IXRSelectInteractor interactor, out Quaternion rotation)
        {
            // We need to query the raw device rotation from the interactor; however,
            // the controller may have its rotation bound to the pointerRotation, which is unsuitable
            // for modeling rotations with far rays. Therefore, we cast down to the base TrackedDevice,
            // and query the device rotation directly. If any of this is un-castable, we return the
            // interactor's attachTransform's rotation.
            if (interactor is XRBaseControllerInteractor controllerInteractor &&
                controllerInteractor.xrController is ActionBasedController abController &&
                abController.rotationAction.action?.activeControl?.device is TrackedDevice device)
            {
                rotation = device.deviceRotation.ReadValue();
                return true;
            }

            rotation = interactor.GetAttachTransform(this).rotation;
            return true;
        }


        public class MoveTransformation : ITransformation
        {
            private IXRSelectInteractable interactable;

            public MoveTransformation(IXRSelectInteractable affectedInteractable)
            {
                interactable = affectedInteractable;
            }

            public ManipulationLogic<Vector3> logic;
            internal List<IXRSelectInteractor> interactorsSelecting;
            internal bool useCenteredAnchor;

            public int ExecutionPriority => throw new NotImplementedException();

            public (Vector3, Quaternion, Vector3) ApplyTransformation(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialLocalScale)
            {
                var targetTransform = new MixedRealityTransform(initialPosition, initialRotation, initialLocalScale);
                Vector3 newPosition = logic.Update(interactorsSelecting, interactable, targetTransform, useCenteredAnchor);
                return (newPosition, initialRotation, initialLocalScale);
            }
        }

        public class RotateTransformation : ITransformation
        {
            private IXRSelectInteractable interactable;

            public RotateTransformation(IXRSelectInteractable affectedInteractable)
            {
                interactable = affectedInteractable;
            }

            public ManipulationLogic<Quaternion> logic;
            internal List<IXRSelectInteractor> interactorsSelecting;
            internal bool useCenteredAnchor;

            public int ExecutionPriority => throw new NotImplementedException();

            public (Vector3, Quaternion, Vector3) ApplyTransformation(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialLocalScale)
            {
                var targetTransform = new MixedRealityTransform(initialPosition, initialRotation, initialLocalScale);
                Quaternion newRotation = logic.Update(interactorsSelecting, interactable, targetTransform, useCenteredAnchor);
                return (initialPosition, newRotation, initialLocalScale);
            }
        }

        public class ScaleTransformation : ITransformation
        {
            private IXRSelectInteractable interactable;

            public ScaleTransformation(IXRSelectInteractable affectedInteractable)
            {
                interactable = affectedInteractable;
            }

            public ManipulationLogic<Vector3> logic;
            internal List<IXRSelectInteractor> interactorsSelecting;
            internal bool useCenteredAnchor;

            public int ExecutionPriority => throw new NotImplementedException();

            public (Vector3, Quaternion, Vector3) ApplyTransformation(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialLocalScale)
            {
                var targetTransform = new MixedRealityTransform(initialPosition, initialRotation, initialLocalScale);
                Vector3 newScale = logic.Update(interactorsSelecting, interactable, targetTransform, useCenteredAnchor);
                return (initialPosition, initialRotation, newScale);
            }
        }
    }
}
