// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
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
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/object-manipulator")]
    [RequireComponent(typeof(ConstraintManager))]
    public class ObjectManipulator : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusChangedHandler, IMixedRealitySourcePoseHandler
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
        public enum RotateInOneHandType
        {
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
        private RotateInOneHandType oneHandRotationModeNear = RotateInOneHandType.RotateAboutGrabPoint;

        /// <summary>
        /// Rotation behavior of object when using one hand near
        /// </summary>
        public RotateInOneHandType OneHandRotationModeNear
        {
            get => oneHandRotationModeNear;
            set => oneHandRotationModeNear = value;
        }

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand at distance")]
        private RotateInOneHandType oneHandRotationModeFar = RotateInOneHandType.RotateAboutGrabPoint;

        /// <summary>
        /// Rotation behavior of object when using one hand at distance
        /// </summary>
        public RotateInOneHandType OneHandRotationModeFar
        {
            get => oneHandRotationModeFar;
            set => oneHandRotationModeFar = value;
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

        /// <summary>
        /// Obsolete: Whether to enable frame-rate independent smoothing.
        /// </summary>
        [Obsolete("SmoothingActive is obsolete and will be removed in a future version. Applications should use SmoothingFar, SmoothingNear or a combination of the two.")]
        public bool SmoothingActive
        {
            get => smoothingFar;
            set => smoothingFar = value;
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

        [SerializeField]
        [Tooltip("Elastics Manager slot to enable elastics simulation when manipulating the object.")]
        private ElasticsManager elasticsManager;
        /// <summary>
        /// Elastics Manager slot to enable elastics simulation when manipulating the object.
        /// </summary>
        public ElasticsManager ElasticsManager
        {
            get => elasticsManager;
            set => elasticsManager = value;
        }

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

        #endregion Event Handlers

        #region Private Properties

        private ManipulationMoveLogic moveLogic;
        private TwoHandScaleLogic scaleLogic;
        private TwoHandRotateLogic rotateLogic;
        private ITransformSmoothingLogic smoothingLogic;

        /// <summary>
        /// Holds the pointer and the initial intersection point of the pointer ray
        /// with the object on pointer down in pointer space
        /// </summary>
        private readonly struct PointerData
        {
            public PointerData(IMixedRealityPointer pointer, Vector3 worldGrabPoint) : this()
            {
                initialGrabPointInPointer = Quaternion.Inverse(pointer.Rotation) * (worldGrabPoint - pointer.Position);
                Pointer = pointer;
                IsNearPointer = pointer is IMixedRealityNearPointer;
            }

            private readonly Vector3 initialGrabPointInPointer;

            public IMixedRealityPointer Pointer { get; }

            public bool IsNearPointer { get; }

            /// <summary>
            /// Returns the grab point on the manipulated object in world space.
            /// </summary>
            public Vector3 GrabPoint => (Pointer.Rotation * initialGrabPointInPointer) + Pointer.Position;
        }

        private List<PointerData> pointerDataList = new List<PointerData>();

        private Quaternion objectToGripRotation;
        private bool isNearManipulation;
        private bool isManipulationStarted;
        private bool isSmoothing;

        private Rigidbody rigidBody;
        private bool wasGravity = false;
        private bool wasKinematic = false;

        private bool IsOneHandedManipulationEnabled => manipulationType.IsMaskSet(ManipulationHandFlags.OneHanded) && pointerDataList.Count == 1;
        private bool IsTwoHandedManipulationEnabled => manipulationType.IsMaskSet(ManipulationHandFlags.TwoHanded) && pointerDataList.Count > 1;

        private Quaternion leftHandRotation;
        private Quaternion rightHandRotation;

        #endregion Private Properties

        #region MonoBehaviour Functions

        private void Awake()
        {
            moveLogic = new ManipulationMoveLogic();
            rotateLogic = new TwoHandRotateLogic();
            scaleLogic = new TwoHandScaleLogic();
            smoothingLogic = Activator.CreateInstance(transformSmoothingLogicType) as ITransformSmoothingLogic;

            if (elasticsManager)
            {
                elasticsManager.InitializeElastics(HostTransform);
            }
        }
        protected virtual void Start()
        {
            rigidBody = HostTransform.GetComponent<Rigidbody>();
            if (constraintsManager == null && EnableConstraints)
            {
                constraintsManager = gameObject.EnsureComponent<ConstraintManager>();
            }

            // Get child objects with NearInteractionGrabbable attached
            var children = GetComponentsInChildren<NearInteractionGrabbable>();

            if (children.Length == 0)
            {
                Debug.Log($"Near interactions are not enabled for {gameObject.name}. To enable near interactions, add a " +
                    $"{nameof(NearInteractionGrabbable)} component to {gameObject.name} or to a child object of {gameObject.name} that contains a collider.");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the unweighted average, or centroid, of all pointers'
        /// grab points, as defined by the PointerData.GrabPoint property.
        /// Does not use the rotation of each pointer; represents a pure
        /// geometric centroid of the grab points in world space.
        /// </summary>
        /// <returns>
        /// Worldspace grab point centroid of all pointers
        /// in pointerIdToPointerMap.
        /// </returns>
        private Vector3 GetPointersGrabPoint()
        {
            Vector3 sum = Vector3.zero;
            int pointerDataListCount = pointerDataList.Count;
            for (int i = 0; i < pointerDataListCount; i++)
            {
                PointerData pointerData = pointerDataList[i];
                sum += pointerData.GrabPoint;
            }
            return sum / Math.Max(1, pointerDataList.Count);
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
        private MixedRealityPose GetPointersPose()
        {
            Vector3 sumPos = Vector3.zero;
            Vector3 sumDir = Vector3.zero;
            int pointerDataListCount = pointerDataList.Count;
            for (int i = 0; i < pointerDataListCount; i++)
            {
                PointerData pointerData = pointerDataList[i];
                sumPos += pointerData.Pointer.Position;
                sumDir += pointerData.Pointer.Rotation * Vector3.forward;
            }

            int divisor = Math.Max(1, pointerDataList.Count);
            return new MixedRealityPose
            {
                Position = sumPos / divisor,
                Rotation = Quaternion.LookRotation(sumDir / divisor)
            };
        }

        private Vector3 GetPointersVelocity()
        {
            Vector3 sum = Vector3.zero;
            int numControllers = 0;
            int pointerDataListCount = pointerDataList.Count;
            for (int i = 0; i < pointerDataListCount; i++)
            {
                IMixedRealityController controller = pointerDataList[i].Pointer.Controller;
                // Check pointer has a valid controller (e.g. gaze pointer doesn't)
                if (controller != null)
                {
                    numControllers++;
                    sum += controller.Velocity;
                }
            }
            return sum / Math.Max(1, numControllers);
        }

        private Vector3 GetPointersAngularVelocity()
        {
            Vector3 sum = Vector3.zero;
            int numControllers = 0;
            int pointerDataListCount = pointerDataList.Count;
            for (int i = 0; i < pointerDataListCount; i++)
            {
                IMixedRealityController controller = pointerDataList[i].Pointer.Controller;
                // Check pointer has a valid controller (e.g. gaze pointer doesn't)
                if (controller != null)
                {
                    numControllers++;
                    sum += controller.AngularVelocity;
                }
            }
            return sum / Math.Max(1, numControllers);
        }

        private bool IsNearManipulation()
        {
            int pointerDataListCount = pointerDataList.Count;
            for (int i = 0; i < pointerDataListCount; i++)
            {
                if (pointerDataList[i].IsNearPointer)
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
            pointerDataList.Clear();
        }

        /// <summary>
        /// Gets the grab point for the given pointer id.
        /// Only use if you know that your given pointer id corresponds to a pointer that has grabbed
        /// this component.
        /// </summary>
        public Vector3 GetPointerGrabPoint(uint pointerId)
        {
            if (TryGetPointerDataWithId(pointerId, out PointerData pointerData))
            {
                return pointerData.GrabPoint;
            }
            return Vector3.zero;
        }

        #endregion Public Methods

        #region Hand Event Handlers

        /// <inheritdoc />
        public virtual void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (eventData.used ||
                eventData.Pointer == null ||
                eventData.Pointer.Result == null ||
                (!allowFarManipulation && eventData.Pointer as IMixedRealityNearPointer == null))
            {
                return;
            }

            // If we only allow one handed manipulations, check there is no hand interacting yet.
            if (manipulationType != ManipulationHandFlags.OneHanded || pointerDataList.Count == 0)
            {
                if (!TryGetPointerDataWithId(eventData.Pointer.PointerId, out _))
                {
                    pointerDataList.Add(new PointerData(eventData.Pointer, eventData.Pointer.Result.Details.Point));

                    // Re-initialize elastic systems.
                    if (elasticsManager)
                    {
                        elasticsManager.InitializeElastics(HostTransform);
                    }

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

            if (pointerDataList.Count > 0)
            {
                // Always mark the pointer data as used to prevent any other behavior to handle pointer events
                // as long as the ObjectManipulator is active.
                // This is due to us reacting to both "Select" and "Grip" events.
                eventData.Use();
            }
        }

        bool hasFirstPointerDraggedThisFrame = false;

        public virtual void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // Call manipulation updated handlers
            if (IsOneHandedManipulationEnabled)
            {
                HandleOneHandMoveUpdated();
            }
            else if (IsTwoHandedManipulationEnabled)
            {
                if (hasFirstPointerDraggedThisFrame)
                {
                    HandleTwoHandManipulationUpdated();
                    hasFirstPointerDraggedThisFrame = false;
                }
                else
                {
                    hasFirstPointerDraggedThisFrame = true;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            // Get pointer data before they are removed from the map
            Vector3 grabPoint = GetPointersGrabPoint();
            Vector3 velocity = GetPointersVelocity();
            Vector3 angularVelocity = GetPointersAngularVelocity();

            if (TryGetPointerDataWithId(eventData.Pointer.PointerId, out PointerData pointerDataToRemove))
            {
                pointerDataList.Remove(pointerDataToRemove);
            }

            // Call manipulation ended handlers
            if (manipulationType.IsMaskSet(ManipulationHandFlags.TwoHanded) && pointerDataList.Count == 1)
            {
                if (manipulationType.IsMaskSet(ManipulationHandFlags.OneHanded))
                {
                    HandleOneHandMoveStarted();
                    hasFirstPointerDraggedThisFrame = false;
                }
                else
                {
                    HandleManipulationEnded(grabPoint, velocity, angularVelocity);
                }
            }
            else if (isManipulationStarted && pointerDataList.Count == 0)
            {
                HandleManipulationEnded(grabPoint, velocity, angularVelocity);
            }

            eventData.Use();
        }

        #endregion Hand Event Handlers

        #region Private Event Handlers

        private void HandleTwoHandManipulationStarted()
        {
            Vector3[] handPositionArray = GetHandPositionArray();

            if (twoHandedManipulationType.IsMaskSet(TransformFlags.Rotate))
            {
                rotateLogic.Setup(handPositionArray, HostTransform);
            }
            if (twoHandedManipulationType.IsMaskSet(TransformFlags.Move))
            {
                // If near manipulation, a pure grab point centroid is used for
                // the initial pointer pose; if far manipulation, a more complex
                // look-rotation-based pointer pose is used.
                MixedRealityPose pointerPose = IsNearManipulation() ? new MixedRealityPose(GetPointersGrabPoint()) : GetPointersPose();
                MixedRealityPose hostPose = new MixedRealityPose(HostTransform.position, HostTransform.rotation);
                moveLogic.Setup(pointerPose, GetPointersGrabPoint(), hostPose, HostTransform.localScale);
            }
            if (twoHandedManipulationType.IsMaskSet(TransformFlags.Scale))
            {
                scaleLogic.Setup(handPositionArray, HostTransform);
            }
        }

        private void HandleTwoHandManipulationUpdated()
        {
            var targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);

            Vector3[] handPositionArray = GetHandPositionArray();

            if (twoHandedManipulationType.IsMaskSet(TransformFlags.Scale))
            {
                targetTransform.Scale = scaleLogic.UpdateMap(handPositionArray);
                if (EnableConstraints && constraintsManager != null)
                {
                    constraintsManager.ApplyScaleConstraints(ref targetTransform, false, IsNearManipulation());
                }
            }
            if (twoHandedManipulationType.IsMaskSet(TransformFlags.Rotate))
            {
                targetTransform.Rotation = rotateLogic.Update(handPositionArray, targetTransform.Rotation);
                if (EnableConstraints && constraintsManager != null)
                {
                    constraintsManager.ApplyRotationConstraints(ref targetTransform, false, IsNearManipulation());
                }
            }
            if (twoHandedManipulationType.IsMaskSet(TransformFlags.Move))
            {
                // If near manipulation, a pure GrabPoint centroid is used for
                // the pointer pose; if far manipulation, a more complex
                // look-rotation-based pointer pose is used.
                MixedRealityPose pose = IsNearManipulation() ? new MixedRealityPose(GetPointersGrabPoint()) : GetPointersPose();
                targetTransform.Position = moveLogic.UpdateTransform(pose, targetTransform, true, IsNearManipulation());
                if (EnableConstraints && constraintsManager != null)
                {
                    constraintsManager.ApplyTranslationConstraints(ref targetTransform, false, IsNearManipulation());
                }
            }

            ApplyTargetTransform(targetTransform);
        }

        private void HandleOneHandMoveStarted()
        {
            Assert.IsTrue(pointerDataList.Count == 1);
            PointerData pointerData = pointerDataList[0];
            IMixedRealityPointer pointer = pointerData.Pointer;

            // Calculate relative transform from object to grip.
            TryGetGripRotation(pointer, out Quaternion gripRotation);
            Quaternion worldToGripRotation = Quaternion.Inverse(gripRotation);
            objectToGripRotation = worldToGripRotation * HostTransform.rotation;

            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            MixedRealityPose hostPose = new MixedRealityPose(HostTransform.position, HostTransform.rotation);
            moveLogic.Setup(pointerPose, pointerData.GrabPoint, hostPose, HostTransform.localScale);
        }

        private void HandleOneHandMoveUpdated()
        {
            Debug.Assert(pointerDataList.Count == 1);
            IMixedRealityPointer pointer = pointerDataList[0].Pointer;

            var targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);

            if (EnableConstraints && constraintsManager != null)
            {
                constraintsManager.ApplyScaleConstraints(ref targetTransform, true, IsNearManipulation());
            }

            TryGetGripRotation(pointer, out Quaternion gripRotation);
            targetTransform.Rotation = gripRotation * objectToGripRotation;

            if (EnableConstraints && constraintsManager != null)
            {
                constraintsManager.ApplyRotationConstraints(ref targetTransform, true, IsNearManipulation());
            }

            RotateInOneHandType rotateInOneHandType = isNearManipulation ? oneHandRotationModeNear : oneHandRotationModeFar;
            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            targetTransform.Position = moveLogic.UpdateTransform(pointerPose, targetTransform, rotateInOneHandType != RotateInOneHandType.RotateAboutObjectCenter, IsNearManipulation());

            if (EnableConstraints && constraintsManager != null)
            {
                constraintsManager.ApplyTranslationConstraints(ref targetTransform, true, IsNearManipulation());
            }

            ApplyTargetTransform(targetTransform);
        }

        private void HandleManipulationStarted()
        {
            isManipulationStarted = true;
            isNearManipulation = IsNearManipulation();
            isSmoothing = (isNearManipulation ? smoothingNear : smoothingFar);

            // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
            // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
            if (OnManipulationStarted != null)
            {
                OnManipulationStarted.Invoke(new ManipulationEventData
                {
                    ManipulationSource = gameObject,
                    IsNearInteraction = isNearManipulation,
                    Pointer = pointerDataList[0].Pointer,
                    PointerCentroid = GetPointersGrabPoint(),
                    PointerVelocity = GetPointersVelocity(),
                    PointerAngularVelocity = GetPointersAngularVelocity()
                });
            }

            if (rigidBody != null)
            {
                wasGravity = rigidBody.useGravity;
                wasKinematic = rigidBody.isKinematic;
                rigidBody.useGravity = false;
                rigidBody.isKinematic = false;
            }

            if (EnableConstraints && constraintsManager != null)
            {
                constraintsManager.Initialize(new MixedRealityTransform(HostTransform));
            }
            if (elasticsManager != null)
            {
                elasticsManager.EnableElasticsUpdate = false;
            }
        }

        private void HandleManipulationEnded(Vector3 pointerGrabPoint, Vector3 pointerVelocity, Vector3 pointerAnglularVelocity)
        {
            isManipulationStarted = false;
            hasFirstPointerDraggedThisFrame = false;
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
            if (elasticsManager != null)
            {
                elasticsManager.EnableElasticsUpdate = true;
            }
        }

        #endregion Private Event Handlers

        #region Unused Event Handlers

        /// <inheritdoc />
        public virtual void OnPointerClicked(MixedRealityPointerEventData eventData) { }
        /// <inheritdoc />
        public void OnBeforeFocusChange(FocusEventData eventData) { }

        #endregion Unused Event Handlers

        #region Private methods

        private bool TryGetPointerDataWithId(uint id, out PointerData pointerData)
        {
            int pointerDataListCount = pointerDataList.Count;
            for (int i = 0; i < pointerDataListCount; i++)
            {
                PointerData data = pointerDataList[i];
                if (data.Pointer.PointerId == id)
                {
                    pointerData = data;
                    return true;
                }
            }

            pointerData = default(PointerData);
            return false;
        }

        private void ApplyTargetTransform(MixedRealityTransform targetTransform)
        {
            bool applySmoothing = isSmoothing && smoothingLogic != null;

            if (rigidBody == null)
            {
                TransformFlags transformUpdated = 0;
                if (elasticsManager != null)
                {
                    transformUpdated = elasticsManager.ApplyTargetTransform(targetTransform);
                }
                if (!transformUpdated.IsMaskSet(TransformFlags.Move))
                {
                    HostTransform.position = applySmoothing ? smoothingLogic.SmoothPosition(HostTransform.position, targetTransform.Position, moveLerpTime, Time.deltaTime) : targetTransform.Position;
                }
                if (!transformUpdated.IsMaskSet(TransformFlags.Rotate))
                {
                    HostTransform.rotation = applySmoothing ? smoothingLogic.SmoothRotation(HostTransform.rotation, targetTransform.Rotation, rotateLerpTime, Time.deltaTime) : targetTransform.Rotation;
                }
                if (!transformUpdated.IsMaskSet(TransformFlags.Scale))
                {
                    HostTransform.localScale = applySmoothing ? smoothingLogic.SmoothScale(HostTransform.localScale, targetTransform.Scale, scaleLerpTime, Time.deltaTime) : targetTransform.Scale;
                }
            }
            else
            {
                // There is a RigidBody. Potential different paths for near vs far manipulation
                if (isNearManipulation && !useForcesForNearManipulation)
                {
                    // This is a near manipulation and we're not using forces
                    // Apply direct updates but account for smoothing

                    if (applySmoothing)
                    {
                        rigidBody.MovePosition(smoothingLogic.SmoothPosition(rigidBody.position, targetTransform.Position, moveLerpTime, Time.deltaTime));
                        rigidBody.MoveRotation(smoothingLogic.SmoothRotation(rigidBody.rotation, targetTransform.Rotation, rotateLerpTime, Time.deltaTime));
                    }
                    else
                    {
                        rigidBody.MovePosition(targetTransform.Position);
                        rigidBody.MoveRotation(targetTransform.Rotation);
                    }
                }
                else
                {
                    // We are using forces
                    rigidBody.velocity = ((1f - Mathf.Pow(moveLerpTime, Time.deltaTime)) / Time.deltaTime) * (targetTransform.Position - HostTransform.position);

                    var relativeRotation = targetTransform.Rotation * Quaternion.Inverse(HostTransform.rotation);
                    relativeRotation.ToAngleAxis(out float angle, out Vector3 axis);

                    if (axis.IsValidVector())
                    {
                        if (angle > 180f)
                        {
                            angle -= 360f;
                        }
                        rigidBody.angularVelocity = ((1f - Mathf.Pow(rotateLerpTime, Time.deltaTime)) / Time.deltaTime) * (axis.normalized * angle * Mathf.Deg2Rad);
                    }
                }

                HostTransform.localScale = applySmoothing ? smoothingLogic.SmoothScale(HostTransform.localScale, targetTransform.Scale, scaleLerpTime, Time.deltaTime) : targetTransform.Scale;
            }
        }

        private Vector3[] handPositionMap = null;

        private Vector3[] GetHandPositionArray()
        {
            if (handPositionMap?.Length != pointerDataList.Count)
            {
                handPositionMap = new Vector3[pointerDataList.Count];
            }

            uint index = 0;
            int pointerDataListCount = pointerDataList.Count;
            for (int i = 0; i < pointerDataListCount; i++)
            {
                handPositionMap[index++] = pointerDataList[i].Pointer.Position;
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

        private void ReleaseRigidBody(Vector3 velocity, Vector3 angularVelocity)
        {
            if (rigidBody != null)
            {
                rigidBody.useGravity = wasGravity;
                rigidBody.isKinematic = wasKinematic;

                // Match the object's velocity to the controller for near interactions
                // Otherwise keep the objects current velocity so that it's not dampened unnaturally
                if (isNearManipulation)
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

        private bool TryGetGripRotation(IMixedRealityPointer pointer, out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            switch (pointer.Controller?.ControllerHandedness)
            {
                case Handedness.Left:
                    rotation = leftHandRotation;
                    break;
                case Handedness.Right:
                    rotation = rightHandRotation;
                    break;
                default:
                    return false;
            }
            return true;
        }

        #endregion

        #region Source Pose Handler Implementation

        /// <summary>
        /// Raised when the source pose tracking state is changed.
        /// </summary>
        public void OnSourcePoseChanged(SourcePoseEventData<TrackingState> eventData) { }

        /// <summary>
        /// Raised when the source position is changed.
        /// </summary>
        public void OnSourcePoseChanged(SourcePoseEventData<Vector2> eventData) { }

        /// <summary>
        /// Raised when the source position is changed.
        /// </summary>
        public void OnSourcePoseChanged(SourcePoseEventData<Vector3> eventData) { }

        /// <summary>
        /// Raised when the source rotation is changed.
        /// </summary>
        public void OnSourcePoseChanged(SourcePoseEventData<Quaternion> eventData) { }

        /// <summary>
        /// Raised when the source pose is changed.
        /// </summary>
        public void OnSourcePoseChanged(SourcePoseEventData<MixedRealityPose> eventData)
        {
            switch (eventData.Controller?.ControllerHandedness)
            {
                case Handedness.Left:
                    leftHandRotation = eventData.SourceData.Rotation;
                    break;
                case Handedness.Right:
                    rightHandRotation = eventData.SourceData.Rotation;
                    break;
                default:
                    break;
            }
        }

        public void OnSourceDetected(SourceStateEventData eventData) { }

        public void OnSourceLost(SourceStateEventData eventData) { }

        #endregion
    }
}
