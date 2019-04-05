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
    public class ManipulationHandler : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler
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

        public Transform HostTransform => hostTransform;

        [Header("Manipulation")]
        [SerializeField]
        [Tooltip("Can manipulation be done only with one hand, only with two hands, or with both?")]
        private HandMovementType manipulationType = HandMovementType.OneAndTwoHanded;

        public HandMovementType ManipulationType => manipulationType;

        [SerializeField]
        [Tooltip("What manipulation will two hands perform?")]
        private TwoHandedManipulation twoHandedManipulationType = TwoHandedManipulation.MoveRotateScale;

        public TwoHandedManipulation TwoHandedManipulationType => twoHandedManipulationType;

        [SerializeField]
        [Tooltip("Specifies whether manipulation can be done using far interaction with pointers.")]
        private bool allowFarManipulation = true;

        public bool AllowFarManipulation => allowFarManipulation;

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand near")]
        private RotateInOneHandType oneHandRotationModeNear = RotateInOneHandType.RotateAboutGrabPoint;

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand at distance")]
        private RotateInOneHandType oneHandRotationModeFar = RotateInOneHandType.RotateAboutGrabPoint;

        [SerializeField]
        [EnumFlags]
        [Tooltip("Rigid body behavior of the dragged object when releasing it.")]
        private ReleaseBehaviorType releaseBehavior = ReleaseBehaviorType.KeepVelocity | ReleaseBehaviorType.KeepAngularVelocity;

        public ReleaseBehaviorType ReleaseBehavior => releaseBehavior;

        [Header("Constraints")]
        [SerializeField]
        [Tooltip("Constrain rotation along an axis")]
        private RotationConstraintType constraintOnRotation = RotationConstraintType.None;

        public RotationConstraintType ConstraintOnRotation => constraintOnRotation;

        [SerializeField]
        [Tooltip("Constrain movement")]
        private MovementConstraintType constraintOnMovement = MovementConstraintType.None;

        public MovementConstraintType ConstraintOnMovement => constraintOnMovement;

        [Header("Smoothing")]
        [SerializeField]
        [Tooltip("Check to enable frame-rate independent smoothing. ")]
        private bool smoothingActive = true;

        public bool SmoothingActive => smoothingActive;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the movement, scale, rotation.  Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float smoothingAmountOneHandManip = 0.001f;

        public float SmoothingAmoutOneHandManip => smoothingAmountOneHandManip;

        #endregion Serialized Fields

        #region Event handlers
        [Header("Manipulation Events")]
        public ManipulationEvent OnManipulationStarted;
        public ManipulationEvent OnManipulationEnded;
        public ManipulationEvent OnHoverEntered;
        public ManipulationEvent OnHoverExited;
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
        private TwoHandMoveLogic m_moveLogic;
        private TwoHandScaleLogic m_scaleLogic;
        private TwoHandRotateLogic m_rotateLogic;
        private Dictionary<uint, IMixedRealityPointer> pointerIdToPointerMap = new Dictionary<uint, IMixedRealityPointer>();

        private Quaternion objectToHandRotation;
        private Vector3 objectToHandTranslation;
        private bool isNearManipulation;
        // This can probably be consolidated so that we use same for one hand and two hands
        private Quaternion targetRotationTwoHands;

        private Rigidbody rigidBody;
        private bool wasKinematic = false;

        private Quaternion startObjectRotationCameraSpace;
        private Quaternion startObjectRotationFlatCameraSpace;

        #endregion

        #region MonoBehaviour Functions
        private void Awake()
        {
            m_moveLogic = new TwoHandMoveLogic(constraintOnMovement);
            m_rotateLogic = new TwoHandRotateLogic();
            m_scaleLogic = new TwoHandScaleLogic();
        }
        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = transform;
            }
        }
        private void Update()
        {
            if (currentState != State.Start)
            {
                UpdateStateMachine();
            }
        }
        #endregion MonoBehaviour Functions

        #region Private Methods
        private Vector3 GetPointersCentroid()
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                sum += p.Position;
                count++;
            }
            return sum / Math.Max(1, count);
        }

        private Vector3 GetPointersVelocity()
        {
            Vector3 sum = Vector3.zero;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                sum += p.Controller.Velocity;
            }
            return sum / Math.Max(1, pointerIdToPointerMap.Count);
        }

        private Vector3 GetPointersAngularVelocity()
        {
            Vector3 sum = Vector3.zero;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                sum += p.Controller.AngularVelocity;
            }
            return sum / Math.Max(1, pointerIdToPointerMap.Count);
        }

        private bool IsNearManipulation()
        {
            foreach (var item in pointerIdToPointerMap)
            {
                if (item.Value is IMixedRealityNearPointer)
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
            switch (currentState)
            {
                case State.Start:
                case State.Moving:
                    if (handsPressedCount == 0)
                    {
                        newState = State.Start;
                    }
                    else if (handsPressedCount == 1 && manipulationType != HandMovementType.TwoHandedOnly)
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
                    // TODO: if < 2, make this go to start state ('drop it')
                    if (handsPressedCount == 0)
                    {
                        newState = State.Start;
                    }
                    else if (handsPressedCount == 1)
                    {
                        newState = State.Moving;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

        #region Hand Event Handlers

        private MixedRealityInteractionMapping GetSpatialGripInfoForController(IMixedRealityController controller)
        {
            if (controller == null)
            {
                return null;
            }

            return controller.Interactions?.First(x => x.InputType == DeviceInputType.SpatialGrip);
        }

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
                    pointerIdToPointerMap.Add(id, eventData.Pointer);

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

        /// <inheritdoc />
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            uint id = eventData.Pointer.PointerId;
            if (pointerIdToPointerMap.ContainsKey(id))
            {
                if (pointerIdToPointerMap.Count == 1 && rigidBody != null)
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

            if ((currentState & State.Moving) > 0)
            {
                targetPosition = m_moveLogic.Update(GetPointersCentroid(), IsNearManipulation());
            }

            var handPositionMap = GetHandPositionMap();

            if ((currentState & State.Rotating) > 0)
            {
                targetRotationTwoHands = m_rotateLogic.Update(handPositionMap, targetRotationTwoHands, constraintOnRotation);
            }
            if ((currentState & State.Scaling) > 0)
            {
                targetScale = m_scaleLogic.UpdateMap(handPositionMap);
            }

            float lerpAmount = GetLerpAmount();
            hostTransform.position = Vector3.Lerp(hostTransform.position, targetPosition, lerpAmount);
            // Currently the two hand rotation algorithm doesn't allow for lerping, but it should. Fix this.
            hostTransform.rotation = Quaternion.Lerp(hostTransform.rotation, targetRotationTwoHands, lerpAmount);
            hostTransform.localScale = Vector3.Lerp(hostTransform.localScale, targetScale, lerpAmount);
        }

        private void HandleOneHandMoveUpdated()
        {
            Debug.Assert(pointerIdToPointerMap.Count == 1);
            IMixedRealityPointer pointer = pointerIdToPointerMap.Values.First();

            var interactionMapping = GetSpatialGripInfoForController(pointer.Controller);
            if (interactionMapping != null)
            {
                Quaternion targetRotation = Quaternion.identity;
                RotateInOneHandType rotateInOneHandType = isNearManipulation ? oneHandRotationModeNear : oneHandRotationModeFar;
                if (rotateInOneHandType == RotateInOneHandType.MaintainOriginalRotation)
                {
                    targetRotation = hostTransform.rotation;
                }
                else if (rotateInOneHandType == RotateInOneHandType.MaintainRotationToUser)
                {
                    targetRotation = CameraCache.Main.transform.rotation * startObjectRotationCameraSpace;
                }
                else if (rotateInOneHandType == RotateInOneHandType.GravityAlignedMaintainRotationToUser)
                {
                    var cameraForwardFlat = CameraCache.Main.transform.forward;
                    cameraForwardFlat.y = 0;
                    targetRotation = Quaternion.LookRotation(cameraForwardFlat, Vector3.up) * startObjectRotationFlatCameraSpace;
                }
                else if (rotateInOneHandType == RotateInOneHandType.FaceUser)
                {
                    Vector3 directionToTarget = hostTransform.position - CameraCache.Main.transform.position;
                    targetRotation = Quaternion.LookRotation(-directionToTarget);
                }
                else if (rotateInOneHandType == RotateInOneHandType.FaceAwayFromUser)
                {
                    Vector3 directionToTarget = hostTransform.position - CameraCache.Main.transform.position;
                    targetRotation = Quaternion.LookRotation(directionToTarget);
                }
                else
                {
                    targetRotation = interactionMapping.PoseData.Rotation * objectToHandRotation;
                    switch (constraintOnRotation)
                    {
                        case RotationConstraintType.XAxisOnly:
                            targetRotation.eulerAngles = Vector3.Scale(targetRotation.eulerAngles, Vector3.right);
                            break;
                        case RotationConstraintType.YAxisOnly:
                            targetRotation.eulerAngles = Vector3.Scale(targetRotation.eulerAngles, Vector3.up);
                            break;
                        case RotationConstraintType.ZAxisOnly:
                            targetRotation.eulerAngles = Vector3.Scale(targetRotation.eulerAngles, Vector3.forward);
                            break;
                    }
                }

                Vector3 targetPosition;
                if (IsNearManipulation())
                {
                    if (oneHandRotationModeNear == RotateInOneHandType.RotateAboutGrabPoint)
                    {
                        targetPosition = (interactionMapping.PoseData.Rotation * objectToHandTranslation) + interactionMapping.PoseData.Position;
                    }
                    else // RotateAboutCenter or DoNotRotateInOneHand
                    {
                        targetPosition = objectToHandTranslation + interactionMapping.PoseData.Position;
                    }
                }
                else
                {
                    targetPosition = m_moveLogic.Update(GetPointersCentroid(), IsNearManipulation());
                }

                float lerpAmount = GetLerpAmount();
                Quaternion smoothedRotation = Quaternion.Lerp(hostTransform.rotation, targetRotation, lerpAmount);
                Vector3 smoothedPosition = Vector3.Lerp(hostTransform.position, targetPosition, lerpAmount);
                hostTransform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
            }
        }

        private void HandleTwoHandManipulationStarted(State newState)
        {
            var handPositionMap = GetHandPositionMap();
            targetRotationTwoHands = hostTransform.rotation;

            if ((newState & State.Rotating) > 0)
            {
                m_rotateLogic.Setup(handPositionMap, hostTransform, ConstraintOnRotation);
            }
            if ((newState & State.Moving) > 0)
            {
                m_moveLogic.Setup(GetPointersCentroid(), hostTransform.position);
            }
            if ((newState & State.Scaling) > 0)
            {
                m_scaleLogic.Setup(handPositionMap, hostTransform);
            }
        }
        private void HandleTwoHandManipulationEnded() { }

        private void HandleOneHandMoveStarted()
        {
            Assert.IsTrue(pointerIdToPointerMap.Count == 1);
            IMixedRealityPointer pointer = pointerIdToPointerMap.Values.First();

            m_moveLogic.Setup(GetPointersCentroid(), hostTransform.position);

            var interactionMapping = GetSpatialGripInfoForController(pointer.Controller);
            if (interactionMapping != null)
            {
                // Calculate relative transform from object to hand.
                Quaternion worldToPalmRotation = Quaternion.Inverse(interactionMapping.PoseData.Rotation);
                objectToHandRotation = worldToPalmRotation * hostTransform.rotation;
                objectToHandTranslation = (hostTransform.position - interactionMapping.PoseData.Position);
                if (oneHandRotationModeNear == RotateInOneHandType.RotateAboutGrabPoint)
                {
                    objectToHandTranslation = worldToPalmRotation * objectToHandTranslation;
                }
            }
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
            OnManipulationStarted.Invoke(new ManipulationEventData { IsNearInteraction = isNearManipulation });

        }
        private void HandleManipulationEnded()
        {
            // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
            // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
            OnManipulationEnded.Invoke(new ManipulationEventData { IsNearInteraction = isNearManipulation });

        }
        #endregion Private Event Handlers

        #region Unused Event Handlers
        /// <inheritdoc />
        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
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
                handPositionMap.Add(item.Key, item.Value.Position);
            }
            return handPositionMap;
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            bool isFar = !(eventData.Pointer is IMixedRealityNearPointer);
            if (isFar && !AllowFarManipulation)
            {
                return;
            }
            OnHoverEntered.Invoke(new ManipulationEventData { IsNearInteraction = !isFar });
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            bool isFar = !(eventData.Pointer is IMixedRealityNearPointer);
            if (isFar && !AllowFarManipulation)
            {
                return;
            }
            OnHoverExited.Invoke(new ManipulationEventData { IsNearInteraction = !isFar });
        }
        #endregion
    }
}
