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
        private GameObject movingButtonVisuals = null;

        [SerializeField]
        [Header("Press Settings")]
        [Tooltip("Maximum push distance")]
        private float maxPushDistance = 0.2f;

        [SerializeField]
        [FormerlySerializedAs("minPressDepth")]
        [Tooltip("Distance the button must be pushed until it is considered pressed.")]
        private float pressDistance = 0.02f;

        [SerializeField]
        [FormerlySerializedAs("withdrawActivationAmount")]
        [Tooltip("Withdraw amount needed to transition from Pressed to Released.")]
        private float releaseDistanceDelta = 0.01f;

        [SerializeField]
        [Tooltip("Speed of the object movement on release.")]
        private float returnRate = 25.0f;

        [SerializeField]
        [Tooltip("Ensures that the button can only be pushed from the front. Touching the button from the back or side is prevented.")]
        private bool enforceFrontPush = true;

        [Header("Position markers")]
        [Tooltip("Used to mark where button movement begins. If null, it will be automatically generated.")]

        [Header("Events")]
        public UnityEvent TouchBegin;
        public UnityEvent TouchEnd;
        public UnityEvent ButtonPressed;
        public UnityEvent ButtonReleased;

        #region Private Members

        // The maximum distance before the button is reset to its initial position when retracting.
        private const float MaxRetractDistanceBeforeReset = 0.0001f;

        private float currentPushDistance = 0.0f;

        private Dictionary<IMixedRealityController, Vector3> touchPoints = new Dictionary<IMixedRealityController, Vector3>();

        [Header("Button State")]
        [ReadOnly]
        [SerializeField]
        private bool isTouching = false;

        [ReadOnly]
        [SerializeField]
        private bool isPressing = false;

        private Transform initialTransform;

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
                        isPressing = false;

                        TouchEnd.Invoke();
                    }
                }
            }
        }

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

        #endregion

        private void Start()
        {
            if (gameObject.layer == 2)
            {
                Debug.LogWarning("PressableButton will not work if game object layer is set to 'Ignore Raycast'.");
            }
        }

        private void Update()
        {
            IsTouching = (touchPoints.Count != 0);

            if (IsTouching)
            {
                currentPushDistance = GetFarthestPushDistanceAlongButtonAxis();

                UpdateMovingVisualsPosition();

                // Hand Press is only allowed to happen while touching.
                UpdatePressedState(currentPushDistance);
            }
            else if (currentPushDistance > 0.0f)
            {
                // Retract the button.
                currentPushDistance = Mathf.Max(0.0f, currentPushDistance - currentPushDistance * returnRate * Time.deltaTime);

                if (currentPushDistance < MaxRetractDistanceBeforeReset)
                {
                    currentPushDistance = 0.0f;
                }

                UpdateMovingVisualsPosition();
            }
        }

        #region OnTouch

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            FindOrCreatePathMarkers();

            if (enforceFrontPush)
            {
                // Back-Press Detection:
                // Accept touch only if controller pushed from the front.
                // Extrapolate to get previous position.
                Vector3 previousPosition = eventData.InputData - eventData.Controller.Velocity * Time.deltaTime;
                float previousDistance = GetProjectedDistance(initialTransform.position, WorldSpacePressDirection, previousPosition);

                if (previousDistance > 0.0f)
                {
                    return;
                }
            }

            Debug.Assert(!touchPoints.ContainsKey(eventData.Controller));
            touchPoints.Add(eventData.Controller, eventData.InputData);

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

                eventData.Use();
            }
        }

        #endregion OnTouch

        #region private Methods

        private void FindOrCreatePathMarkers()
        {
            Transform sourcePositionTransform = (movingButtonVisuals != null) ? movingButtonVisuals.transform : transform;

            // First try to search for our markers
            if (initialTransform == null)
            {
                initialTransform = transform.Find(InitialMarkerTransformName);
            }

            // If we don't find them, create them
            if (initialTransform == null)
            {
                initialTransform = new GameObject(InitialMarkerTransformName).transform;
                initialTransform.parent = sourcePositionTransform.parent;
                initialTransform.position = sourcePositionTransform.position;
            }
        }

        private void UpdateMovingVisualsPosition()
        {
            if (movingButtonVisuals != null)
            {
                Debug.Assert(initialTransform != null);
                movingButtonVisuals.transform.position = initialTransform.position + WorldSpacePressDirection * currentPushDistance;
            }
        }

        // This function projects the current touch positions onto the 1D push direction of the button.
        // It will output the farthest pushed distance from the button's initial position.
        private float GetFarthestPushDistanceAlongButtonAxis()
        {
            Debug.Assert(initialTransform != null);

            float farthestDistance = 0.0f;

            foreach (var touchEntry in touchPoints)
            {
                float testDistance = GetProjectedDistance(initialTransform.position, WorldSpacePressDirection, touchEntry.Value);
                farthestDistance = Mathf.Max(testDistance, farthestDistance);
            }

            return Mathf.Clamp(farthestDistance, 0.0f, maxPushDistance);
        }

        private void UpdatePressedState(float pushDistance)
        {
            // If we aren't in a press and can't start a simple one.
            if (!isPressing)
            {
                // Compare to our previous push depth. Use previous push distance to handle back-presses.
                if (pushDistance >= pressDistance)
                {
                    isPressing = true;
                    ButtonPressed.Invoke();
                }
            }
            // If we're in a press, check if the press is released now.
            else
            {
                float releaseDistance = pressDistance - releaseDistanceDelta;
                if (pushDistance <= releaseDistance)
                {
                    isPressing = false;
                    ButtonReleased.Invoke();
                }
            }
        }

        private Vector3 ProjectPointToRay(Vector3 rayStart, Vector3 rayDir, Vector3 point, out float distance)
        {
            Vector3 localPoint = point - rayStart;
            distance = Vector3.Dot(localPoint, rayDir);
            return rayStart + (rayDir * distance);
        }

        private float GetProjectedDistance(Vector3 rayStart, Vector3 rayDir, Vector3 point)
        {
            Vector3 localPoint = point - rayStart;
            return Vector3.Dot(localPoint, rayDir);
        }

        #endregion
    }
}