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

        void Start()
        {
            // Check if the device type is HoloLens or Immersive HMD
            if (HolographicSettings.IsDisplayOpaque)
            {
                // Optimize the default postion of the objects for Immersive HMD
                containerObject.transform.position = new Vector3(0.05f, 1.2f, 1.08f);

            }
            else
            {
                // Optimize the default postion of the objects for HoloLens
                containerObject.transform.position = new Vector3(0.05f, -0.65f, 1.65f);

                // Remove skybox for HoloLens
                RenderSettings.skybox = null;
            }
        }

    }
}
