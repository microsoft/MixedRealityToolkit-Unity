// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ControllerVisualizer : MonoBehaviour
    {
        GameObject leftController, leftController2;
        GameObject rightController, rightController2;
        Vector3 controllerPosition = Vector3.zero;
        Quaternion controllerRotation = Quaternion.identity;

        InteractionMapping leftControllerPointerDefinition;
        InteractionMapping leftControllerMenuDefinition;
        InteractionMapping rightControllerPointerDefinition;
        InteractionMapping rightControllerMenuDefinition;

        public enum renderMode
        {
            Both,MRTK,InteractionManager
        }
        public renderMode curentRenderMode = renderMode.MRTK;

        private void UpdateControllerVisuals()
        {
            var controllers = Internal.Managers.MixedRealityManager.Instance.ActiveDevice.GetActiveControllers();
            foreach (var controller in controllers)
            {
                GameObject controllerVisual;
                if (leftController == null && controller.ControllerHandedness == Handedness.Left)
                {
                    controllerVisual = Internal.Managers.MixedRealityManager.Instance.ActiveProfile.ControllersProfile.OverrideLeftHandModel;
                    leftController = Instantiate(controllerVisual, CameraCache.Main.transform.parent);
                    leftController.GetComponent<MeshRenderer>().material.color = Color.red;
                    leftControllerPointerDefinition = controller.Interactions[DeviceInputType.SpatialPointer];
                    leftControllerMenuDefinition = controller.Interactions[DeviceInputType.Menu];

                    if (curentRenderMode == renderMode.InteractionManager || curentRenderMode == renderMode.Both)
                    {
                        leftController2 = Instantiate(controllerVisual, CameraCache.Main.transform.parent);
                        var mesh2 = leftController2.GetComponent<MeshRenderer>();
                        mesh2.material.color = Color.magenta;
                    }
                }
                else if (rightController == null && controller.ControllerHandedness == Handedness.Right)
                {
                    controllerVisual = Internal.Managers.MixedRealityManager.Instance.ActiveProfile.ControllersProfile.OverrideRightHandModel;
                    rightController = Instantiate(controllerVisual, CameraCache.Main.transform.parent);
                    rightController.GetComponent<MeshRenderer>().material.color = Color.green;
                    rightControllerPointerDefinition = controller.Interactions[DeviceInputType.SpatialPointer];
                    rightControllerMenuDefinition = controller.Interactions[DeviceInputType.Menu];

                    if (curentRenderMode == renderMode.InteractionManager || curentRenderMode == renderMode.Both)
                    {
                        rightController2 = Instantiate(controllerVisual, CameraCache.Main.transform.parent);
                        var mesh2 = rightController2.GetComponent<MeshRenderer>();
                        mesh2.material.color = Color.cyan;
                    }
                }

                if (curentRenderMode == renderMode.MRTK || curentRenderMode == renderMode.Both)
                {
                    var controller6DoF = controller.Interactions[DeviceInputType.SpatialPointer].GetValue<Tuple<Vector3, Quaternion>>();
                    Debug.Log($"MRTK - Controller Position {controller6DoF.Item1} - Rotation {controller6DoF.Item2}");
                    bool menuPress = false;

                    if (controller.ControllerHandedness == Handedness.Left)
                    {
                        if (menuPress)
                        {
                            leftController.GetComponent<MeshRenderer>().material.color = Color.black;
                        }
                        leftController.transform.localPosition = controller6DoF.Item1;
                        leftController.transform.localRotation = controller6DoF.Item2;
                    }
                    else
                    {
                        rightController.transform.localPosition = controller6DoF.Item1;
                        rightController.transform.localRotation = controller6DoF.Item2;
                        if (menuPress)
                        {
                            rightController.GetComponent<MeshRenderer>().material.color = Color.blue;
                        }
                    }
                }

            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateControllerVisuals();

            if (curentRenderMode == renderMode.InteractionManager || curentRenderMode == renderMode.Both)
            {
                UpdateControllerFromInteractionManager();
            }
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


        private void UpdateControllerFromInteractionManager()
        {
            InteractionSourceState[] states = InteractionManager.GetCurrentReading();

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            for (var i = 0; i < states.Length; i++)
            {
                states[i].sourcePose.TryGetPosition(out controllerPosition, InteractionSourceNode.Pointer);
                states[i].sourcePose.TryGetRotation(out controllerRotation, InteractionSourceNode.Pointer);
                if (CameraCache.Main.transform.parent != null)
                {
                    controllerPosition = CameraCache.Main.transform.parent.TransformPoint(controllerPosition);
                    controllerRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(controllerRotation.eulerAngles);
                }
                if (states[i].source.handedness == InteractionSourceHandedness.Left)
                {
                    leftController2?.transform.SetPositionAndRotation(controllerPosition, controllerRotation);
                }
                else
                {
                    rightController2?.transform.SetPositionAndRotation(controllerPosition, controllerRotation);
                }
                Debug.Log($"Controller Position {controllerPosition} - Rotation {controllerRotation}");
            }

        }
    }
}