// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR;
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

        // The controller characteristics we want the input device to have;
        private InternedString targetUsage;

        /// <inheritdoc />
        protected bool isControllerTracked => xrController.currentControllerState.inputTrackingState.HasPositionAndRotation();

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
                    break;
                case XRNode.RightHand:
                    controllerModelProvider = ControllerModel.Right;
                    xrController = lookups[0].RightHandController;
                    targetUsage = UnityInputSystem.CommonUsages.RightHand;
                    break;
                default:
                    break;
            }

            UnityInputSystem.InputSystem.onDeviceChange += CheckToShowVisuals;
        }

        protected void OnDisable()
        {
            UnityInputSystem.InputSystem.onDeviceChange -= CheckToShowVisuals;

            // Hide the controller model
            xrController.hideControllerModel = true;
        }

        private void CheckToShowVisuals(UnityInputSystem.InputDevice inputDevice, UnityInputSystem.InputDeviceChange change)
        {
            if (change == UnityInputSystem.InputDeviceChange.Added ||
                change == UnityInputSystem.InputDeviceChange.UsageChanged ||
                change == UnityInputSystem.InputDeviceChange.ConfigurationChanged)
            {
                if (inputDevice is UnityInputSystem.XR.XRController xrInputDevice)
                {
                    if (xrInputDevice.usages.Contains(targetUsage))
                    {
                        InstantiateControllerVisuals();
                    }
                }
            }
        }

        private async void InstantiateControllerVisuals()
        {
            GameObject ControllerGameObject = null;
            if (controllerModelProvider != null)
            {
                ControllerGameObject = await OpenXRControllerModelSubsystem.TryGenerateControllerModelFromPlatformSDK(controllerModelProvider);
                if(ControllerGameObject == null)
                {
                    ControllerGameObject = Instantiate(fallbackControllerModel);
                }
            }

            if (ControllerGameObject != null)
            {
                xrController.model = ControllerGameObject.transform;
            }

            xrController.model.transform.parent = xrController.modelParent;
        }

        public void Update()
        {
            xrController.hideControllerModel = !isControllerTracked;
        }
    }
}
