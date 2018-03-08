// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

namespace MixedRealityToolkit.InputModule.Utilities.Interations
{
    /// <summary>
    /// This class is used to select input for the Editor and applications built outside of the UWP build target.
    /// </summary>
    public class CustomInputSelector : MonoBehaviour
    {
        private enum InputSourceType
        {
            Hand,
            Mouse
        }

        private enum InputSourceNumber
        {
            One,
            Two
        }

        [SerializeField]
        private bool simulateHandsInEditor = true;

        [SerializeField]
        private InputSourceType sourceType;

        [SerializeField]
        private InputSourceNumber sourceNumber;

        public List<GameObject> Inputs = new List<GameObject>(0);

        [SerializeField]
        private GameObject mouse = null;

        [SerializeField]
        private GameObject leftHand = null;

        [SerializeField]
        private GameObject rightHand = null;

        private void Awake()
        {
            bool spawnControllers = false;

#if UNITY_2017_2_OR_NEWER
            spawnControllers = !XRDevice.isPresent && XRSettings.enabled && simulateHandsInEditor;
#else
            spawnControllers = simulateHandsInEditor;
#endif
            if (spawnControllers)
            {
                sourceType = InputSourceType.Hand;
                sourceNumber = InputSourceNumber.Two;
            }

            if (!spawnControllers) { return; }

            switch (sourceType)
            {
                case InputSourceType.Hand:
                    GameObject newRightInputSource = Instantiate(rightHand);

                    newRightInputSource.name = "Right_" + sourceType.ToString();
                    newRightInputSource.transform.SetParent(transform);
                    Inputs.Add(newRightInputSource);

                    if (sourceNumber == InputSourceNumber.Two)
                    {
                        GameObject newLeftInputSource = Instantiate(leftHand);
                        newLeftInputSource.name = "Left_" + sourceType.ToString();
                        newLeftInputSource.transform.SetParent(transform);
                        Inputs.Add(newLeftInputSource);
                    }
                    break;
                case InputSourceType.Mouse:
                    GameObject newMouseInputSource = Instantiate(mouse);
                    newMouseInputSource.transform.SetParent(transform);
                    Inputs.Add(newMouseInputSource);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
