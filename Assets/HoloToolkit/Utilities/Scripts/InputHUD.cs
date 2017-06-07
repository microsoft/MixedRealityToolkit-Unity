// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.Input;
using System.Collections;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// InputHUD provides a live HUD monitor for controller and hand inputs for the purpose of debugging multiple input modalities. 
    /// </summary>
    public class InputHUD : MonoBehaviour
    {
        #region Variables
        [Tooltip("The InteractionSourceKind of the input modality to be monitored using this HUD")]
        public InteractionSourceKind sourceTypeToMonitor;

        /// <summary>
        /// Determines the amount of time to turn on the 
        /// </summary>
        private float tapIndicationDuration = 0.3f;

        /// <summary>
        /// The local GestureRecognizer to intercept triggered gesture events
        /// </summary>
        public GestureRecognizer gestureRecognizer;

        /// <summary>
        /// The toggle box used to display the state of the tap input from the monitored InteractionSourceKind
        /// </summary>
        public Toggle tapToggle;

        /// <summary>
        /// The slider used to display x-axis input data from the monitored InteractionSourceKind
        /// </summary>
        public Slider xAxisSlider;

        /// <summary>
        /// The slider used to display y-axis input data from the monitored InteractionSourceKind
        /// </summary>
        public Slider yAxisSlider;
        #endregion

        #region Runtime Code
        /// <summary>
        /// Setup for the display toggle and sliders, as well as instantiation of the gesture recognizer and subscription of 
        /// all gestureRecognizer events 
        /// </summary>
        void Start()
        {
            // Assign the Toggle and Slider components from the children to this script
            tapToggle = this.GetComponentInChildren<Toggle>();
            xAxisSlider = this.GetComponentsInChildren<Slider>()[0];
            yAxisSlider = this.GetComponentsInChildren<Slider>()[1];

            // Instantiate and subscribe GestureRecognizer input types 
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold | GestureSettings.NavigationX | GestureSettings.NavigationY | GestureSettings.NavigationZ);
            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;
            gestureRecognizer.HoldCompletedEvent += GestureRecognizer_HoldCompletedEvent;
            gestureRecognizer.HoldStartedEvent += GestureRecognizer_HoldStartedEvent;
            gestureRecognizer.NavigationStartedEvent += GestureRecognizer_NavigationStartedEvent;
            gestureRecognizer.NavigationUpdatedEvent += GestureRecognizer_NavigationUpdatedEvent;
            gestureRecognizer.NavigationCompletedEvent += GestureRecognizer_NavigationCompletedEvent;
            gestureRecognizer.NavigationCanceledEvent += GestureRecognizer_NavigationCanceledEvent;

            // Start Capturing Gestures 
            gestureRecognizer.StartCapturingGestures();
        }
        #endregion

        #region Gesture Input Event Handlers
        /// The below event handlers all follow a uniform structure, wherein the InteractionSourceKind of the 
        /// gesture is checked against the sourceTypeToMonitor. If they match, the appropriate HUD element is
        /// updated accordingly. 
        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            if (source != sourceTypeToMonitor)
                return;
            StartCoroutine(TapSeen());
        }

        private void GestureRecognizer_HoldStartedEvent(InteractionSourceKind source, Ray headRay)
        {
            if (source != sourceTypeToMonitor)
                return;
            tapToggle.isOn = true;
        }

        private void GestureRecognizer_HoldCompletedEvent(InteractionSourceKind source, Ray headRay)
        {
            if (source != sourceTypeToMonitor)
                return;
            tapToggle.isOn = false;
        }

        private void GestureRecognizer_NavigationStartedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            if (source != sourceTypeToMonitor)
                return;
            tapToggle.isOn = true;
            xAxisSlider.value = normalizedOffset.x;
            yAxisSlider.value = normalizedOffset.y;
        }

        private void GestureRecognizer_NavigationUpdatedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            if (source != sourceTypeToMonitor)
                return;
            tapToggle.isOn = true;
            xAxisSlider.value = normalizedOffset.x;
            yAxisSlider.value = normalizedOffset.y;
        }

        private void GestureRecognizer_NavigationCompletedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            if (source != sourceTypeToMonitor)
                return;
            tapToggle.isOn = false;
            tapToggle.isOn = true;
            xAxisSlider.value = 0;
            yAxisSlider.value = 0;
        }

        private void GestureRecognizer_NavigationCanceledEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
        {
            if (source != sourceTypeToMonitor)
                return;
            tapToggle.isOn = false;
            tapToggle.isOn = true;
            xAxisSlider.value = 0;
            yAxisSlider.value = 0;
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// A coroutine to leave the TapToggle activated for tapIndicationDuration seconds
        /// </summary>
        /// <returns> No return value</returns>
        IEnumerator TapSeen()
        {
            tapToggle.isOn = true;
            yield return new WaitForSeconds(tapIndicationDuration);
            tapToggle.isOn = false;
        }
        #endregion
    }
}


