// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace HoloToolkit.Unity.InputModule
{
    public class SimulatedInputManager : Singleton<SimulatedInputManager>
    {
        private enum InputSourceType
        {
            Hand
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

        public List<GameObject> SimulatedInputs = new List<GameObject>(0);

        [SerializeField]
        private GameObject leftHand;

        [SerializeField]
        private GameObject rightHand;

        protected override void Awake()
        {
            base.Awake();

            bool spawnControllers =  !XRDevice.isPresent && XRSettings.enabled && simulateHandsInEditor;

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
                    SimulatedInputs.Add(newRightInputSource);

                    if (sourceNumber == InputSourceNumber.Two)
                    {
                        GameObject newLeftInputSource = Instantiate(leftHand);
                        newLeftInputSource.name = "Left_" + sourceType.ToString();
                        newLeftInputSource.transform.SetParent(transform);
                        SimulatedInputs.Add(newLeftInputSource);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
#endif
