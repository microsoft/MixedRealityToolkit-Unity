// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Controllers
{
    /// <summary>
    /// The Mixed Reality Visualization component is primarily responsible for rendering the user's controllers in a scene.
    /// It will render either the specific model configured against the controller definition or fall back to using the generic/global default models defined in the Controllers Profile.
    /// If no model found, then nothing is rendered, in which case the controllers will still operate as normal.
    /// The visualizer can be enabled / disabled on demand once the project is started. If disabled on start, it cannot function.
    /// </summary>
    /// <example>
    /// To Start, ensure there is a "Controllers Profile" configured against the main Mixed Reality Configuration Profile and that Controller Rendering is enabled in that profile, as well as configuring at least one controller for each platform you intend to support.
    /// Once ready, then simply add this script to a GameObject in the scene, for example the Camera / head.
    /// </example>
    /// <remarks>
    /// For Alpha, this visualizer only does basic rendering and management of controllers.  In future versions, it will expose specific transforms on a controller model (if it has them) to be able to attach items to any part of the model
    /// It will also support animation of controller parts and possibly fading in and our of controllers.
    /// </remarks>
    /// <seealso cref="Internal.Definitions.Devices.MixedRealityControllerMappingProfile"/>
    public class MixedRealityControllerVisualizer : InputSystemGlobalListener, IMixedRealitySourcePoseHandler, IMixedRealityInputHandler
    {

        #region Private Variables

        private static GameObject leftControllerModel;
        private static GameObject leftControllerHand;
        private static IMixedRealityController leftController;

        private static GameObject rightControllerModel;
        private static GameObject rightControllerHand;
        private static IMixedRealityController rightController;

        #endregion Private Variables

        #region IMixedRealitySourcePoseHandler Implementation

        /// <summary>
        /// The selected controller is moving in the scene, update it's pose in relation to the users movements
        /// </summary>
        /// <param name="eventData"></param>
        void IMixedRealitySourcePoseHandler.OnSourcePoseChanged(SourcePoseEventData eventData)
        {
            // Update the respective controller if it has been initialized
            switch (eventData.Controller?.ControllerHandedness)
            {
                case Handedness.Left:
                    if (leftControllerHand != null)
                    {
                        leftControllerHand.transform.localPosition = eventData.MixedRealityPose.Position;
                        leftControllerHand.transform.localRotation = eventData.MixedRealityPose.Rotation;
                    }
                    break;
                case Handedness.Right:
                    if (rightControllerHand != null)
                    {
                        rightControllerHand.transform.localPosition = eventData.MixedRealityPose.Position;
                        rightControllerHand.transform.localRotation = eventData.MixedRealityPose.Rotation;
                    }
                    break;
            }
        }

        /// <summary>
        /// Controller found, create the configured model and position it in the scene
        /// </summary>
        /// <param name="eventData"></param>
        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            // Capture the respective controller when it's detected.  However, It'll only be rendered if visualization is enabled.
            if (eventData.Controller != null)
            {
                switch (eventData.Controller.ControllerHandedness)
                {
                    case Handedness.Right:
                        rightController = eventData.Controller;
                        break;
                    case Handedness.Left:
                        leftController = eventData.Controller;
                        break;
                }

                CreateControllerInstance(eventData.Controller.ControllerHandedness);
            }
        }

        /// <summary>
        /// Controller removed, remove it from the scene
        /// </summary>
        /// <param name="eventData"></param>
        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            // Clean up when a controller is disconnected
            if (eventData.Controller != null)
            {
                DestroyControllerInstance(eventData.Controller.ControllerHandedness);
            }
        }

        #endregion IMixedRealityInputHandler Implementation

        #region IMixedRealityInputHandler Implementation

        /// <summary>
        /// Visualize the pressed button in the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData)
        {
            //Visualize button down
        }

        /// <summary>
        /// Visualize the released button in the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
        {
            //Visualize button up
        }

        /// <summary>
        /// Visualize the held trigger in the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        void IMixedRealityInputHandler.OnInputPressed(InputEventData<float> eventData)
        {
            //Visualize single axis controls
        }

        /// <summary>
        /// Visualize the movement of a dual axis input in the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        void IMixedRealityInputHandler.OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            //Visualize dual axis controls
        }

        #endregion IMixedRealityInputHandler Implementation

        #region Monobehaviour Implementation

        /// <summary>
        /// When the visualizer is enabled, create any attached controllers in the scene.
        /// </summary>
        /// <remarks>
        /// Visualizers must start enabled in the scene in order to capture the controllers from the platform.
        /// </remarks>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (MixedRealityManager.Instance.ActiveProfile.IsControllerMappingEnabled && MixedRealityManager.Instance.ActiveProfile.ControllerMappingProfile.RenderMotionControllers)
            {
                CreateControllerInstance(Handedness.Left);
                CreateControllerInstance(Handedness.Right);
            }
        }

        /// <summary>
        /// If the visualizer is disabled, ensure all rendered controllers are cleaned up
        /// </summary>
        protected override void OnDisable()
        {
            DestroyControllerInstance(Handedness.Left);
            DestroyControllerInstance(Handedness.Right);

            base.OnDisable();
        }

        #endregion Monobehaviour Implementation

        #region Controller Visualization Methods

        /// <summary>
        /// Get the currently configured model for the controller
        /// </summary>
        /// <remarks>
        /// Controller is detected in the following order:
        /// 1: The controller model attached to the specific controller type's configuration profile
        /// 2: The generic controller model attached to the main controller configuration profile
        /// </remarks>
        /// <param name="sourceController"></param>
        /// <param name="controllerModel"></param>
        /// <returns>Returns true if a controller model is found and outputs the GameObject definition.  if no controller is found, the response is false</returns>
        public static bool TryGetControllerModel(IMixedRealityController sourceController, out GameObject controllerModel)
        {
            controllerModel = null;

            // Try and get the controller model from the specific Controller definition
            if (MixedRealityManager.Instance.ActiveProfile.IsControllerMappingEnabled)
            {
                MixedRealityManager.Instance.ActiveProfile.ControllerMappingProfile.MixedRealityControllerMappingProfiles.GetControllerModelOverride(sourceController.GetType(), sourceController.ControllerHandedness, out controllerModel);
            }
            else
            {
                Debug.LogWarning("No Controller model found. Missing Controller Profile.");
                return false;
            }

            if (controllerModel != null)
            {
                return true;
            }

            // If no specific controller model found for the device type, try and get the generic override models from the main Controllers profile
            if (sourceController.ControllerHandedness == Handedness.Left && MixedRealityManager.Instance.ActiveProfile.ControllerMappingProfile.GlobalLeftHandModel != null)
            {
                controllerModel = MixedRealityManager.Instance.ActiveProfile.ControllerMappingProfile.GlobalLeftHandModel;
                return true;
            }

            if (sourceController.ControllerHandedness == Handedness.Right && MixedRealityManager.Instance.ActiveProfile.ControllerMappingProfile.GlobalRightHandModel != null)
            {
                controllerModel = MixedRealityManager.Instance.ActiveProfile.ControllerMappingProfile.GlobalRightHandModel;
                return true;
            }

            // No model found, give up, go home and bake cookies.  Nothing to see here.
            return false;
        }

        private void CreateControllerInstance(Handedness controllingHand)
        {
            GameObject controllerModelGameObject;

            switch (controllingHand)
            {
                case Handedness.Left:
                    if (leftController != null && TryGetControllerModel(leftController, out controllerModelGameObject))
                    {
                        if (leftControllerHand == null)
                        {
                            leftControllerHand = new GameObject("Left Hand");
                        }

                        leftControllerHand.transform.parent = CameraCache.Main.transform.parent;
                        leftControllerModel = Instantiate(controllerModelGameObject, leftControllerHand.transform);
                    }

                    break;
                case Handedness.Right:
                    if (rightController != null && TryGetControllerModel(rightController, out controllerModelGameObject))
                    {
                        if (rightControllerHand == null)
                        {
                            rightControllerHand = new GameObject("Right Hand");
                        }

                        rightControllerHand.transform.parent = CameraCache.Main.transform.parent;
                        rightControllerModel = Instantiate(controllerModelGameObject, rightControllerHand.transform);
                    }

                    break;
            }
        }

        private static void DestroyControllerInstance(Handedness controllingHand)
        {
            switch (controllingHand)
            {
                case Handedness.Left:
                    Destroy(leftControllerModel);
                    Destroy(leftControllerHand);
                    leftControllerModel = null;
                    leftControllerHand = null;
                    break;
                case Handedness.Right:
                    Destroy(rightControllerModel);
                    Destroy(rightControllerHand);
                    rightControllerModel = null;
                    rightControllerHand = null;
                    break;
            }
        }

        #endregion Controller Visualization Methods
    }
}