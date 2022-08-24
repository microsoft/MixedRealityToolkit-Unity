// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Input.Simulation;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityInputSystem = UnityEngine.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Basic controller visualizer which draws a generic motion controller when one is detected
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

        // caching the controller model provider we are using we belong to
        private ControllerModel controllerModelProvider;

        // The controller usages we want the input device to have;
        private InternedString targetUsage;

        // The characteristics we want to check to ensure that the input device is for the appropriate hand.
        private InputDeviceCharacteristics handednessCharacteristic;

        // The controller characteristics we want the input device to have;
        private InputDeviceCharacteristics validControllerCharacteristiscs = InputDeviceCharacteristics.HeldInHand & InputDeviceCharacteristics.TrackedDevice;

        /// <inheritdoc />
        protected bool IsControllerTracked => xrController.currentControllerState.inputTrackingState.HasPositionAndRotation();

        protected void OnEnable()
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            ControllerLookup[] lookups = GameObject.FindObjectsOfType(typeof(ControllerLookup)) as ControllerLookup[];

            switch (handNode)
            {
                case XRNode.LeftHand:
                    controllerModelProvider = ControllerModel.Left;
                    xrController = lookups[0].LeftHandController;
                    targetUsage = UnityInputSystem.CommonUsages.LeftHand;
                    handednessCharacteristic = InputDeviceCharacteristics.Left;
                    break;
                case XRNode.RightHand:
                    controllerModelProvider = ControllerModel.Right;
                    xrController = lookups[0].RightHandController;
                    targetUsage = UnityInputSystem.CommonUsages.RightHand;
                    handednessCharacteristic = InputDeviceCharacteristics.Right;
                    break;
                default:
                    break;
            }

            // This process may change in the future as unity updates its input subsystem.
            // In the future, there will be a different way of distinguishing between phsyical controllers
            // and tracked hands, forgoing the UnityEngine.XR.InputDevices route
            UnityInputSystem.InputSystem.onDeviceChange += CheckToShowVisuals;
            InputDevices.deviceConnected += CheckToShowVisuals;
            InputDevices.deviceDisconnected += RemoveVisuals;
        }

        protected void OnDisable()
        {
            UnityInputSystem.InputSystem.onDeviceChange -= CheckToShowVisuals;
            InputDevices.deviceConnected -= CheckToShowVisuals;
            InputDevices.deviceDisconnected -= RemoveVisuals;

            // Hide the controller model
            xrController.hideControllerModel = true;
        }

        private void CheckToShowVisuals(UnityInputSystem.InputDevice inputDevice, UnityInputSystem.InputDeviceChange change)
        {
            // Upon detecting a generic input device with the appropriate usages, load or remove the controller visuals
            // when appropriate
            if (inputDevice is UnityInputSystem.XR.XRController xrInputDevice && xrInputDevice.usages.Contains(targetUsage))
            {
                switch (change)
                {
                    case UnityInputSystem.InputDeviceChange.Added:
                    case UnityInputSystem.InputDeviceChange.Reconnected:
                    case UnityInputSystem.InputDeviceChange.Enabled:
                    case UnityInputSystem.InputDeviceChange.UsageChanged:
                    case UnityInputSystem.InputDeviceChange.ConfigurationChanged:
                    case UnityInputSystem.InputDeviceChange.SoftReset:
                    case UnityInputSystem.InputDeviceChange.HardReset:
                        // Clear the exiting visuals before proceding to try to render new ones
                        ClearControllerVisuals();

                        // Fallback visuals are only used if NO hand joints are detected
                        // OR the input device is specifically a simulated controller that is in the MotionController Simulation Mode.
                        bool useFallbackVisuals = !HandsUtils.GetSubsystem().TryGetJoint(TrackedHandJoint.Palm, handNode, out _);
                        bool isSimulatedController = false;
                        if (xrInputDevice is MRTKSimulatedController simulatedController)
                        {
                            isSimulatedController = simulatedController.SimulationMode == ControllerSimulationMode.MotionController;
                            useFallbackVisuals |= isSimulatedController;
                        }

                        InstantiateControllerVisuals(!isSimulatedController, useFallbackVisuals);
                        break;
                    case UnityInputSystem.InputDeviceChange.Disabled:
                    case UnityInputSystem.InputDeviceChange.Removed:
                    case UnityInputSystem.InputDeviceChange.Disconnected:
                        ClearControllerVisuals();
                        break;
                }
            }
        }

        private void CheckToShowVisuals(InputDevice inputDevice)
        {
            if ((inputDevice.characteristics & handednessCharacteristic) == handednessCharacteristic &&
                (inputDevice.characteristics & validControllerCharacteristiscs) == validControllerCharacteristiscs)
            {
                // Clear the exiting visuals before proceding to try to render new ones
                ClearControllerVisuals();

                // Instantiate the controller visuals if this input device matches the characteristics for a valid controller.
                InstantiateControllerVisuals(true, true);
            }
        }

        private void RemoveVisuals(InputDevice inputDevice)
        {
            if ((inputDevice.characteristics & handednessCharacteristic) == handednessCharacteristic &&
                (inputDevice.characteristics & validControllerCharacteristiscs) == validControllerCharacteristiscs)
            {
                // Clear the controller visuals if they have been previously instantiated
                ClearControllerVisuals();
            }
        }

        private GameObject ControllerGameObject;
        /// <summary>
        /// Tries to instantiate controller visuals for the specified hand node.
        /// </summary>
        /// <param name="useFallbackVisuals">Whether or not to use the fallback controller visuals</param>
        private async void InstantiateControllerVisuals(bool usePlatformVisuals, bool useFallbackVisuals)
        {
            // Initialize the ControllerGameObject if it has not yet been initialized
            // First try to load the controller model from the platform
            if (ControllerGameObject == null)
            {
                if (usePlatformVisuals && controllerModelProvider != null)
                {
                    GameObject PlatformLoadedGameObject = await ControllerModelLoader.TryGenerateControllerModelFromPlatformSDK(controllerModelProvider);
                    if (PlatformLoadedGameObject != null)
                    {
                        // Platform models are "rotated" 180 degrees due to the forward vector for a controller pointing towards the user.
                        // We need to rotate these models in order to have them pointing in the correct direction on device
                        GameObject rotationAdjustedGameObject = new GameObject(PlatformLoadedGameObject.name + " Root");
                        PlatformLoadedGameObject.transform.parent = rotationAdjustedGameObject.transform;
                        PlatformLoadedGameObject.transform.SetPositionAndRotation(rotationAdjustedGameObject.transform.position, rotationAdjustedGameObject.transform.rotation * Quaternion.Euler(0, 180, 0));
                        ControllerGameObject = rotationAdjustedGameObject;
                    }
                }

                // If the ControllerGameObject is still not initialized after this, then use the fallback model if told to
                if (ControllerGameObject == null && useFallbackVisuals)
                {
                    ControllerGameObject = Instantiate(fallbackControllerModel);
                }
            }

            if (ControllerGameObject != null)
            {
                ControllerGameObject.transform.parent = transform;
                ControllerGameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
        }

        /// <summary>
        /// Clears the controller visuals if they have been instantiated
        /// </summary>
        private void ClearControllerVisuals()
        {
            if (ControllerGameObject != null)
            {
                Destroy(ControllerGameObject);
            }
        }

        public void Update()
        {
            if(ControllerGameObject != null && ControllerGameObject.activeSelf != IsControllerTracked)
            {
                ControllerGameObject.SetActive(IsControllerTracked);
            }
        }
    }
}
