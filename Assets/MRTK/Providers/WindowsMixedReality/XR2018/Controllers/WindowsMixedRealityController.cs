// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Input;

#if UNITY_WSA
using Unity.Profiling;
using UnityEngine.XR.WSA.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    /// <summary>
    /// A Windows Mixed Reality Controller Instance.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right },
        "Textures/MotionController",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
    public class WindowsMixedRealityController : BaseWindowsMixedRealitySource, IMixedRealityHapticFeedback
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowsMixedRealityController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : this(trackingState, controllerHandedness, new WindowsMixedRealityControllerDefinition(controllerHandedness), inputSource, interactions)
        { }

        public WindowsMixedRealityController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSourceDefinition definition,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, definition)
        { }

#if UNITY_WSA
        #region Update data functions

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityController.UpdateController");

        /// <summary>
        /// Update the controller data from the provided platform state.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public override void UpdateController(InteractionSourceState interactionSourceState)
        {
            if (!Enabled) { return; }

            using (UpdateControllerPerfMarker.Auto())
            {
                base.UpdateController(interactionSourceState);

                for (int i = 0; i < Interactions?.Length; i++)
                {
                    switch (Interactions[i].InputType)
                    {
                        case DeviceInputType.None:
                            break;
                        case DeviceInputType.ThumbStick:
                        case DeviceInputType.ThumbStickPress:
                            UpdateThumbstickData(interactionSourceState, Interactions[i]);
                            break;
                        case DeviceInputType.Touchpad:
                        case DeviceInputType.TouchpadTouch:
                        case DeviceInputType.TouchpadPress:
                            UpdateTouchpadData(interactionSourceState, Interactions[i]);
                            break;
                        case DeviceInputType.Menu:
                            UpdateMenuData(interactionSourceState, Interactions[i]);
                            break;
                    }
                }
            }
        }

        private static readonly ProfilerMarker UpdateTouchpadDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityController.UpdateTouchpadData");

        /// <summary>
        /// Update the touchpad input from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateTouchpadData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            using (UpdateTouchpadDataPerfMarker.Auto())
            {
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TouchpadTouch:
                        {
                            // Update the interaction data source
                            interactionMapping.BoolData = interactionSourceState.touchpadTouched;

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                // Raise input system event if it's enabled
                                if (interactionSourceState.touchpadTouched)
                                {
                                    CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                }
                                else
                                {
                                    CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                }
                            }
                            break;
                        }
                    case DeviceInputType.TouchpadPress:
                        {
                            // Update the interaction data source
                            interactionMapping.BoolData = interactionSourceState.touchpadPressed;

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                // Raise input system event if it's enabled
                                if (interactionSourceState.touchpadPressed)
                                {
                                    CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                }
                                else
                                {
                                    CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                }
                            }
                            break;
                        }
                    case DeviceInputType.Touchpad:
                        {
                            // Update the interaction data source
                            interactionMapping.Vector2Data = interactionSourceState.touchpadPosition;

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                // Raise input system event if it's enabled
                                CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.touchpadPosition);
                            }
                            break;
                        }
                }
            }
        }

        private static readonly ProfilerMarker UpdateThumbstickDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityController.UpdateThumbstickData");

        /// <summary>
        /// Update the thumbstick input from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateThumbstickData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            using (UpdateThumbstickDataPerfMarker.Auto())
            {
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.ThumbStickPress:
                        {
                            // Update the interaction data source
                            interactionMapping.BoolData = interactionSourceState.thumbstickPressed;

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                // Raise input system event if it's enabled
                                if (interactionSourceState.thumbstickPressed)
                                {
                                    CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                }
                                else
                                {
                                    CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                }
                            }
                            break;
                        }
                    case DeviceInputType.ThumbStick:
                        {
                            // Update the interaction data source
                            interactionMapping.Vector2Data = interactionSourceState.thumbstickPosition;

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                // Raise input system event if it's enabled
                                CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.thumbstickPosition);
                            }
                            break;
                        }
                }
            }
        }

        private static readonly ProfilerMarker UpdateMenuDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityController.UpdateMenuData");

        /// <summary>
        /// Update the menu button state.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateMenuData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            using (UpdateMenuDataPerfMarker.Auto())
            {
                // Update the interaction data source
                interactionMapping.BoolData = interactionSourceState.menuPressed;

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise input system event if it's enabled
                    if (interactionSourceState.menuPressed)
                    {
                        CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                    else
                    {
                        CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                }
            }
        }

        #endregion Update data functions

        #region Controller model functions

#if WINDOWS_UWP
        private WindowsMixedRealityControllerModelProvider controllerModelProvider;

        /// <inheritdoc />
        protected override bool TryRenderControllerModel(System.Type controllerType, InputSourceType inputSourceType)
        {
            if (GetControllerVisualizationProfile() == null ||
                !GetControllerVisualizationProfile().GetUsePlatformModelsOverride(GetType(), ControllerHandedness))
            {
                return base.TryRenderControllerModel(controllerType, inputSourceType);
            }
            else
            {
                TryRenderControllerModelWithModelProvider();
                return true;
            }
        }

        private async void TryRenderControllerModelWithModelProvider()
        {
            if (controllerModelProvider == null)
            {
                controllerModelProvider = new WindowsMixedRealityControllerModelProvider(ControllerHandedness);
            }

            UnityEngine.GameObject controllerModel = await controllerModelProvider.TryGenerateControllerModelFromPlatformSDK();

            if (this != null)
            {
                if (controllerModel != null
                    && MixedRealityControllerModelHelpers.TryAddVisualizationScript(controllerModel, GetType(), ControllerHandedness)
                    && TryAddControllerModelToSceneHierarchy(controllerModel))
                {
                    controllerModel.SetActive(true);
                    return;
                }

                UnityEngine.Debug.LogWarning("Failed to create controller model from driver; defaulting to BaseController behavior.");
                base.TryRenderControllerModel(GetType(), InputSource.SourceType);
            }

            if (controllerModel != null)
            {
                // If we didn't successfully set up the model and add it to the hierarchy (which returns early), set it inactive.
                controllerModel.SetActive(false);
            }
        }
#endif

        #endregion Controller model functions
#endif // UNITY_WSA

        #region Haptic feedback functions

        public bool StartHapticImpulse(float intensity, float durationInSeconds = float.MaxValue) =>
#if UNITY_WSA
            LastSourceStateReading.source.StartHaptics(intensity, durationInSeconds);
#else
            false;
#endif // UNITY_WSA

        public void StopHapticFeedback()
        {
#if UNITY_WSA
            LastSourceStateReading.source.StopHaptics();
#endif // UNITY_WSA
        }

        #endregion Haptic feedback functions
    }
}
