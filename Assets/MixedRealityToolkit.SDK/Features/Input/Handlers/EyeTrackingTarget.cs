// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A game object with the "EyeTrackingTarget" script attached reacts to being looked at independent of other available inputs.
    /// </summary>
    public class EyeTrackingTarget : InputSystemGlobalListener, IMixedRealityPointerHandler, IMixedRealitySpeechHandler
    {
        [Tooltip("Select action that are specific to when the target is looked at.")]
        [SerializeField]
        private MixedRealityInputAction selectAction = MixedRealityInputAction.None;

        [Tooltip("List of voice commands to trigger selecting this target only if it is looked at.")]
        [SerializeField]
        [FormerlySerializedAs("voice_select")]
        private MixedRealityInputAction[] voiceSelect = null;

        [Tooltip("Duration in seconds that the user needs to keep looking at the target to select it via dwell activation.")]
        [Range(0, 10)]
        [SerializeField]
        private float dwellTimeInSec = 0.8f;

        [Tooltip("Event is triggered when the user starts to look at the target.")]        
        public UnityEvent OnLookAtStart = null;

        [Tooltip("Event is triggered when the user continues to look at the target.")]
        public UnityEvent WhileLookingAtTarget = null;

        [Tooltip("Event to be triggered when the user is looking away from the target.")]
        public UnityEvent OnLookAway = null;

        [Tooltip("Event to be triggered when the target has been looked at for a given predefined duration.")]
        public UnityEvent OnDwell = null;

        [Tooltip("Event to be triggered when the looked at target is selected.")]
        public UnityEvent OnSelected = null;
        
        /// <summary>
        /// Returns true if the user looks at the target or more specifically when the eye gaze ray intersects 
        /// with the target's bounding box.
        /// </summary>
        public bool IsLookedAt { get; private set; }

        /// <summary>
        /// If true, the eye cursor (if enabled) will snap to the center of this object. 
        /// </summary>
        public bool eyeCursorSnapToTargetCenter = false;

        private bool isDwelledOn = false;

        /// <summary>
        /// Returns true if the user has been looking at the target for a certain amount of time specified by dwellTimeInSec.
        /// </summary>
        public bool IsDwelledOn
        {
            get { return isDwelledOn; }
        }

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }
        
        private DateTime lookAtStartTime;
        private float EyeTrackerFramerate = 30; // In Hz -> This means that every 1000ms/30 = 33.33ms a new sample should arrive. 
        private float EyeTrackingTimeoutInMilliseconds = 100; // To account for blinks

        private static DateTime lastTimeStamp = DateTime.MinValue;
        public static GameObject LookedAtTarget { get;  private set; }
        public static EyeTrackingTarget LookedAtEyeTarget { get; private set; }
        public static Vector3 LookedAtPoint { get; private set; }

        #region Focus handling
        private void Start()
        {
            IsLookedAt = false;
            LookedAtTarget = null;
            LookedAtEyeTarget = null;
        }
        private void Update()
        {
            // Try to manually poll the eye tracking data
            if ((InputSystem != null) && (InputSystem.EyeGazeProvider != null) && 
                InputSystem.EyeGazeProvider.UseEyeTracking && 
                InputSystem.EyeGazeProvider.IsEyeGazeValid)
            {
                UpdateHitTarget();

                bool isLookedAtNow = (LookedAtTarget == this.gameObject);
                                
                if (IsLookedAt && (!isLookedAtNow))
                {
                    // Stopped looking at the target
                    OnEyeFocusStop();
                }
                else if ((!IsLookedAt) && (isLookedAtNow))
                {
                    // Started looking at the target
                    OnEyeFocusStart();
                }
                else if (IsLookedAt && (isLookedAtNow))
                {
                    // Keep looking at the target
                    OnEyeFocusStay();
                }
            }
        }

        private void OnDisable()
        {
            OnEyeFocusStop();
        }

        private void UpdateHitTarget()
        {
            if (lastTimeStamp != InputSystem?.EyeGazeProvider?.Timestamp)
            {
                lastTimeStamp = InputSystem.EyeGazeProvider.Timestamp;

                // ToDo: Handle raycasting layers
                RaycastHit hitInfo = default(RaycastHit);
                Ray lookRay = new Ray(InputSystem.EyeGazeProvider.GazeOrigin, InputSystem.EyeGazeProvider.GazeDirection.normalized);
                bool isHit = UnityEngine.Physics.Raycast(lookRay, out hitInfo);

                if (isHit)
                {
                    LookedAtTarget = hitInfo.collider.gameObject;
                    LookedAtEyeTarget = LookedAtTarget.GetComponent<EyeTrackingTarget>();
                    LookedAtPoint = hitInfo.point;
                }
                else
                {
                    LookedAtTarget = null;
                    LookedAtEyeTarget = null;
                }
            }
            else if ((DateTime.UtcNow - InputSystem.EyeGazeProvider.Timestamp).TotalMilliseconds > EyeTrackingTimeoutInMilliseconds)
            {
                LookedAtTarget = null;
                LookedAtEyeTarget = null;
                IsLookedAt = false;
            }
        }
        
        protected void OnEyeFocusStart()
        {
            lookAtStartTime = DateTime.UtcNow;
            IsLookedAt = true;
            OnLookAtStart.Invoke();            
        }

        protected void OnEyeFocusStay()
        {
            WhileLookingAtTarget.Invoke();

            if ((!isDwelledOn) && (DateTime.UtcNow - lookAtStartTime).TotalSeconds > dwellTimeInSec)
            {
                OnEyeFocusDwell();
            }
        }

        protected void OnEyeFocusDwell()
        {
            isDwelledOn = true;
            OnDwell.Invoke();
        }

        protected void OnEyeFocusStop()
        {
            isDwelledOn = false;
            IsLookedAt = false;
            OnLookAway.Invoke();            
        }

        #endregion 

        #region IMixedRealityPointerHandler
        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if ((eventData.MixedRealityInputAction == selectAction) && IsLookedAt)
            {
                OnSelected.Invoke();
            }
        }
        
        void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if ((IsLookedAt) && (this.gameObject == LookedAtTarget))
            {
                if (voiceSelect != null)
                {
                    for (int i = 0; i < voiceSelect.Length; i++)
                    {
                        if (eventData.MixedRealityInputAction == voiceSelect[i])
                        {
                            OnSelected.Invoke();
                        }
                    }
                }
            }
        }
        #endregion
    }
}