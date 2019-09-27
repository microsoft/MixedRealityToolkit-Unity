// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;

#if UNITY_WSA
using UnityEngine;
using UnityEngine.XR.WSA.Input;
#endif

#if WINDOWS_UWP
using Windows.Foundation;
using Windows.Perception;
using Windows.UI.Input.Spatial;
using Windows.Storage.Streams;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    /// <summary>
    /// A Windows Mixed Reality Controller Instance.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right },
        "StandardAssets/Textures/MotionController")]
    public class WindowsMixedRealityController : BaseWindowsMixedRealitySource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowsMixedRealityController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The Windows Mixed Reality Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Grip Press", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(6, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(8, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(9, "Menu Press", AxisType.Digital, DeviceInputType.Menu, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(11, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
        };

#if UNITY_WSA

        private bool controllerModelInitialized = false;
        private bool failedToObtainControllerModel = false;

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public override void UpdateController(InteractionSourceState interactionSourceState)
        {
            if (!Enabled) { return; }

            EnsureControllerModel(interactionSourceState);

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

        /// <summary>
        /// Update the touchpad input from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateTouchpadData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
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
                        // Raise input system Event if it enabled
                        if (interactionSourceState.touchpadTouched)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
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
                        // Raise input system Event if it enabled
                        if (interactionSourceState.touchpadPressed)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
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
                        // Raise input system Event if it enabled
                        InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.touchpadPosition);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Update the thumbstick input from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateThumbstickData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
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
                        // Raise input system Event if it enabled
                        if (interactionSourceState.thumbstickPressed)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
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
                        // Raise input system Event if it enabled
                        InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.thumbstickPosition);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Update the menu button state.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateMenuData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            // Update the interaction data source
            interactionMapping.BoolData = interactionSourceState.menuPressed;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                if (interactionSourceState.menuPressed)
                {
                    InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
        }

        #endregion Update data functions

        #region Controller model functions

        /// <summary>
        /// Ensure that if a controller model was desired that we have attempted initialization.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void EnsureControllerModel(InteractionSourceState interactionSourceState)
        {
            if (controllerModelInitialized ||
                GetControllerVisualizationProfile() == null ||
                !GetControllerVisualizationProfile().GetUseDefaultModelsOverride(GetType(), ControllerHandedness))
            {
                controllerModelInitialized = true;
                return;
            }

            controllerModelInitialized = true;
            CreateControllerModelFromPlatformSDK(interactionSourceState.source.id);
        }

        protected override bool TryRenderControllerModel(Type controllerType, InputSourceType inputSourceType)
        {
            // Intercept this call if we are using the default driver provided models.
            // Note: Obtaining models from the driver will require access to the InteractionSource.
            // It's unclear whether the interaction source will be available during setup, so we attempt to create
            // the controller model on an input update
            if (failedToObtainControllerModel ||
                GetControllerVisualizationProfile() == null ||
                !GetControllerVisualizationProfile().GetUseDefaultModelsOverride(GetType(), ControllerHandedness))
            {
                controllerModelInitialized = true;
                return base.TryRenderControllerModel(controllerType, inputSourceType);
            }

            return false;
        }

        private async void CreateControllerModelFromPlatformSDK(uint interactionSourceId)
        {
            Debug.Log("Creating controller model from platform SDK");
            byte[] fileBytes = null;

#if WINDOWS_UWP
            var controllerModelStream = await TryGetRenderableModelAsync(interactionSourceId);
            if (controllerModelStream == null ||
                controllerModelStream.Size == 0)
            {
                Debug.LogError("Failed to obtain controller model from driver");
            }
            else
            {
                fileBytes = new byte[controllerModelStream.Size];
                using (DataReader reader = new DataReader(controllerModelStream))
                {
                    await reader.LoadAsync((uint)controllerModelStream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
#endif

            GameObject gltfGameObject = null;
            if (fileBytes != null)
            {
                var gltfObject = GltfUtility.GetGltfObjectFromGlb(fileBytes);
                gltfGameObject = await gltfObject.ConstructAsync();
                if (gltfGameObject != null)
                {
                    var visualizationProfile = GetControllerVisualizationProfile();
                    if (visualizationProfile != null)
                    {
                        var visualizationType = visualizationProfile.GetControllerVisualizationTypeOverride(GetType(), ControllerHandedness);
                        if (visualizationType != null)
                        {
                            gltfGameObject.AddComponent(visualizationType.Type);
                            TryAddControllerModelToSceneHierarchy(gltfGameObject);
                        }
                        else
                        {
                            Debug.LogError("Controller visualization type not defined for controller visualization profile");
                            GameObject.Destroy(gltfGameObject);
                            gltfGameObject = null;
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to obtain a controller visualization profile");
                    }
                }
            }


            failedToObtainControllerModel = (gltfGameObject == null);
            if (failedToObtainControllerModel)
            {
                Debug.LogWarning("Failed to create controller model from driver, defaulting to BaseController behavior");
                TryRenderControllerModel(GetType(), InputSourceType.Controller);
            }
        }

#if WINDOWS_UWP
        private IAsyncOperation<IRandomAccessStreamWithContentType> TryGetRenderableModelAsync(uint interactionSourceId)
        {
            IAsyncOperation<IRandomAccessStreamWithContentType> returnValue = null;
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                var sources = SpatialInteractionManager.GetForCurrentView()?.GetDetectedSourcesAtTimestamp(PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));
                if (sources != null)
                {
                    foreach (SpatialInteractionSourceState sourceState in sources)
                    {
                        if (sourceState.Source.Id.Equals(interactionSourceId))
                        {
                            returnValue = sourceState.Source.Controller.TryGetRenderableModelAsync();
                        }
                    }
                }
            }, true);

            return returnValue;
        }
#endif

        #endregion Controller model functions

#endif // UNITY_WSA
    }
}
