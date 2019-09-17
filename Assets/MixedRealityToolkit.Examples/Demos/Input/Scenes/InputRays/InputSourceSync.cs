using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Security.Policy;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class InputSourceSync : MonoBehaviour
    {
        public InputSourceType sourceType;
        public Handedness handedness;
        public void Update()
        {
            Ray myRay;
            if(InputUtils.TryGetRay(sourceType, handedness, out myRay))
            {
                transform.localPosition= myRay.origin;
                transform.localRotation = Quaternion.LookRotation(myRay.direction, Vector3.up);
            }
            else
            {
                Debug.Log("Input type " + sourceType + " is not available!");
            }
        }
    }
}

