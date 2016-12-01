// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for gestures information from the WSA APIs, which gives access to various system-supported gestures.
    /// This is a wrapper on top of GestureRecognizer.
    /// </summary>
    public class GesturesInput : BaseInputSource
    {
        [Tooltip("Set to true to use the use rails (guides) for the navigation gesture, as opposed to full 3D navigation.")]
        public bool UseRailsNavigation = false;

        private GestureRecognizer gestureRecognizer;
        private GestureRecognizer navigationGestureRecognizer;

        public override SupportedInputEvents SupportedEvents
        {
            get
            {
                return SupportedInputEvents.SourceClicked |
                        SupportedInputEvents.Hold |
                        SupportedInputEvents.Manipulation |
                        SupportedInputEvents.Navigation;
            }
        }

        private void Awake()
        {
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
            gestureRecognizer.StartCapturingGestures();

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
            navigationGestureRecognizer.StartCapturingGestures();
        }

        protected override void OnDestroy()
        {
            if (gestureRecognizer != null)
            {
                gestureRecognizer.StopCapturingGestures();
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

            base.OnDestroy();
        }

        private void OnTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            SourceClickEventArgs args = new SourceClickEventArgs(this, 0, tapCount);
            RaiseSourceClickedEvent(args);
        }

        private void OnHoldStartedEvent(InteractionSourceKind source, Ray headray)
        {
            HoldEventArgs args = new HoldEventArgs(this, 0);
            RaiseHoldStartedEvent(args);
        }

        private void OnHoldCanceledEvent(InteractionSourceKind source, Ray headray)
        {
            HoldEventArgs args = new HoldEventArgs(this, 0);
            RaiseHoldCanceledEvent(args);
        }

        private void OnHoldCompletedEvent(InteractionSourceKind source, Ray headray)
        {
            HoldEventArgs args = new HoldEventArgs(this, 0);
            RaiseHoldCompletedEvent(args);
        }

        private void OnManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            ManipulationEventArgs args = new ManipulationEventArgs(this, 0, cumulativeDelta);
            RaiseManipulationStartedEvent(args);
        }

        private void OnManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            ManipulationEventArgs args = new ManipulationEventArgs(this, 0, cumulativeDelta);
            RaiseManipulationUpdatedEvent(args);
        }

        private void OnManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            ManipulationEventArgs args = new ManipulationEventArgs(this, 0, cumulativeDelta);
            RaiseManipulationCompletedEvent(args);
        }

        private void OnManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            ManipulationEventArgs args = new ManipulationEventArgs(this, 0, cumulativeDelta);
            RaiseManipulationCanceledEvent(args);
        }

        private void OnNavigationStartedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            NavigationEventArgs args = new NavigationEventArgs(this, 0, normalizedOffset);
            RaiseNavigationStartedEvent(args);
        }

        private void OnNavigationUpdatedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            NavigationEventArgs args = new NavigationEventArgs(this, 0, normalizedOffset);
            RaiseNavigationUpdatedEvent(args);
        }

        private void OnNavigationCompletedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            NavigationEventArgs args = new NavigationEventArgs(this, 0, normalizedOffset);
            RaiseNavigationCompletedEvent(args);
        }

        private void OnNavigationCanceledEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headray)
        {
            NavigationEventArgs args = new NavigationEventArgs(this, 0, normalizedOffset);
            RaiseNavigationCanceledEvent(args);
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
