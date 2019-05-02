//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class ShowSliderValue : MonoBehaviour
    {
        [SerializeField]
        private TextMesh textMesh = null;

        public void OnSliderUpdated(SliderEventData eventData)
        {
            if (textMesh == null)
            {
                textMesh = GetComponent<TextMesh>();
            }

            if (textMesh != null)
            {
                textMesh.text = $"{eventData.NewValue:F2}";
            }
        }
    }
}
