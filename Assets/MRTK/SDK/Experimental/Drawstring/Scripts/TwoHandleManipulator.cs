// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// TODO insert drawstring summary
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_ObjectManipulator.html")]
    public class TwoHandleManipulator : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusChangedHandler
    {
        #region Public Enums

        /// <summary>
        /// Specifies the stretch behavior when grabbing with only one handle.
        /// 
        /// StretchAboutCenter will make one-handed stretching essentially identical
        /// to two-handed stretching, with both handles stretching away from the center.
        /// 
        /// KeepOtherEndSteady will anchor the un-grabbed handle, causing the center
        /// of the manipulator to drift, but apppearing more natural/realistic.
        /// </summary>
        public enum OneHandStretchType
        {
            StretchAboutCenter,
            KeepOtherEndSteady
        };

        /// <summary>
        /// Indicates which side/handle a MixedRealityPointer is
        /// grabbing.
        /// </summary>
        public enum HandleSide
        {
            Left,
            Right
        };
        #endregion Public Enums

        #region Serialized Fields

        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform = null;

        /// <summary>
        /// Transform that will be dragged. Defaults to the object of the component.
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
            set => hostTransform = value;
        }

        [SerializeField]
        [Tooltip("Transform representing the left handle")]
        private Transform leftHandle = null;

        /// <summary>
        /// Transform representing the left handle.
        /// </summary>
        public Transform LeftHandle
        {
            get => leftHandle;
            set => leftHandle = value;
        }

        [SerializeField]
        [Tooltip("Transform representing the right handle")]
        private Transform rightHandle = null;

        /// <summary>
        /// Transform representing the right handle.
        /// </summary>
        public Transform RightHandle
        {
            get => rightHandle;
            set => rightHandle = value;
        }

        [SerializeField]
        [Tooltip("Properties of the damped harmonic oscillator differential system")]
        public ElasticProperties elasticProperties;

        // Necessary because Unity cannot show generics in inspector.
        [SerializeField]
        public float minStretch;
        [SerializeField]
        public float maxStretch;
        [SerializeField]
        public bool snapToMax;
        [SerializeField]
        public float[] snapPoints;

        [SerializeField]
        [Tooltip("Stretching behavior when grabbing with only one handle")]
        private OneHandStretchType oneHandStretchBehavior = OneHandStretchType.StretchAboutCenter;

        /// <summary>
        /// Stretching behavior when grabbing with only one handle
        /// </summary>
        public OneHandStretchType OneHandStretchBehavior
        {
            get => oneHandStretchBehavior;
            set => oneHandStretchBehavior = value;
        }


        [SerializeField]
        [Tooltip("DataProvider for the line connecting the handles")]
        private SimpleLineDataProvider lineDataProvider = null;

        /// <summary>
        /// DataProvider for the line connecting the handles.
        /// </summary>
        public SimpleLineDataProvider LineDataProvider
        {
            get => lineDataProvider;
            set => lineDataProvider = value;
        }

        [SerializeField]
        [Tooltip("Text label showing current drawstring value")]
        private TextMeshPro valueLabel = null;

        /// <summary>
        /// DataProvider for the line connecting the handles.
        /// </summary>
        public TextMeshPro ValueLabel
        {
            get => valueLabel;
            set => valueLabel = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("Can manipulation be done only with one hand, only with two hands, or with both?")]
        private ManipulationHandFlags manipulationType = ManipulationHandFlags.OneHanded | ManipulationHandFlags.TwoHanded;

        /// <summary>
        /// Can manipulation be done only with one hand, only with two hands, or with both?
        /// </summary>
        public ManipulationHandFlags ManipulationType
        {
            get => manipulationType;
            set => manipulationType = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("What manipulation will two hands perform?")]
        private TransformFlags twoHandedManipulationType = TransformFlags.Move | TransformFlags.Rotate | TransformFlags.Scale;

        /// <summary>
        /// What manipulation will two hands perform?
        /// </summary>
        public TransformFlags TwoHandedManipulationType
        {
            get => twoHandedManipulationType;
            set => twoHandedManipulationType = value;
        }

        [SerializeField]
        [Tooltip("Specifies whether manipulation can be done using far interaction with pointers.")]
        private bool allowFarManipulation = true;

        /// <summary>
        /// Specifies whether manipulation can be done using far interaction with pointers.
        /// </summary>
        public bool AllowFarManipulation
        {
            get => allowFarManipulation;
            set => allowFarManipulation = value;
        }

        [SerializeField]
        [Tooltip("Check to enable frame-rate independent smoothing.")]
        private bool smoothingActive = true;

        /// <summary>
        /// Check to enable frame-rate independent smoothing.
        /// </summary>
        public bool SmoothingActive
        {
            get => smoothingActive;
            set => smoothingActive = value;
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

        /// <summary>
        /// Returns true when one or more handles are being interacted with.
        /// </summary>
        public bool IsGrabbed => pointerIdToPointerMap.Count > 0;

        /// <summary>
        /// Returns true when one or more handles are being interacted with.
        /// </summary>
        public float HandleDistance => (leftHandle.localPosition - rightHandle.localPosition).magnitude;

        #endregion Serialized Fields

        #region Event handlers
        [Header("Manipulation Events")]
        [SerializeField]
        [FormerlySerializedAs("OnManipulationStarted")]
        private ManipulationEvent onManipulationStarted = new ManipulationEvent();

        /// <summary>
        /// Unity event raised on manipulation started
        /// </summary>
        public ManipulationEvent OnManipulationStarted
        {
            get => onManipulationStarted;
            set => onManipulationStarted = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnManipulationEnded")]
        private ManipulationEvent onManipulationEnded = new ManipulationEvent();

        /// <summary>
        /// Unity event raised on manipulation ended
        /// </summary>
        public ManipulationEvent OnManipulationEnded
        {
            get => onManipulationEnded;
            set => onManipulationEnded = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnHoverEntered")]
        private ManipulationEvent onHoverEntered = new ManipulationEvent();

        /// <summary>
        /// Unity event raised on hover started
        /// </summary>
        public ManipulationEvent OnHoverEntered
        {
            get => onHoverEntered;
            set => onHoverEntered = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnHoverExited")]
        private ManipulationEvent onHoverExited = new ManipulationEvent();

        /// <summary>
        /// Unity event raised on hover ended
        /// </summary>
        public ManipulationEvent OnHoverExited
        {
            get => onHoverExited;
            set => onHoverExited = value;
        }
        #endregion

        #region Private Properties

        private ManipulationMoveLogic moveLogic;
        private TwoHandScaleLogic scaleLogic;
        private TwoHandRotateLogic rotateLogic;
        private ElasticHandleLogic elasticLogic;

        private Vector3 leftLineOffset, rightLineOffset;

        private float lastUpdateTime = 0.0f;


        /// <summary>
        /// Holds the pointer and the initial intersection point of the pointer ray 
        /// with the object on pointer down in pointer space
        /// </summary>
        private struct PointerData
        {
            public IMixedRealityPointer pointer;
            public HandleSide side;
            private Vector3 initialGrabPointInPointer;

            public PointerData(IMixedRealityPointer pointer, Vector3 worldGrabPoint, HandleSide side) : this()
            {
                this.pointer = pointer;
                this.side = side;
                this.initialGrabPointInPointer = Quaternion.Inverse(pointer.Rotation) * (worldGrabPoint - pointer.Position);
            }

            public bool IsNearPointer => pointer is IMixedRealityNearPointer;

            /// Returns the grab point on the manipulated object in world space
            public Vector3 GrabPoint => (pointer.Rotation * initialGrabPointInPointer) + pointer.Position;
        }

        private Dictionary<uint, PointerData> pointerIdToPointerMap = new Dictionary<uint, PointerData>();
        private Quaternion objectToGripRotation;
        private bool isNearManipulation;
        private bool isManipulationStarted;

        private Rigidbody rigidBody;
        private bool wasKinematic = false;

        private ConstraintManager constraints;

        private bool IsOneHandedManipulationEnabled => manipulationType.HasFlag(ManipulationHandFlags.OneHanded) && pointerIdToPointerMap.Count == 1;
        private bool IsTwoHandedManipulationEnabled => manipulationType.HasFlag(ManipulationHandFlags.TwoHanded) && pointerIdToPointerMap.Count > 1;

        #endregion

        #region MonoBehaviour Functions

        private void Awake()
        {
            moveLogic = new ManipulationMoveLogic();
            rotateLogic = new TwoHandRotateLogic();
            elasticLogic = new ElasticHandleLogic();
        }
        private void Start()
        {
            rigidBody = HostTransform.GetComponent<Rigidbody>();
            constraints = new ConstraintManager(gameObject);

            leftLineOffset = lineDataProvider.transform.localPosition - leftHandle.localPosition;
            rightLineOffset = (lineDataProvider.transform.localPosition + lineDataProvider.EndPoint.Position) - rightHandle.localPosition;
            
        }

        private void Update()
        {
            if(pointerIdToPointerMap.Count == 0)
            {
                var elasticDistance = elasticLogic.Update(null, null, Time.time - lastUpdateTime);
                lastUpdateTime = Time.time;
                leftHandle.localPosition = Vector3.right * -elasticDistance / 2.0f;
                rightHandle.localPosition = Vector3.right * elasticDistance / 2.0f;

                UpdateLineData();
            }
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
            if (eventData.used ||
                (!allowFarManipulation && eventData.Pointer as IMixedRealityNearPointer == null))
            {
                return;
            }

            // If we only allow one handed manipulations, check there is no hand interacting yet. 
            if (manipulationType != ManipulationHandFlags.OneHanded || pointerIdToPointerMap.Count == 0)
            {
                uint id = eventData.Pointer.PointerId;
                // Ignore poke pointer events
                if (!pointerIdToPointerMap.ContainsKey(id))
                {
                    var whichHandle = HandleSide.Left;
                    if(Vector3.Distance(eventData.Pointer.Result.Details.Point, rightHandle.position) <
                        Vector3.Distance(eventData.Pointer.Result.Details.Point, leftHandle.position))
                    {
                        whichHandle = HandleSide.Right;
                    }
                    // cache start ptr grab point
                    pointerIdToPointerMap.Add(id, new PointerData(eventData.Pointer, eventData.Pointer.Result.Details.Point, whichHandle));

                    // Call manipulation started handlers
                    if (IsTwoHandedManipulationEnabled)
                    {
                        if (!isManipulationStarted)
                        {
                            HandleManipulationStarted();
                        }
                        HandleTwoHandManipulationStarted();
                    }
                    else if (IsOneHandedManipulationEnabled)
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
                // as long as the ObjectManipulator is active.
                // This is due to us reacting to both "Select" and "Grip" events.
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // Call manipulation updated handlers
            if (IsOneHandedManipulationEnabled)
            {
                HandleOneHandMoveUpdated();
            }
            else if (IsTwoHandedManipulationEnabled)
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
            if (manipulationType.HasFlag(ManipulationHandFlags.TwoHanded) && handsPressedCount == 1)
            {
                if (manipulationType.HasFlag(ManipulationHandFlags.OneHanded))
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

            var leftHandlePointer = GetHandlePointer(HandleSide.Left).Value;
            var rightHandlePointer = GetHandlePointer(HandleSide.Right).Value;
            var elasticExtent = new ElasticExtentProperties<float>
            {
                minStretch = minStretch,
                maxStretch = maxStretch,
                snapToMax = snapToMax,
                snapPoints = snapPoints
            };
            elasticLogic.Setup(leftHandle.position, rightHandle.position, elasticExtent,
                elasticProperties);

            if (twoHandedManipulationType.HasFlag(TransformFlags.Rotate))
            {
                rotateLogic.Setup(handPositionArray, HostTransform);
            }
            if (twoHandedManipulationType.HasFlag(TransformFlags.Move))
            {
                MixedRealityPose pointerPose = new MixedRealityPose((leftHandlePointer.GrabPoint + rightHandlePointer.GrabPoint) / 2.0f);
                MixedRealityPose hostPose = new MixedRealityPose(HostTransform.position, HostTransform.rotation);
                moveLogic.Setup(pointerPose, GetPointersGrabPoint(), hostPose, HostTransform.localScale);
            }
        }

        private void HandleTwoHandManipulationUpdated()
        {

            var targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);
            var handPositionArray = GetHandPositionArray();
            var leftHandlePointer = GetHandlePointer(HandleSide.Left).Value;
            var rightHandlePointer = GetHandlePointer(HandleSide.Right).Value;

            var elasticDistance = elasticLogic.Update(leftHandlePointer.GrabPoint, rightHandlePointer.GrabPoint, Time.time - lastUpdateTime, transform.right);
            lastUpdateTime = Time.time;

            // Set handle positions based on the calculated elastic dynamics.
            leftHandle.localPosition = Vector3.right * -elasticDistance / 2.0f;
            rightHandle.localPosition = Vector3.right * elasticDistance / 2.0f;

            // We use the standard rotateLogic from ObjectManipulator.
            if (twoHandedManipulationType.HasFlag(TransformFlags.Rotate))
            {
                targetTransform.Rotation = rotateLogic.Update(handPositionArray, targetTransform.Rotation);
                constraints.ApplyRotationConstraints(ref targetTransform, false, IsNearManipulation());
            }

            // Query the center position from the elastic logic. Given that we're already guaranteed to have
            // two pointers active on this squeeze session, this is basically equivalent to the
            // geometric mean of the two pointer locations. But we query the elasticLogic regardless, for consistency.
            targetTransform.Position = elasticLogic.GetCenterPosition(leftHandlePointer.GrabPoint, rightHandlePointer.GrabPoint);

            ApplyTargetTransform(targetTransform);
            UpdateLineData();
        }

        private void HandleOneHandMoveStarted()
        {
            Assert.IsTrue(pointerIdToPointerMap.Count == 1);
            var elasticExtent = new ElasticExtentProperties<float>
            {
                minStretch = minStretch,
                maxStretch = maxStretch,
                snapToMax = snapToMax,
                snapPoints = snapPoints
            };
            elasticLogic.Setup(leftHandle.position, rightHandle.position, elasticExtent,
                elasticProperties);
        }

        private void HandleOneHandMoveUpdated()
        {
            Debug.Assert(pointerIdToPointerMap.Count == 1);

            var targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);
            
            // Nullable references to these pointers.
            PointerData? leftHandlePointer = GetHandlePointer(HandleSide.Left);
            PointerData? rightHandlePointer = GetHandlePointer(HandleSide.Right);

            float elasticDistance;
            if (oneHandStretchBehavior == OneHandStretchType.KeepOtherEndSteady) {
                // If the one-hand behavior is to keep the other handle "steady",
                // we fabricate an imaginary pointer that is held at the location
                // of the handle that is *not* being grabbed.
                elasticDistance = elasticLogic.Update(
                    leftHandlePointer.HasValue ? leftHandlePointer.Value.GrabPoint : leftHandle.position,
                    rightHandlePointer.HasValue ? rightHandlePointer.Value.GrabPoint : rightHandle.position,
                    Time.time - lastUpdateTime,
                    transform.right // Handle axis is the right vector.
                );
                lastUpdateTime = Time.time;
            } else
            {
                // If we are one-hand stretching around the center, elasticLogic.Update()
                // will handle the missing pointers for us and calculate a proper
                // centered stretch behavior.
                elasticDistance = elasticLogic.Update(leftHandlePointer?.GrabPoint, rightHandlePointer?.GrabPoint, Time.time - lastUpdateTime);
                lastUpdateTime = Time.time;
            }

            // Set handle positions based on the calculated elastic dynamics.
            leftHandle.localPosition = Vector3.right * -elasticDistance / 2.0f;
            rightHandle.localPosition = Vector3.right * elasticDistance / 2.0f;

            // If we are "keeping the other end steady", we need to
            // recalculate the centerpoint every frame based on handle locations.
            if(oneHandStretchBehavior == OneHandStretchType.KeepOtherEndSteady)
            {
                targetTransform.Position = elasticLogic.GetCenterPosition(leftHandlePointer?.GrabPoint, rightHandlePointer?.GrabPoint);
            }

            ApplyTargetTransform(targetTransform);
            UpdateLineData();
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
                    Pointer = GetFirstPointer().pointer,
                    PointerCentroid = GetPointersGrabPoint(),
                    PointerVelocity = GetPointersVelocity(),
                    PointerAngularVelocity = GetPointersAngularVelocity()
                });
            }

            if (rigidBody != null)
            {
                wasKinematic = rigidBody.isKinematic;
                rigidBody.isKinematic = false;
            }

            constraints.Initialize(new MixedRealityPose(HostTransform.position, HostTransform.rotation));
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

            //ReleaseRigidBody(pointerVelocity, pointerAnglularVelocity);
        }

        #endregion Private Event Handlers

        #region Unused Event Handlers
        /// <inheritdoc />
        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
        public void OnBeforeFocusChange(FocusEventData eventData) { }

        #endregion Unused Event Handlers

        #region Private methods

        private void ApplyTargetTransform(MixedRealityTransform targetTransform)
        {
            if (rigidBody == null)
            {
                HostTransform.position = SmoothTo(HostTransform.position, targetTransform.Position, moveLerpTime);
                HostTransform.rotation = SmoothTo(HostTransform.rotation, targetTransform.Rotation, rotateLerpTime);
                HostTransform.localScale = SmoothTo(HostTransform.localScale, targetTransform.Scale, scaleLerpTime);
            }
            else
            {
                rigidBody.velocity = ((1f - Mathf.Pow(moveLerpTime, Time.deltaTime)) / Time.deltaTime) * (targetTransform.Position - HostTransform.position);

                var relativeRotation = targetTransform.Rotation * Quaternion.Inverse(HostTransform.rotation);
                relativeRotation.ToAngleAxis(out float angle, out Vector3 axis);
                if (angle > 180f)
                    angle -= 360f;
                if (axis.IsValidVector())
                {
                    rigidBody.angularVelocity = ((1f - Mathf.Pow(rotateLerpTime, Time.deltaTime)) / Time.deltaTime) * (axis.normalized * angle * Mathf.Deg2Rad);
                }

                HostTransform.localScale = SmoothTo(HostTransform.localScale, targetTransform.Scale, scaleLerpTime);
            }
        }

        private void UpdateLineData()
        {
            lineDataProvider.transform.localPosition = leftHandle.localPosition + leftLineOffset;
            lineDataProvider.EndPoint = new MixedRealityPose(rightHandle.localPosition - leftHandle.localPosition + rightLineOffset);
        }

        private Vector3 SmoothTo(Vector3 source, Vector3 goal, float lerpTime)
        {
            return Vector3.Lerp(source, goal, (!smoothingActive || lerpTime == 0f) ? 1f : 1f - Mathf.Pow(lerpTime, Time.deltaTime));
        }

        private Quaternion SmoothTo(Quaternion source, Quaternion goal, float slerpTime)
        {
            return Quaternion.Slerp(source, goal, (!smoothingActive || slerpTime == 0f) ? 1f : 1f - Mathf.Pow(slerpTime, Time.deltaTime));
        }

        /// <summary>
        /// Acquire the current position of the pointer that is
        /// grabbing the specified handle.
        /// </summary>
        /// <returns>Nullable PointerData</returns>
        private PointerData? GetHandlePointer(HandleSide side)
        {
            if(pointerIdToPointerMap.Any(x => x.Value.side == side))
            {
                return pointerIdToPointerMap.First(x => x.Value.side == side).Value;
            } else
            {
                return null;
            }
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
                        Pointer = eventData.Pointer,
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
                        Pointer = eventData.Pointer,
                        IsNearInteraction = !isFar
                    });
                }
            }
        }

        //private void ReleaseRigidBody(Vector3 velocity, Vector3 angularVelocity)
        //{
        //    if (rigidBody != null)
        //    {
        //        rigidBody.isKinematic = wasKinematic;

        //        if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepVelocity))
        //        {
        //            rigidBody.velocity = velocity;
        //        }

        //        if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepAngularVelocity))
        //        {
        //            rigidBody.angularVelocity = angularVelocity;
        //        }
        //    }
        //}

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
