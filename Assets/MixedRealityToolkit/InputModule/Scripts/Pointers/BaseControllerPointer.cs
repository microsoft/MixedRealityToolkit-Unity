// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.Cursor;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.InputModule.Gaze;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.InputModule.Utilities;
using System.Collections;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace MixedRealityToolkit.InputModule.Pointers
{
    /// <summary>
    /// Base Pointer class for pointers that exist in the scene as GameObjects.
    /// </summary>
    public abstract class BaseControllerPointer : AttachToController, IInputHandler, IPointer
    {
        [Header("Cursor")]
        [SerializeField]
        protected GameObject CursorPrefab;

        [Header("Interaction")]
        [SerializeField]
        private bool interactionEnabled = true;

        [SerializeField]
        [Range(0f, 360f)]
        protected float CurrentPointerOrientation;

        [SerializeField]
        [Range(0.5f, 50f)]
        private float pointerExtent = 2f;

        [SerializeField]
        [Tooltip("Source transform for raycast origin - leave null to use default transform")]
        protected Transform RaycastOrigin;

        [SerializeField]
        private KeyCode interactionEnabledKeyCode = KeyCode.None;

        [SerializeField]
        private KeyCode activeHoldKeyCode = KeyCode.None;

#if UNITY_WSA
        [SerializeField]
        private InteractionSourcePressType activeHoldPressType = InteractionSourcePressType.Select;

        [SerializeField]
        private InteractionSourcePressType interactionEnabledPressType = InteractionSourcePressType.Select;
#endif

        [SerializeField]
        private bool interactionRequiresHold = false;

        protected override void OnAttachToController()
        {
            // Subscribe to interaction events
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        protected override void OnDetachFromController()
        {
            // Unsubscribe from interaction events
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        // True if select is pressed right now
        protected bool SelectPressed = false;

        // True if select has been pressed once since startup
        protected bool SelectPressedOnce = false;

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
        public virtual Vector3 PointerDirection
        {
            get { return RaycastOrigin != null ? RaycastOrigin.forward : transform.forward; }
        }

        #region Monobehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();
            SelectPressed = false;

            if (BaseCursor != null)
            {
                BaseCursor.enabled = true;
            }
        }

        protected virtual void Start()
        {
            FocusManager.AssertIsInitialized();
            InputManager.AssertIsInitialized();
            Debug.Assert(InputManager.GlobalListeners.Contains(FocusManager.Instance.gameObject));
            Debug.Assert(InputSourceParent != null, "This Pointer must have a Input Source Assigned");

            SetCursor();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SelectPressed = false;

            if (BaseCursor != null)
            {
                BaseCursor.enabled = false;
            }
        }

        protected override void OnDestroy()
        {
            if (BaseCursor != null)
            {
                Destroy(BaseCursor.gameObject);
            }

            base.OnDestroy();
        }

        #endregion  Monobehaviour Implementation

        public void SetCursor(GameObject newCursor = null)
        {
            CursorPrefab = newCursor == null ? CursorPrefab : newCursor;

            if (CursorPrefab != null)
            {
                var cursorObj = Instantiate(CursorPrefab, transform);
                cursorObj.name = string.Format("{0}_Cursor", name);
                BaseCursor = cursorObj.GetComponent<BaseCursor>();
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

        public virtual void OnInputReleased()
        {
            SelectPressed = false;
        }

        #region IPointer Implementation

        private uint pointerId;
        public uint PointerId
        {
            get
            {
                if (pointerId == 0)
                {
                    pointerId = FocusManager.GenerateNewPointerId();
                }

                return pointerId;
            }
        }

        public string PointerName
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }

        public IInputSource InputSourceParent { get; set; }

        public BaseCursor BaseCursor { get; set; }

        public ICursorModifier CursorModifier { get; set; }

        public ITeleportTarget TeleportTarget { get; set; }

        public virtual bool InteractionEnabled
        {
            get { return interactionEnabled; }
            set { interactionEnabled = value; }
        }

        public bool FocusLocked { get; set; }

        public float? PointerExtent
        {
            get { return pointerExtent; }
            set { pointerExtent = value ?? FocusManager.GlobalPointingExtent; }
        }

        public RayStep[] Rays { get; protected set; }

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        public IFocusHandler FocusTarget { get; set; }

        public PointerResult Result { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public virtual void OnPreRaycast() { }

        public virtual void OnPostRaycast() { }

        /// <summary>
        /// The world origin of the targeting ray
        /// </summary>
        public virtual bool TryGetPointerPosition(out Vector3 position)
        {
            position = RaycastOrigin != null ? RaycastOrigin.position : transform.position;
            return true;
        }

        public bool TryGetPointingRay(out Ray pointingRay)
        {
            Vector3 pointerPosition;
            TryGetPointerPosition(out pointerPosition);
            pointingRay = new Ray(pointerPosition, PointerDirection);
            return true;
        }

        public bool TryGetPointerRotation(out Quaternion rotation)
        {
            Vector3 pointerRotation = RaycastOrigin != null ? RaycastOrigin.eulerAngles : transform.eulerAngles;
            rotation = Quaternion.Euler(pointerRotation.x, PointerOrientation, pointerRotation.z);
            return true;
        }

        #region IEquality Implementation

        public static bool Equals(IPointer left, IPointer right)
        {
            return left.Equals(right);
        }

        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IPointer)obj);
        }

        private bool Equals(IPointer other)
        {
            return other != null && PointerId == other.PointerId && string.Equals(PointerName, other.PointerName);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

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

        #endregion IPointer Implementation

        #region IInputHandler Implementation

        public virtual void OnInputUp(InputEventData eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                bool interactionPressed = false;
#if UNITY_WSA
                interactionPressed = eventData.PressType == activeHoldPressType;
#endif
                if (interactionRequiresHold && (eventData.KeyCode == activeHoldKeyCode || interactionPressed))
                {
                    InteractionEnabled = false;
                }
#if UNITY_WSA
                interactionPressed = eventData.PressType == interactionEnabledPressType;
#endif
                if (eventData.KeyCode == interactionEnabledKeyCode || interactionPressed)
                {
                    OnInputReleased();
                }
            }
        }

        public virtual void OnInputDown(InputEventData eventData)
        {
            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                bool interactionPressed = false;
#if UNITY_WSA
                interactionPressed = eventData.PressType == activeHoldPressType;
#endif
                if (interactionRequiresHold && (eventData.KeyCode == activeHoldKeyCode || interactionPressed))
                {
                    InteractionEnabled = true;
                }

#if UNITY_WSA
                interactionPressed = eventData.PressType == interactionEnabledPressType;
#endif
                if (eventData.KeyCode == interactionEnabledKeyCode || interactionPressed)
                {
                    OnSelectPressed();
                }
            }
        }

        public virtual void OnInputPressed(InputPressedEventData eventData) { }

        /// <summary>
        /// Updates target point orientation via thumbstick
        /// </summary>
        public virtual void OnInputPositionChanged(InputPositionEventData eventData) { }

        #endregion  IInputHandler Implementation
    }
}
