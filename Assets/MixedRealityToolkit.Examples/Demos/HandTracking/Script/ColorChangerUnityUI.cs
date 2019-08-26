// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    class ColorChangerUnityUI : MonoBehaviour
    {
        [SerializeField]
        private Graphic graphic;

        private void Start()
        {
            if (graphic == null)
            {
                graphic = GetComponent<Graphic>();
            }
        }

        public void ChangeColor()
        {
            graphic.color = UnityEngine.Random.ColorHSV();
        }
    }
}
