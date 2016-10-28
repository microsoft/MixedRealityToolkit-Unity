//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

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
        private GestureRecognizer gestureRecognizer;

        public override SupportedInputEvents SupportedEvents
        {
            get
            {
                return SupportedInputEvents.SourceClicked |
                        SupportedInputEvents.Hold |
                        SupportedInputEvents.Manipulation;
            }
        }

        public override SupportedInputInfo SupportedInputInfo
        {
            get { return SupportedInputInfo.None; }
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
            
            gestureRecognizer.StartCapturingGestures();
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
            RaiseSourceClickedEvent(0);
        }

        private void OnHoldStartedEvent(InteractionSourceKind source, Ray headray)
        {
            RaiseHoldStartedEvent(0);
        }

        private void OnHoldCanceledEvent(InteractionSourceKind source, Ray headray)
        {
            RaiseHoldCanceledEvent(0);
        }

        private void OnHoldCompletedEvent(InteractionSourceKind source, Ray headray)
        {
            RaiseHoldCompletedEvent(0);
        }

        private void OnManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            RaiseManipulationStartedEvent(0, cumulativeDelta);
        }

        private void OnManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            RaiseManipulationUpdatedEvent(0, cumulativeDelta);
        }

        private void OnManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            RaiseManipulationCompletedEvent(0, cumulativeDelta);
        }

        private void OnManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headray)
        {
            RaiseManipulationCanceledEvent(0, cumulativeDelta);
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
    }
}
