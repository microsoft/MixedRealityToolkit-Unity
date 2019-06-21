// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    ///<summary>
    /// A button that can be pushed via direct touch.
    /// You can use <see cref="Microsoft.MixedReality.Toolkit.Examples.Demos.PhysicalPressEventRouter"/> to route these events to <see cref="Interactable"/>.
    ///</summary>
    [RequireComponent(typeof(BoxCollider))]
    public class PressableButton : MonoBehaviour, IMixedRealityTouchHandler
    {
        const string InitialMarkerTransformName = "Initial Marker";

        [SerializeField]
        [Tooltip("The object that is being pushed.")]
        protected GameObject movingButtonVisuals = null;

        [SerializeField]
        [Header("Press Settings")]
        [Tooltip("The offset at which pushing starts. Offset is relative to the pivot of either the moving visuals if there's any or the button itself.")]
        protected float startPushDistance = 0.0f;
        public float StartPushDistance { get => startPushDistance; set => startPushDistance = value; }

        [SerializeField]
        [Tooltip("Maximum push distance. Distance is relative to the pivot of either the moving visuals if there's any or the button itself.")]
        private float maxPushDistance = 0.2f;
        public float MaxPushDistance { get => maxPushDistance; set => maxPushDistance = value; }

        [SerializeField]
        [FormerlySerializedAs("minPressDepth")]
        [Tooltip("Distance the button must be pushed until it is considered pressed. Distance is relative to the pivot of either the moving visuals if there's any or the button itself.")]
        private float pressDistance = 0.02f;
        public float PressDistance { get => pressDistance; set => pressDistance = value; }

        [SerializeField]
        [FormerlySerializedAs("withdrawActivationAmount")]
        [Tooltip("Withdraw amount needed to transition from Pressed to Released.")]
        private float releaseDistanceDelta = 0.01f;
        public float ReleaseDistanceDelta { get => releaseDistanceDelta; set => releaseDistanceDelta = value; }

        [SerializeField]
        [Tooltip("Speed for retracting the moving button visuals on release.")]
        [FormerlySerializedAs("returnRate")]
        private float returnSpeed = 25.0f;

        [SerializeField]
        [Tooltip("Ensures that the button can only be pushed from the front. Touching the button from the back or side is prevented.")]
        private bool enforceFrontPush = true;

        public enum SpaceMode
        {
            World,
            Local
        }

        [SerializeField]
        [HideInInspector]
        private SpaceMode distanceSpaceMode = SpaceMode.World;

        public SpaceMode DistanceSpaceMode
        {
            get => distanceSpaceMode;
            set
            {
                // Convert world to local distances and vice versa whenever we switch the mode
                if (value != distanceSpaceMode)
                {
                    distanceSpaceMode = value;
                    float scale = (distanceSpaceMode == SpaceMode.Local) ? WorldToLocalScale : LocalToWorldScale;
                    startPushDistance *= scale;
                    maxPushDistance *= scale;
                    pressDistance *= scale;
                    releaseDistanceDelta *= scale;
                }
            }
        }

        [Header("Events")]
        public UnityEvent TouchBegin;
        public UnityEvent TouchEnd;
        public UnityEvent ButtonPressed;
        public UnityEvent ButtonReleased;

        #region Private Members

        // The maximum distance before the button is reset to its initial position when retracting.
        private const float MaxRetractDistanceBeforeReset = 0.0001f;

        public float currentPushDistance = 0.0f;
        public float CurrentPushDistance { get => currentPushDistance; set => currentPushDistance = value; }

        private Dictionary<IMixedRealityController, Vector3> touchPoints = new Dictionary<IMixedRealityController, Vector3>();

        private bool isTouching = false;

        ///<summary>
        /// Represents the state of whether or not a finger is currently touching this button.
        ///</summary>
        public bool IsTouching
        {
            get
            {
                return isTouching;
            }

            private set
            {
                if (value != isTouching)
                {
                    isTouching = value;

                    if (isTouching)
                    {
                        TouchBegin.Invoke();
                    }
                    else
                    {
                        // Abort press.
                        IsPressing = false;

                        TouchEnd.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Represents the state of whether the button is currently being pressed.
        /// </summary>
        public bool IsPressing { get; private set; }

        /// <summary>
        /// The press direction of the button as defined by a NearInteractionTouchable.
        /// </summary>
        private Vector3 WorldSpacePressDirection
        {
            get
            {
                var nearInteractionTouchable = GetComponent<NearInteractionTouchable>();
                if (nearInteractionTouchable != null)
                {
                    return -1.0f * nearInteractionTouchable.Forward;
                }
                
                return transform.forward;
            }
        }

        private Transform PushSpaceSourceTransform
        {
            get { return movingButtonVisuals != null ? movingButtonVisuals.transform : transform; }
        }

        private float WorldToLocalScale
        {
            get
            {
                return transform.InverseTransformVector(WorldSpacePressDirection).magnitude;
            }
        }

        private float LocalToWorldScale
        {
            get
            {
                return 1.0f / WorldToLocalScale;
            }
        }
        
        /// <summary>
        /// Initial offset from moving visuals to button
        /// </summary>
        private Vector3 initialOffsetMovingVisuals = Vector3.zero;

        /// <summary>
        /// The position from where the button starts to move. 
        /// </summary>
        private Vector3 InitialPosition
        {
            get
            {
                if (Application.isPlaying && movingButtonVisuals) // we're using a cached position in play mode as the moving visuals will be moved during button interaction
                {
                    return PushSpaceSourceTransform.parent.position + initialOffsetMovingVisuals;
                }
                else
                {
                    return PushSpaceSourceTransform.position;
                }

            }
        }

        #endregion

        private void OnEnable()
        {
            currentPushDistance = startPushDistance;    
        }

        private void Start()
        {
            if (gameObject.layer == 2)
            {
                Debug.LogWarning("PressableButton will not work if game object layer is set to 'Ignore Raycast'.");
            }

            initialOffsetMovingVisuals = PushSpaceSourceTransform.position - PushSpaceSourceTransform.parent.position;
        }

        void OnDisable()
        {
            // clear touch points in case we get disabled and can't receive the touch end event anymore
            touchPoints.Clear();

            // make sure button doesn't stay in a pressed state in case we disable the button while pressing it
            currentPushDistance = startPushDistance;
            UpdateMovingVisualsPosition();
        }

        private void Update()
        {
            if (IsTouching)
            {
                currentPushDistance = GetFarthestDistanceAlongPressDirection();

                UpdateMovingVisualsPosition();

                // Hand Press is only allowed to happen while touching.
                UpdatePressedState(currentPushDistance);
            }
            else if (currentPushDistance > startPushDistance)
            {
                // Retract the button.
                float retractDistance = currentPushDistance - startPushDistance;
                retractDistance = retractDistance - retractDistance * returnSpeed * Time.deltaTime;

                // Apply inverse scale of local z-axis. This constant should always have the same value in world units.
                float localMaxRetractDistanceBeforeReset =
                    MaxRetractDistanceBeforeReset * WorldSpacePressDirection.magnitude;
                if (retractDistance < localMaxRetractDistanceBeforeReset)
                {
                    currentPushDistance = startPushDistance;
                }
                else
                {
                    currentPushDistance = startPushDistance + retractDistance;
                }

                UpdateMovingVisualsPosition();
            }
        }

        #region IMixedRealityTouchHandler implementation

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (touchPoints.ContainsKey(eventData.Controller))
            {
                return;
            }

            if (enforceFrontPush)
            {
                // Back-Press Detection:
                // Accept touch only if controller pushed from the front.
                // Extrapolate to get previous position.
                Vector3 previousPosition = eventData.InputData - eventData.Controller.Velocity * Time.deltaTime;
                float previousDistance = GetDistanceAlongPushDirection(previousPosition);

                if (previousDistance > startPushDistance)
                {
                    return;
                }
            }

            touchPoints.Add(eventData.Controller, eventData.InputData);
            IsTouching = true;

            // Pulse each proximity light on pointer cursors' interacting with this button.
            foreach (var pointer in eventData.InputSource.Pointers)
            {
                ProximityLight[] proximityLights = pointer.BaseCursor?.GameObjectReference?.GetComponentsInChildren<ProximityLight>();

                if (proximityLights != null)
                {
                    foreach (var proximityLight in proximityLights)
                    {
                        proximityLight.Pulse();
                    }
                }
            }

            eventData.Use();
        }

        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            if (touchPoints.ContainsKey(eventData.Controller))
            {
                touchPoints[eventData.Controller] = eventData.InputData;

                eventData.Use();
            }
        }

        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            if (touchPoints.ContainsKey(eventData.Controller))
            {
                touchPoints.Remove(eventData.Controller);
                IsTouching = (touchPoints.Count > 0);
                eventData.Use();
            }
        }

        #endregion OnTouch

        #region public transform utils

        /// <summary>
        /// Returns world space position along the push direction for the given local distance
        /// </summary>
        /// <param name="localDistance"></param>
        /// <returns></returns>
        /// 
        public Vector3 GetWorldPositionAlongPushDirection(float localDistance)
        {
            float distance = (distanceSpaceMode == SpaceMode.Local) ? localDistance * LocalToWorldScale : localDistance;
            return InitialPosition + WorldSpacePressDirection.normalized * distance;
        }


        /// <summary>
        /// Returns the local distance along the push direction for the passed in world position
        /// </summary>
        /// <param name="positionWorldSpace"></param>
        /// <returns></returns>
        public float GetDistanceAlongPushDirection(Vector3 positionWorldSpace)
        {
            Vector3 localPosition = positionWorldSpace - InitialPosition;
            float distance = Vector3.Dot(localPosition, WorldSpacePressDirection.normalized);
            return (distanceSpaceMode == SpaceMode.Local) ? distance / LocalToWorldScale : distance;
        }

        #endregion

        #region private Methods

        protected virtual void UpdateMovingVisualsPosition()
        {
            if (movingButtonVisuals != null)
            {
                // Always move relative to startPushDistance
                movingButtonVisuals.transform.position = GetWorldPositionAlongPushDirection(currentPushDistance - startPushDistance);
            }
        }

        // This function projects the current touch positions onto the 1D press direction of the button.
        // It will output the farthest pushed distance from the button's initial position.
        private float GetFarthestDistanceAlongPressDirection()
        {
            float farthestDistance = startPushDistance;

            foreach (var touchEntry in touchPoints)
            {
                float testDistance = GetDistanceAlongPushDirection(touchEntry.Value);
                farthestDistance = Mathf.Max(testDistance, farthestDistance);
            }

            return Mathf.Clamp(farthestDistance, startPushDistance, maxPushDistance);
        }


        private void UpdatePressedState(float pushDistance)
        {
            // If we aren't in a press and can't start a simple one.
            if (!IsPressing)
            {
                // Compare to our previous push depth. Use previous push distance to handle back-presses.
                if (pushDistance >= pressDistance)
                {
                    IsPressing = true;
                    ButtonPressed.Invoke();
                }
            }
            // If we're in a press, check if the press is released now.
            else
            {
                float releaseDistance = pressDistance - releaseDistanceDelta;
                if (pushDistance <= releaseDistance)
                {
                    IsPressing = false;
                    ButtonReleased.Invoke();
                }
            }
        }

        #endregion
    }
}