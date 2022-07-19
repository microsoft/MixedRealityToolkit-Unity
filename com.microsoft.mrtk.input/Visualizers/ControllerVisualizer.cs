// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

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
        private InputDeviceCharacteristics controllerCharacteristics;

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
                    controllerCharacteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left;
                    break;
                case XRNode.RightHand:
                    controllerModelProvider = ControllerModel.Right;
                    xrController = lookups[0].RightHandController;
                    controllerCharacteristics = InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right;
                    break;
                default:
                    break;
            }

            InputDevices.deviceConnected += CheckToShowVisuals;
        }

        protected void OnDisable()
        {
            InputDevices.deviceConnected -= CheckToShowVisuals;

            // Hide the controller model
            xrController.hideControllerModel = true;
        }

        private void CheckToShowVisuals(InputDevice inputDevice)
        {
            if (inputDevice.isValid && (inputDevice.characteristics & controllerCharacteristics) == controllerCharacteristics)
            {
                InstantiateControllerVisuals(); 
            }
        }

        private async void InstantiateControllerVisuals()
        {
            if (controllerModelProvider != null)
            {
                GameObject ControllerGameObject = await OpenXRControllerModelSubsystem.TryGenerateControllerModelFromPlatformSDK(controllerModelProvider);
                if(ControllerGameObject != null)
                {
                    xrController.model = ControllerGameObject.transform;
                }
                else
                {
                    xrController.model = Instantiate(fallbackControllerModel).transform;
                }
            }
            xrController.model.transform.parent = xrController.modelParent;
        }

        public void Update()
        {
            if (xrController.model == null)
            {
                xrController.model = Instantiate(fallbackControllerModel).transform;
                xrController.model.transform.parent = xrController.modelParent;
            }
            xrController.hideControllerModel = !isControllerTracked;
        }
    }
}
