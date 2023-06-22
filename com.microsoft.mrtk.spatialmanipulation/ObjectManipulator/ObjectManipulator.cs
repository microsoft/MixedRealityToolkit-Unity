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
    [RequireComponent(typeof(ConstraintManager))]
    [AddComponentMenu("MRTK/Spatial Manipulation/Object Manipulator")]
    public class ObjectManipulator : StatefulInteractable
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
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform = null;

        /// <summary>
        /// Transform to be manipulated. Defaults to the object of the component.
        /// </summary>
        public Transform HostTransform
        {
            get
            {
                if (hostTransform == null)
                {
                    hostTransform = gameObject.transform;
                }

                return hostTransform;
            }
            set
            {
                if (interactorsSelecting.Count != 0)
                {
                    Debug.LogWarning("Changing the host transform while the object is being manipulated is not yet supported. " + 
                        "Check interactorsSelecting.Count before changing the host transform.");
                    return;
                }
                if (hostTransform != value )
                {
                    hostTransform = value;

                    // If we're using constraints, make sure to re-initialize
                    // the constraints manager with a fresh HostTransform.
                    if (constraintsManager != null)
                    {
                        constraintsManager.Setup(new MixedRealityTransform(HostTransform));
                    }
                  
                    // Re-aquire reference to the rigidbody.
                    rigidBody = HostTransform.GetComponent<Rigidbody>();
                }
            }
        }

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
        [Tooltip("Which types of interactions are allowed to manipulate this object?")]
        private InteractionFlags allowedInteractionTypes = InteractionFlags.Near | InteractionFlags.Ray | InteractionFlags.Gaze | InteractionFlags.Generic;

        /// <summary>
        /// Which types of interactions are allowed to manipulate this object?
        /// </summary>
        public InteractionFlags AllowedInteractionTypes
        {
            get => allowedInteractionTypes;
            set => allowedInteractionTypes = value;
        }

        [SerializeField]
        [Tooltip(
            "Apply torque to control orientation of the body")]
        private bool applyTorque = true;

        /// <summary>
        /// Apply torque to control orientation of the body
        /// </summary>
        public bool ApplyTorque
        {
            get => applyTorque;
            set => applyTorque = value;
        }

        [SerializeField]
        [Range(0.001f, 2.0f)]
        [Tooltip("The time scale at which a Rigidbody reacts to input movement defined as oscillation period of the dampened spring force.")]
        private float springForceSoftness = 0.1f;

        /// <summary>
        /// The time scale at which a Rigidbody reacts to input movement defined as oscillation period of the dampened spring force.
        /// </summary>
        public float SpringForceSoftness
        {
            get => springForceSoftness;
            set => springForceSoftness = value;
        }

        [SerializeField]
        [Range(0.001f, 2.0f)]
        [Tooltip("The time scale at which a Rigidbody reacts to input rotation defined as oscillation period of the dampened spring torque.")]
        private float springTorqueSoftness = 0.1f;

        /// <summary>
        /// The time scale at which a Rigidbody reacts to input rotation defined as oscillation period of the dampened angular spring force.
        /// </summary>
        public float SpringTorqueSoftness
        {
            get => springTorqueSoftness;
            set => springTorqueSoftness = value;
        }

        [SerializeField]
        [Range(0, 2.0f)]
        [Tooltip("The damping of the spring force&torque: 1.0f corresponds to critical damping, lower values lead to underdamping (i.e. oscillation).")]
        private float springDamping = 1.0f;

        /// <summary>
        /// The damping of the spring force&torque: 1.0f corresponds to critical damping, lower values lead to underdamping (i.e. oscillation).
        /// </summary>
        public float SpringDamping
        {
            get => springDamping;
            set => springDamping = value;
        }

        [SerializeField]
        [Range(0, 10000f)]
        [Tooltip("The maximum acceleration applied by the spring force to avoid trembling when pushing a body against a static object.")]
        private float springForceLimit = 100.0f;

        /// <summary>
        /// The maximum acceleration applied by the spring force to avoid trembling when pushing a body against a static object.
        /// </summary>
        public float SpringForceLimit
        {
            get => springForceLimit;
            set => springForceLimit = value;
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

        [SerializeField]
        [Tooltip("The concrete type of TransformSmoothingLogic to use for smoothing between transforms.")]
        [Implements(typeof(ITransformSmoothingLogic), TypeGrouping.ByNamespaceFlat)]
        private SystemType transformSmoothingLogicType = typeof(DefaultTransformSmoothingLogic);

        /// <summary>
        /// The concrete type of <see cref="TransformSmoothingLogic"/> to use for smoothing between transforms.
        /// </summary>
        /// <remarks>
        /// Setting this field at runtime can be expensive. Use with caution. Best used at startup or when
        /// instantiating ObjectManipulators from code.
        /// </remarks>
        public SystemType TransformSmoothingLogicType
        {
            get => transformSmoothingLogicType;
            set
            {
                // Re-instantiating smoothing logics is expensive and can interrupt ongoing interactions.
                transformSmoothingLogicType = value;
                smoothingLogic ??= Activator.CreateInstance(transformSmoothingLogicType) as ITransformSmoothingLogic;
            }
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

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float moveLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float MoveLerpTime
        {
            get => moveLerpTime;
            set => moveLerpTime = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float rotateLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float RotateLerpTime
        {
            get => rotateLerpTime;
            set => rotateLerpTime = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float scaleLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float ScaleLerpTime
        {
            get => scaleLerpTime;
            set => scaleLerpTime = value;
        }

        [SerializeField]
        [Tooltip("Enable or disable constraint support of this component. When enabled transform " +
            "changes will be post processed by the linked constraint manager.")]
        private bool enableConstraints = true;
        /// <summary>
        /// Enable or disable constraint support of this component. When enabled, transform
        /// changes will be post processed by the linked constraint manager.
        /// </summary>
        public bool EnableConstraints
        {
            get => enableConstraints;
            set => enableConstraints = value;
        }

        [SerializeField]
        [Tooltip("Constraint manager slot to enable constraints when manipulating the object.")]
        private ConstraintManager constraintsManager;

        /// <summary>
        /// Constraint manager slot to enable constraints when manipulating the object.
        /// </summary>
        public ConstraintManager ConstraintsManager
        {
            get => constraintsManager;
            set => constraintsManager = value;
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
                InstantiateManipulationLogic();
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

        /// <summary>
        /// The concrete implementations of the manipulation logic for a given interaction type.
        /// </summary>
        protected struct LogicImplementation
        {
            public ManipulationLogic<Vector3> moveLogic;
            public ManipulationLogic<Quaternion> rotateLogic;
            public ManipulationLogic<Vector3> scaleLogic;
        }

        /// <summary>
        /// The instantiated manipulation logic objects, as specified by the types in <see cref="ManipulationLogicTypes"/>.
        /// </summary>
        protected LogicImplementation ManipulationLogic { get; private set; }

        #endregion Protected Properties

        #region Private Properties

        private ITransformSmoothingLogic smoothingLogic;

        private bool ShouldSmooth => (IsGrabSelected && SmoothingNear) || (!IsGrabSelected && SmoothingFar);

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

            ApplyRequiredSettings();

            rigidBody = HostTransform.GetComponent<Rigidbody>();

            if (constraintsManager == null && EnableConstraints)
            {
                constraintsManager = gameObject.EnsureComponent<ConstraintManager>();
            }

            if (constraintsManager != null)
            {
                constraintsManager.Setup(new MixedRealityTransform(HostTransform));
            }

            smoothingLogic ??= Activator.CreateInstance(transformSmoothingLogicType) as ITransformSmoothingLogic;

            InstantiateManipulationLogic();
        }

        #endregion

        private void InstantiateManipulationLogic()
        {
            // Re-instantiate the manipulation logic objects.
            ManipulationLogic = new LogicImplementation()
            {
                moveLogic = Activator.CreateInstance(ManipulationLogicTypes.moveLogicType) as ManipulationLogic<Vector3>,
                rotateLogic = Activator.CreateInstance(ManipulationLogicTypes.rotateLogicType) as ManipulationLogic<Quaternion>,
                scaleLogic = Activator.CreateInstance(ManipulationLogicTypes.scaleLogicType) as ManipulationLogic<Vector3>,
            };
        }

        private InteractionFlags GetInteractionFlagsFromInteractor(IXRInteractor interactor)
        {
            InteractionFlags flags = InteractionFlags.None;
            if (interactor is IGrabInteractor)
            {
                flags |= InteractionFlags.Near;
            }
            if (interactor is IRayInteractor)
            {
                flags |= InteractionFlags.Ray;
            }
            if (interactor is IGazeInteractor || interactor is IGazePinchInteractor)
            {
                flags |= InteractionFlags.Gaze;
            }

            // If none have been set, default to generic.
            if (flags == InteractionFlags.None)
            {
                flags = InteractionFlags.Generic;
            }

            return flags;
        }

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return base.IsSelectableBy(interactor) && AllowedInteractionTypes.IsMaskSet(GetInteractionFlagsFromInteractor(interactor));
        }

        // When the player is carrying a Rigidbody, the physics damping of interaction should act within the moving frame of reference of the player.
        // The reference frame logic allows compensating for that 
        private Transform referenceFrameTransform = null;
        private bool referenceFrameHasLastPos = false;
        private Vector3 referenceFrameLastPos;

        private MixedRealityTransform targetTransform;
        private bool useForces;

        private static readonly ProfilerMarker OnSelectEnteredPerfMarker =
            new ProfilerMarker("[MRTK] ObjectManipulator.OnSelectEntered");

        /// <summary>
        /// Override this class to provide the transform of the reference frame (e.g. the camera) against which to compute the damping.
        ///
        /// This intended for the situation of FPS-style controllers moving forward at constant speed while holding an object,
        /// to prevent damping from pushing the body towards the player.
        /// </summary>
        /// <param name="args">Arguments of the OnSelectEntered event that called this function</param>
        /// <returns>The Transform that should be used to define the reference frame or null to use the global reference frame</returns>
        protected virtual Transform GetReferenceFrameTransform(SelectEnterEventArgs args) => null;

        /// <inheritdoc />
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            using (OnSelectEnteredPerfMarker.Auto())
            {
                base.OnSelectEntered(args);

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

                targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);

                ManipulationLogic.scaleLogic.Setup(interactorsSelecting, this, targetTransform);
                ManipulationLogic.rotateLogic.Setup(interactorsSelecting, this, targetTransform);
                ManipulationLogic.moveLogic.Setup(interactorsSelecting, this, targetTransform);

                if (constraintsManager != null && EnableConstraints)
                {
                    constraintsManager.OnManipulationStarted(targetTransform);
                }

                useForces = rigidBody != null && !rigidBody.isKinematic;

                // ideally, the reference frame should be that of the camera. Here the interactorObject transform is the best available alternative.
                referenceFrameTransform = GetReferenceFrameTransform(args);
                referenceFrameHasLastPos = false;
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

                // Only release the rigidbody (restore rigidbody settings/configuration)
                // if this is the last select event!
                if (rigidBody != null && interactorsSelecting.Count == 0)
                {
                    ReleaseRigidBody(rigidBody.velocity, rigidBody.angularVelocity);
                }
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

                    targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);

                    using (ScaleLogicMarker.Auto())
                    {
                        if (allowedManipulations.IsMaskSet(TransformFlags.Scale))
                        {
                            targetTransform.Scale = ManipulationLogic.scaleLogic.Update(interactorsSelecting, this, targetTransform, useCenteredAnchor);
                        }
                    }

                    // Immediately apply scale constraints after computing the user's desired scale input.
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyScaleConstraints(ref targetTransform, isOneHanded, IsGrabSelected);
                    }

                    using (RotateLogicMarker.Auto())
                    {
                        if (allowedManipulations.IsMaskSet(TransformFlags.Rotate))
                        {
                            targetTransform.Rotation = ManipulationLogic.rotateLogic.Update(interactorsSelecting, this, targetTransform, useCenteredAnchor);
                        }
                    }

                    // Immediately apply rotation constraints after computing the user's desired rotation input.
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyRotationConstraints(ref targetTransform, isOneHanded, IsGrabSelected);
                    }

                    using (MoveLogicMarker.Auto())
                    {
                        if (allowedManipulations.IsMaskSet(TransformFlags.Move))
                        {
                            targetTransform.Position = ManipulationLogic.moveLogic.Update(interactorsSelecting, this, targetTransform, useCenteredAnchor);
                        }
                    }

                    // Immediately apply translation constraints after computing the user's desired scale input.
                    if (EnableConstraints && constraintsManager != null)
                    {
                        constraintsManager.ApplyTranslationConstraints(ref targetTransform, isOneHanded, IsGrabSelected);
                    }

                    ApplyTargetTransform();
                }
                else if (useForces && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
                {
                    ApplyForcesToRigidbody();
                }
            }
        }

        /// <summary>
        /// Once the <paramref name="targetTransform"/> has been determined, this method is called
        /// to apply the target pose to the object. Calls <see cref="ModifyTargetPose"/> before
        /// applying, to adjust the pose with smoothing, constraints, etc.
        /// </summary>
        private void ApplyTargetTransform()
        {
            // modifiedTransformFlags currently unused.
            TransformFlags modifiedTransformFlags = TransformFlags.None;
            ModifyTargetPose(ref targetTransform, ref modifiedTransformFlags);

            if (rigidBody == null)
            {
                HostTransform.SetPositionAndRotation(targetTransform.Position, targetTransform.Rotation);
                HostTransform.localScale = targetTransform.Scale;
            }
            else
            {
                // There is a Rigidbody. Potential different paths for near vs far manipulation
                if (!useForces)
                {
                    rigidBody.MovePosition(targetTransform.Position);
                    rigidBody.MoveRotation(targetTransform.Rotation);
                }

                HostTransform.localScale = targetTransform.Scale;
            }
        }

        /// <summary>
        /// In case a Rigidbody gets the targetTransform applied using physical forcees, this function is called within the
        /// FixedUpdate() routine with physics-conforming time stepping.
        /// </summary>
        private void ApplyForcesToRigidbody()
        {
            var referenceFrameVelocity = Vector3.zero;

            if (referenceFrameTransform != null)
            {
                if (referenceFrameHasLastPos)
                {
                    referenceFrameVelocity = (referenceFrameTransform.position - referenceFrameLastPos) / Time.fixedDeltaTime;
                }

                referenceFrameLastPos = referenceFrameTransform.position;
                referenceFrameHasLastPos = true;
            }

            // implement critically dampened spring force, scaled to mass-independent frequency
            float omega = Mathf.PI / springForceSoftness;  // angular frequency, sqrt(k/m)

            Vector3 distance = HostTransform.position - targetTransform.Position;

            // when player is moving, we need to anticipate where the targetTransform is going to be one time step from now
            distance -= referenceFrameVelocity * Time.fixedDeltaTime;

            var velocity = rigidBody.velocity;

            var acceleration = omega * omega * -distance;  // acceleration caused by spring force

            var accelerationMagnitude = acceleration.magnitude;

            // apply springForceLimit only for slow-moving body (e.g. pressed against wall)
            // when body is already moving fast, also allow strong acceleration
            var maxAcceleration = Mathf.Max(springForceLimit, 10 * velocity.magnitude / Time.fixedDeltaTime);

            if (accelerationMagnitude > maxAcceleration)
            {
                acceleration *= maxAcceleration / accelerationMagnitude;
            }

            // Apply damping - mathematically, we need e^(-2 * omega * dt)
            // To compensate for the finite time step, this is split in two equal factors,
            // one applied before, the other after the spring force
            // equivalent with applying damping as well as spring force continuously
            float halfDampingFactor = Mathf.Exp(-springDamping * omega * Time.fixedDeltaTime);

            velocity -= referenceFrameVelocity;  // change to the player's frame of reference before damping

            velocity *= halfDampingFactor;  // 1/2 damping
            velocity += acceleration * Time.fixedDeltaTime; // integration step of spring force
            velocity *= halfDampingFactor;  // 1/2 damping

            velocity += referenceFrameVelocity;  // change back to global frame of reference

            rigidBody.velocity = velocity;

            if (applyTorque)
            {
                // Torque calculations: same calculation & parameters as for linear velocity
                // skipping referenceFrameVelocity and springForceLimit which do not exactly apply here

                // implement critically dampened spring force, scaled to mass-independent frequency
                float angularOmega = Mathf.PI / springTorqueSoftness;  // angular frequency, sqrt(k/m)

                var angularDistance = HostTransform.rotation * Quaternion.Inverse(targetTransform.Rotation);
                angularDistance.ToAngleAxis(out float angle, out Vector3 axis);

                if (!axis.IsValidVector())
                {
                    // ToAngleAxis is numerically unstable, returning NaN axis for near-zero angles
                    angle = 0;
                    axis = Vector3.up;
                }

                if (angle > 180f)
                {
                    angle -= 360f;
                }

                var angularVelocity = rigidBody.angularVelocity;

                var angularAcceleration = -angle * angularOmega * angularOmega;  // acceleration caused by spring force

                angularVelocity *= halfDampingFactor;  // 1/2 damping
                angularVelocity += angularAcceleration * Time.fixedDeltaTime * Mathf.Deg2Rad * axis.normalized; // integration step of spring force
                angularVelocity *= halfDampingFactor;  // 1/2 damping

                rigidBody.angularVelocity = angularVelocity;
            }
        }

        /// <summary>
        /// Called by ApplyTargetPose to modify the target pose with the relevant constraints, smoothing,
        /// elastic, or any other derived/overridden behavior.
        /// </summary>
        /// <param name="targetPose">
        /// The target position, rotation, and scale, pre-smoothing, but post-input and post-constraints. Modified by-reference.
        /// <param/>
        /// <param name="modifiedTransformFlags">
        /// Flags which parts of the transform (position, rotation, scale) have been altered by an external source (like Elastics).
        /// Modified by-reference.
        /// <param/>
        protected virtual void ModifyTargetPose(ref MixedRealityTransform targetPose, ref TransformFlags modifiedTransformFlags)
        {
            // TODO: Elastics. Compute elastics here and apply to modifiedTransformFlags.

            bool applySmoothing = ShouldSmooth && smoothingLogic != null;

            targetPose.Position = (applySmoothing && !useForces) ? smoothingLogic.SmoothPosition(HostTransform.position, targetPose.Position, moveLerpTime, Time.deltaTime) : targetPose.Position;
            targetPose.Rotation = (applySmoothing && !useForces) ? smoothingLogic.SmoothRotation(HostTransform.rotation, targetPose.Rotation, rotateLerpTime, Time.deltaTime) : targetPose.Rotation;
            targetPose.Scale = applySmoothing ? smoothingLogic.SmoothScale(HostTransform.localScale, targetPose.Scale, scaleLerpTime, Time.deltaTime) : targetPose.Scale;
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
                    if (releaseBehavior.IsMaskSet(ReleaseBehaviorType.KeepVelocity))
                    {
                        rigidBody.velocity = velocity;
                    }

                    if (releaseBehavior.IsMaskSet(ReleaseBehaviorType.KeepAngularVelocity))
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
    }

    /// <summary>
    /// Extension methods specific to the <see cref="ReleaseBehaviorType"/> enum.
    /// </summary>
    public static class ReleaseBehaviorTypeExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="ObjectManipulator.ReleaseBehaviorType"/> value.</param>
        /// <param name="b"><see cref="ObjectManipulator.ReleaseBehaviorType"/> mask.</param>
        /// <returns>True if all of the bits in the specified mask are set in the
        /// current value.</returns>
        public static bool IsMaskSet(this ObjectManipulator.ReleaseBehaviorType a, ObjectManipulator.ReleaseBehaviorType b)
        {
            return ((a & b) == b);
        }
    }
}
