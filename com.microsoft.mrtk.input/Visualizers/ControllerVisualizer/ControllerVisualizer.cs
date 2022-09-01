// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input.Simulation;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityInputSystem = UnityEngine.InputSystem;
using InputAction = UnityEngine.InputSystem.InputAction;
using InputActionProperty = UnityEngine.InputSystem.InputActionProperty;

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

        // caching the controller we belong to
        private XRBaseController xrController;

        // The controller usages we want the input device to have;
        private InternedString targetUsage;

        // A gameobject representing the currently loaded platform models.
        private GameObject platformLoadedGameObject;

        // A gameobject representing the root which contains any loaded platform models.
        // This root is necessary since platform models are rotated 180 degrees by default.
        private GameObject platformLoadedGameObjectRoot;

        // A gameobject representing the fallback controller model.
        private GameObject fallbackGameObject;

        [SerializeField]
        [Tooltip("The input action we key into to determine whether this controller is tracked or not")]
        private InputActionProperty controllerDetectedAction;

        protected void OnEnable()
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            ControllerLookup[] lookups = FindObjectsOfType(typeof(ControllerLookup)) as ControllerLookup[];

            switch (handNode)
            {
                case XRNode.LeftHand:
                    xrController = lookups[0].LeftHandController;
                    targetUsage = UnityInputSystem.CommonUsages.LeftHand;
                    break;
                case XRNode.RightHand:
                    xrController = lookups[0].RightHandController;
                    targetUsage = UnityInputSystem.CommonUsages.RightHand;
                    break;
                default:
                    break;
            }
            
            if (controllerDetectedAction == null  || controllerDetectedAction.action == null) { return; }
            controllerDetectedAction.action.started += RenderControllerVisuals;
            controllerDetectedAction.action.canceled += RemoveControllerVisuals;
            controllerDetectedAction.action.Enable();
        }

        protected void OnDisable()
        {
            if (controllerDetectedAction == null || controllerDetectedAction.action == null) { return; }
            controllerDetectedAction.action.Disable();
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
            // In the future, there will be a different way of distinguishing between phsyical controllers
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
                    useFallbackVisuals = !HandsUtils.GetSubsystem().TryGetJoint(TrackedHandJoint.Palm, handNode, out _);
                    isSimulatedController = false;
                }
                InstantiateControllerVisuals(!isSimulatedController, useFallbackVisuals);
            }
        }

        // Private reference to the gameobject which represents the visualized controller
        // Needs to be explicitly set to null in cases where no controller visuals are ever loaded.
        private GameObject ControllerGameObject = null;

        /// <summary>
        /// Tries to instantiate controller visuals for the specified hand node.
        /// </summary>
        /// <param name="usePlatformVisuals">Whether or not to try to load visuals from the platform provider</param>
        /// <param name="useFallbackVisuals">Whether or not to use the fallback controller visuals</param>
        private async void InstantiateControllerVisuals(bool usePlatformVisuals, bool useFallbackVisuals)
        {
            // Disable any pre-existing controller models before trying to render new ones.
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
                platformLoadedGameObject = await ControllerModelLoader.TryGenerateControllerModelFromPlatformSDK(handNode.ToHandedness());
                if (platformLoadedGameObject != null)
                {
                    // Platform models are "rotated" 180 degrees due to the forward vector for a controller pointing towards the user.
                    // We need to rotate these models in order to have them pointing in the correct direction on device.
                    if (platformLoadedGameObjectRoot == null)
                    {
                        platformLoadedGameObjectRoot = new GameObject("Platform Model Root");
                    }
                    platformLoadedGameObject.transform.parent = platformLoadedGameObjectRoot.transform;
                    platformLoadedGameObject.transform.SetPositionAndRotation(platformLoadedGameObjectRoot.transform.position, platformLoadedGameObjectRoot.transform.rotation * Quaternion.Euler(0, 180, 0));

                    ControllerGameObject = platformLoadedGameObjectRoot;
                }
            }

            // If the ControllerGameObject is still not initialized after this, then use the fallback model if told to
            if (useFallbackVisuals && ControllerGameObject == null)
            {
                if (fallbackGameObject == null && fallbackControllerModel != null)
                {
                    fallbackGameObject = Instantiate(fallbackControllerModel);
                }

                ControllerGameObject = fallbackGameObject;
            }

            if (ControllerGameObject != null)
            {
                ControllerGameObject.SetActive(usePlatformVisuals || useFallbackVisuals);
                ControllerGameObject.transform.parent = transform;
                ControllerGameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
        }

        private void RemoveControllerVisuals(InputAction.CallbackContext obj)
        {
            if (ControllerGameObject != null)
            {
                ControllerGameObject.SetActive(false);
            }
        }
    }
}
