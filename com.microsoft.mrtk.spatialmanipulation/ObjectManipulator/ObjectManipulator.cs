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
             "Whether physics forces are used to move the object when performing near manipulations. " +
             "Off will make the object feel more directly connected to the hand. On will honor the mass and inertia of the object. " +
             "The default is off.")]
        private bool useForcesForNearManipulation = false;

        /// <summary>
        /// Whether physics forces are used to move the object when performing near manipulations.
        /// </summary>
        /// <remarks>
        /// <para>Setting this to <c>false</c> will make the object feel more directly connected to the
        /// users hand. Setting this to <c>true</c> will honor the mass and inertia of the object,
        /// but may feel as though the object is connected through a spring. The default is <c>false</c>.</para>
        /// </remarks>
        public bool UseForcesForNearManipulation
        {
            get => useForcesForNearManipulation;
            set => useForcesForNearManipulation = value;
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

            if (smoothingLogic == null)
            {
                smoothingLogic = Activator.CreateInstance(transformSmoothingLogicType) as ITransformSmoothingLogic;
            }

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

        private static readonly ProfilerMarker OnSelectEnteredPerfMarker =
            new ProfilerMarker("[MRTK] ObjectManipulator.OnSelectEntered");

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

                MixedRealityTransform targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);

                ManipulationLogic.scaleLogic.Setup(interactorsSelecting, this, targetTransform);
                ManipulationLogic.rotateLogic.Setup(interactorsSelecting, this, targetTransform);
                ManipulationLogic.moveLogic.Setup(interactorsSelecting, this, targetTransform);

                if (constraintsManager != null && EnableConstraints)
                {
                    constraintsManager.OnManipulationStarted(targetTransform);
                }
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

                MixedRealityTransform targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);
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

                // Update during Fixed if we are using physics.
                // Update during Dynamic if we are not.
                // TODO: Why does FixedUpdate make querying deviceRotation break???
                // This is 99% a Unity bug. deviceRotation returns (0,0,0,0) when queried
                // during a fixed update cycle.
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    if (isSelected)
                    {
                        RotateAnchorType rotateType = CurrentInteractionType == InteractionFlags.Near ? RotationAnchorNear : RotationAnchorFar;
                        bool useCenteredAnchor = rotateType == RotateAnchorType.RotateAboutObjectCenter;
                        bool isOneHanded = interactorsSelecting.Count == 1;

                        MixedRealityTransform targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);

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

                        ApplyTargetPose(targetTransform);
                    }
                }
            }
        }

        /// <summary>
        /// Once the <paramref name="targetPose"/> has been determined, this method is called
        /// to apply the target pose to the object. Calls <see cref="ModifyTargetPose"/> before
        /// applying, to adjust the pose with smoothing, constraints, etc.
        /// </summary>
        /// <param name="targetPose">
        /// The target position, rotation, and scale to set the object to.
        /// <param/>
        private void ApplyTargetPose(MixedRealityTransform targetPose)
        {
            // modifiedTransformFlags currently unused.
            TransformFlags modifiedTransformFlags = TransformFlags.None;
            ModifyTargetPose(ref targetPose, ref modifiedTransformFlags);

            if (rigidBody == null)
            {
                HostTransform.SetPositionAndRotation(targetPose.Position, targetPose.Rotation);
                HostTransform.localScale = targetPose.Scale;
            }
            else
            {
                // There is a Rigidbody. Potential different paths for near vs far manipulation
                if (IsGrabSelected && !useForcesForNearManipulation)
                {
                    rigidBody.MovePosition(targetPose.Position);
                    rigidBody.MoveRotation(targetPose.Rotation);
                }
                else
                {
                    // We are using forces
                    rigidBody.velocity = ((1f - Mathf.Pow(moveLerpTime, Time.deltaTime)) / Time.deltaTime) * (targetPose.Position - HostTransform.position);

                    var relativeRotation = targetPose.Rotation * Quaternion.Inverse(HostTransform.rotation);
                    relativeRotation.ToAngleAxis(out float angle, out Vector3 axis);

                    if (axis.IsValidVector())
                    {
                        if (angle > 180f)
                        {
                            angle -= 360f;
                        }
                        rigidBody.angularVelocity = ((1f - Mathf.Pow(rotateLerpTime, Time.deltaTime)) / Time.deltaTime) * (angle * Mathf.Deg2Rad * axis.normalized);
                    }
                }

                HostTransform.localScale = targetPose.Scale;
            }
        }

        /// <summary>
        /// Called by ApplyTargetPose to apply smoothing and give subclassed implementations the chance to apply their
        /// own modifications to the object's pose.
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

            bool applySmoothing = ShouldSmooth && smoothingLogic != null && (rigidBody == null || rigidBody.isKinematic);

            targetPose.Position = applySmoothing ? smoothingLogic.SmoothPosition(HostTransform.position, targetPose.Position, moveLerpTime, Time.deltaTime) : targetPose.Position;
            targetPose.Rotation = applySmoothing ? smoothingLogic.SmoothRotation(HostTransform.rotation, targetPose.Rotation, rotateLerpTime, Time.deltaTime) : targetPose.Rotation;
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
