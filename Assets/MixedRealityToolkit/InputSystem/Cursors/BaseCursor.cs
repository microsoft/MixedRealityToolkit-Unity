// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.InputSystem.Cursors
{
    /// <summary>
    /// Object that represents a cursor in 3D space controlled by gaze.
    /// </summary>
    public class BaseCursor : MonoBehaviour, IMixedRealityCursor
    {
        public CursorStateEnum CursorState { get; private set; } = CursorStateEnum.None;

        /// <summary>
        /// Maximum distance for cursor if nothing is hit
        /// </summary>
        [Tooltip("The maximum distance the cursor can be with nothing hit")]
        protected float DefaultCursorDistance = 2.0f;

        /// <summary>
        /// Surface distance to place the cursor off of the surface at
        /// </summary>
        [Tooltip("The distance from the hit surface to place the cursor")]
        private float surfaceCursorDistance = 0.02f;

        [Header("Motion")]
        [Tooltip("When lerping, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        private bool useUnscaledTime = true;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        private float positionLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        private float scaleLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        private float rotationLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        [Range(0, 1)]
        private float lookRotationBlend = 0.5f;

        /// <summary>
        /// Visual that is displayed when cursor is active normally
        /// </summary>
        [SerializeField]
        [Header("Transform References")]
        private Transform primaryCursorVisual = null;

        /// <summary>
        /// Indicates if the source is detected.
        /// </summary>
        protected bool IsHandDetected = false;

        /// <summary>
        /// Indicates pointer or air tap down
        /// </summary>
        protected bool IsPointerDown = false;

        protected GameObject TargetedObject = null;

        private uint visibleHandsCount = 0;
        private bool isVisible = true;

        // Position, scale and rotational goals for cursor
        private Vector3 targetPosition;
        private Vector3 targetScale;
        private Quaternion targetRotation;

        #region IMixedRealityCursor Implementation

        /// <summary>
        /// The pointer that this cursor should follow and process input from.
        /// </summary>
        public virtual IMixedRealityPointer Pointer
        {
            get { return pointer; }
            set
            {
                pointer = value;
                pointer.BaseCursor = this;
                RegisterManagers();
            }
        }

        private IMixedRealityPointer pointer;

        private IMixedRealityInputSystem inputSystem;

        public virtual Vector3 Position => transform.position;

        public virtual Quaternion Rotation => transform.rotation;

        public virtual Vector3 LocalScale => transform.localScale;

        /// <summary>
        /// Indicates if the cursor should be visible
        /// </summary>
        public bool IsVisible
        {
            set
            {
                isVisible = value;
                SetVisibility(isVisible);
            }
        }

        #endregion IMixedRealityCursor Implementation

        #region IMixedRealitySourceStateHandler Implementation

        /// <summary>
        /// Input source detected callback for the cursor
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            var controller = eventData.Controller as WindowsMixedRealityController;
#if UNITY_WSA
            if (controller?.LastSourceStateReading.source.kind == InteractionSourceKind.Hand)
            {
                visibleHandsCount++;
            }
#endif

            if (visibleHandsCount > 0)
            {
                IsHandDetected = true;
            }
        }

        /// <summary>
        /// Input source lost callback for the cursor
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            var controller = eventData.Controller as WindowsMixedRealityController;
#if UNITY_WSA
            if (controller?.LastSourceStateReading.source.kind == InteractionSourceKind.Hand)
            {
                visibleHandsCount--;
            }
#endif

            if (visibleHandsCount == 0)
            {
                IsHandDetected = false;
                IsPointerDown = false;
            }
        }

        #endregion IMixedRealitySourceStateHandler Implementation

        #region IMixedRealityFocusChangedHandler Implementation

        /// <summary>
        /// Updates the currently targeted object and cursor modifier upon getting
        /// an event indicating that the focused object has changed.
        /// </summary>
        public virtual void OnBeforeFocusChange(FocusEventData eventData)
        {
            if (Pointer.PointerId == eventData.Pointer.PointerId)
            {
                TargetedObject = eventData.NewFocusedObject;
            }
        }

        public virtual void OnFocusChanged(FocusEventData eventData) { }

        #endregion IMixedRealityFocusChangedHandler Implementation

        #region IMixedRealityPointerHandler Implementation

        /// <summary>
        /// Function for receiving OnPointerDown events from InputManager
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            foreach (var sourcePointer in eventData.InputSource.Pointers)
            {
                if (sourcePointer.PointerId == Pointer.PointerId)
                {
                    IsPointerDown = true;
                }
            }
        }

        /// <summary>
        /// Function for receiving OnPointerClicked events from InputManager
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClicked(MixedRealityPointerEventData eventData) { }

        /// <summary>
        /// Function for receiving OnPointerUp events from InputManager
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            foreach (var sourcePointer in eventData.InputSource.Pointers)
            {
                if (sourcePointer.PointerId == Pointer.PointerId)
                {
                    IsPointerDown = false;
                }
            }
        }

        #endregion IMixedRealityPointerHandler Implementation

        #region MonoBehaviour Impementation

        private void Awake()
        {
            // Use the setter to update visibility of the cursor at startup based on user preferences
            IsVisible = isVisible;
            SetVisibility(isVisible);
            inputSystem = Internal.Managers.MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
        }

        private void Update()
        {
            UpdateCursorState();
            UpdateCursorTransform();
        }

        /// <summary>
        /// Override for enable functions
        /// </summary>
        protected virtual void OnEnable()
        {
            OnCursorStateChange(CursorStateEnum.None);
        }

        /// <summary>
        /// Override for disable functions
        /// </summary>
        protected virtual void OnDisable()
        {
            TargetedObject = null;
            visibleHandsCount = 0;
            IsHandDetected = false;
            OnCursorStateChange(CursorStateEnum.Contextual);
        }

        private void OnDestroy()
        {
            UnregisterManagers();
        }

        #endregion MonoBehaviour Impementation

        /// <summary>
        /// Register to events from the managers the cursor needs.
        /// </summary>
        protected virtual void RegisterManagers()
        {
            // Register the cursor as a listener, so that it can always get input events it cares about
            inputSystem.Register(gameObject);

            // Setup the cursor to be able to respond to input being globally enabled / disabled
            if (inputSystem.IsInputEnabled)
            {
                OnInputEnabled();
            }
            else
            {
                OnInputDisabled();
            }

            inputSystem.InputEnabled += OnInputEnabled;
            inputSystem.InputDisabled += OnInputDisabled;
        }

        /// <summary>
        /// Unregister from events from the managers the cursor needs.
        /// </summary>
        protected virtual void UnregisterManagers()
        {
            inputSystem.InputEnabled -= OnInputEnabled;
            inputSystem.InputDisabled -= OnInputDisabled;
            inputSystem.Unregister(gameObject);
        }

        /// <summary>
        /// Update the cursor's transform
        /// </summary>
        protected virtual void UpdateCursorTransform()
        {
            Debug.Assert(Pointer != null, "No Pointer has been assigned!");

            FocusDetails focusDetails;
            if (!Pointer.InputSystem.FocusProvider.TryGetFocusDetails(Pointer, out focusDetails))
            {
                if (Pointer.InputSystem.FocusProvider.IsPointerRegistered(Pointer))
                {
                    Debug.LogError($"{name}: Unable to get focus details for {pointer.GetType().Name}!");
                }
                else
                {
                    Debug.LogError($"{pointer.GetType().Name} has not been registered!");
                }

                return;
            }

            GameObject newTargetedObject = Pointer.InputSystem.FocusProvider.GetFocusedObject(Pointer);
            Vector3 lookForward;

            // Normalize scale on before update
            targetScale = Vector3.one;

            // If no game object is hit, put the cursor at the default distance
            if (newTargetedObject == null)
            {
                TargetedObject = null;

                targetPosition = RayStep.GetPointByDistance(Pointer.Rays, DefaultCursorDistance);
                lookForward = -RayStep.GetDirectionByDistance(Pointer.Rays, DefaultCursorDistance);
                targetRotation = lookForward.magnitude > 0 ? Quaternion.LookRotation(lookForward, Vector3.up) : transform.rotation;
            }
            else
            {
                // Update currently targeted object
                TargetedObject = newTargetedObject;

                if (Pointer.CursorModifier != null)
                {
                    Pointer.CursorModifier.GetModifiedTransform(this, out targetPosition, out targetRotation, out targetScale);
                }
                else
                {
                    // If no modifier is on the target, just use the hit result to set cursor position
                    // Get the look forward by using distance between pointer origin and target position
                    // (This may not be strictly accurate for extremely wobbly pointers, but it should produce usable results)
                    float distanceToTarget = Vector3.Distance(Pointer.Rays[0].Origin, focusDetails.Point);
                    lookForward = -RayStep.GetDirectionByDistance(Pointer.Rays, distanceToTarget);
                    targetPosition = focusDetails.Point + (lookForward * surfaceCursorDistance);
                    Vector3 lookRotation = Vector3.Slerp(focusDetails.Normal, lookForward, lookRotationBlend);
                    targetRotation = Quaternion.LookRotation(lookRotation == Vector3.zero ? lookForward : lookRotation, Vector3.up);
                }
            }

            float deltaTime = useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            // Use the lerp times to blend the position to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, deltaTime / positionLerpTime);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, deltaTime / scaleLerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, deltaTime / rotationLerpTime);
        }

        /// <summary>
        /// Updates the visual representation of the cursor.
        /// </summary>
        public virtual void SetVisibility(bool visible)
        {
            if (primaryCursorVisual != null)
            {
                primaryCursorVisual.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Returns the <see cref="BaseCursor"/>'s <see cref="GameObject"/> reference.
        /// </summary>
        /// <returns></returns>
        public GameObject GetGameObjectReference()
        {
            return gameObject;
        }

        /// <summary>
        /// Disable input and set to contextual to override input
        /// </summary>
        public virtual void OnInputDisabled()
        {
            // Reset visible hands on disable
            visibleHandsCount = 0;
            IsHandDetected = false;

            OnCursorStateChange(CursorStateEnum.Contextual);
        }

        /// <summary>
        /// Enable input and set to none to reset cursor
        /// </summary>
        public virtual void OnInputEnabled()
        {
            OnCursorStateChange(CursorStateEnum.None);
        }

        /// <summary>
        /// Internal update to check for cursor state changes
        /// </summary>
        private void UpdateCursorState()
        {
            CursorStateEnum newState = CheckCursorState();
            if (CursorState != newState)
            {
                OnCursorStateChange(newState);
            }
        }

        /// <summary>
        /// Virtual function for checking state changes.
        /// </summary>
        public virtual CursorStateEnum CheckCursorState()
        {
            if (CursorState != CursorStateEnum.Contextual)
            {
                if (IsPointerDown)
                {
                    return CursorStateEnum.Select;
                }

                if (CursorState == CursorStateEnum.Select)
                {
                    return CursorStateEnum.Release;
                }

                if (IsHandDetected)
                {
                    return TargetedObject != null ? CursorStateEnum.InteractHover : CursorStateEnum.Interact;
                }

                return TargetedObject != null ? CursorStateEnum.ObserveHover : CursorStateEnum.Observe;
            }

            return CursorStateEnum.Contextual;
        }

        /// <summary>
        /// Change the cursor state to the new state.  Override in cursor implementations.
        /// </summary>
        /// <param name="state"></param>
        public virtual void OnCursorStateChange(CursorStateEnum state)
        {
            CursorState = state;
        }
    }
}