// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SphereGlobalKeywords : MonoBehaviour, ISpeechHandler
    {
        private Material[] cachedChildMaterials;

        private void Awake()
        {
            Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
            if (childRenderers != null && childRenderers.Length > 0)
            {
                cachedChildMaterials = new Material[childRenderers.Length];
                for (int i = 0; i < childRenderers.Length; i++)
                {
                    cachedChildMaterials[i] = childRenderers[i].material;
                }
            }
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            switch (eventData.RecognizedText.ToLower())
            {
                case "reset all":
                    foreach (Material cachedChildMaterial in cachedChildMaterials)
                    {
                        cachedChildMaterial.SetColor("_Color", Color.gray);
                    }
                    break;
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < cachedChildMaterials.Length; i++)
            {
                DestroyImmediate(cachedChildMaterials[i]);
            }
        }
    }
}