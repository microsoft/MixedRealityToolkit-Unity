// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Teleport;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Physics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Pointers
{
    /// <summary>
    /// Base Pointer class for pointers that exist in the scene as GameObjects.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseControllerPointer : ControllerPoseSynchronizer, IMixedRealityPointer, IMixedRealityTeleportHandler
    {
        [SerializeField]
        private GameObject cursorPrefab = null;

        [SerializeField]
        private bool disableCursorOnStart = false;

        protected bool DisableCursorOnStart => disableCursorOnStart;

        [SerializeField]
        private bool setCursorVisibilityOnSourceDetected = false;

        private GameObject cursorInstance = null;

        [SerializeField]
        [Tooltip("Source transform for raycast origin - leave null to use default transform")]
        private Transform raycastOrigin = null;

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

        protected bool IsTeleportRequestActive = false;

        private bool lateRegisterTeleport = true;

        /// <summary>
        /// The forward direction of the targeting ray
        /// </summary>
        public virtual Vector3 PointerDirection => raycastOrigin != null ? raycastOrigin.forward : transform.forward;

        /// <summary>
        /// Set a new cursor for this <see cref="IMixedRealityPointer"/>
        /// </summary>
        /// <remarks>This <see cref="GameObject"/> must have a <see cref="IMixedRealityCursor"/> attached to it.</remarks>
        /// <param name="newCursor">The new cursor</param>
        public virtual void SetCursor(GameObject newCursor = null)
        {
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

            if (cursorInstance == null && cursorPrefab != null)
            {
                cursorInstance = Instantiate(cursorPrefab, transform);
            }

            if (cursorInstance != null)
            {
                cursorInstance.name = $"{Handedness}_{name}_Cursor";
                BaseCursor = cursorInstance.GetComponent<IMixedRealityCursor>();

                if (BaseCursor != null)
                {
                    BaseCursor.DefaultCursorDistance = PointerExtent;
                    BaseCursor.Pointer = this;
                    BaseCursor.SetVisibilityOnSourceDetected = setCursorVisibilityOnSourceDetected;

                    if (disableCursorOnStart)
                    {
                        BaseCursor.SetVisibility(false);
                    }
                }
                else
                {
                    Debug.LogError($"No IMixedRealityCursor component found on {cursorInstance.name}");
                }
            }
        }

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();

            if (MixedRealityToolkit.IsInitialized && MixedRealityToolkit.TeleportSystem != null && !lateRegisterTeleport)
            {
                MixedRealityToolkit.TeleportSystem.Register(gameObject);
            }
        }

        protected override async void Start()
        {
            base.Start();

            if (lateRegisterTeleport && MixedRealityToolkit.Instance.ActiveProfile.IsTeleportSystemEnabled)
            {
                if (MixedRealityToolkit.TeleportSystem == null)
                {
                    await new WaitUntil(() => MixedRealityToolkit.TeleportSystem != null);

                    // We've been destroyed during the await.
                    if (this == null)
                    {
                        return;
                    }
                }

                lateRegisterTeleport = false;
                MixedRealityToolkit.TeleportSystem.Register(gameObject);
            }

            if (MixedRealityToolkit.InputSystem == null)
            {
                await WaitUntilInputSystemValid;
            }

            // We've been destroyed during the await.
            if (this == null)
            {
                return;
            }

            SetCursor();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MixedRealityToolkit.TeleportSystem?.Unregister(gameObject);

            IsHoldPressed = false;
            IsSelectPressed = false;
            HasSelectPressedOnce = false;
            BaseCursor?.SetVisibility(false);
        }

        #endregion  MonoBehaviour Implementation

        #region IMixedRealityPointer Implementation

        /// <inheritdoc cref="IMixedRealityController" />
        public override IMixedRealityController Controller
        {
            get { return base.Controller; }
            set
            {
                base.Controller = value;
                pointerName = gameObject.name;
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
                    pointerId = MixedRealityToolkit.InputSystem.FocusProvider.GenerateNewPointerId();
                }

                return pointerId;
            }
        }

        private string pointerName = string.Empty;

        /// <inheritdoc />
        public string PointerName
        {
            get { return pointerName; }
            set
            {
                pointerName = value;
                gameObject.name = value;
            }
        }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSourceParent { get; protected set; }

        /// <inheritdoc />
        public IMixedRealityCursor BaseCursor { get; set; }

        /// <inheritdoc />
        public ICursorModifier CursorModifier { get; set; }

        /// <inheritdoc />
        public IMixedRealityTeleportHotSpot TeleportHotSpot { get; set; }

        /// <inheritdoc />
        public virtual bool IsInteractionEnabled
        {
            get
            {
                if (IsTeleportRequestActive)
                {
                    return false;
                }

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
            get
            {
                if (overrideGlobalPointerExtent)
                {
                    if (MixedRealityToolkit.InputSystem?.FocusProvider != null)
                    {
                        return MixedRealityToolkit.InputSystem.FocusProvider.GlobalPointingExtent;
                    }
                }

                return pointerExtent;
            }
            set { pointerExtent = value; }
        }

        /// <inheritdoc />
        public RayStep[] Rays { get; protected set; } = { new RayStep(Vector3.zero, Vector3.forward) };

        /// <inheritdoc />
        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        /// <inheritdoc />
        public IMixedRealityFocusHandler FocusTarget { get; set; }

        /// <inheritdoc />
        public IPointerResult Result { get; set; }

        /// <inheritdoc />
        public IBaseRayStabilizer RayStabilizer { get; set; }

        /// <inheritdoc />
        public RaycastMode RaycastMode { get; set; } = RaycastMode.Simple;

        /// <inheritdoc />
        public float SphereCastRadius { get; set; } = 0.1f;

        [SerializeField]
        [Range(0f, 360f)]
        [Tooltip("The Y orientation of the pointer - used for rotation and navigation")]
        private float pointerOrientation = 0f;

        /// <inheritdoc />
        public virtual float PointerOrientation
        {
            get
            {
                return pointerOrientation + (raycastOrigin != null ? raycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            }
            set
            {
                pointerOrientation = value < 0
                    ? Mathf.Clamp(value, -360f, 0f)
                    : Mathf.Clamp(value, 0f, 360f);
            }
        }

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
        public virtual bool TryGetPointingRay(out Ray pointingRay)
        {
            Vector3 pointerPosition;
            TryGetPointerPosition(out pointerPosition);
            pointingRay = pointerRay;
            pointingRay.origin = pointerPosition;
            pointingRay.direction = PointerDirection;
            return true;
        }

        private readonly Ray pointerRay = new Ray();

        /// <inheritdoc />
        public virtual bool TryGetPointerRotation(out Quaternion rotation)
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

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            base.OnInputUp(eventData);

            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (requiresHoldAction && eventData.MixedRealityInputAction == activeHoldAction)
                {
                    IsHoldPressed = false;
                }

                if (eventData.MixedRealityInputAction == pointerAction)
                {
                    IsSelectPressed = false;
                    MixedRealityToolkit.InputSystem.RaisePointerClicked(this, pointerAction, 0, Handedness);
                    MixedRealityToolkit.InputSystem.RaisePointerUp(this, pointerAction, Handedness);
                }
            }
        }

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);

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
                    MixedRealityToolkit.InputSystem.RaisePointerDown(this, pointerAction, Handedness);
                }
            }
        }

        #endregion  IMixedRealityInputHandler Implementation

        #region IMixedRealityTeleportHandler Implementation

        /// <inheritdoc />
        public virtual void OnTeleportRequest(TeleportEventData eventData)
        {
            // Only turn off pointers that aren't making the request.
            IsTeleportRequestActive = true;
            BaseCursor?.SetVisibility(false);
        }

        /// <inheritdoc />
        public virtual void OnTeleportStarted(TeleportEventData eventData)
        {
            // Turn off all pointers while we teleport.
            IsTeleportRequestActive = true;
            BaseCursor?.SetVisibility(false);
        }

        /// <inheritdoc />
        public virtual void OnTeleportCompleted(TeleportEventData eventData)
        {
            // Turn all our pointers back on.
            IsTeleportRequestActive = false;
            BaseCursor?.SetVisibility(true);
        }

        /// <inheritdoc />
        public virtual void OnTeleportCanceled(TeleportEventData eventData)
        {
            // Turn all our pointers back on.
            IsTeleportRequestActive = false;
            BaseCursor?.SetVisibility(true);
        }

        #endregion IMixedRealityTeleportHandler Implementation
    }
}
