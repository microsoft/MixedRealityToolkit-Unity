// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    /// <summary>
    /// Input source supporting input from gesture recognizer.
    /// </summary>
    public class WindowsGestureInputSource : BaseGenericInputSource
    {
        /// <summary>
        /// Pointer Action for Gesture Tap or "Click"
        /// </summary>
        public static MixedRealityInputAction PointerAction { get; set; } = MixedRealityInputAction.None;

        /// <summary>
        /// Hold action to use when events are raised.
        /// </summary>
        public static MixedRealityInputAction HoldAction { get; set; } = MixedRealityInputAction.None;

        /// <summary>
        /// Manipulation action to use when events are raised.
        /// </summary>
        public static MixedRealityInputAction ManipulationAction { get; set; } = MixedRealityInputAction.None;

        /// <summary>
        /// Navigation action to use when events are raised.
        /// </summary>
        public static MixedRealityInputAction NavigationAction { get; set; } = MixedRealityInputAction.None;

        private static bool gestureRecognizerEnabled;

        /// <summary>
        /// Enables or disables the gesture recognizer.
        /// </summary>
        /// <remarks>
        /// Automatically disabled navigation recognizer if enabled.
        /// </remarks>
        public static bool GestureRecognizerEnabled
        {
            get
            {
                return gestureRecognizerEnabled;
            }
            set
            {
                if (gestureRecognizer == null)
                {
                    gestureRecognizerEnabled = false;
                    return;
                }

                gestureRecognizerEnabled = value;

                if (!gestureRecognizer.IsCapturingGestures() && gestureRecognizerEnabled)
                {
                    NavigationRecognizerEnabled = false;
                    gestureRecognizer.StartCapturingGestures();
                }

                if (gestureRecognizer.IsCapturingGestures() && !gestureRecognizerEnabled)
                {
                    gestureRecognizer.StopCapturingGestures();
                }
            }
        }

        private static bool navigationRecognizerEnabled;

        /// <summary>
        /// Enables or disables the navigation recognizer.
        /// </summary>
        /// <remarks>
        /// Automatically disables the gesture recognizer if enabled.
        /// </remarks>
        public static bool NavigationRecognizerEnabled
        {
            get
            {
                return navigationRecognizerEnabled;
            }
            set
            {
                if (navigationGestureRecognizer == null)
                {
                    navigationRecognizerEnabled = false;
                    return;
                }

                navigationRecognizerEnabled = value;

                if (!navigationGestureRecognizer.IsCapturingGestures() && navigationRecognizerEnabled)
                {
                    GestureRecognizerEnabled = false;
                    navigationGestureRecognizer.StartCapturingGestures();
                }

                if (navigationGestureRecognizer.IsCapturingGestures() && !navigationRecognizerEnabled)
                {
                    navigationGestureRecognizer.StopCapturingGestures();
                }
            }
        }

        private static GestureRecognizer gestureRecognizer;
        private static GestureRecognizer navigationGestureRecognizer;

        private static IMixedRealityDeviceManager deviceManager = null;
        private static IMixedRealityDeviceManager DeviceManager => deviceManager ?? (deviceManager = MixedRealityManager.Instance.GetManager(typeof(WindowsMixedRealityDeviceManager)) as IMixedRealityDeviceManager);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="useRailsNavigation">Should the gesture input source use rails navigation?</param>
        public WindowsGestureInputSource(bool useRailsNavigation) : base("Gesture Input Source")
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
        }

        /// <inheritdoc />
        public override void Dispose()
        {
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

        #region Raise GestureRecognizer Events

        private void GestureRecognizer_Tapped(TappedEventArgs args)
        {
            if (args.source.kind == InteractionSourceKind.Hand)
            {
                InputSystem.RaisePointerClicked(InputSystem.GazeProvider.GazePointer, (Handedness)args.source.handedness, PointerAction, args.tapCount);
            }
            else if (args.source.kind == InteractionSourceKind.Controller)
            {
                // TODO Do we even need this?  Shouldn't pressing the trigger in the WMR Controller raise the same event?
                var activeControllers = DeviceManager.GetActiveControllers();

                for (int i = 0; i < activeControllers.Length; i++)
                {
                    var controller = activeControllers[i] as WindowsMixedRealityController;

                    if (controller != null && controller.LastSourceStateReading.source.id == args.source.id)
                    {
                        // TODO How do we want to figure out which pointer to raise the click event? Does it matter? Should we instead replace pointer param with Input Source?
                        InputSystem.RaisePointerClicked(controller.InputSource.Pointers[0], (Handedness)args.source.handedness, PointerAction, args.tapCount);
                    }
                }
            }
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
