// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.ColorPicker
{
    public class GazeableColorPicker : FocusTarget, IInputClickHandler
    {
        public Renderer rendererComponent;

        [System.Serializable]
        public class PickedColorCallback : UnityEvent<Color> { }

        public PickedColorCallback OnGazedColor = new PickedColorCallback();
        public PickedColorCallback OnPickedColor = new PickedColorCallback();

        private void Update()
        {
            if (HasFocus == false) return;
            UpdatePickedColor(OnGazedColor);
        }

        private void UpdatePickedColor(PickedColorCallback cb)
        {
            RaycastHit hit = GazePointer.Instance.HitInfo;

            if (hit.transform.gameObject != rendererComponent.gameObject) { return; }

            var texture = (Texture2D)rendererComponent.material.mainTexture;

            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;

            Color col = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            cb.Invoke(col);
        }
        
        public void OnInputClicked(InputClickedEventData eventData)
        {
            UpdatePickedColor(OnPickedColor);
        }
    }
}