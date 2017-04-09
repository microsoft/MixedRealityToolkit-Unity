// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Object that represents a cursor in 3D space controlled by gaze.
    /// </summary>
    public abstract class Cursor : MonoBehaviour, ICursor
    {
        /// <summary>
        /// Enum for current cursor state
        /// </summary>
        public enum CursorStateEnum
        {
            /// <summary>
            /// Useful for releasing external override.
            /// See <c>CursorStateEnum.Contextual</c>
            /// </summary>
            None = -1,
            /// <summary>
            /// Not IsHandVisible
            /// </summary>
            Observe,
            /// <summary>
            /// Not IsHandVisible AND not IsInputSourceDown AND TargetedObject exists
            /// </summary>
            ObserveHover,
            /// <summary>
            /// IsHandVisible AND not IsInputSourceDown AND TargetedObject is NULL
            /// </summary>
            Interact,
            /// <summary>
            /// IsHandVisible AND not IsInputSourceDown AND TargetedObject exists
            /// </summary>
            InteractHover,
            /// <summary>
            /// IsHandVisible AND IsInputSourceDown
            /// </summary>
            Select,
            /// <summary>
            /// Available for use by classes that extend Cursor.
            /// No logic for setting Release state exists in the base Cursor class.
            /// </summary>
            Release,
            /// <summary>
            /// Allows for external override
            /// </summary>
            Contextual
        }

        public CursorStateEnum CursorState { get { return cursorState; } }
        private CursorStateEnum cursorState = CursorStateEnum.None;

        /// <summary>
        /// Minimum distance for cursor if nothing is hit
        /// </summary>
        [Header("Cusor Distance")]
        [Tooltip("The minimum distance the cursor can be with nothing hit")]
        public float MinCursorDistance = 1.0f;

        /// <summary>
        /// Maximum distance for cursor if nothing is hit
        /// </summary>
        [Tooltip("The maximum distance the cursor can be with nothing hit")]
        public float DefaultCursorDistance = 2.0f;

        /// <summary>
        /// Surface distance to place the cursor off of the surface at
        /// </summary>
        [Tooltip("The distance from the hit surface to place the cursor")]
        public float SurfaceCursorDistance = 0.02f;

        [Header("Motion")]
        [Tooltip("When lerping, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        public bool UseUnscaledTime = true;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        public float PositionLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        public float ScaleLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        public float RotationLerpTime = 0.01f;

        /// <summary>
        /// Blend value for surface normal to user facing lerp
        /// </summary>
        [Range(0, 1)]
        public float LookRotationBlend = 0.5f;

        /// <summary>
        /// Visual that is displayed when cursor is active normally
        /// </summary>
        [Header("Tranform References")]
        public Transform PrimaryCursorVisual;

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public Quaternion Rotation
        {
            get { return transform.rotation; }
        }

        public Vector3 LocalScale
        {
            get { return transform.localScale; }
        }

        /// <summary>
        /// Indicates if hand is current in the view
        /// </summary>
        protected bool IsHandVisible;

        /// <summary>
        /// Indicates air tap down
        /// </summary>
        protected bool IsInputSourceDown;

        protected GameObject TargetedObject;
        protected ICursorModifier TargetedCursorModifier;

        private uint visibleHandsCount = 0;
        private bool isVisible = true;

        private GazeManager gazeManager;

        /// <summary>
        /// Position, scale and rotational goals for cursor
        /// </summary>
        private Vector3 targetPosition;
        private Vector3 targetScale;
        private Quaternion targetRotation;

        /// <summary>
        /// Indicates if the cursor should be visible
        /// </summary>
        public bool IsVisible
        {
            set
            {
                isVisible = value;
                SetVisiblity(isVisible);
            }
        }

        #region MonoBehaviour Functions

        private void Awake()
        {
            // Use the setter to update visibility of the cursor at startup based on user preferences
            IsVisible = isVisible;
            SetVisiblity(isVisible);
        }

        private void Start()
        {
            gazeManager = GazeManager.Instance;
            RegisterManagers();
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
            if (gazeManager)
            {
                OnFocusedObjectChanged(null, gazeManager.HitObject);
            }
            OnCursorStateChange(CursorStateEnum.None);
        }

        /// <summary>
        /// Override for disable functions
        /// </summary>
        protected virtual void OnDisable()
        {
            TargetedObject = null;
            TargetedCursorModifier = null;
            visibleHandsCount = 0;
            IsHandVisible = false;
            OnCursorStateChange(CursorStateEnum.Contextual);
        }

        private void OnDestroy()
        {
            UnregisterManagers();
        }

        #endregion

        /// <summary>
        /// Register to events from the managers the cursor needs.
        /// </summary>
        protected virtual void RegisterManagers()
        {
            // Register to gaze events
            gazeManager.FocusedObjectChanged += OnFocusedObjectChanged;

            // Register the cursor as a global listener, so that it can always get input events it cares about
            InputManager.Instance.AddGlobalListener(gameObject);

            // Setup the cursor to be able to respond to input being globally enabled / disabled
            if (InputManager.Instance.IsInputEnabled)
            {
                OnInputEnabled();
            }
            else
            {
                OnInputDisabled();
            }

            InputManager.Instance.InputEnabled += OnInputEnabled;
            InputManager.Instance.InputDisabled += OnInputDisabled;
        }

        /// <summary>
        /// Unregister from events from the managers the cursor needs.
        /// </summary>
        protected virtual void UnregisterManagers()
        {
            if (gazeManager != null)
            {
                gazeManager.FocusedObjectChanged -= OnFocusedObjectChanged;
            }

            if (InputManager.Instance != null)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
                InputManager.Instance.InputEnabled -= OnInputEnabled;
                InputManager.Instance.InputDisabled -= OnInputDisabled;
            }
        }

        /// <summary>
        /// Updates the currently targeted object and cursor modifier upon getting
        /// an event indicating that the focused object has changed.
        /// </summary>
        /// <param name="previousObject">Object that was previously being focused.</param>
        /// <param name="newObject">New object being focused.</param>
        protected virtual void OnFocusedObjectChanged(GameObject previousObject, GameObject newObject)
        {
            TargetedObject = newObject;
            if (newObject != null)
            {
                OnActiveModifier(newObject.GetComponent<CursorModifier>());
            }
        }

        /// <summary>
        /// Override function when a new modifier is found or no modifier is valid
        /// </summary>
        /// <param name="modifier"></param>
        protected virtual void OnActiveModifier(CursorModifier modifier)
        {
            TargetedCursorModifier = modifier;
        }

        /// <summary>
        /// Update the cursor's transform
        /// </summary>
        protected virtual void UpdateCursorTransform()
        {
            // Get the necessary info from the gaze source
            RaycastHit hitResult = gazeManager.HitInfo;
            GameObject newTargetedObject = gazeManager.HitObject;

            // Get the forward vector looking back at camera
            Vector3 lookForward = -gazeManager.GazeNormal;

            // Normalize scale on before update
            targetScale = Vector3.one;

            // If no game object is hit, put the cursor at the default distance
            if (TargetedObject == null)
            {
                this.TargetedObject = null;
                this.TargetedCursorModifier = null;
                targetPosition = gazeManager.GazeOrigin + gazeManager.GazeNormal * DefaultCursorDistance;
                targetRotation = lookForward.magnitude > 0 ? Quaternion.LookRotation(lookForward, Vector3.up) : transform.rotation;
            }
            else
            {
                // Update currently targeted object
                this.TargetedObject = newTargetedObject;

                if (TargetedCursorModifier != null)
                {
                    TargetedCursorModifier.GetModifiedTransform(this, out targetPosition, out targetRotation, out targetScale);
                }
                else
                {
                    // If no modifier is on the target, just use the hit result to set cursor position
                    targetPosition = hitResult.point + (lookForward * SurfaceCursorDistance);
                    targetRotation = Quaternion.LookRotation(Vector3.Lerp(hitResult.normal, lookForward, LookRotationBlend), Vector3.up);
                }
            }

            float deltaTime = UseUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            // Use the lerp times to blend the position to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, deltaTime / PositionLerpTime);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, deltaTime / ScaleLerpTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, deltaTime / RotationLerpTime);
        }

        /// <summary>
        /// Updates the visual representation of the cursor.
        /// </summary>
        public void SetVisiblity(bool visible)
        {
            if (PrimaryCursorVisual != null)
            {
                PrimaryCursorVisual.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Disable input and set to contextual to override input
        /// </summary>
        public virtual void OnInputDisabled()
        {
            // Reset visible hands on disable
            visibleHandsCount = 0;
            IsHandVisible = false;

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
        /// Function for consuming the OnInputUp events
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInputUp(InputEventData eventData)
        {
            IsInputSourceDown = false;
        }

        /// <summary>
        /// Function for receiving OnInputDown events from InputManager
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInputDown(InputEventData eventData)
        {
            IsInputSourceDown = true;
        }

        /// <summary>
        /// Function for receiving OnInputClicked events from InputManager
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInputClicked(InputClickedEventData eventData)
        {
            // Open input socket for other cool stuff...
        }


        /// <summary>
        /// Input source detected callback for the cursor
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            visibleHandsCount++;
            IsHandVisible = true;
        }


        /// <summary>
        /// Input source lost callback for the cursor
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            visibleHandsCount--;
            if (visibleHandsCount == 0)
            {
                IsHandVisible = false;
                IsInputSourceDown = false;
            }
        }

        /// <summary>
        /// Internal update to check for cursor state changes
        /// </summary>
        private void UpdateCursorState()
        {
            CursorStateEnum newState = CheckCursorState();
            if (cursorState != newState)
            {
                OnCursorStateChange(newState);
            }
        }

        /// <summary>
        /// Virtual function for checking state changess.
        /// </summary>
        public virtual CursorStateEnum CheckCursorState()
        {
            if (cursorState != CursorStateEnum.Contextual)
            {
                if (IsInputSourceDown)
                {
                    return CursorStateEnum.Select;
                }
                else if (cursorState == CursorStateEnum.Select)
                {
                    return CursorStateEnum.Release;
                }

                if (IsHandVisible)
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
            cursorState = state;
        }
    }
}
