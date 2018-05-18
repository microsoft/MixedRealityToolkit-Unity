// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.SDK
{
    public class ControllerVisualizer : MonoBehaviour
    {
        GameObject leftController;
        GameObject rightController;
        bool initialised = false;

        private void UpdateControllerVisuals()
        {
            var controllers = Internal.Managers.MixedRealityManager.Instance.ActiveDevice.GetActiveControllers();
            foreach (var controller in controllers)
            {
                GameObject controllerVisual;
                if (!initialised)
                {
                    if (controller.Handedness == Internal.Definitions.Handedness.Left)
                    {
                        controllerVisual = Internal.Managers.MixedRealityManager.Instance.ActiveProfile.LeftControllerModel;
                        leftController = Instantiate(controllerVisual, CameraCache.Main.transform.parent);
                        var mesh2 = leftController.GetComponent<MeshRenderer>();
                        mesh2.material.color = Color.red;
                    }
                    else
                    {
                        controllerVisual = Internal.Managers.MixedRealityManager.Instance.ActiveProfile.RightControllerModel;
                        rightController = Instantiate(controllerVisual, CameraCache.Main.transform.parent);
                        var mesh2 = rightController.GetComponent<MeshRenderer>();
                        mesh2.material.color = Color.green;
                    }

                    initialised = true;
                }
                if (controller.Handedness == Internal.Definitions.Handedness.Left)
                {
                    //leftController.transform.localPosition = controller.ControllerPosition;
                    //leftController.transform.localRotation = controller.ControllerRotation;
                }
                else
                {
                    //rightController.transform.localPosition = controller.ControllerPosition;
                    //rightController.transform.localRotation = controller.ControllerRotation;
                }

            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateControllerVisuals();
        }

        private bool ValidRotation(Quaternion newRotation)
        {
            return !float.IsNaN(newRotation.x) && !float.IsNaN(newRotation.y) && !float.IsNaN(newRotation.z) && !float.IsNaN(newRotation.w) &&
                !float.IsInfinity(newRotation.x) && !float.IsInfinity(newRotation.y) && !float.IsInfinity(newRotation.z) && !float.IsInfinity(newRotation.w);
        }

        private bool ValidPosition(Vector3 newPosition)
        {
            return !float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y) && !float.IsNaN(newPosition.z) &&
                !float.IsInfinity(newPosition.x) && !float.IsInfinity(newPosition.y) && !float.IsInfinity(newPosition.z);
        }
    }
}