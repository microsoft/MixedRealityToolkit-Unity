//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Input;
using System;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Slider with a thumb (a GameObject), endpoints are defined by two GameObjects
    /// </summary>
    public class PinchSlider : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler
    {

        #region Serialized Fields and Properties
        [Tooltip("The gameObject that contains the slider thumb.")]
        [SerializeField]
        private GameObject thumbRoot = null;

        [Range(0, 1)]
        [SerializeField]
        private float sliderValue = 0.5f;
        public float SliderValue
        {
            get { return sliderValue; }
            set
            {
                var oldSliderValue = sliderValue;
                sliderValue = value;
                UpdateUI();
                OnValueUpdated.Invoke(new SliderEventData(oldSliderValue, value, activePointer is IMixedRealityPointer));
            }
        }

        [Header("Slider Track")]

        [Tooltip("The axis the slider moves along")]
        [SerializeField]
        private SliderAxis sliderAxis = SliderAxis.XAxis;
        [Serializable]
        /// <summary>
        /// The axis the slider moves along
        /// </summary>
        private enum SliderAxis
        {
            XAxis = 0,
            YAxis,
            ZAxis
        }

        [SerializeField]
        [Tooltip("Where the slider track starts, in local coordinates")]
        private Vector3 sliderStartPosition = new Vector3(-0.5f, 0, 0);
        [SerializeField]
        [Tooltip("Where the slider track ends, in local coordinates")]
        private Vector3 sliderEndPosition = new Vector3(0.5f, 0, 0);

        /// <summary>
        /// Gets the start position of the slider, in world space, or zero if invalid.
        /// Sets the start position of the slider, in world space, projected to the slider's axis.
        /// </summary>
        public Vector3 SliderStartPosition
        {
            get { return transform.TransformPoint(sliderStartPosition); }
            set { sliderStartPosition = Vector3.Project(transform.InverseTransformPoint(value), GetSliderAxis()); }
        }

        /// <summary>
        /// Gets the end position of the slider, in world space, or zero if invalid.
        /// Sets the end position of the slider, in world space, projected to the slider's axis.
        /// </summary>
        public Vector3 SliderEndPosition
        {
            get { return transform.TransformPoint(sliderEndPosition); }
            set { sliderEndPosition = Vector3.Project(transform.InverseTransformPoint(value), GetSliderAxis()); }
        }
        #endregion

        #region Event Handlers
        [Header("Events")]
        [Tooltip("Invoked when the component starts running")]
        public SliderEvent OnStartRunning;
        public SliderEvent OnValueUpdated;
        public SliderEvent OnInteractionStarted;
        public SliderEvent OnInteractionEnded;
        public SliderEvent OnHoverEntered;
        public SliderEvent OnHoverExited;
        #endregion

        #region Private Members
        private float startSliderValue;
        private Vector3 startPointerPosition;
        private Vector3 startSliderPosition;
        private IMixedRealityPointer activePointer;
        #endregion

        #region Unity methods
        public void Start()
        {
            UpdateUI();
            OnStartRunning.Invoke(new SliderEventData(sliderValue, sliderValue, false));
        }

        private void OnDisable()
        {
            if (activePointer != null)
            {
                EndInteraction();
            }
        }
        #endregion

        #region Private Methods
        private Vector3 GetSliderAxis()
        {
            switch (sliderAxis)
            {
                case SliderAxis.XAxis:
                    return Vector3.right;
                case SliderAxis.YAxis:
                    return Vector3.up;
                case SliderAxis.ZAxis:
                    return Vector3.forward;
                default:
                    throw new ArgumentOutOfRangeException("Invalid slider axis");
            }
        }

        private void UpdateUI()
        {
            var p0ToP1 = SliderEndPosition - SliderStartPosition;
            var newSliderPos = SliderStartPosition + p0ToP1 * sliderValue;

            thumbRoot.transform.position = newSliderPos;
        }

        private void EndInteraction()
        {
            if (OnInteractionEnded != null)
            {
                OnInteractionEnded.Invoke(new SliderEventData(sliderValue, sliderValue, activePointer is IMixedRealityPointer));
            }
            activePointer = null;
        }

        #endregion

        #region IMixedRealityFocusHandler
        public void OnFocusEnter(FocusEventData eventData)
        {
            OnHoverEntered.Invoke(new SliderEventData(sliderValue, sliderValue, eventData.Pointer is IMixedRealityNearPointer));
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            OnHoverExited.Invoke(new SliderEventData(sliderValue, sliderValue, eventData.Pointer is IMixedRealityNearPointer));
        }
        #endregion

        #region IMixedRealityPointerHandler

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer)
            {
                EndInteraction();
            }
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (activePointer == null)
            {
                activePointer = eventData.Pointer;
                startSliderValue = sliderValue;
                startPointerPosition = activePointer.Position;
                startSliderPosition = gameObject.transform.position;
                if (OnInteractionStarted != null)
                {
                    OnInteractionStarted.Invoke(new SliderEventData(sliderValue, sliderValue, activePointer is IMixedRealityPointer));
                }
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer)
            {
                var delta = activePointer.Position - startPointerPosition;
                var p0ToP1 = SliderEndPosition - SliderStartPosition;
                var handDelta = Vector3.Dot(p0ToP1.normalized, delta);

                SliderValue = Mathf.Clamp(startSliderValue + handDelta / p0ToP1.magnitude, 0, 1);
            }
        }
        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
        #endregion
    }
}
