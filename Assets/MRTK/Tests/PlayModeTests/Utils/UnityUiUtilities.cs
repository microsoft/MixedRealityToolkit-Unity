// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point, we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Utilities for testing Unity UI elements
    public static class UnityUiUtilities
    {
        private static GameObject CreateUiObject()
        {
            var obj = new GameObject("TestObject", typeof(RectTransform));
            return obj;
        }

        private static GameObject CreateRenderableObject()
        {
            var obj = CreateUiObject();
            var renderer = obj.AddComponent<CanvasRenderer>();
            return obj;
        }

        public static Image CreateImage(Color color)
        {
            var obj = CreateRenderableObject();
            var image = obj.AddComponent<Image>();
            image.color = color;
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            image.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            return image;
        }

        public static Canvas CreateCanvas(float scale)
        {
            var obj = CreateUiObject();
            obj.transform.localScale = Vector3.one * scale;

            var canvas = obj.AddComponent<Canvas>();
            var canvasUtil = obj.AddComponent<CanvasUtility>();
            var canvasScaler = obj.AddComponent<CanvasScaler>();
            var raycaster = obj.AddComponent<GraphicRaycaster>();
            var touchable = obj.AddComponent<NearInteractionTouchableUnityUI>();
            touchable.EventsToReceive = TouchableEventType.Pointer;

            return canvas;
        }

        public static Text CreateText(string text)
        {
            var obj = CreateRenderableObject();
            var textComp = obj.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textComp.alignment = TextAnchor.MiddleCenter;
            return textComp;
        }

        public static Button CreateButton(Color normalColor, Color highlightColor, Color pressedColor)
        {
            var img = CreateImage(normalColor);

            var button = img.gameObject.AddComponent<Button>();
            button.targetGraphic = img;
            var colors = new ColorBlock();
            colors.normalColor = normalColor;
            colors.highlightedColor = highlightColor;
            colors.pressedColor = pressedColor;
            colors.colorMultiplier = 1.0f;
            button.colors = colors;

            return button;
        }

        public static Toggle CreateToggle(Color normalColor, Color highlightColor, Color pressedColor)
        {
            var img = CreateImage(normalColor);

            var toggle = img.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = img;
            var colors = new ColorBlock();
            colors.normalColor = normalColor;
            colors.highlightedColor = highlightColor;
            colors.pressedColor = pressedColor;
            colors.colorMultiplier = 1.0f;
            toggle.colors = colors;

            return toggle;
        }
    }
}
#endif