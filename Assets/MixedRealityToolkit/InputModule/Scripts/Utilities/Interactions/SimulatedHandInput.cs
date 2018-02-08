// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace MixedRealityToolkit.InputModule.Utilities.Interations
{
    public class SimulatedHandInput : Singleton<SimulatedHandInput>
    {
        [SerializeField]
        private bool simulateHandsInEditor = true;

        public List<GameObject> SimulatedInputs = new List<GameObject>(0);

        [SerializeField]
        private GameObject leftHand;

        [SerializeField]
        private GameObject rightHand;

        private void Start()
        {
            if (!XRDevice.isPresent && XRSettings.enabled && simulateHandsInEditor)
            {
                GameObject newRightInputSource = Instantiate(rightHand);

                newRightInputSource.name = "Right_Hand";
                newRightInputSource.transform.SetParent(transform);
                SimulatedInputs.Add(newRightInputSource);

                GameObject newLeftInputSource = Instantiate(leftHand);
                newLeftInputSource.name = "Left_Hand";
                newLeftInputSource.transform.SetParent(transform);
                SimulatedInputs.Add(newLeftInputSource);
            }
        }
    }
}
#endif
