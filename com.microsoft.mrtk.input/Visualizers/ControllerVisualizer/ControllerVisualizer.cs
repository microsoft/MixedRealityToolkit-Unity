// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.MixedReality.Toolkit.Input.Simulation;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityInputSystem = UnityEngine.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Basic controller visualizer which renders the a controller model when one is detected.
    /// The platform controller model is used when available, otherwise a generic controller model is used.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Controller Visualizer")]
    public class ControllerVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRNode on which this hand is located.")]
        private XRNode handNode = XRNode.LeftHand;

        /// <summary> The XRNode on which this hand is located. </summary>
        public XRNode HandNode { get => handNode; set => handNode = value; }

        [SerializeField]
        [Tooltip("A fallback controller model to render in case the platform model fails to load")]
        private GameObject fallbackControllerModel;

        // The controller usages we want the input device to have;
        private InternedString targetUsage;

        // A GameObject representing the currently loaded platform models.
        private GameObject platformLoadedGameObject;

        // A GameObject representing the root which contains any loaded platform models.
        // This root is necessary since platform models are rotated 180 degrees by default.
        private GameObject platformLoadedGameObjectRoot;

        // A GameObject representing the fallback controller model.
        private GameObject fallbackGameObject;

        /// <summary>
        /// Cached reference to hands aggregator for efficient per-frame use.
        /// </summary>
        [Obsolete("Deprecated, please use XRSubsystemHelpers.HandsAggregator instead.")]
        protected HandsAggregatorSubsystem HandsAggregator => XRSubsystemHelpers.HandsAggregator as HandsAggregatorSubsystem;

        [SerializeField]
        [Tooltip("The input action we key into to determine whether this controller is tracked or not")]
        private InputActionProperty controllerDetectedAction;

        protected void OnEnable()
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            switch (handNode)
            {
                case XRNode.LeftHand:
                    targetUsage = UnityInputSystem.CommonUsages.LeftHand;
                    break;
                case XRNode.RightHand:
                    targetUsage = UnityInputSystem.CommonUsages.RightHand;
                    break;
                default:
                    break;
            }

            if (controllerDetectedAction == null || controllerDetectedAction.action == null) { return; }
            controllerDetectedAction.action.started += RenderControllerVisuals;
            controllerDetectedAction.action.canceled += RemoveControllerVisuals;
            controllerDetectedAction.EnableDirectAction();
        }

        protected void OnDisable()
        {
            if (controllerDetectedAction == null || controllerDetectedAction.action == null) { return; }
            controllerDetectedAction.DisableDirectAction();
            controllerDetectedAction.action.started -= RenderControllerVisuals;
            controllerDetectedAction.action.canceled -= RemoveControllerVisuals;
        }

        private void RenderControllerVisuals(InputAction.CallbackContext context)
        {
            RenderControllerVisuals(context.control.device);
        }

        private void RenderControllerVisuals(UnityInputSystem.InputDevice inputDevice)
        {
            // This process may change in the future as unity updates its input subsystem.
            // In the future, there will be a different way of distinguishing between physical controllers
            // and tracked hands, forgoing the UnityEngine.XR.InputDevices route

            // Upon detecting a generic input device with the appropriate usages, load or remove the controller visuals
            // when appropriate
            if (inputDevice is UnityInputSystem.XR.XRController xrInputDevice && xrInputDevice.usages.Contains(targetUsage))
            {
                // Fallback visuals are only used if NO hand joints are detected
                // OR the input device is specifically a simulated controller that is in the MotionController Simulation Mode.
                bool useFallbackVisuals;
                bool isSimulatedController;
                if (xrInputDevice is MRTKSimulatedController simulatedController)
                {
                    useFallbackVisuals = simulatedController.SimulationMode == ControllerSimulationMode.MotionController;
                    isSimulatedController = true;
                }
                else
                {
                    useFallbackVisuals = XRSubsystemHelpers.HandsAggregator == null ||
                                        !XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, handNode, out _);
                    isSimulatedController = false;
                }

                InstantiateControllerVisuals(inputDevice, !isSimulatedController, useFallbackVisuals);
            }
        }

        // Private reference to the GameObject which represents the visualized controller
        // Needs to be explicitly set to null in cases where no controller visuals are ever loaded.
        private GameObject controllerGameObject = null;

        // Platform models are "rotated" 180 degrees because their forward vector points towards the user.
        private static readonly Quaternion controllerModelRotatedOffset = Quaternion.Euler(0, 180, 0);

        /// <summary>
        /// Tries to instantiate controller visuals for the specified input device
        /// </summary>
        /// <param name="inputDevice">The input device we want to generate visuals for</param>
        /// <param name="usePlatformVisuals">Whether or not to try to load visuals from the platform provider</param>
        /// <param name="useFallbackVisuals">Whether or not to use the fallback controller visuals</param>
        private async void InstantiateControllerVisuals(UnityInputSystem.InputDevice inputDevice, bool usePlatformVisuals, bool useFallbackVisuals)
        {
            // Disable any preexisting controller models before trying to render new ones.
            if (platformLoadedGameObject != null)
            {
                platformLoadedGameObject.SetActive(false);
            }
            if (platformLoadedGameObjectRoot != null)
            {
                platformLoadedGameObjectRoot.SetActive(false);
            }
            if (fallbackGameObject != null)
            {
                fallbackGameObject.SetActive(false);
            }

            // Try to load the controller model from the platform
            if (usePlatformVisuals)
            {
                platformLoadedGameObject = await ControllerModelLoader.TryGenerateControllerModelFromPlatformSDK(inputDevice, handNode.ToHandedness());
                if (platformLoadedGameObject != null)
                {
                    // Platform models are "rotated" 180 degrees because their forward vector points towards the user.
                    // We need to rotate these models in order to have them pointing in the correct direction on device.
                    if (platformLoadedGameObjectRoot == null)
                    {
                        platformLoadedGameObjectRoot = new GameObject("Platform Model Root");
                    }
                    platformLoadedGameObject.transform.parent = platformLoadedGameObjectRoot.transform;
                    platformLoadedGameObject.transform.SetPositionAndRotation(platformLoadedGameObjectRoot.transform.position, platformLoadedGameObjectRoot.transform.rotation * controllerModelRotatedOffset);

                    controllerGameObject = platformLoadedGameObjectRoot;
                }

            }

            // If the ControllerGameObject is still not initialized after this, then use the fallback model if told to
            if (useFallbackVisuals && controllerGameObject == null)
            {
                if (fallbackGameObject == null && fallbackControllerModel != null)
                {
                    fallbackGameObject = Instantiate(fallbackControllerModel);
                }

                controllerGameObject = fallbackGameObject;
            }

            if (controllerGameObject != null)
            {
                controllerGameObject.SetActive(usePlatformVisuals || useFallbackVisuals);
                controllerGameObject.transform.parent = transform;
                controllerGameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
        }

        private void RemoveControllerVisuals(InputAction.CallbackContext obj)
        {
            if (controllerGameObject != null)
            {
                controllerGameObject.SetActive(false);
            }
        }
    }
}
