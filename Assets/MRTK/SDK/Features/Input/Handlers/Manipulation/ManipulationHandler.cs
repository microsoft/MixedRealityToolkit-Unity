// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This script allows for an object to be movable, scalable, and rotatable with one or two hands. 
    /// You may also configure the script on only enable certain manipulations. The script works with 
    /// both HoloLens' gesture input and immersive headset's motion controller input.
    /// </summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/manipulation-handler")]
    [AddComponentMenu("Scripts/MRTK/SDK/ManipulationHandler")]
    public class ManipulationHandler : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusChangedHandler
    {
        #region Public Enums

        public enum HandMovementType
        {
            OneHandedOnly = 0,
            TwoHandedOnly,
            OneAndTwoHanded
        }

        public enum TwoHandedManipulation
        {
            Scale,
            Rotate,
            MoveScale,
            MoveRotate,
            RotateScale,
            MoveRotateScale
        }

        public enum RotateInOneHandType
        {
            MaintainRotationToUser,
            GravityAlignedMaintainRotationToUser,
            FaceUser,
            FaceAwayFromUser,
            MaintainOriginalRotation,
            RotateAboutObjectCenter,
            RotateAboutGrabPoint
        }

        [Flags]
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

        public Transform HostTransform
        {
            get => hostTransform;
            set => hostTransform = value;
        }

        [SerializeField]
        [Tooltip("Can manipulation be done only with one hand, only with two hands, or with both?")]
        private HandMovementType manipulationType = HandMovementType.OneAndTwoHanded;

        public HandMovementType ManipulationType
        {
            get => manipulationType;
            set => manipulationType = value;
        }

        [SerializeField]
        [Tooltip("What manipulation will two hands perform?")]
        private TwoHandedManipulation twoHandedManipulationType = TwoHandedManipulation.MoveRotateScale;

        public TwoHandedManipulation TwoHandedManipulationType
        {
            get => twoHandedManipulationType;
            set => twoHandedManipulationType = value;
        }

        [SerializeField]
        [Tooltip("Specifies whether manipulation can be done using far interaction with pointers.")]
        private bool allowFarManipulation = true;

        public bool AllowFarManipulation
        {
            get => allowFarManipulation;
            set => allowFarManipulation = value;
        }

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand near")]
        private RotateInOneHandType oneHandRotationModeNear = RotateInOneHandType.RotateAboutGrabPoint;

        public RotateInOneHandType OneHandRotationModeNear
        {
            get => oneHandRotationModeNear;
            set => oneHandRotationModeNear = value;
        }

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand at distance")]
        private RotateInOneHandType oneHandRotationModeFar = RotateInOneHandType.RotateAboutGrabPoint;

        public RotateInOneHandType OneHandRotationModeFar
        {
            get => oneHandRotationModeFar;
            set => oneHandRotationModeFar = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("Rigid body behavior of the dragged object when releasing it.")]
        private ReleaseBehaviorType releaseBehavior = ReleaseBehaviorType.KeepVelocity | ReleaseBehaviorType.KeepAngularVelocity;

        public ReleaseBehaviorType ReleaseBehavior
        {
            get => releaseBehavior;
            set => releaseBehavior = value;
        }

        [SerializeField]
        [Tooltip("Constrain rotation along an axis")]
        private RotationConstraintType constraintOnRotation = RotationConstraintType.None;

        public RotationConstraintType ConstraintOnRotation
        {
            get => constraintOnRotation;
            set
            {
                constraintOnRotation = value;
                rotateConstraint.ConstraintOnRotation = RotationConstraintHelper.ConvertToAxisFlags(constraintOnRotation);
            }
        }

        [SerializeField]
        [Tooltip("Check if object rotation should be in local space of object being manipulated instead of world space.")]
        private bool useLocalSpaceForConstraint = false;

        /// <summary>
        /// Gets or sets whether the constraints should be applied in local space of the object being manipulated or world space.
        /// </summary>
        public bool UseLocalSpaceForConstraint
        {
            get => rotateConstraint != null && rotateConstraint.UseLocalSpaceForConstraint;
            set
            {
                if (rotateConstraint != null)
                {
                    rotateConstraint.UseLocalSpaceForConstraint = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Constrain movement")]
        private MovementConstraintType constraintOnMovement = MovementConstraintType.None;

        public MovementConstraintType ConstraintOnMovement
        {
            get => constraintOnMovement;
            set => constraintOnMovement = value;
        }

        [SerializeField]
        [Tooltip("Check to enable frame-rate independent smoothing. ")]
        private bool smoothingActive = true;

        public bool SmoothingActive
        {
            get => smoothingActive;
            set => smoothingActive = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the movement, scale, rotation.  Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float smoothingAmountOneHandManip = 0.001f;

        public float SmoothingAmoutOneHandManip
        {
            get => smoothingAmountOneHandManip;
            set => smoothingAmountOneHandManip = value;
        }

        #endregion Serialized Fields

        #region Event handlers
        [SerializeField]
        [FormerlySerializedAs("OnManipulationStarted")]
        private ManipulationEvent onManipulationStarted = new ManipulationEvent();
        public ManipulationEvent OnManipulationStarted
        {
            get => onManipulationStarted;
            set => onManipulationStarted = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnManipulationEnded")]
        private ManipulationEvent onManipulationEnded = new ManipulationEvent();
        public ManipulationEvent OnManipulationEnded
        {
            get => onManipulationEnded;
            set => onManipulationEnded = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnHoverEntered")]
        private ManipulationEvent onHoverEntered = new ManipulationEvent();
        public ManipulationEvent OnHoverEntered
        {
            get => onHoverEntered;
            set => onHoverEntered = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnHoverExited")]
        private ManipulationEvent onHoverExited = new ManipulationEvent();
        public ManipulationEvent OnHoverExited
        {
            get => onHoverExited;
            set => onHoverExited = value;
        }
        #endregion

        #region Private Properties

        [System.Flags]
        private enum State
        {
            Start = 0x000,
            Moving = 0x001,
            Scaling = 0x010,
            Rotating = 0x100,
            MovingRotating = Moving | Rotating,
            MovingScaling = Moving | Scaling,
            RotatingScaling = Rotating | Scaling,
            MovingRotatingScaling = Moving | Rotating | Scaling
        };

        private State currentState = State.Start;
        private ManipulationMoveLogic moveLogic;
        private TwoHandScaleLogic scaleLogic;
        private TwoHandRotateLogic rotateLogic;
        /// <summary>
        /// Holds the pointer and the initial intersection point of the pointer ray 
        /// with the object on pointer down in pointer space
        /// </summary>
        private struct PointerData
        {
            public IMixedRealityPointer pointer;
            private Vector3 initialGrabPointInPointer;

            public PointerData(IMixedRealityPointer pointer, Vector3 initialGrabPointInPointer) : this()
            {
                this.pointer = pointer;
                this.initialGrabPointInPointer = initialGrabPointInPointer;
            }

            public bool IsNearPointer()
            {
                return (pointer is IMixedRealityNearPointer);
            }

            /// Returns the grab point on the manipulated object in world space
            public Vector3 GrabPoint
            {
                get
                {
                    return (pointer.Rotation * initialGrabPointInPointer) + pointer.Position;
                }
            }
        }
        private Dictionary<uint, PointerData> pointerIdToPointerMap = new Dictionary<uint, PointerData>();
        private Quaternion objectToHandRotation;
        private Quaternion objectToGripRotation;
        private bool isNearManipulation;

        private Rigidbody rigidBody;
        private bool wasKinematic = false;

        private Quaternion startObjectRotationCameraSpace;
        private Quaternion startObjectRotationFlatCameraSpace;
        private Quaternion hostWorldRotationOnManipulationStart;

        private FixedDistanceConstraint moveConstraint;
        private RotationAxisConstraint rotateConstraint;
        private MinMaxScaleConstraint scaleHandler;

        #endregion

        #region MonoBehaviour Functions

        private void Awake()
        {
            moveLogic = new ManipulationMoveLogic();
            rotateLogic = new TwoHandRotateLogic();
            scaleLogic = new TwoHandScaleLogic();
        }
        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = transform;
            }

            moveConstraint = this.EnsureComponent<FixedDistanceConstraint>();
            moveConstraint.ConstraintTransform = CameraCache.Main.transform;

            rotateConstraint = this.EnsureComponent<RotationAxisConstraint>();
            rotateConstraint.ConstraintOnRotation = RotationConstraintHelper.ConvertToAxisFlags(constraintOnRotation);
            rotateConstraint.UseLocalSpaceForConstraint = useLocalSpaceForConstraint;

            scaleHandler = this.GetComponent<MinMaxScaleConstraint>();
        }
        #endregion MonoBehaviour Functions

        #region Private Methods

        /// <summary>
        /// Calculates the unweighted average, or centroid, of all pointers'
        /// grab points, as defined by the PointerData.GrabPoint property.
        /// Does not use the rotation of each pointer; represents a pure
        /// geometric centroid  of the grab points in world space.
        /// </summary>
        /// <returns>
        /// Worldspace grab point centroid of all pointers 
        /// in pointerIdToPointerMap.
        /// </returns>
        private Vector3 GetPointersCentroid()
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                sum += p.GrabPoint;
                count++;
            }
            return sum / Math.Max(1, count);
        }

        /// <summary>
        /// Calculates the multiple-handed pointer pose, used for
        /// far-interaction hand-ray-based manipulations. Uses the
        /// unweighted vector average of the pointers' forward vectors
        /// to calculate a compound pose that takes into account the
        /// pointing direction of each pointer.
        /// </summary>
        /// <returns>
        /// Compound pose calculated as the average of the poses
        /// corresponding to all of the pointers in pointerIdToPointerMap.
        /// </returns>
        private MixedRealityPose GetAveragePointerPose()
        {
            Vector3 sumPos = Vector3.zero;
            Vector3 sumDir = Vector3.zero;
            int count = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                sumPos += p.pointer.Position;
                sumDir += p.pointer.Rotation * Vector3.forward;
                count++;
            }

            MixedRealityPose pose = new MixedRealityPose();

            if (count > 0)
            {
                pose.Position = sumPos / count;
                pose.Rotation = Quaternion.LookRotation(sumDir / count);
            }

            return pose;
        }

        private Vector3 GetPointersVelocity()
        {
            Vector3 sum = Vector3.zero;
            int numControllers = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                // Check pointer has a valid controller (e.g. gaze pointer doesn't)
                if (p.pointer.Controller != null)
                {
                    numControllers++;
                    sum += p.pointer.Controller.Velocity;
                }
            }
            return sum / Math.Max(1, numControllers);
        }

        private Vector3 GetPointersAngularVelocity()
        {
            Vector3 sum = Vector3.zero;
            int numControllers = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                // Check pointer has a valid controller (e.g. gaze pointer doesn't)
                if (p.pointer.Controller != null)
                {
                    numControllers++;
                    sum += p.pointer.Controller.AngularVelocity;
                }
            }
            return sum / Math.Max(1, numControllers);
        }

        private bool IsNearManipulation()
        {
            foreach (var item in pointerIdToPointerMap)
            {
                if (item.Value.IsNearPointer())
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateStateMachine()
        {
            var handsPressedCount = pointerIdToPointerMap.Count;
            State newState = currentState;
            // early out for no hands or one hand if TwoHandedOnly is active
            if (handsPressedCount == 0 || (handsPressedCount == 1 && manipulationType == HandMovementType.TwoHandedOnly))
            {
                newState = State.Start;
            }
            else
            {
                switch (currentState)
                {
                    case State.Start:
                    case State.Moving:
                        if (handsPressedCount == 1)
                        {
                            newState = State.Moving;
                        }
                        else if (handsPressedCount > 1 && manipulationType != HandMovementType.OneHandedOnly)
                        {
                            switch (twoHandedManipulationType)
                            {
                                case TwoHandedManipulation.Scale:
                                    newState = State.Scaling;
                                    break;
                                case TwoHandedManipulation.Rotate:
                                    newState = State.Rotating;
                                    break;
                                case TwoHandedManipulation.MoveRotate:
                                    newState = State.MovingRotating;
                                    break;
                                case TwoHandedManipulation.MoveScale:
                                    newState = State.MovingScaling;
                                    break;
                                case TwoHandedManipulation.RotateScale:
                                    newState = State.RotatingScaling;
                                    break;
                                case TwoHandedManipulation.MoveRotateScale:
                                    newState = State.MovingRotatingScaling;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        break;
                    case State.Scaling:
                    case State.Rotating:
                    case State.MovingScaling:
                    case State.MovingRotating:
                    case State.RotatingScaling:
                    case State.MovingRotatingScaling:
                        // one hand only supports move for now
                        if (handsPressedCount == 1)
                        {
                            newState = State.Moving;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            InvokeStateUpdateFunctions(currentState, newState);
            currentState = newState;
        }

        private void InvokeStateUpdateFunctions(State oldState, State newState)
        {
            if (newState != oldState)
            {
                switch (newState)
                {
                    case State.Moving:
                        HandleOneHandMoveStarted();
                        break;
                    case State.Start:
                        HandleManipulationEnded();
                        break;
                    case State.RotatingScaling:
                    case State.MovingRotating:
                    case State.MovingRotatingScaling:
                    case State.Scaling:
                    case State.Rotating:
                    case State.MovingScaling:
                        HandleTwoHandManipulationStarted(newState);
                        break;
                }
                switch (oldState)
                {
                    case State.Start:
                        HandleManipulationStarted();
                        break;
                    case State.Scaling:
                    case State.Rotating:
                    case State.RotatingScaling:
                    case State.MovingRotating:
                    case State.MovingRotatingScaling:
                    case State.MovingScaling:
                        HandleTwoHandManipulationEnded();
                        break;
                }
            }
            else
            {
                switch (newState)
                {
                    case State.Moving:
                        HandleOneHandMoveUpdated();
                        break;
                    case State.Scaling:
                    case State.Rotating:
                    case State.RotatingScaling:
                    case State.MovingRotating:
                    case State.MovingRotatingScaling:
                    case State.MovingScaling:
                        HandleTwoHandManipulationUpdated();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Releases the object that is currently manipulated
        /// </summary>
        public void ForceEndManipulation()
        {
            // release rigidbody and clear pointers
            ReleaseRigidBody();
            pointerIdToPointerMap.Clear();

            // end manipulation
            State newState = State.Start;
            InvokeStateUpdateFunctions(currentState, newState);
            currentState = newState;
        }

        /// <summary>
        /// Gets the grab point for the given pointer id.
        /// Only use if you know that your given pointer id corresponds to a pointer that has grabbed
        /// this component.
        /// </summary>
        public Vector3 GetPointerGrabPoint(uint pointerId)
        {
            Assert.IsTrue(pointerIdToPointerMap.ContainsKey(pointerId));
            return pointerIdToPointerMap[pointerId].GrabPoint;
        }

        #endregion Public Methods

        #region Hand Event Handlers

        /// <inheritdoc />
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (eventData.used ||
                (!allowFarManipulation && eventData.Pointer as IMixedRealityNearPointer == null))
            {
                return;
            }

            // If we only allow one handed manipulations, check there is no hand interacting yet. 
            if (manipulationType != HandMovementType.OneHandedOnly || pointerIdToPointerMap.Count == 0)
            {
                uint id = eventData.Pointer.PointerId;
                // Ignore poke pointer events
                if (!pointerIdToPointerMap.ContainsKey(eventData.Pointer.PointerId))
                {
                    if (pointerIdToPointerMap.Count == 0)
                    {
                        rigidBody = GetComponent<Rigidbody>();
                        if (rigidBody != null)
                        {
                            wasKinematic = rigidBody.isKinematic;
                            rigidBody.isKinematic = true;
                        }
                    }

                    // cache start ptr grab point
                    Vector3 initialGrabPoint = Quaternion.Inverse(eventData.Pointer.Rotation) * (eventData.Pointer.Result.Details.Point - eventData.Pointer.Position);
                    pointerIdToPointerMap.Add(id, new PointerData(eventData.Pointer, initialGrabPoint));

                    UpdateStateMachine();
                }
            }

            if (pointerIdToPointerMap.Count > 0)
            {
                // Always mark the pointer data as used to prevent any other behavior to handle pointer events
                // as long as the ManipulationHandler is active.
                // This is due to us reacting to both "Select" and "Grip" events.
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (currentState != State.Start)
            {
                UpdateStateMachine();
            }
        }

        /// <inheritdoc />
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            uint id = eventData.Pointer.PointerId;
            if (pointerIdToPointerMap.ContainsKey(id))
            {
                if (pointerIdToPointerMap.Count == 1 && rigidBody != null)
                {
                    ReleaseRigidBody();
                }

                pointerIdToPointerMap.Remove(id);
            }

            UpdateStateMachine();
            eventData.Use();
        }

        #endregion Hand Event Handlers

        #region Private Event Handlers
        private void HandleTwoHandManipulationUpdated()
        {
            var targetTransform = new MixedRealityTransform(hostTransform.position, hostTransform.rotation, hostTransform.localScale);

            var handPositionArray = GetHandPositionArray();

            if ((currentState & State.Scaling) > 0)
            {
                targetTransform.Scale = scaleLogic.UpdateMap(handPositionArray);
                if (scaleHandler != null)
                {
                    scaleHandler.ApplyConstraint(ref targetTransform);
                }
            }
            if ((currentState & State.Rotating) > 0)
            {
                targetTransform.Rotation = rotateLogic.Update(handPositionArray, targetTransform.Rotation);
                if (rotateConstraint != null)
                {
                    rotateConstraint.ApplyConstraint(ref targetTransform);
                }
            }
            if ((currentState & State.Moving) > 0)
            {
                // If near manipulation, a pure grabpoint centroid is used for
                // the initial pointer pose; if far manipulation, a more complex
                // look-rotation-based pointer pose is used.
                MixedRealityPose pose = IsNearManipulation() ? new MixedRealityPose(GetPointersCentroid()) : GetAveragePointerPose();

                // The manipulation handler is not built to handle near manipulation properly, please use the object manipulator
                targetTransform.Position = moveLogic.UpdateTransform(pose, targetTransform, true, false);
                if (constraintOnMovement == MovementConstraintType.FixDistanceFromHead && moveConstraint != null)
                {
                    moveConstraint.ApplyConstraint(ref targetTransform);
                }
            }

            float lerpAmount = GetLerpAmount();

            hostTransform.position = Vector3.Lerp(hostTransform.position, targetTransform.Position, lerpAmount);
            hostTransform.rotation = Quaternion.Lerp(hostTransform.rotation, targetTransform.Rotation, lerpAmount);
            hostTransform.localScale = Vector3.Lerp(hostTransform.localScale, targetTransform.Scale, lerpAmount);
        }

        private void HandleOneHandMoveUpdated()
        {
            Debug.Assert(pointerIdToPointerMap.Count == 1);
            PointerData pointerData = GetFirstPointer();
            IMixedRealityPointer pointer = pointerData.pointer;

            var targetTransform = new MixedRealityTransform(hostTransform.position, hostTransform.rotation, hostTransform.localScale);

            RotateInOneHandType rotateInOneHandType = isNearManipulation ? oneHandRotationModeNear : oneHandRotationModeFar;
            switch (rotateInOneHandType)
            {
                case RotateInOneHandType.MaintainOriginalRotation:
                    targetTransform.Rotation = hostTransform.rotation;
                    break;
                case RotateInOneHandType.MaintainRotationToUser:
                    Vector3 euler = CameraCache.Main.transform.rotation.eulerAngles;
                    // don't use roll (feels awkward) - just maintain yaw / pitch angle
                    targetTransform.Rotation = Quaternion.Euler(euler.x, euler.y, 0) * startObjectRotationCameraSpace;
                    break;
                case RotateInOneHandType.GravityAlignedMaintainRotationToUser:
                    var cameraForwardFlat = CameraCache.Main.transform.forward;
                    cameraForwardFlat.y = 0;
                    targetTransform.Rotation = Quaternion.LookRotation(cameraForwardFlat, Vector3.up) * startObjectRotationFlatCameraSpace;
                    break;
                case RotateInOneHandType.FaceUser:
                {
                    Vector3 directionToTarget = pointerData.GrabPoint - CameraCache.Main.transform.position;
                    // Vector3 directionToTarget = hostTransform.position - CameraCache.Main.transform.position;
                    targetTransform.Rotation = Quaternion.LookRotation(-directionToTarget);
                    break;
                }
                case RotateInOneHandType.FaceAwayFromUser:
                {
                    Vector3 directionToTarget = pointerData.GrabPoint - CameraCache.Main.transform.position;
                    targetTransform.Rotation = Quaternion.LookRotation(directionToTarget);
                    break;
                }
                case RotateInOneHandType.RotateAboutObjectCenter:
                case RotateInOneHandType.RotateAboutGrabPoint:
                    Quaternion gripRotation;
                    TryGetGripRotation(pointer, out gripRotation);
                    targetTransform.Rotation = gripRotation * objectToGripRotation;
                    break;
            }
            if (rotateConstraint != null)
            {
                rotateConstraint.ApplyConstraint(ref targetTransform);
            }

            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);

            // The manipulation handler is not built to handle near manipulation properly, please use the object manipulator
            targetTransform.Position = moveLogic.UpdateTransform(pointerPose, targetTransform, rotateInOneHandType != RotateInOneHandType.RotateAboutObjectCenter, false);
            if (constraintOnMovement == MovementConstraintType.FixDistanceFromHead && moveConstraint != null)
            {
                moveConstraint.ApplyConstraint(ref targetTransform);
            }

            float lerpAmount = GetLerpAmount();
            Quaternion smoothedRotation = Quaternion.Lerp(hostTransform.rotation, targetTransform.Rotation, lerpAmount);
            Vector3 smoothedPosition = Vector3.Lerp(hostTransform.position, targetTransform.Position, lerpAmount);

            hostTransform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
        }

        private void HandleTwoHandManipulationStarted(State newState)
        {
            var handPositionArray = GetHandPositionArray();

            if ((newState & State.Rotating) > 0)
            {
                rotateLogic.Setup(handPositionArray, hostTransform);
            }
            if ((newState & State.Moving) > 0)
            {
                // If near manipulation, a pure grabpoint centroid is used for
                // the initial pointer pose; if far manipulation, a more complex
                // look-rotation-based pointer pose is used.
                MixedRealityPose pointerPose = IsNearManipulation() ? new MixedRealityPose(GetPointersCentroid()) : GetAveragePointerPose();
                MixedRealityPose hostPose = new MixedRealityPose(hostTransform.position, hostTransform.rotation);
                moveLogic.Setup(pointerPose, GetPointersCentroid(), hostPose, hostTransform.localScale);
            }
            if ((newState & State.Scaling) > 0)
            {
                scaleLogic.Setup(handPositionArray, hostTransform);
            }
        }
        private void HandleTwoHandManipulationEnded() { }

        private void HandleOneHandMoveStarted()
        {
            Assert.IsTrue(pointerIdToPointerMap.Count == 1);
            PointerData pointerData = GetFirstPointer();
            IMixedRealityPointer pointer = pointerData.pointer;

            // cache objects rotation on start to have a reference for constraint calculations
            // if we don't cache this on manipulation start the near rotation might drift off the hand
            // over time
            hostWorldRotationOnManipulationStart = hostTransform.rotation;

            // Calculate relative transform from object to hand.
            Quaternion worldToPalmRotation = Quaternion.Inverse(pointer.Rotation);
            objectToHandRotation = worldToPalmRotation * hostTransform.rotation;

            // Calculate relative transform from object to grip.
            Quaternion gripRotation;
            TryGetGripRotation(pointer, out gripRotation);
            Quaternion worldToGripRotation = Quaternion.Inverse(gripRotation);
            objectToGripRotation = worldToGripRotation * hostTransform.rotation;

            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            MixedRealityPose hostPose = new MixedRealityPose(hostTransform.position, hostTransform.rotation);
            moveLogic.Setup(pointerPose, pointerData.GrabPoint, hostPose, hostTransform.localScale);

            Vector3 worldGrabPoint = pointerData.GrabPoint;

            startObjectRotationCameraSpace = Quaternion.Inverse(CameraCache.Main.transform.rotation) * hostTransform.rotation;
            var cameraFlat = CameraCache.Main.transform.forward;
            cameraFlat.y = 0;
            var hostForwardFlat = hostTransform.forward;
            hostForwardFlat.y = 0;
            var hostRotFlat = Quaternion.LookRotation(hostForwardFlat, Vector3.up);
            startObjectRotationFlatCameraSpace = Quaternion.Inverse(Quaternion.LookRotation(cameraFlat, Vector3.up)) * hostRotFlat;
        }

        private void HandleManipulationStarted()
        {
            isNearManipulation = IsNearManipulation();
            // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
            // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
            if (OnManipulationStarted != null)
            {
                OnManipulationStarted.Invoke(new ManipulationEventData
                {
                    ManipulationSource = gameObject,
                    IsNearInteraction = isNearManipulation,
                    Pointer = GetFirstPointer().pointer,
                    PointerCentroid = GetPointersCentroid(),
                    PointerVelocity = GetPointersVelocity(),
                    PointerAngularVelocity = GetPointersAngularVelocity()
                });
            }

            var pose = new MixedRealityTransform(hostTransform);
            if (constraintOnMovement == MovementConstraintType.FixDistanceFromHead && moveConstraint != null)
            {
                moveConstraint.Initialize(pose);
            }
            if (rotateConstraint != null)
            {
                rotateConstraint.Initialize(pose);
            }
            if (scaleHandler != null)
            {
                scaleHandler.Initialize(pose);
            }
        }

        private void HandleManipulationEnded()
        {
            // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
            // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
            if (OnManipulationEnded != null)
            {
                OnManipulationEnded.Invoke(new ManipulationEventData
                {
                    ManipulationSource = gameObject,
                    IsNearInteraction = isNearManipulation,
                    PointerCentroid = GetPointersCentroid(),
                    PointerVelocity = GetPointersVelocity(),
                    PointerAngularVelocity = GetPointersAngularVelocity()
                });
            }
        }

        #endregion Private Event Handlers

        #region Unused Event Handlers
        /// <inheritdoc />
        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
        public void OnBeforeFocusChange(FocusEventData eventData) { }

        #endregion Unused Event Handlers

        #region Private methods

        private float GetLerpAmount()
        {
            if (smoothingActive == false || smoothingAmountOneHandManip == 0)
            {
                return 1;
            }
            // Obtained from "Frame-rate independent smoothing"
            // www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
            // We divide by max value to give the slider a bit more sensitivity.
            return 1.0f - Mathf.Pow(smoothingAmountOneHandManip, Time.deltaTime);
        }

        private Vector3[] GetHandPositionArray()
        {
            var handPositionMap = new Vector3[pointerIdToPointerMap.Count];
            int index = 0;
            foreach (var item in pointerIdToPointerMap)
            {
                handPositionMap[index++] = item.Value.pointer.Position;
            }
            return handPositionMap;
        }

        public void OnFocusChanged(FocusEventData eventData)
        {
            bool isFar = !(eventData.Pointer is IMixedRealityNearPointer);
            if (eventData.OldFocusedObject == null ||
                !eventData.OldFocusedObject.transform.IsChildOf(transform))
            {
                if (isFar && !AllowFarManipulation)
                {
                    return;
                }
                if (OnHoverEntered != null)
                {
                    OnHoverEntered.Invoke(new ManipulationEventData
                    {
                        ManipulationSource = gameObject,
                        Pointer = eventData.Pointer,
                        IsNearInteraction = !isFar
                    });
                }
            }
            else if (eventData.NewFocusedObject == null ||
                    !eventData.NewFocusedObject.transform.IsChildOf(transform))
            {
                if (isFar && !AllowFarManipulation)
                {
                    return;
                }
                if (OnHoverExited != null)
                {
                    OnHoverExited.Invoke(new ManipulationEventData
                    {
                        ManipulationSource = gameObject,
                        Pointer = eventData.Pointer,
                        IsNearInteraction = !isFar
                    });
                }
            }
        }

        private void ReleaseRigidBody()
        {
            if (rigidBody != null)
            {
                rigidBody.isKinematic = wasKinematic;

                if (releaseBehavior.IsMaskSet(ReleaseBehaviorType.KeepVelocity))
                {
                    rigidBody.velocity = GetPointersVelocity();
                }

                if (releaseBehavior.IsMaskSet(ReleaseBehaviorType.KeepAngularVelocity))
                {
                    rigidBody.angularVelocity = GetPointersAngularVelocity();
                }

                rigidBody = null;
            }
        }

        private PointerData GetFirstPointer()
        {
            // We may be able to do this without allocating memory.
            // Moving to a method for later investigation.
            return pointerIdToPointerMap.Values.First();
        }

        private bool TryGetGripRotation(IMixedRealityPointer pointer, out Quaternion rotation)
        {
            for (int i = 0; i < (pointer.Controller?.Interactions?.Length ?? 0); i++)
            {
                if (pointer.Controller.Interactions[i].InputType == DeviceInputType.SpatialGrip)
                {
                    rotation = pointer.Controller.Interactions[i].RotationData;
                    return true;
                }
            }
            rotation = Quaternion.identity;
            return false;
        }

        #endregion
    }
}
