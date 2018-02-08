// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.InputModule.Gaze;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;
using UnityEngine.Events;

namespace MixedRealityToolkit.Examples.ColorPicker
{
    public class GazeableColorPicker : FocusTarget, IPointerHandler
    {
        public Renderer RendererComponent;

        [System.Serializable]
        public class PickedColorCallback : UnityEvent<Color> { }

        public PickedColorCallback OnGazedColor = new PickedColorCallback();
        public PickedColorCallback OnPickedColor = new PickedColorCallback();

        private void Update()
        {
            if (!HasFocus) { return; }
            UpdatePickedColor(OnGazedColor);
        }

        private void UpdatePickedColor(PickedColorCallback cb)
        {
            if (GazeManager.HitInfo.transform.gameObject != RendererComponent.gameObject) { return; }

            var texture = (Texture2D)RendererComponent.material.mainTexture;

            Vector2 pixelUV = GazeManager.HitInfo.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;

            Color col = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            cb.Invoke(col);
        }

        void IPointerHandler.OnPointerUp(ClickEventData eventData) { }

        void IPointerHandler.OnPointerDown(ClickEventData eventData) { }

        void IPointerHandler.OnPointerClicked(ClickEventData eventData)
        {
            UpdatePickedColor(OnPickedColor);
        }
    }
}