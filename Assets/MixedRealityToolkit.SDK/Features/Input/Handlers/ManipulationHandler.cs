// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This script allows for an object to be movable, scalable, and rotatable with one or two hands. 
    /// You may also configure the script on only enable certain manipulations. The script works with 
    /// both HoloLens' gesture input and immersive headset's motion controller input.
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_ManipulationHandler.html")]
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
        };
        public enum RotateInOneHandType
        {
            MaintainRotationToUser,
            GravityAlignedMaintainRotationToUser,
            FaceUser,
            FaceAwayFromUser,
            MaintainOriginalRotation,
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

        public Transform HostTransform
        {
            get => hostTransform;
            set => hostTransform = value;
        }

        [Header("Manipulation")]
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

        [Header("Constraints")]
        [SerializeField]
        [Tooltip("Constrain rotation along an axis")]
        private RotationConstraintType constraintOnRotation = RotationConstraintType.None;

        public RotationConstraintType ConstraintOnRotation
        {
            get => constraintOnRotation;
            set => constraintOnRotation = value;
        }

        [SerializeField]
        [Tooltip("Check if object rotation should be in local space of object being manipulated instead of world space.")]
        private bool useLocalSpaceForConstraint = false;

        /// <summary>
        /// Gets or sets whether the constraints should be applied in local space of the object being manipulated or world space.
        /// </summary>
        public bool UseLocalSpaceForConstraint
        {
            get => useLocalSpaceForConstraint;
            set => useLocalSpaceForConstraint = value;
        }

        [SerializeField]
        [Tooltip("Constrain movement")]
        private MovementConstraintType constraintOnMovement = MovementConstraintType.None;

        public MovementConstraintType ConstraintOnMovement
        {
            get => constraintOnMovement;
            set => constraintOnMovement = value;
        }

        [Header("Smoothing")]
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
        [Header("Manipulation Events")]
        public ManipulationEvent OnManipulationStarted = new ManipulationEvent();
        public ManipulationEvent OnManipulationEnded = new ManipulationEvent();
        public ManipulationEvent OnHoverEntered = new ManipulationEvent();
        public ManipulationEvent OnHoverExited = new ManipulationEvent();
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
        private TwoHandMoveLogic moveLogic;
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
        // This can probably be consolidated so that we use same for one hand and two hands
        private Quaternion targetRotationTwoHands;

        private Rigidbody rigidBody;
        private bool wasKinematic = false;

        private Quaternion startObjectRotationCameraSpace;
        private Quaternion startObjectRotationFlatCameraSpace;
        private Quaternion hostWorldRotationOnManipulationStart;

        private TransformScaleHandler scaleHandler;

        #endregion

        #region MonoBehaviour Functions

        private void Awake()
        {
            moveLogic = new TwoHandMoveLogic();
            rotateLogic = new TwoHandRotateLogic();
            scaleLogic = new TwoHandScaleLogic();
        }
        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = transform;
            }

            scaleHandler = this.GetComponent<TransformScaleHandler>();
        }
        private void OnDisable()
        {
            ForceEndManipulation();
        }
        #endregion MonoBehaviour Functions

        #region Private Methods
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
            if (!allowFarManipulation && eventData.Pointer as IMixedRealityNearPointer == null)
            {
                return;
            }

            // If we only allow one handed manipulations, check there is no hand interacting yet. 
            if (manipulationType != HandMovementType.OneHandedOnly || pointerIdToPointerMap.Count == 0)
            {
                uint id = eventData.Pointer.PointerId;
                // Ignore poke pointer events
                if (!eventData.used
                    && !pointerIdToPointerMap.ContainsKey(eventData.Pointer.PointerId))
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
            var targetPosition = hostTransform.position;
            var targetScale = hostTransform.localScale;

            var handPositionMap = GetHandPositionMap();

            if ((currentState & State.Rotating) > 0)
            {
                targetRotationTwoHands = rotateLogic.Update(handPositionMap, targetRotationTwoHands, constraintOnRotation, useLocalSpaceForConstraint);
            }
            if ((currentState & State.Scaling) > 0)
            {
                targetScale = scaleLogic.UpdateMap(handPositionMap);
            }

            if ((currentState & State.Moving) > 0)
            {
                MixedRealityPose pose = GetAveragePointerPose();
                targetPosition = moveLogic.Update(pose, targetRotationTwoHands, targetScale, true, constraintOnMovement);
            }

            float lerpAmount = GetLerpAmount();
            hostTransform.position = Vector3.Lerp(hostTransform.position, targetPosition, lerpAmount);
            // Currently the two hand rotation algorithm doesn't allow for lerping, but it should. Fix this.
            hostTransform.rotation = Quaternion.Lerp(hostTransform.rotation, targetRotationTwoHands, lerpAmount);

            if (scaleHandler != null)
            {
                targetScale = scaleHandler.ClampScale(targetScale);
            }
            hostTransform.localScale = Vector3.Lerp(hostTransform.localScale, targetScale, lerpAmount);
        }

        private Quaternion ApplyConstraints(Quaternion newRotation)
        {
            // apply constraint on rotation diff
            Quaternion diffRotation = newRotation * Quaternion.Inverse(hostWorldRotationOnManipulationStart);
            switch (constraintOnRotation)
            {
                case RotationConstraintType.XAxisOnly:
                    diffRotation.eulerAngles = Vector3.Scale(diffRotation.eulerAngles, Vector3.right);
                    break;
                case RotationConstraintType.YAxisOnly:
                    diffRotation.eulerAngles = Vector3.Scale(diffRotation.eulerAngles, Vector3.up);
                    break;
                case RotationConstraintType.ZAxisOnly:
                    diffRotation.eulerAngles = Vector3.Scale(diffRotation.eulerAngles, Vector3.forward);
                    break;
            }

            return useLocalSpaceForConstraint
                ? hostWorldRotationOnManipulationStart * diffRotation
                : diffRotation * hostWorldRotationOnManipulationStart;
        }

        private void HandleOneHandMoveUpdated()
        {
            Debug.Assert(pointerIdToPointerMap.Count == 1);
            PointerData pointerData = GetFirstPointer();
            IMixedRealityPointer pointer = pointerData.pointer;

            Quaternion targetRotation = Quaternion.identity;
            RotateInOneHandType rotateInOneHandType = isNearManipulation ? oneHandRotationModeNear : oneHandRotationModeFar;
            switch (rotateInOneHandType)
            {
                case RotateInOneHandType.MaintainOriginalRotation:
                    targetRotation = hostTransform.rotation;
                    break;
                case RotateInOneHandType.MaintainRotationToUser:
                    Vector3 euler = CameraCache.Main.transform.rotation.eulerAngles;
                    // don't use roll (feels awkward) - just maintain yaw / pitch angle
                    targetRotation = Quaternion.Euler(euler.x, euler.y, 0) * startObjectRotationCameraSpace;
                    break;
                case RotateInOneHandType.GravityAlignedMaintainRotationToUser:
                    var cameraForwardFlat = CameraCache.Main.transform.forward;
                    cameraForwardFlat.y = 0;
                    targetRotation = Quaternion.LookRotation(cameraForwardFlat, Vector3.up) * startObjectRotationFlatCameraSpace;
                    break;
                case RotateInOneHandType.FaceUser:
                {
                    Vector3 directionToTarget = pointerData.GrabPoint - CameraCache.Main.transform.position;
                    // Vector3 directionToTarget = hostTransform.position - CameraCache.Main.transform.position;
                    targetRotation = Quaternion.LookRotation(-directionToTarget);
                    break;
                }
                case RotateInOneHandType.FaceAwayFromUser:
                {
                    Vector3 directionToTarget = pointerData.GrabPoint - CameraCache.Main.transform.position;
                    targetRotation = Quaternion.LookRotation(directionToTarget);
                    break;
                }
                case RotateInOneHandType.RotateAboutObjectCenter:
                case RotateInOneHandType.RotateAboutGrabPoint:
                    Quaternion gripRotation;
                    TryGetGripRotation(pointer, out gripRotation);
                    targetRotation = gripRotation * objectToGripRotation;
                    break;
            }

            targetRotation = ApplyConstraints(targetRotation);
            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            Vector3 targetPosition = moveLogic.Update(pointerPose, targetRotation, hostTransform.localScale, rotateInOneHandType != RotateInOneHandType.RotateAboutObjectCenter, constraintOnMovement);

            float lerpAmount = GetLerpAmount();
            Quaternion smoothedRotation = Quaternion.Lerp(hostTransform.rotation, targetRotation, lerpAmount);
            Vector3 smoothedPosition = Vector3.Lerp(hostTransform.position, targetPosition, lerpAmount);
            hostTransform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
        }

        private void HandleTwoHandManipulationStarted(State newState)
        {
            var handPositionMap = GetHandPositionMap();
            targetRotationTwoHands = hostTransform.rotation;

            if ((newState & State.Rotating) > 0)
            {
                rotateLogic.Setup(handPositionMap, hostTransform, ConstraintOnRotation);
            }
            if ((newState & State.Moving) > 0)
            {
                MixedRealityPose pointerPose = GetAveragePointerPose();
                MixedRealityPose hostPose = new MixedRealityPose(hostTransform.position, hostTransform.rotation);
                moveLogic.Setup(pointerPose, GetPointersCentroid(), hostPose, hostTransform.localScale);
            }
            if ((newState & State.Scaling) > 0)
            {
                scaleLogic.Setup(handPositionMap, hostTransform);
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
                    ManipulationSource = this,
                    IsNearInteraction = isNearManipulation,
                    Pointer = GetFirstPointer().pointer,
                    PointerCentroid = GetPointersCentroid(),
                    PointerVelocity = GetPointersVelocity(),
                    PointerAngularVelocity = GetPointersAngularVelocity()
                });
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
                    ManipulationSource = this,
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

        private Dictionary<uint, Vector3> GetHandPositionMap()
        {
            var handPositionMap = new Dictionary<uint, Vector3>();
            foreach (var item in pointerIdToPointerMap)
            {
                handPositionMap.Add(item.Key, item.Value.pointer.Position);
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
                        ManipulationSource = this,
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
                        ManipulationSource = this,
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

                if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepVelocity))
                {
                    rigidBody.velocity = GetPointersVelocity();
                }

                if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepAngularVelocity))
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
            for (int i = 0; i < pointer.Controller.Interactions.Length; i++)
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
