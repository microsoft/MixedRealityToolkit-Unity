// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public enum InputSourceType
    {
        Hand
    }

    public enum InputSourceNumber
    {
        One,
        Two
    }

    public class EditorInputSelector : MonoBehaviour
    {
        public InputSourceType SourceType;
        public InputSourceNumber SourceNumber;

        public List<GameObject> Inputs;

        public GameObject LeftHand;
        public GameObject RightHand;

        private void Awake()
        {
            if (Application.isEditor)
            {
                if (UnityEngine.XR.XRDevice.isPresent)
                {
                    Destroy(this);
                    return;
                }
            }
            else
            {
                Destroy(this);
                return;
            }

            Inputs = new List<GameObject>();

            GameObject newRightInputSource = null;
            GameObject newLeftInputSource = null;

            switch (SourceType)
            {
                case InputSourceType.Hand:
                    newRightInputSource = Instantiate(RightHand);

                    if (SourceNumber == InputSourceNumber.Two)
                    {
                        newLeftInputSource = Instantiate(LeftHand);
                    }
                    break;
            }

            newRightInputSource.name = "Right " + SourceType.ToString();
            newRightInputSource.transform.SetParent(transform);
            Inputs.Add(newRightInputSource);

            if (newLeftInputSource != null)
            {
                newLeftInputSource.name = "Left " + SourceType.ToString();
                newLeftInputSource.transform.SetParent(transform);
                Inputs.Add(newLeftInputSource);
            }
        }
    }
}