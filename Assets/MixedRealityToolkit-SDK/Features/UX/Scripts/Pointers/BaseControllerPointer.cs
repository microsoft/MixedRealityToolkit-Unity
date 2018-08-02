// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Physics;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    /// <summary>
    /// Base Pointer class for pointers that exist in the scene as GameObjects.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseControllerPointer : ControllerPoseSynchronizer, IMixedRealityPointer, IMixedRealitySpatialInputHandler
    {
        [SerializeField]
        private GameObject cursorPrefab = null;

        private GameObject cursorInstance = null;

        [SerializeField]
        [Tooltip("Source transform for raycast origin - leave null to use default transform")]
        private Transform raycastOrigin = null;

        [SerializeField]
        [Range(0f, 360f)]
        [Tooltip("The Y orientation of the pointer - used for touchpad rotation and navigation")]
        private float pointerOrientation = 0f;

        /// <summary>
        /// The Y orientation of the pointer - used for touchpad rotation and navigation
        /// </summary>
        public virtual float PointerOrientation
        {
            get
            {
                return pointerOrientation + (raycastOrigin != null ? raycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            }
            set
            {
                pointerOrientation = Mathf.Clamp(value, 0f, 360f);
            }
        }

        [SerializeField]
        [Tooltip("Should the pointer's position be driven from the source pose or from input handler?")]
        private bool useSourcePoseData = false;

        [SerializeField]
        [Tooltip("The input action that will drive the pointer's pose, position, or rotation.")]
        private MixedRealityInputAction inputSourceAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("The hold action that will enable the raise the input event for this pointer.")]
        private MixedRealityInputAction activeHoldAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("The action that will enable the raise the input event for this pointer.")]
        private MixedRealityInputAction pointerAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("Does the interaction require hold?")]
        private bool requiresHoldAction = false;

        /// <summary>
        /// True if select is pressed right now
        /// </summary>
        protected bool IsSelectPressed = false;

        /// <summary>
        /// True if select has been pressed once since this component was enabled
        /// </summary>
        protected bool HasSelectPressedOnce = false;

        protected bool IsHoldPressed = false;

        private bool delayPointerRegistration = true;

        /// <summary>
        /// The forward direction of the targeting ray
        /// </summary>
        public virtual Vector3 PointerDirection => raycastOrigin != null ? raycastOrigin.forward : transform.forward;

        /// <summary>
        /// Set a new cursor for this <see cref="IMixedRealityPointer"/>
        /// </summary>
        /// <remarks>This <see cref="GameObject"/> must have a <see cref="IMixedRealityCursor"/> attached to it.</remarks>
        /// <param name="newCursor"></param>
        public void SetCursor(GameObject newCursor = null)
        {
            // Destroy the old cursor instance.
            if (cursorInstance != null)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(cursorInstance);
                }
                else
                {
                    Destroy(cursorInstance);
                }

                cursorInstance = newCursor;
            }

            if (cursorInstance == null)
            {
                cursorInstance = Instantiate(cursorPrefab, transform);
                cursorInstance.name = $"{name}_Cursor";
                BaseCursor = cursorInstance.GetComponent<IMixedRealityCursor>();
                Debug.Assert(BaseCursor != null, "Failed to load cursor");
                BaseCursor.DefaultCursorDistance = PointerExtent;
                BaseCursor.Pointer = this;
                Debug.Assert(BaseCursor.Pointer != null, "Failed to assign cursor!");
            }
        }

        #region Monobehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();

            BaseCursor?.SetVisibility(true);

            if (!delayPointerRegistration)
            {
                InputSystem.FocusProvider.RegisterPointer(this);
            }
        }

        protected virtual void Start()
        {
            Debug.Assert(InputSourceParent != null, "This Pointer must have a Input Source Assigned");

            InputSystem.FocusProvider.RegisterPointer(this);
            delayPointerRegistration = false;

            SetCursor();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            IsHoldPressed = false;
            IsSelectPressed = false;
            HasSelectPressedOnce = false;
            BaseCursor?.SetVisibility(false);
            InputSystem.FocusProvider.UnregisterPointer(this);
        }

        #endregion  Monobehaviour Implementation

        #region IMixedRealityPointer Implementation

        IMixedRealityInputSystem IMixedRealityPointer.InputSystem => InputSystem;

        /// <inheritdoc />
        public override IMixedRealityController Controller
        {
            get { return base.Controller; }
            set
            {
                base.Controller = value;
                InputSourceParent = base.Controller.InputSource;
            }
        }

        private uint pointerId;

        /// <inheritdoc />
        public uint PointerId
        {
            get
            {
                if (pointerId == 0)
                {
                    pointerId = InputSystem.FocusProvider.GenerateNewPointerId();
                }

                return pointerId;
            }
        }

        /// <inheritdoc />
        public string PointerName
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSourceParent { get; private set; }

        /// <inheritdoc />
        public IMixedRealityCursor BaseCursor { get; set; }

        /// <inheritdoc />
        public ICursorModifier CursorModifier { get; set; }

        /// <inheritdoc />
        public ITeleportTarget TeleportTarget { get; set; }

        /// <inheritdoc />
        public virtual bool IsInteractionEnabled
        {
            get
            {
                if (requiresHoldAction && IsHoldPressed)
                {
                    return true;
                }

                if (IsSelectPressed)
                {
                    return true;
                }

                return HasSelectPressedOnce;
            }
        }

        /// <inheritdoc />
        public bool IsFocusLocked { get; set; }

        [SerializeField]
        private bool overrideGlobalPointerExtent = false;

        [SerializeField]
        private float pointerExtent = 10f;

        /// <inheritdoc />
        public float PointerExtent
        {
            get { return overrideGlobalPointerExtent ? InputSystem.FocusProvider.GlobalPointingExtent : pointerExtent; }
            set { pointerExtent = value; }
        }

        /// <inheritdoc />
        public RayStep[] Rays { get; protected set; }

        /// <inheritdoc />
        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        /// <inheritdoc />
        public IMixedRealityFocusHandler FocusTarget { get; set; }

        /// <inheritdoc />
        public IPointerResult Result { get; set; }

        /// <inheritdoc />
        public IBaseRayStabilizer RayStabilizer { get; set; }

        /// <inheritdoc />
        public RaycastModeType RaycastMode { get; set; } = RaycastModeType.Simple;

        /// <inheritdoc />
        public float SphereCastRadius { get; set; } = 0.1f;

        /// <inheritdoc />
        public virtual void OnPreRaycast() { }

        /// <inheritdoc />
        public virtual void OnPostRaycast() { }

        /// <inheritdoc />
        public virtual bool TryGetPointerPosition(out Vector3 position)
        {
            position = raycastOrigin != null ? raycastOrigin.position : transform.position;
            return true;
        }

        /// <inheritdoc />
        public bool TryGetPointingRay(out Ray pointingRay)
        {
            Vector3 pointerPosition;
            TryGetPointerPosition(out pointerPosition);
            pointingRay = new Ray(pointerPosition, PointerDirection);
            return true;
        }

        /// <inheritdoc />
        public bool TryGetPointerRotation(out Quaternion rotation)
        {
            Vector3 pointerRotation = raycastOrigin != null ? raycastOrigin.eulerAngles : transform.eulerAngles;
            rotation = Quaternion.Euler(pointerRotation.x, PointerOrientation, pointerRotation.z);
            return true;
        }

        #region IEquality Implementation

        private static bool Equals(IMixedRealityPointer left, IMixedRealityPointer right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityPointer)obj);
        }

        private bool Equals(IMixedRealityPointer other)
        {
            return other != null && PointerId == other.PointerId && string.Equals(PointerName, other.PointerName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)PointerId;
                hashCode = (hashCode * 397) ^ (PointerName != null ? PointerName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        #endregion IMixedRealityPointer Implementation

        #region IMixedRealitySourcePoseHandler Implementation

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            Destroy(gameObject);
        }

        /// <inheritdoc />
        public override void OnSourcePoseChanged(SourcePoseEventData eventData)
        {
            if (useSourcePoseData)
            {
                base.OnSourcePoseChanged(eventData);
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public virtual void OnInputUp(InputEventData eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (requiresHoldAction && eventData.MixedRealityInputAction == activeHoldAction)
                {
                    IsHoldPressed = false;
                }

                if (eventData.MixedRealityInputAction == pointerAction)
                {
                    IsSelectPressed = false;
                    InputSystem.RaisePointerClicked(this, Handedness, pointerAction, 0);
                    InputSystem.RaisePointerUp(this, Handedness, pointerAction);
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputDown(InputEventData eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (requiresHoldAction && eventData.MixedRealityInputAction == activeHoldAction)
                {
                    IsHoldPressed = true;
                }

                if (eventData.MixedRealityInputAction == pointerAction)
                {
                    IsSelectPressed = true;
                    HasSelectPressedOnce = true;
                    InputSystem.RaisePointerDown(this, Handedness, pointerAction);
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputPressed(InputEventData<float> eventData) { }

        /// <inheritdoc />
        public virtual void OnPositionInputChanged(InputEventData<Vector2> eventData) { }

        #endregion  IMixedRealityInputHandler Implementation

        #region IMixedRealitySpatialInputHandler Implementation

        /// <inheritdoc />
        public virtual void OnPositionChanged(InputEventData<Vector3> eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (!useSourcePoseData &&
                    inputSourceAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.position = eventData.InputData;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnRotationChanged(InputEventData<Quaternion> eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (!useSourcePoseData &&
                    inputSourceAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.rotation = eventData.InputData;
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnPoseInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (!useSourcePoseData &&
                    inputSourceAction == eventData.MixedRealityInputAction)
                {
                    IsTracked = true;
                    TrackingState = TrackingState.Tracked;
                    transform.position = eventData.InputData.Position;
                    transform.rotation = eventData.InputData.Rotation;
                }
            }
        }

        #endregion IMixedRealitySpatialInputHandler Implementation 
    }
}
