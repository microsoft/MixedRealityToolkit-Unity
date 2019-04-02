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

        [Header("Events")]
        public UnityEvent TouchBegin;
        public UnityEvent TouchEnd;
        public UnityEvent ButtonPressed;
        public UnityEvent ButtonReleased;

        #region Private Members

        // The maximum distance before the button is reset to its initial position when retracting.
        private const float MaxRetractDistanceBeforeReset = 0.0001f;

        private float currentPushDistance = 0.0f;

        private List<Vector3> touchPoints = new List<Vector3>();
        private Transform initialTransform;
        private BoxCollider boxCollider;

        private bool isTouching = false;
        private bool isPressing = false;

        ///<summary>
        /// Represents the state of whether or not a finger is currently touching this button.
        ///</summary>
        private bool IsTouching
        {
            get
            {
                return isTouching;
            }

            set
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

                Vector3 localSpacePressDirection = new Vector3(0, 0, 1);
                return transform.TransformDirection(localSpacePressDirection);
            }
        }

        #endregion

        private void Awake()
        {
            boxCollider = gameObject.EnsureComponent<BoxCollider>();
        }

        private void OnEnable()
        {
            boxCollider.enabled = true;
        }

        private void OnDisable()
        {
            IsTouching = false;
            boxCollider.enabled = false;
        }

        private void Start()
        {
            if (gameObject.layer == 2)
            {
                Debug.LogWarning("PhysicalButtonMovement will not work if game object layer is set to 'Ignore Raycast'.");
            }
        }

        private void Update()
        {
            IsTouching = (touchPoints.Count != 0);

            if (IsTouching)
            {
                float previousPushDistance = currentPushDistance;
                currentPushDistance = GetFarthestPushDistanceAlongButtonAxis();
                UpdateMovingVisualsPosition();

                // Hand Press is only allowed to happen while touching.
                UpdatePressedState(currentPushDistance, previousPushDistance);
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
            else
            {
                ClearPathMarkers();
            }

            touchPoints.Clear();
        }

        ///<summary>
        /// Handles drawing some editor visual elements to give you an idea of the movement and size of the button.
        ///</summary>
        void OnDrawGizmos()
        {
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                Vector3 worldPressDirection = WorldSpacePressDirection;

                Vector3 boundsCenter = collider.bounds.center;
                Vector3 startPoint;
                if (movingButtonVisuals != null)
                {
                    startPoint = movingButtonVisuals.transform.position;
                }
                else
                {
                    startPoint = transform.position;
                }
                float distance;
                startPoint = ProjectPointToRay(boundsCenter, worldPressDirection, startPoint, out distance);

                Vector3 endPoint = startPoint + worldPressDirection * maxPushDistance;
                Vector3 pushedPoint = startPoint + worldPressDirection * currentPushDistance;

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(startPoint, pushedPoint);
                Vector3 lastPoint = pushedPoint;

                float releaseDistance = pressDistance - releaseDistanceDelta;
                if (releaseDistance > currentPushDistance)
                {
                    Gizmos.color = Color.yellow;
                    Vector3 releasePoint = startPoint + worldPressDirection * releaseDistance;
                    Gizmos.DrawLine(lastPoint, releasePoint);
                    lastPoint = releasePoint;
                }

                if (pressDistance > currentPushDistance)
                {
                    Gizmos.color = Color.cyan;
                    Vector3 pressPoint = startPoint + worldPressDirection * pressDistance;
                    Gizmos.DrawLine(lastPoint, pressPoint);
                    lastPoint = pressPoint;
                }

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(lastPoint, endPoint);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(endPoint, endPoint + transform.rotation * Vector3.up * collider.bounds.extents.y);
                Gizmos.DrawLine(endPoint, endPoint - transform.rotation * Vector3.up * collider.bounds.extents.y);
                Gizmos.DrawLine(endPoint, endPoint + transform.rotation * Vector3.right * collider.bounds.extents.x);
                Gizmos.DrawLine(endPoint, endPoint - transform.rotation * Vector3.right * collider.bounds.extents.x);
            }
        }

        #region OnTouch

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            touchPoints.Add(eventData.InputData);

            if (initialTransform == null)
            {
                SetPathMarkers();
                // Make sure to initialize currentPushDistance now to correctly handle back-presses in
                // HandlePressProgress().
                currentPushDistance = GetFarthestPushDistanceAlongButtonAxis();
            }

            // Pulse each proximity light on pointer cursors's interacting with this button.
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
            touchPoints.Add(eventData.InputData);
            eventData.Use();
        }
        
        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            eventData.Use();
        }
        
        #endregion OnTouch

        #region private Methods

        private void SetPathMarkers()
        {
            GameObject initialMarker = new GameObject("Initial");
            initialMarker.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

            if (movingButtonVisuals != null)
            {
                initialMarker.transform.position = movingButtonVisuals.transform.position;
                initialMarker.transform.parent = movingButtonVisuals.transform.parent;
            }
            else
            {
                initialMarker.transform.position = transform.position;
                initialMarker.transform.parent = transform.parent;
            }

            initialTransform = initialMarker.transform;
        }

        private void ClearPathMarkers()
        {
            if (initialTransform != null)
            {
                initialTransform.parent = null;
                DestroyImmediate(initialTransform.gameObject);
                initialTransform = null;
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
            foreach (Vector3 p in touchPoints)
            {
                float testDistance = GetProjectedDistance(initialTransform.position, WorldSpacePressDirection, p);
                farthestDistance = Mathf.Max(farthestDistance, testDistance);
            }

            return Mathf.Clamp(farthestDistance, 0.0f, maxPushDistance);
        }

        private void UpdatePressedState(float pushDistance, float previousPushDistance)
        {
            // If we aren't in a press and can't start a simple one.
            if (!isPressing)
            {
                // Compare to our previous push depth. Use previous push distance to handle back-presses.
                if (pushDistance >= pressDistance && previousPushDistance < pressDistance)
                {
                    isPressing = true;
                    ButtonPressed.Invoke();
                }
            }
            // If we're in a press, check if the press is released now.
            else
            {
                float releaseDistance = pressDistance - releaseDistanceDelta;
                if (pushDistance <= releaseDistance && previousPushDistance > releaseDistance)
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