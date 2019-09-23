// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIMaterialInstancing : MonoBehaviour
    {
        private void Awake()
        {
            var graphics =  gameObject.GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in graphics)
            {
                graphic.material = Instantiate(graphic.material);
            }

            enabled = false;
        }
    }
}
