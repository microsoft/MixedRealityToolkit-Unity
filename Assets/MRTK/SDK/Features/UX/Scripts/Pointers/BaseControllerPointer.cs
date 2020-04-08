// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base Pointer class for pointers that exist in the scene as GameObjects.
    /// </summary>
    [DisallowMultipleComponent]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/Pointers.html")]
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

        private bool isCursorInstantiatedFromPrefab = false;

        /// <summary>
        /// Set a new cursor for this <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer"/>
        /// </summary>
        /// <remarks>This <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> must have a <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityCursor"/> attached to it.</remarks>
        /// <param name="newCursor">The new cursor</param>
        public virtual void SetCursor(GameObject newCursor = null)
        {
            if (cursorInstance != null)
            {
                DestroyCursorInstance();
                cursorInstance = newCursor;
            }

            if (cursorInstance == null && cursorPrefab != null)
            {
                cursorInstance = Instantiate(cursorPrefab, transform);
                isCursorInstantiatedFromPrefab = true;
            }

            if (cursorInstance != null)
            {
                cursorInstance.name = $"{Handedness}_{name}_Cursor";

                BaseCursor oldC = BaseCursor as BaseCursor;
                if (oldC != null && enabled)
                {
                    oldC.VisibleSourcesCount--;
                }

                BaseCursor = cursorInstance.GetComponent<IMixedRealityCursor>();

                BaseCursor newC = BaseCursor as BaseCursor;
                if (newC != null && enabled)
                {
                    newC.VisibleSourcesCount++;
                }

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

        private void DestroyCursorInstance()
        {
            if (cursorInstance != null)
            {
                // Destroy correctly depending on if in play mode or edit mode
                GameObjectExtensions.DestroyGameObject(cursorInstance);
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

            SetCursor();
        }

        protected override async void Start()
        {
            base.Start();

            await EnsureInputSystemValid();

            // We've been destroyed during the await.
            if (this == null)
            {
                return;
            }

            // The pointer's input source was lost during the await.
            if (Controller == null)
            {
                GameObjectExtensions.DestroyGameObject(gameObject);
                return;
            }
        }

        protected override void OnDisable()
        {
            if (IsSelectPressed)
            {
                CoreServices.InputSystem?.RaisePointerUp(this, pointerAction, Handedness);
            }

            base.OnDisable();

            IsHoldPressed = false;
            IsSelectPressed = false;
            HasSelectPressedOnce = false;
            BaseCursor?.SetVisibility(false);

            BaseCursor c = BaseCursor as BaseCursor;
            if (c != null)
            {
                c.VisibleSourcesCount--;
            }

            // Need to destroy instantiated cursor prefab if it was added by the controller itself in 'OnEnable'
            if (isCursorInstantiatedFromPrefab)
            {
                // Manually reset base cursor before destroying it
                BaseCursor?.Destroy();
                DestroyCursorInstance();
                isCursorInstantiatedFromPrefab = false;
            }
        }

        #endregion  MonoBehaviour Implementation

        #region IMixedRealityPointer Implementation

        /// <inheritdoc />
        public override IMixedRealityController Controller
        {
            get => base.Controller;
            set
            {
                base.Controller = value;

                if (base.Controller != null && this != null)
                {
                    PointerName = gameObject.name;
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
                    pointerId = CoreServices.InputSystem.FocusProvider.GenerateNewPointerId();
                }

                return pointerId;
            }
        }

        private string pointerName = string.Empty;

        /// <inheritdoc />
        public string PointerName
        {
            get => pointerName;
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

        /// <inheritdoc />
        public virtual bool IsActive { get; set; }

        /// <inheritdoc />
        public bool IsFocusLocked { get; set; }

        /// <summary>
        /// Specifies whether the pointer's target position (cursor) is locked to the target object when focus is locked.
        /// Most pointers want the cursor to "stick" to the object when manipulating, so set this to true by default.
        /// </summary>
        public virtual bool IsTargetPositionLockedOnFocusLock { get; set; } = true;

        [SerializeField]
        private bool overrideGlobalPointerExtent = false;

        [SerializeField]
        [Tooltip("Maximum distance at which all pointers can collide with a GameObject, unless it has an override extent.")]
        private float pointerExtent = 10f;

        /// <summary>
        /// Maximum distance at which all pointers can collide with a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>, unless it has an override extent.
        /// </summary>
        public float PointerExtent
        {
            get
            {
                if (overrideGlobalPointerExtent)
                {
                    if (CoreServices.InputSystem?.FocusProvider != null)
                    {
                        return CoreServices.InputSystem.FocusProvider.GlobalPointingExtent;
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
        [Tooltip("The length of the pointer when nothing is hit")]
        private float defaultPointerExtent = 10f;

        /// <summary>
        /// The length of the pointer when nothing is hit.
        /// </summary>
        public float DefaultPointerExtent
        {
            get => Mathf.Min(defaultPointerExtent, PointerExtent);
            set => defaultPointerExtent = value;
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
        [Tooltip("How far controller needs to be from object before object can be grabbed / focused.")]
        private float sphereCastRadius = 0.1f;

        /// <inheritdoc />
        public float SphereCastRadius
        {
            get => sphereCastRadius;
            set => sphereCastRadius = value;
        }

        /// <inheritdoc />
        public virtual Vector3 Position => raycastOrigin != null ? raycastOrigin.position : transform.position;

        /// <inheritdoc />
        public virtual Quaternion Rotation => raycastOrigin != null ? raycastOrigin.rotation : transform.rotation;

        /// <inheritdoc />
        public virtual void OnPreSceneQuery() { }

        /// <inheritdoc />
        public virtual void OnPostSceneQuery()
        {
            if (IsSelectPressed)
            {
                CoreServices.InputSystem.RaisePointerDragged(this, MixedRealityInputAction.None, Handedness);
            }
        }

        /// <inheritdoc />
        public virtual void OnPreCurrentPointerTargetChange() { }

        /// <inheritdoc />
        public virtual void Reset()
        {
            Controller = null;
            IsActive = false;
            IsFocusLocked = false;
        }

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
                    CoreServices.InputSystem.RaisePointerUp(this, pointerAction, Handedness);
                }

                IsSelectPressed = false;
            }
        }

        #endregion IMixedRealitySourcePoseHandler Implementation

        #region IMixedRealityInputHandler Implementation

        /// <inheritdoc />
        public override void OnInputUp(InputEventData eventData)
        {
            if(!IsInteractionEnabled) { return; }

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

                    CoreServices.InputSystem.RaisePointerClicked(this, pointerAction, 0, Handedness);
                    CoreServices.InputSystem.RaisePointerUp(this, pointerAction, Handedness);
                }
            }
        }

        /// <inheritdoc />
        public override void OnInputDown(InputEventData eventData)
        {
            if (!IsInteractionEnabled) { return; }

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
                        CoreServices.InputSystem.RaisePointerDown(this, pointerAction, Handedness);
                    }
                }
            }
        }

        #endregion  IMixedRealityInputHandler Implementation
    }
}
