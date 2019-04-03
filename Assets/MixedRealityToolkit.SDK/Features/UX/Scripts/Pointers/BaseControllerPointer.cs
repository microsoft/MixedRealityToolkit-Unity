// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base Pointer class for pointers that exist in the scene as GameObjects.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseControllerPointer : ControllerPoseSynchronizer, IMixedRealityPointer
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
        protected Transform raycastOrigin = null;

        [SerializeField]
        [Tooltip("The hold action that will enable the raise the input event for this pointer.")]
        private MixedRealityInputAction activeHoldAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("The action that will enable the raise the input event for this pointer.")]
        protected MixedRealityInputAction pointerAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("Does the interaction require hold?")]
        private bool requiresHoldAction = false;

        [SerializeField]
        [Tooltip("Does the interaction require the action to occur at least once first?")]
        private bool requiresActionBeforeEnabling = true;

        /// <summary>
        /// True if select is pressed right now
        /// </summary>
        protected bool IsSelectPressed = false;

        /// <summary>
        /// True if select has been pressed once since this component was enabled
        /// </summary>
        protected bool HasSelectPressedOnce = false;

        protected bool IsHoldPressed = false;

        /// <summary>
        /// Set a new cursor for this <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer"/>
        /// </summary>
        /// <remarks>This <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> must have a <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityCursor"/> attached to it.</remarks>
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
                    BaseCursor.DefaultCursorDistance = DefaultPointerExtent;
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

            // Disable renderers so that they don't display before having been processed (which manifests as a flash at the origin).
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    renderer.enabled = false;
                }
            }
        }

        protected override async void Start()
        {
            base.Start();

            if (MixedRealityToolkit.InputSystem == null)
            {
                await WaitUntilInputSystemValid;
            }

            // We've been destroyed during the await.
            if (this == null)
            {
                return;
            }

            // The pointer's input source was lost during the await.
            if (Controller == null)
            {
                Destroy(gameObject);
                return;
            }

            SetCursor();
        }

        protected override void OnDisable()
        {
            if (IsSelectPressed && MixedRealityToolkit.InputSystem != null)
            {
                MixedRealityToolkit.InputSystem.RaisePointerUp(this, pointerAction, Handedness);
            }

            base.OnDisable();

            IsHoldPressed = false;
            IsSelectPressed = false;
            HasSelectPressedOnce = false;
            BaseCursor?.SetVisibility(false);
        }

        #endregion  MonoBehaviour Implementation

        #region IMixedRealityPointer Implementation

        /// <inheritdoc />
        public override IMixedRealityController Controller
        {
            get { return base.Controller; }
            set
            {
                base.Controller = value;

                if (base.Controller != null && this != null)
                {
                    pointerName = gameObject.name;
                    InputSourceParent = base.Controller.InputSource;
                }
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
                if (this != null)
                {
                    gameObject.name = value;
                }
            }
        }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSourceParent { get; protected set; }

        /// <inheritdoc />
        public IMixedRealityCursor BaseCursor { get; set; }

        /// <inheritdoc />
        public ICursorModifier CursorModifier { get; set; }

        /// <inheritdoc />
        public virtual bool IsInteractionEnabled
        {
            get
            {
                if (IsFocusLocked)
                {
                    return true;
                }

                if (!IsActive)
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

                return HasSelectPressedOnce || !requiresActionBeforeEnabling;
            }
        }

        public virtual bool IsActive { get; set; }


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
            set
            {
                pointerExtent = value;
                overrideGlobalPointerExtent = false;
            }
        }

        [SerializeField]
        private float defaultPointerExtent = 10f;

        /// <summary>
        /// The length of the pointer when nothing is hit.
        /// </summary>
        public float DefaultPointerExtent
        {
            get { return Mathf.Min(defaultPointerExtent, PointerExtent); }
            set { defaultPointerExtent = value; }
        }

        /// <inheritdoc />
        public RayStep[] Rays { get; protected set; } = { new RayStep(Vector3.zero, Vector3.forward) };

        /// <inheritdoc />
        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        /// <inheritdoc />
        public IMixedRealityFocusHandler FocusTarget { get; set; }

        /// <inheritdoc />
        public IPointerResult Result { get; set; }

        /// <summary>
        /// Ray stabilizer used when calculating position of pointer end point.
        /// </summary>
        public IBaseRayStabilizer RayStabilizer { get; set; }

        /// <inheritdoc />
        public virtual SceneQueryType SceneQueryType { get; set; } = SceneQueryType.SimpleRaycast;

        [SerializeField]
        [Tooltip("The radius to use when SceneQueryType is set to Sphere or SphereColliders.")]
        private float sphereCastRadius = 0.1f;

        /// <inheritdoc />
        public float SphereCastRadius
        {
            get { return sphereCastRadius; }
            set { sphereCastRadius = value; }
        }

        /// <inheritdoc />
        public virtual Vector3 Position => raycastOrigin != null ? raycastOrigin.position : transform.position;

        /// <inheritdoc />
        public virtual Quaternion Rotation => raycastOrigin != null ? raycastOrigin.rotation : transform.rotation;

        /// <inheritdoc />
        public virtual void OnPreSceneQuery() { }

        /// <inheritdoc />
        public virtual void OnPostSceneQuery() { }

        ///  <inheritdoc />
        public virtual void OnPreCurrentPointerTargetChange() { }

        #endregion IMixedRealityPointer Implementation

        #region IEquality Implementation

        private static bool Equals(IMixedRealityPointer left, IMixedRealityPointer right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left != null && left.Equals(right);
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

        #region IMixedRealitySourcePoseHandler Implementation

        /// <inheritdoc />
        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);

            if (eventData.SourceId == InputSourceParent.SourceId)
            {
                if (requiresHoldAction)
                {
                    IsHoldPressed = false;
                }

                if (IsSelectPressed)
                {
                    MixedRealityToolkit.InputSystem.RaisePointerUp(this, pointerAction, Handedness);
                }

                IsSelectPressed = false;
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

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

                    if (IsInteractionEnabled)
                    {
                        MixedRealityToolkit.InputSystem.RaisePointerDown(this, pointerAction, Handedness);
                    }
                }
            }
        }

        #endregion  IMixedRealityInputHandler Implementation
    }
}
