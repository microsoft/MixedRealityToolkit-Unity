// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    /// <summary>
    /// Input source supporting input from gesture recognizer.
    /// </summary>
    public class GestureInputSource : BaseGenericInputSource
    {
        /// <summary>
        /// Pointer Action for Gesture Tap or "Click"
        /// </summary>
        public MixedRealityInputAction PointerAction { get; set; } = MixedRealityInputAction.None;

        /// <summary>
        /// Hold action to use when events are raised.
        /// </summary>
        public MixedRealityInputAction HoldAction { get; set; } = MixedRealityInputAction.None;

        /// <summary>
        /// Manipulation action to use when events are raised.
        /// </summary>
        public MixedRealityInputAction ManipulationAction { get; set; } = MixedRealityInputAction.None;

        /// <summary>
        /// Navigation action to use when events are raised.
        /// </summary>
        public MixedRealityInputAction NavigationAction { get; set; } = MixedRealityInputAction.None;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GestureInputSource(bool useRailsNavigation) : base("Gesture Input Source")
        {
            gestureRecognizer = new GestureRecognizer();

            gestureRecognizer.Tapped += GestureRecognizer_Tapped;

            gestureRecognizer.HoldStarted += GestureRecognizer_HoldStarted;
            gestureRecognizer.HoldCompleted += GestureRecognizer_HoldCompleted;
            gestureRecognizer.HoldCanceled += GestureRecognizer_HoldCanceled;

            gestureRecognizer.ManipulationStarted += GestureRecognizer_ManipulationStarted;
            gestureRecognizer.ManipulationUpdated += GestureRecognizer_ManipulationUpdated;
            gestureRecognizer.ManipulationCompleted += GestureRecognizer_ManipulationCompleted;
            gestureRecognizer.ManipulationCanceled += GestureRecognizer_ManipulationCanceled;

            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.ManipulationTranslate | GestureSettings.Hold);

            // We need a separate gesture recognizer for navigation, since it isn't compatible with manipulation
            navigationGestureRecognizer = new GestureRecognizer();

            navigationGestureRecognizer.NavigationStarted += NavigationGestureRecognizer_NavigationStarted;
            navigationGestureRecognizer.NavigationUpdated += NavigationGestureRecognizer_NavigationUpdated;
            navigationGestureRecognizer.NavigationCompleted += NavigationGestureRecognizer_NavigationCompleted;
            navigationGestureRecognizer.NavigationCanceled += NavigationGestureRecognizer_NavigationCanceled;

            if (useRailsNavigation)
            {
                navigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationRailsX | GestureSettings.NavigationRailsY | GestureSettings.NavigationRailsZ);
            }
            else
            {
                navigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationX | GestureSettings.NavigationY | GestureSettings.NavigationZ);
            }

            if (gestureRecognizer != null && !gestureRecognizer.IsCapturingGestures())
            {
                gestureRecognizer.StartCapturingGestures();
            }

            if (navigationGestureRecognizer != null && !navigationGestureRecognizer.IsCapturingGestures())
            {
                navigationGestureRecognizer.StartCapturingGestures();
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (gestureRecognizer != null && gestureRecognizer.IsCapturingGestures())
            {
                gestureRecognizer.StopCapturingGestures();
            }

            if (navigationGestureRecognizer != null && navigationGestureRecognizer.IsCapturingGestures())
            {
                navigationGestureRecognizer.StopCapturingGestures();
            }

            if (gestureRecognizer != null)
            {
                gestureRecognizer.Tapped -= GestureRecognizer_Tapped;

                gestureRecognizer.HoldStarted -= GestureRecognizer_HoldStarted;
                gestureRecognizer.HoldCompleted -= GestureRecognizer_HoldCompleted;
                gestureRecognizer.HoldCanceled -= GestureRecognizer_HoldCanceled;

                gestureRecognizer.ManipulationStarted -= GestureRecognizer_ManipulationStarted;
                gestureRecognizer.ManipulationUpdated -= GestureRecognizer_ManipulationUpdated;
                gestureRecognizer.ManipulationCompleted -= GestureRecognizer_ManipulationCompleted;
                gestureRecognizer.ManipulationCanceled -= GestureRecognizer_ManipulationCanceled;

                gestureRecognizer.Dispose();
            }

            if (navigationGestureRecognizer != null)
            {
                navigationGestureRecognizer.NavigationStarted -= NavigationGestureRecognizer_NavigationStarted;
                navigationGestureRecognizer.NavigationUpdated -= NavigationGestureRecognizer_NavigationUpdated;
                navigationGestureRecognizer.NavigationCompleted -= NavigationGestureRecognizer_NavigationCompleted;
                navigationGestureRecognizer.NavigationCanceled -= NavigationGestureRecognizer_NavigationCanceled;

                navigationGestureRecognizer.Dispose();
            }
        }

        private readonly GestureRecognizer gestureRecognizer;
        private readonly GestureRecognizer navigationGestureRecognizer;

        #region Raise GestureRecognizer Events

        private void GestureRecognizer_Tapped(TappedEventArgs args)
        {
            InputSystem.RaisePointerClicked(InputSystem.GazeProvider.GazePointer, (Handedness)args.source.handedness, PointerAction, args.tapCount);
        }

        private void GestureRecognizer_HoldStarted(HoldStartedEventArgs args)
        {
            InputSystem.RaiseHoldStarted(this, (Handedness)args.source.handedness, HoldAction);
        }

        private void GestureRecognizer_HoldCanceled(HoldCanceledEventArgs args)
        {
            InputSystem.RaiseHoldCanceled(this, (Handedness)args.source.handedness, HoldAction);
        }

        private void GestureRecognizer_HoldCompleted(HoldCompletedEventArgs args)
        {
            InputSystem.RaiseHoldCompleted(this, (Handedness)args.source.handedness, HoldAction);
        }

        private void GestureRecognizer_ManipulationStarted(ManipulationStartedEventArgs args)
        {
            InputSystem.RaiseManipulationStarted(this, (Handedness)args.source.handedness, ManipulationAction);
        }

        private void GestureRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs args)
        {
            InputSystem.RaiseManipulationUpdated(this, (Handedness)args.source.handedness, ManipulationAction, args.cumulativeDelta);
        }

        private void GestureRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            InputSystem.RaiseManipulationCompleted(this, (Handedness)args.source.handedness, ManipulationAction, args.cumulativeDelta);
        }

        private void GestureRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs args)
        {
            InputSystem.RaiseManipulationCanceled(this, (Handedness)args.source.handedness, ManipulationAction);
        }

        private void NavigationGestureRecognizer_NavigationStarted(NavigationStartedEventArgs args)
        {
            InputSystem.RaiseNavigationStarted(this, (Handedness)args.source.handedness, NavigationAction);
        }

        private void NavigationGestureRecognizer_NavigationUpdated(NavigationUpdatedEventArgs args)
        {
            InputSystem.RaiseNavigationUpdated(this, (Handedness)args.source.handedness, NavigationAction, args.normalizedOffset);
        }

        private void NavigationGestureRecognizer_NavigationCompleted(NavigationCompletedEventArgs args)
        {
            InputSystem.RaiseNavigationCompleted(this, (Handedness)args.source.handedness, NavigationAction, args.normalizedOffset);
        }

        private void NavigationGestureRecognizer_NavigationCanceled(NavigationCanceledEventArgs args)
        {
            InputSystem.RaiseNavigationCanceled(this, (Handedness)args.source.handedness, NavigationAction);
        }

        #endregion Raise GestureRecognizer Events
    }
}
