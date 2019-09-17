using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Security.Policy;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class InputDataExampleGizmo : MonoBehaviour
    {
        public InputSourceType sourceType;
        public Handedness handedness;
        private bool isDataAvailable = true;
        private bool IsDataAvailable
        {
            get => isDataAvailable;
            set
            {
                if (value != isDataAvailable)
                {
                    foreach (var item in GetComponentsInChildren<Renderer>())
                    {
                        item.enabled = value;
                    }
                }
                isDataAvailable = value;
            }
        }
        public void Update()
        {
            Ray myRay;
            if(InputUtils.TryGetRay(sourceType, handedness, out myRay))
            {
                transform.localPosition= myRay.origin;
                transform.localRotation = Quaternion.LookRotation(myRay.direction, Vector3.up);
                IsDataAvailable = true;
            }
            else
            {
                IsDataAvailable = false;
            }
        }
    }
}

