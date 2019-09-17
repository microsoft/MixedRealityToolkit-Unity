// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This script allows for an object to be movable, scalable, and rotatable with one or two hands. 
    /// You may also configure the script on only enable certain manipulations. The script works with 
    /// both HoloLens' gesture input and immersive headset's motion controller input.
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_ManipulationHandler.html")]
    public class ObjectManipulator : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusChangedHandler
    {
        #region Public Enums
        [System.Flags]
        public enum HandMovementType
        {
            OneHanded = 1 << 0,
            TwoHanded = 1 << 1,
        }
        [System.Flags]
        public enum TwoHandedManipulation
        {
            Move = 1 << 0,
            Rotate = 1 << 1,
            Scale = 1 << 2
        };
        public enum RotateInOneHandType
        {
            MaintainRotationToUser,
            GravityAlignedMaintainRotationToUser,
            FaceUser,
            FaceAwayFromUser,
            MaintainOriginalRotation,
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
        
        [SerializeField]
        [EnumFlags]
        [Tooltip("Can manipulation be done only with one hand, only with two hands, or with both?")]
        private HandMovementType manipulationType = HandMovementType.OneHanded | HandMovementType.TwoHanded;

        public HandMovementType ManipulationType
        {
            get => manipulationType;
            set => manipulationType = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("What manipulation will two hands perform?")]
        private TwoHandedManipulation twoHandedManipulationType = TwoHandedManipulation.Move | TwoHandedManipulation.Rotate | TwoHandedManipulation.Scale;
        
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
            set => constraintOnRotation = value;
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
        private float smoothingAmount = 0.001f;

        public float SmoothingAmount
        {
            get => smoothingAmount;
            set => smoothingAmount = value;
        }

        #endregion Serialized Fields

        #region Event handlers
        public ManipulationEvent OnManipulationStarted = new ManipulationEvent();
        public ManipulationEvent OnManipulationEnded = new ManipulationEvent();
        public ManipulationEvent OnHoverEntered = new ManipulationEvent();
        public ManipulationEvent OnHoverExited = new ManipulationEvent();
        #endregion

        #region Private Properties
        
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

            public PointerData(IMixedRealityPointer pointer, Vector3 worldGrabPoint) : this()
            {
                this.pointer = pointer;
                this.initialGrabPointInPointer = Quaternion.Inverse(pointer.Rotation) * (worldGrabPoint - pointer.Position);
            }

            public bool IsNearPointer => pointer is IMixedRealityNearPointer;

            /// Returns the grab point on the manipulated object in world space
            public Vector3 GrabPoint => (pointer.Rotation * initialGrabPointInPointer) + pointer.Position;
        }

        private Dictionary<uint, PointerData> pointerIdToPointerMap = new Dictionary<uint, PointerData>();
        private Quaternion objectToHandRotation;
        private bool isNearManipulation;
        private bool isManipulationStarted;

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

            rigidBody = GetComponent<Rigidbody>();
            scaleHandler = GetComponent<TransformScaleHandler>();
        }
        #endregion MonoBehaviour Functions

        #region Private Methods
        private Vector3 GetPointersGrabPoint()
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

        private MixedRealityPose GetPointersPose()
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

            return new MixedRealityPose
            {
                Position = sumPos / Math.Max(1, count),
                Rotation = Quaternion.LookRotation(sumDir / Math.Max(1, count))
            };
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
                if (item.Value.IsNearPointer)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Releases the object that is currently manipulated
        /// </summary>
        public void ForceEndManipulation()
        {
            // end manipulation
            if (isManipulationStarted)
            {
                HandleManipulationEnded(GetPointersGrabPoint(), GetPointersVelocity(), GetPointersAngularVelocity());
            }
            pointerIdToPointerMap.Clear();
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
            if (manipulationType != HandMovementType.OneHanded || pointerIdToPointerMap.Count == 0)
            {
                uint id = eventData.Pointer.PointerId;
                // Ignore poke pointer events
                if (!eventData.used && !pointerIdToPointerMap.ContainsKey(id))
                {
                    // cache start ptr grab point
                    pointerIdToPointerMap.Add(id, new PointerData(eventData.Pointer, eventData.Pointer.Result.Details.Point));

                    // Call manipulation started handlers
                    var handsPressedCount = pointerIdToPointerMap.Count;
                    if (manipulationType.HasFlag(HandMovementType.TwoHanded) && handsPressedCount > 1)
                    {
                        if (!manipulationType.HasFlag(HandMovementType.OneHanded))
                        {
                            HandleManipulationStarted();
                        }
                        HandleTwoHandManipulationStarted();
                    }
                    else if (manipulationType.HasFlag(HandMovementType.OneHanded) && handsPressedCount == 1)
                    {
                        if (!isManipulationStarted)
                        {
                            HandleManipulationStarted();
                        }
                        HandleOneHandMoveStarted();
                    }
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
            // Call manipulation updated handlers
            var handsPressedCount = pointerIdToPointerMap.Count;
            if (manipulationType.HasFlag(HandMovementType.OneHanded) && handsPressedCount == 1)
            {
                HandleOneHandMoveUpdated();
            }
            else if (manipulationType.HasFlag(HandMovementType.TwoHanded) && handsPressedCount > 1)
            {
                HandleTwoHandManipulationUpdated();
            }
        }

        /// <inheritdoc />
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            // Get pointer data before they are removed from the map
            Vector3 grabPoint = GetPointersGrabPoint();
            Vector3 velocity = GetPointersVelocity();
            Vector3 angularVelocity = GetPointersAngularVelocity();

            uint id = eventData.Pointer.PointerId;
            if (pointerIdToPointerMap.ContainsKey(id))
            {
                pointerIdToPointerMap.Remove(id);
            }

            // Call manipulation ended handlers
            var handsPressedCount = pointerIdToPointerMap.Count;
            if (manipulationType.HasFlag(HandMovementType.TwoHanded) && handsPressedCount == 1)
            {
                if (manipulationType.HasFlag(HandMovementType.OneHanded))
                {
                    HandleOneHandMoveStarted();
                }
                else
                {
                    HandleManipulationEnded(grabPoint, velocity, angularVelocity);
                }
            }
            else if (isManipulationStarted && handsPressedCount == 0)
            {
                HandleManipulationEnded(grabPoint, velocity, angularVelocity);
            }

            eventData.Use();
        }

        #endregion Hand Event Handlers

        #region Private Event Handlers
        private void HandleTwoHandManipulationStarted()
        {
            var handPositionArray = GetHandPositionArray();

            if (twoHandedManipulationType.HasFlag(TwoHandedManipulation.Rotate))
            {
                rotateLogic.Setup(handPositionArray, hostTransform, ConstraintOnRotation);
            }
            if (twoHandedManipulationType.HasFlag(TwoHandedManipulation.Move))
            {
                MixedRealityPose pointerPose = GetPointersPose();
                MixedRealityPose hostPose = new MixedRealityPose(hostTransform.position, hostTransform.rotation);
                moveLogic.Setup(pointerPose, GetPointersGrabPoint(), hostPose, hostTransform.localScale);
            }
            if (twoHandedManipulationType.HasFlag(TwoHandedManipulation.Scale))
            {
                scaleLogic.Setup(handPositionArray, hostTransform);
            }
        }

        private void HandleTwoHandManipulationUpdated()
        {
            var targetPosition = hostTransform.position;
            var targetScale = hostTransform.localScale;
            var targetRotation = hostTransform.rotation;

            var handPositionArray = GetHandPositionArray();

            if (twoHandedManipulationType.HasFlag(TwoHandedManipulation.Rotate))
            {
                targetRotation = rotateLogic.Update(handPositionArray, targetRotation, constraintOnRotation);
            }
            if (twoHandedManipulationType.HasFlag(TwoHandedManipulation.Scale))
            {
                targetScale = scaleLogic.UpdateMap(handPositionArray);
            }

            if (twoHandedManipulationType.HasFlag(TwoHandedManipulation.Move))
            {
                MixedRealityPose pose = GetPointersPose();
                targetPosition = moveLogic.Update(pose, targetRotation, targetScale, constraintOnMovement);
            }

            float lerpAmount = GetLerpAmount();
            hostTransform.position = Vector3.Lerp(hostTransform.position, targetPosition, lerpAmount);
            // Currently the two hand rotation algorithm doesn't allow for lerping, but it should. Fix this.
            hostTransform.rotation = Quaternion.Lerp(hostTransform.rotation, targetRotation, lerpAmount);

            if (scaleHandler != null)
            {
                targetScale = scaleHandler.ClampScale(targetScale);
            }
            hostTransform.localScale = Vector3.Lerp(hostTransform.localScale, targetScale, lerpAmount);
        }

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

            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            MixedRealityPose hostPose = new MixedRealityPose(hostTransform.position, hostTransform.rotation);
            moveLogic.Setup(pointerPose, pointerData.GrabPoint, hostPose, hostTransform.localScale);

            startObjectRotationCameraSpace = Quaternion.Inverse(CameraCache.Main.transform.rotation) * hostTransform.rotation;
            var cameraFlat = CameraCache.Main.transform.forward;
            cameraFlat.y = 0;
            var hostForwardFlat = hostTransform.forward;
            hostForwardFlat.y = 0;
            var hostRotFlat = Quaternion.LookRotation(hostForwardFlat, Vector3.up);
            startObjectRotationFlatCameraSpace = Quaternion.Inverse(Quaternion.LookRotation(cameraFlat, Vector3.up)) * hostRotFlat;
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
                case RotateInOneHandType.RotateAboutGrabPoint:
                    targetRotation = pointer.Rotation * objectToHandRotation;
                    break;
            }

            targetRotation = ApplyConstraints(targetRotation);
            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            Vector3 targetPosition = moveLogic.Update(pointerPose, targetRotation, hostTransform.localScale, constraintOnMovement);

            float lerpAmount = GetLerpAmount();
            Quaternion smoothedRotation = Quaternion.Lerp(hostTransform.rotation, targetRotation, lerpAmount);
            Vector3 smoothedPosition = Vector3.Lerp(hostTransform.position, targetPosition, lerpAmount);
            hostTransform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
        }

        private void HandleManipulationStarted()
        {
            isManipulationStarted = true;
            isNearManipulation = IsNearManipulation();
            // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
            // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
            if (OnManipulationStarted != null)
            {
                OnManipulationStarted.Invoke(new ManipulationEventData
                {
                    ManipulationSource = gameObject,
                    IsNearInteraction = isNearManipulation,
                    PointerCentroid = GetPointersGrabPoint(),
                    PointerVelocity = GetPointersVelocity(),
                    PointerAngularVelocity = GetPointersAngularVelocity()
                });
            }

            if (rigidBody != null)
            {
                wasKinematic = rigidBody.isKinematic;
                rigidBody.isKinematic = true;
            }
        }

        private void HandleManipulationEnded(Vector3 pointerGrabPoint, Vector3 pointerVelocity, Vector3 pointerAnglularVelocity)
        {
            isManipulationStarted = false;
            // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
            // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
            if (OnManipulationEnded != null)
            {
                OnManipulationEnded.Invoke(new ManipulationEventData
                {
                    ManipulationSource = gameObject,
                    IsNearInteraction = isNearManipulation,
                    PointerCentroid = pointerGrabPoint,
                    PointerVelocity = pointerVelocity,
                    PointerAngularVelocity = pointerAnglularVelocity
                }); 
            }
            
            ReleaseRigidBody(pointerVelocity, pointerAnglularVelocity);
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
            if (smoothingActive == false || smoothingAmount == 0)
            {
                return 1;
            }
            // Obtained from "Frame-rate independent smoothing"
            // www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
            // We divide by max value to give the slider a bit more sensitivity.
            return 1.0f - Mathf.Pow(smoothingAmount, Time.deltaTime);
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
            if (isFar && !AllowFarManipulation)
            {
                return;
            }

            if (eventData.OldFocusedObject == null ||
                !eventData.OldFocusedObject.transform.IsChildOf(transform))
            {
                if (OnHoverEntered != null)
                {
                    OnHoverEntered.Invoke(new ManipulationEventData
                    {
                        ManipulationSource = gameObject,
                        IsNearInteraction = !isFar
                    });
                }
            }
            else if (eventData.NewFocusedObject == null ||
                    !eventData.NewFocusedObject.transform.IsChildOf(transform))
            {
                if (OnHoverExited != null)
                {
                    OnHoverExited.Invoke(new ManipulationEventData
                    {
                        ManipulationSource = gameObject,
                        IsNearInteraction = !isFar
                    });
                }
            }
        }

        private void ReleaseRigidBody(Vector3 velocity, Vector3 angularVelocity)
        {
            if (rigidBody != null)
            {
                rigidBody.isKinematic = wasKinematic;

                if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepVelocity))
                {
                    rigidBody.velocity = velocity;
                }

                if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepAngularVelocity))
                {
                    rigidBody.angularVelocity = angularVelocity;
                }
            }
        }

        private PointerData GetFirstPointer()
        {
            // We may be able to do this without allocating memory.
            // Moving to a method for later investigation.
            return pointerIdToPointerMap.Values.First();
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

            return diffRotation * hostWorldRotationOnManipulationStart;
        }

        #endregion
    }
}
