// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for gestures information from the WSA APIs, which gives access to various system-supported gestures.
    /// This is a wrapper on top of GestureRecognizer.
    /// </summary>
    public class GesturesInput : BaseInputSource
    {
        // This enumeration gives the manager two different ways to handle the recognizer. Both will
        // set up the recognizer. The first causes the recognizer to start
        // immediately. The second allows the recognizer to be manually started at a later time.
        public enum RecognizerStartBehavior { AutoStart, ManualStart };

        [Tooltip("Whether the recognizer should be activated on start.")]
        public RecognizerStartBehavior RecognizerStart;

        [Tooltip("Set to true to use the use rails (guides) for the navigation gesture, as opposed to full 3D navigation.")]
        public bool UseRailsNavigation = false;

        protected GestureRecognizer gestureRecognizer;
        protected GestureRecognizer navigationGestureRecognizer;

        protected override void Start()
        {
            base.Start();

            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.TappedEvent += OnTappedEvent;
            
            gestureRecognizer.HoldStartedEvent += OnHoldStartedEvent;
            gestureRecognizer.HoldCompletedEvent += OnHoldCompletedEvent;
            gestureRecognizer.HoldCanceledEvent += OnHoldCanceledEvent;

            gestureRecognizer.ManipulationStartedEvent += OnManipulationStartedEvent;
            gestureRecognizer.ManipulationUpdatedEvent += OnManipulationUpdatedEvent;
            gestureRecognizer.ManipulationCompletedEvent += OnManipulationCompletedEvent;
            gestureRecognizer.ManipulationCanceledEvent += OnManipulationCanceledEvent;

            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | 
                                                      GestureSettings.ManipulationTranslate |
                                                      GestureSettings.Hold);

            // We need a separate gesture recognizer for navigation, since it isn't compatible with manipulation
            navigationGestureRecognizer = new GestureRecognizer();

            navigationGestureRecognizer.NavigationStartedEvent += OnNavigationStartedEvent;
            navigationGestureRecognizer.NavigationUpdatedEvent += OnNavigationUpdatedEvent;
            navigationGestureRecognizer.NavigationCompletedEvent += OnNavigationCompletedEvent;
            navigationGestureRecognizer.NavigationCanceledEvent += OnNavigationCanceledEvent;

            if (UseRailsNavigation)
            {
                navigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationRailsX |
                                                                    GestureSettings.NavigationRailsY |
                                                                    GestureSettings.NavigationRailsZ);
            }
            else
            {
                navigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationX |
                                                                    GestureSettings.NavigationY |
                                                                    GestureSettings.NavigationZ);
            }

            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                gestureRecognizer.StartCapturingGestures();
                navigationGestureRecognizer.StartCapturingGestures();
            }
        }

        protected virtual void OnDestroy()
        {
            StopGestureRecognizer();
            if (gestureRecognizer != null)
            {
                gestureRecognizer.TappedEvent -= OnTappedEvent;

                gestureRecognizer.HoldStartedEvent -= OnHoldStartedEvent;
                gestureRecognizer.HoldCompletedEvent -= OnHoldCompletedEvent;
                gestureRecognizer.HoldCanceledEvent -= OnHoldCanceledEvent;

                gestureRecognizer.ManipulationStartedEvent -= OnManipulationStartedEvent;
                gestureRecognizer.ManipulationUpdatedEvent -= OnManipulationUpdatedEvent;
                gestureRecognizer.ManipulationCompletedEvent -= OnManipulationCompletedEvent;
                gestureRecognizer.ManipulationCanceledEvent -= OnManipulationCanceledEvent;

                gestureRecognizer.Dispose();
            }
            if (navigationGestureRecognizer != null)
            {
                navigationGestureRecognizer.NavigationStartedEvent -= OnNavigationStartedEvent;
                navigationGestureRecognizer.NavigationUpdatedEvent -= OnNavigationUpdatedEvent;
                navigationGestureRecognizer.NavigationCompletedEvent -= OnNavigationCompletedEvent;
                navigationGestureRecognizer.NavigationCanceledEvent -= OnNavigationCanceledEvent;

                navigationGestureRecognizer.Dispose();
            }
        }

        protected virtual void OnDisable()
        {
            StopGestureRecognizer();
        }

        protected virtual void OnEnable()
        {
            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartGestureRecognizer();
            }
        }

        /// <summary>
        /// Make sure the gesture recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StartGestureRecognizer()
        {
            if (gestureRecognizer != null && !gestureRecognizer.IsCapturingGestures())
            {
                gestureRecognizer.StartCapturingGestures();
            }
            if (navigationGestureRecognizer != null && !navigationGestureRecognizer.IsCapturingGestures())
            {
                navigationGestureRecognizer.StartCapturingGestures();
            }
        }

        /// <summary>
        /// Make sure the gesture recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StopGestureRecognizer()
        {
            if (gestureRecognizer != null && gestureRecognizer.IsCapturingGestures())
            {
                gestureRecognizer.StopCapturingGestures();
            }
            if (navigationGestureRecognizer != null && navigationGestureRecognizer.IsCapturingGestures())
            {
                navigationGestureRecognizer.StopCapturingGestures();
            }
        }

        protected void OnTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            inputManager.RaiseInputClicked(this, 0, tapCount);
        }

        protected void OnHoldStartedEvent(InteractionSourceKind source, Ray headray)
        {
            inputManager.RaiseHoldStarted(this, 0);
        }

        protected void OnHoldCanceledEvent(InteractionSourceKind source, Ray headray)
        {
            inputManager.RaiseHoldCanceled(this, 0);
        }

        protected void OnHoldCompletedEvent(InteractionSourceKind source, Ray headray)
        {
            inputManager.RaiseHoldCompleted(this, 0);
        }

        protected void OnManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            inputManager.RaiseManipulationStarted(this, 0, cumulativeDelta);
        }

        protected void OnManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            inputManager.RaiseManipulationUpdated(this, 0, cumulativeDelta);
        }

        protected void OnManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            inputManager.RaiseManipulationCompleted(this, 0, cumulativeDelta);
        }

        protected void OnManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            inputManager.RaiseManipulationCanceled(this, 0, cumulativeDelta);
        }

        protected void OnNavigationStartedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            inputManager.RaiseNavigationStarted(this, 0, normalizedOffset);
        }

        protected void OnNavigationUpdatedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            inputManager.RaiseNavigationUpdated(this, 0, normalizedOffset);
        }

        protected void OnNavigationCompletedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            inputManager.RaiseNavigationCompleted(this, 0, normalizedOffset);
        }

        protected void OnNavigationCanceledEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            inputManager.RaiseNavigationCanceled(this, 0, normalizedOffset);
        }

        public override bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetOrientation(uint sourceId, out Quaternion orientation)
        {
            orientation = Quaternion.identity;
            return false;
        }

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            return SupportedInputInfo.None;
        }
    }
}
