// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Utilties for testing Unity UI elements
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
            var touchable = obj.AddComponent<NearInteractionTouchable>();
            touchable.SurfaceType = NearInteractionTouchable.TouchableSurfaceType.UnityUI;
            touchable.EventsToReceive = TouchableEventType.Pointer;

            AdjustTouchableSettingsToRectTransform(touchable, obj.transform as RectTransform);
            Assert.True(touchable.AreLocalVectorsOrthogonal);

            return canvas;
        }

        public static Text CreateText(string text)
        {
            var obj = CreateRenderableObject();
            var textComp = obj.AddComponent<Text>();
            textComp.text = text;
            return textComp;
        }

        public static Button CreateButton(string text, Color normalColor, Color highlightColor, Color pressedColor)
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

            var textObj = CreateText(text);
            textObj.transform.SetParent(img.transform);
            textObj.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textObj.alignment = TextAnchor.MiddleCenter;

            return button;
        }

        private static void AdjustTouchableSettingsToRectTransform(NearInteractionTouchable t, RectTransform rt)
        {
            // Match bounds
            t.Bounds = rt.sizeDelta;
            // Match forward direction to Unity UI
            t.SetLocalForward(new Vector3(0, 0, -1));
        }
    }
}
#endif