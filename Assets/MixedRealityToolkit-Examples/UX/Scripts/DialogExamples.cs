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
        private void Start()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Optimize the content for immersive headset
            if (HolographicSettings.IsDisplayOpaque)
            {
               GameObject buttonCollection = GameObject.Find("ButtonCollection");
               buttonCollection.transform.localScale = new Vector3(2, 2, 2);
            }
#endif
        }
    }
}
