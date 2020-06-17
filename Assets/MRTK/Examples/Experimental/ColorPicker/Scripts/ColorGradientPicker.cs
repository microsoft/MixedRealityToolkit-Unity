// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.ColorPicker
{
    /// <summary>
    /// Example script to demonstrate adding gradient picker to control material values at runtime.
    /// </summary>
    public class ColorGradientPicker : MonoBehaviour
    {
        public GameObject Dragger;
        public GameObject Gradient;
        public TextMeshPro DebugText;
        private float MaxDistance = 0.5f;
        private Vector3 StartPosition;
        private Vector3 CurrentPosition;
        private bool IsDragging = false;
        private void Start()
        {
            StartPosition = Dragger.transform.localPosition;
            CurrentPosition = StartPosition;
        }
        private void Update()
        {
            if (IsDragging)
            {
                ConstrainDragging();
            }
        }
        public void StartDrag()
        {
            IsDragging = true;
        }
        public void StopDrag()
        {
            IsDragging = false;
        }
        private void ConstrainDragging()
        {
            // Horizontal
            if(Dragger.transform.localPosition.x >= StartPosition.x + MaxDistance)
            {
                CurrentPosition.x = StartPosition.x + MaxDistance;
            } else if(Dragger.transform.localPosition.x <= StartPosition.x - MaxDistance)
            {
                CurrentPosition.x = StartPosition.x - MaxDistance;
            } else {
                CurrentPosition.x = Dragger.transform.localPosition.x;
            }
            // Vertical
            if(Dragger.transform.localPosition.y >= StartPosition.y + MaxDistance)
            {
                CurrentPosition.y = StartPosition.y + MaxDistance;
            } else if(Dragger.transform.localPosition.y <= StartPosition.y - MaxDistance)
            {
                CurrentPosition.y = StartPosition.y - MaxDistance;
            } else {
                CurrentPosition.y = Dragger.transform.localPosition.y;
            }
            Dragger.transform.localPosition = CurrentPosition;
            //DebugText.text = CurrentPosition.ToString();
            DebugText.text = "Saturation=" + Mathf.RoundToInt(Mathf.Abs(CurrentPosition.x + (MaxDistance * -1)) * 100) + "%" + "\nBrightness=" + Mathf.RoundToInt((CurrentPosition.y + MaxDistance) * 100) + "%";
        }
        private void UpdateSliderText()
        {
            //
        }
        private void ApplyColor()
        {
            //
        }
        private void ApplySliderValues() {
            //
        }
    }
}
