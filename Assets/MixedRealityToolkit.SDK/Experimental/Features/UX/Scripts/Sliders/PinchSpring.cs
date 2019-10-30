// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// TODO
    /// </summary>
    [RequireComponent(typeof(NearInteractionGrabbable))]
    public class PinchSpring : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler
    {
        #region Serialized Fields and Properties

        [SerializeField, Tooltip("TODO")]
        private Transform handleRoot = null;

        /// <summary>
        /// TODO
        /// </summary>
        public Transform HandleRoot
        {
            get => handleRoot;
            set => handleRoot = value;
        }

        [SerializeField, Tooltip("TODO")]
        private Transform handleConnector = null;

        /// <summary>
        /// TODO
        /// </summary>
        public Transform HandleConnector
        {
            get => handleConnector;
            set => handleConnector = value;
        }

        [SerializeField, Tooltip("TODO")]
        private Transform handleTip = null;

        /// <summary>
        /// TODO
        /// </summary>
        public Transform HandleTip
        {
            get => handleTip;
            set
            {
                handleTip = value;

                if (handleTip != null)
                {
                    tipScaleHander = handleTip.GetComponent<TransformScaleHandler>();
                }
            }
        }

        [SerializeField, Tooltip("TODO")]
        private float restingDistance = 0.05f;

        /// <summary>
        /// TODO
        /// </summary>
        public float RestingDistance
        {
            get => restingDistance;
            set => restingDistance = value;
        }

        [SerializeField, Tooltip("TODO")]
        private Vector3 restingDirection = new Vector3(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// TODO
        /// </summary>
        public Vector3 RestingDirection
        {
            get => restingDirection;
            set
            {
                restingDirection = value;
                restingDirection.Normalize();
            }
        }

        [SerializeField, Tooltip("TODO")]
        private float pointMass = 0.05f;

        /// <summary>
        /// TODO
        /// </summary>
        public float PointMass
        {
            get => pointMass;
            set => pointMass = value;
        }

        [SerializeField, Tooltip("TODO")]
        private float springStiffness = 50.0f;

        /// <summary>
        /// TODO
        /// </summary>
        public float SpringStiffness
        {
            get => springStiffness;
            set => springStiffness = value;
        }

        [SerializeField, Range(0.0f, 1.0f), Tooltip("TODO")]
        private float springDampening = 0.9f;

        /// <summary>
        /// TODO
        /// </summary>
        public float SpringDampening
        {
            get => springDampening;
            set => springDampening = value;
        }

        [SerializeField, Tooltip("TODO")]
        private float snapDistance = 0.03f;

        /// <summary>
        /// TODO
        /// </summary>
        public float SnapDistance
        {
            get => snapDistance;
            set => snapDistance = value;
        }

        [SerializeField, Tooltip("TODO")]
        private float handleTipInterpolateSpeed = 20.0f;

        /// <summary>
        /// TODO
        /// </summary>
        public float HandleTipInterpolateSpeed
        {
            get => handleTipInterpolateSpeed;
            set => handleTipInterpolateSpeed = value;
        }

        #endregion

        #region Private Members

        private TransformScaleHandler tipScaleHander = null; 
        private IMixedRealityPointer manipulatePointer = null;
        private IMixedRealityNearPointer focusedPointer = null;
        private Vector3 velocity = Vector3.zero;
        private bool snapped = false;

        #endregion

        #region MonoBehaviour Implementation

        private void Start()
        {
            restingDirection.Normalize();

            if (handleTip != null)
            {
                tipScaleHander = handleTip.GetComponent<TransformScaleHandler>();
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // Move the handle tip towards the interacting pointer, else spring back to the resting location.
            var currentPosition = handleTip.position;
            var restingPosition = handleRoot.position + (transform.rotation * restingDirection) * restingDistance;

            Vector3 targetPosition;
            Vector3 graspPosition;

            if (focusedPointer != null && focusedPointer.TryGetNearGraspPoint(out graspPosition))
            {
                if (snapped || ((currentPosition - graspPosition).magnitude < snapDistance))
                {
                    // Snap to the grasp position.
                    targetPosition = graspPosition;
                    snapped = true;
                }
                else
                {
                    // Interpolate to the snap position.
                    targetPosition = Vector3.Lerp(currentPosition, graspPosition, handleTipInterpolateSpeed * deltaTime);
                }
            }
            else
            {
                // Spring back to the resting position using Hooke's Law.
                var delta = restingPosition - currentPosition;
                var deltaMagnitude = delta.magnitude;

                if (deltaMagnitude != 0.0f)
                {
                    delta /= deltaMagnitude;

                    // Integrate the point mass.
                    var force = delta * (springStiffness * deltaMagnitude);
                    var acceleration = force / pointMass;

                    velocity += acceleration * deltaTime;
                    targetPosition = currentPosition + (velocity * deltaTime);

                    velocity *= springDampening;
                }
                else
                {
                    targetPosition = restingPosition;
                }

                snapped = false;
            }

            handleTip.position = targetPosition;

            // Update the tip scale.
            if (tipScaleHander != null)
            {
                var tagetScale = (manipulatePointer != null) ? Vector3.one * tipScaleHander.ScaleMinimum : Vector3.one * tipScaleHander.ScaleMaximum;
                handleTip.localScale = Vector3.Lerp(handleTip.localScale, tagetScale, handleTipInterpolateSpeed * deltaTime);
            }

            // Update the connector.
            if (handleConnector != null)
            {
                var delta = handleTip.position - handleRoot.position;
                handleConnector.position = (handleRoot.position + handleTip.position) * 0.5f;
                handleConnector.rotation = Quaternion.LookRotation(delta, handleTip.up);
                var scale = handleConnector.localScale;
                handleConnector.localScale = new Vector3(scale.x, scale.y, delta.magnitude);
            }
        }

        #endregion


        #region IMixedRealityFocusHandler Implementation

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerDownEventHandler =
        delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
        {
            var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
            handler.OnPointerDown(casted);
        };

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            manipulatePointer = eventData.Pointer;

            // Continue to forward the events upward because this component is a passive observer.
            if (!eventData.used && gameObject.transform.parent)
            {
                ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, OnPointerDownEventHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerDraggedEventHandler =
        delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
        {
            var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
            handler.OnPointerDragged(casted);
        };

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // Continue to forward the events upward because this component is a passive observer.
            if (!eventData.used && gameObject.transform.parent)
            {
                ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, OnPointerDraggedEventHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnPointerUpEventHandler =
        delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
        {
            var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
            handler.OnPointerUp(casted);
        };

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            manipulatePointer = null;

            // Continue to forward the events upward because this component is a passive observer.
            if (!eventData.used && gameObject.transform.parent)
            {
                ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, OnPointerUpEventHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityPointerHandler> OnInputClickedEventHandler =
        delegate (IMixedRealityPointerHandler handler, BaseEventData eventData)
        {
            var casted = ExecuteEvents.ValidateEventData<MixedRealityPointerEventData>(eventData);
            handler.OnPointerClicked(casted);
        };

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            // Continue to forward the events upward because this component is a passive observer.
            if (!eventData.used && gameObject.transform.parent)
            {
                ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, OnInputClickedEventHandler);
            }
        }

        #endregion

        #region IMixedRealityFocusHandler Implementation

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusHandler> OnFocusEnterEventHandler =
        delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
        {
            var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
            handler.OnFocusEnter(casted);
        };

        public void OnFocusEnter(FocusEventData eventData)
        {
            focusedPointer = eventData.Pointer as IMixedRealityNearPointer;

            // Continue to forward the events upward because this component is a passive observer.
            if (!eventData.used && gameObject.transform.parent)
            {
                ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, OnFocusEnterEventHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IMixedRealityFocusHandler> OnFocusExitEventHandler =
        delegate (IMixedRealityFocusHandler handler, BaseEventData eventData)
        {
            var casted = ExecuteEvents.ValidateEventData<FocusEventData>(eventData);
            handler.OnFocusExit(casted);
        };

        public void OnFocusExit(FocusEventData eventData)
        {
            focusedPointer = null;

            // Continue to forward the events upward because this component is a passive observer.
            if (!eventData.used && gameObject.transform.parent)
            {
                ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, OnFocusExitEventHandler);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
