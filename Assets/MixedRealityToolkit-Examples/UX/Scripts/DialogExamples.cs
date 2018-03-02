// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

namespace MixedRealityToolkit.Examples.UX
{
    public class DialogExamples : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToScaleBasedOnHMD;

        [SerializeField]
        private Vector3 scaleIfImmersive = new Vector3(1.3f, 1.3f, 1);

        private void Start()
        {

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if (objectToScaleBasedOnHMD)
            {
                if (HolographicSettings.IsDisplayOpaque)
                {
                    objectToScaleBasedOnHMD.transform.localScale = scaleIfImmersive;
                }
            }
#endif
        }
    }
}
