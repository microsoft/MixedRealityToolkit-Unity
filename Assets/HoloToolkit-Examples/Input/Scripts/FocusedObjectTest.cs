// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// This class shows how to handle focus events and speech input events.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class FocusedObjectTest : FocusTarget, ISpeechHandler
    {
        [Tooltip("Object color changes to this when focused.")]
        public Color FocusedColor = Color.red;
        private const float DefaultSizeFactor = 2.0f;

        [Tooltip("Size multiplier to use when scaling the object up and down.")]
        public float SizeFactor = DefaultSizeFactor;

        private Color originalColor;
        private Material cachedMaterial;

        private void Awake()
        {
            cachedMaterial = GetComponent<Renderer>().material;
            originalColor = cachedMaterial.GetColor("_Color");
            if (SizeFactor <= 0.0f)
            {
                SizeFactor = DefaultSizeFactor;
            }
        }

        private void OnDestroy()
        {
            DestroyImmediate(cachedMaterial);
        }

        public void OnMakeBigger()
        {
            Vector3 scale = transform.localScale;
            scale *= SizeFactor;
            transform.localScale = scale;
        }

        public void OnMakeSmaller()
        {
            Vector3 scale = transform.localScale;
            scale /= SizeFactor;
            transform.localScale = scale;
        }

        public override void OnFocusEnter(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);

            cachedMaterial.SetColor("_Color", FocusedColor);
        }

        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);
            cachedMaterial.SetColor("_Color", originalColor);
        }

        void ISpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            switch (eventData.RecognizedText.ToLower())
            {
                case "make bigger":
                    OnMakeBigger();
                    break;
                case "make smaller":
                    OnMakeSmaller();
                    break;
            }
        }
    }
}
