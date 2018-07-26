// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Physics;
using Microsoft.MixedReality.Toolkit.SDK.UX.Controllers;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    /// <summary>
    /// Base Pointer class for pointers that exist in the scene as GameObjects.
    /// </summary>
    public abstract class BaseControllerPointer : AttachToController, IMixedRealityInputHandler, IMixedRealityPointer
    {
        IMixedRealityInputSystem IMixedRealityPointer.InputSystem => InputSystem;

        [Header("Cursor")]
        [SerializeField]
        protected GameObject CursorPrefab = null;

        [Header("Interaction")]
        [SerializeField]
        private bool interactionEnabled = true;

        [SerializeField]
        [Range(0f, 360f)]
        protected float CurrentPointerOrientation = 0f;

        [SerializeField]
        [Range(0.5f, 50f)]
        private float pointerExtent = 2f;

        [SerializeField]
        [Tooltip("Source transform for raycast origin - leave null to use default transform")]
        protected Transform RaycastOrigin = null;

        [SerializeField]
        private MixedRealityInputAction activeHoldAction = MixedRealityInputAction.None;

        [SerializeField]
        private MixedRealityInputAction interactionEnabledAction = MixedRealityInputAction.None;

        [SerializeField]
        private bool interactionRequiresHold = false;

        /// <summary>
        /// True if select is pressed right now
        /// </summary>
        protected bool SelectPressed = false;

        /// <summary>
        /// True if select has been pressed once since startup
        /// </summary>
        protected bool SelectPressedOnce = false;

        private bool delayPointerRegistration = true;

        /// <summary>
        /// The Y orientation of the pointer target - used for touchpad rotation and navigation
        /// </summary>
        public virtual float PointerOrientation
        {
            get
            {
                return CurrentPointerOrientation + (RaycastOrigin != null ? RaycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            }
            set
            {
                CurrentPointerOrientation = value;
            }
        }

        /// <summary>
        /// The forward direction of the targeting ray
        /// </summary>
        public virtual Vector3 PointerDirection => RaycastOrigin != null ? RaycastOrigin.forward : transform.forward;

        #region Monobehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();
            SelectPressed = false;

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
            SelectPressed = false;
            BaseCursor?.SetVisibility(false);
            InputSystem.FocusProvider.UnregisterPointer(this);
        }

        protected virtual void OnDestroy()
        {
            if (BaseCursor != null)
            {
                Destroy(BaseCursor.GetGameObjectReference());
            }
        }

        #endregion  Monobehaviour Implementation

        public void SetCursor(GameObject newCursor = null)
        {
            CursorPrefab = newCursor == null ? CursorPrefab : newCursor;

            if (CursorPrefab != null)
            {
                var cursorObj = Instantiate(CursorPrefab, transform);
                cursorObj.name = $"{name}_Cursor";
                BaseCursor = cursorObj.GetComponent<IMixedRealityCursor>();
                Debug.Assert(BaseCursor != null, "Failed to load cursor");
                BaseCursor.Pointer = this;
                Debug.Assert(BaseCursor.Pointer != null, "Failed to assign cursor!");
            }
        }

        /// <summary>
        /// Call to initiate a select action for this pointer
        /// </summary>
        public virtual void OnSelectPressed()
        {
            SelectPressed = true;
            SelectPressedOnce = true;
        }

        public virtual void OnSelectReleased()
        {
            SelectPressed = false;
        }

        #region IMixedRealityPointer Implementation

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
        public IMixedRealityInputSource InputSourceParent { get; set; }

        /// <inheritdoc />
        public IMixedRealityCursor BaseCursor { get; set; }

        /// <inheritdoc />
        public ICursorModifier CursorModifier { get; set; }

        /// <inheritdoc />
        public ITeleportTarget TeleportTarget { get; set; }

        /// <inheritdoc />
        public virtual bool InteractionEnabled
        {
            get { return interactionEnabled; }
            set { interactionEnabled = value; }
        }

        /// <inheritdoc />
        public bool FocusLocked { get; set; }

        /// <inheritdoc />
        public float? PointerExtent
        {
            get { return pointerExtent; }
            set { pointerExtent = value ?? InputSystem.FocusProvider.GlobalPointingExtent; }
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
            position = RaycastOrigin != null ? RaycastOrigin.position : transform.position;
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
            Vector3 pointerRotation = RaycastOrigin != null ? RaycastOrigin.eulerAngles : transform.eulerAngles;
            rotation = Quaternion.Euler(pointerRotation.x, PointerOrientation, pointerRotation.z);
            return true;
        }

        #region IEquality Implementation

        public static bool Equals(IMixedRealityPointer left, IMixedRealityPointer right)
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

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public virtual void OnInputUp(InputEventData eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (interactionRequiresHold && eventData.MixedRealityInputAction == activeHoldAction)
                {
                    InteractionEnabled = false;
                }

                if (eventData.MixedRealityInputAction == interactionEnabledAction)
                {
                    OnSelectReleased();
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputDown(InputEventData eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (interactionRequiresHold && (eventData.MixedRealityInputAction == activeHoldAction))
                {
                    InteractionEnabled = true;
                }

                if (eventData.MixedRealityInputAction == interactionEnabledAction)
                {
                    OnSelectPressed();
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnInputPressed(InputEventData<float> eventData) { }

        /// <inheritdoc />
        public virtual void OnPositionInputChanged(InputEventData<Vector2> eventData) { }

        #endregion  IMixedRealityInputHandler Implementation
    }
}
