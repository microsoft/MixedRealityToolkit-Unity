//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace HoloToolkit.Unity.Examples
{
    public class OptimizeSceneforDeviceType : MonoBehaviour
    {
        public GameObject containerObject;
        public GameObject environmentObject;
        public Camera cameraObject;

        void Start()
        {

            // Check if the device type is HoloLens or Immersive HMD
            if (HolographicSettings.IsDisplayOpaque)
            {
                Debug.Log("*********************** IHMD ************************************");
                // Set camera clear flags to skybox for IHMD, show floor and table
                cameraObject.clearFlags = CameraClearFlags.Skybox;
                environmentObject.SetActive(true);

                // Optimize the default postion of the objects for Immersive HMD
                containerObject.transform.position = new Vector3(0.05f, 1.2f, 1.08f);
            }
            else
            {
                Debug.Log("*********************** HOLOLENS ************************************");
                // Set camera clear flags to solid color for HoloLens, hide floor and table
                cameraObject.clearFlags = CameraClearFlags.SolidColor;
                environmentObject.SetActive(false);

                // Optimize the default postion of the objects for HoloLens
                containerObject.transform.position = new Vector3(0.05f, -0.65f, 1.65f);
            }
        }

    }
}
