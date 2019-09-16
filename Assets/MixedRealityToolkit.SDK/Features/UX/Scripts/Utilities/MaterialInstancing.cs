// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class MaterialInstancing : MonoBehaviour
    {
        private void Awake()
        {
            var images =  gameObject.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
            {
                image.material = Instantiate(image.material);
            }

            enabled = false;
        }
    }
}
