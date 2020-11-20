// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;

#if UNITY_WSA
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
#endif

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Windows.Input;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    /// <summary>
    /// A Windows Mixed Reality Controller Instance.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right },
        "Textures/MotionController")]
    public class WindowsMixedRealityController : BaseWindowsMixedRealitySource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowsMixedRealityController(TrackingState trackingState, Handedness controllerHandedness, InteractionSource interactionSource, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            controllerDefinition = new WindowsMixedRealityControllerDefinition(inputSource, controllerHandedness);
#if WINDOWS_UWP
            controllerModelProvider = new WindowsMixedRealityControllerModelProvider(this, interactionSource.GetSpatialInteractionSource());
#endif
        }

        private readonly WindowsMixedRealityControllerDefinition controllerDefinition;
#if WINDOWS_UWP
        private readonly WindowsMixedRealityControllerModelProvider controllerModelProvider;
#endif

        /// <summary>
        /// The Windows Mixed Reality Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions => controllerDefinition?.DefaultInteractions;

#if UNITY_WSA
        private bool controllerModelInitialized = false;
        private bool failedToObtainControllerModel = false;

        private static readonly Dictionary<string, GameObject> controllerDictionary = new Dictionary<string, GameObject>(0);

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
        /// <inheritdoc />
        protected override bool TryRenderControllerModel(Type controllerType, InputSourceType inputSourceType)
        {
            // Intercept this call if we are using the default driver provided models.
            // Note: Obtaining models from the driver will require access to the InteractionSource.
            // It's unclear whether the interaction source will be available during setup, so we attempt to create
            // the controller model on an input update
            if (controllerModelProvider.UnableToGenerateSDKModel ||
                GetControllerVisualizationProfile() == null ||
                !GetControllerVisualizationProfile().GetUseDefaultModelsOverride(GetType(), ControllerHandedness))
            {
                controllerModelInitialized = true;
                return base.TryRenderControllerModel(controllerType, inputSourceType);
            }
            else
            {
                TryRenderControllerModelWithModelProvider();
            }

            return !controllerModelProvider.UnableToGenerateSDKModel;
        }

        private async void TryRenderControllerModelWithModelProvider()
        {
            GameObject controllerModel = Task.Run(controllerModelProvider.TryGenerateControllerModelFromPlatformSDK());

            if(controllerModel != null)
            {
                TryAddControllerModelToSceneHierarchy(controllerModel);
            }
        }
#endif

        #endregion Controller model functions

#endif // UNITY_WSA
    }
}
