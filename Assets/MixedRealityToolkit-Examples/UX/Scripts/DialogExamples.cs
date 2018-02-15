// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR.WSA;

namespace MixedRealityToolkit.Examples.UX
{
    public class DialogExamples : MonoBehaviour
    {
        void Start()
        {
            // Optimize the content for immersive headset
            if (HolographicSettings.IsDisplayOpaque)
            {
               GameObject buttonCollection = GameObject.Find("ButtonCollection");
               buttonCollection.transform.localScale = new Vector3(2, 2, 2);
            }
        }
    }
}
