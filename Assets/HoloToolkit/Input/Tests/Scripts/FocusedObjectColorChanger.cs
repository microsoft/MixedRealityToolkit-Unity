// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// FocusedObjectMessageReceiver class shows how to handle focus events.
    /// This particular implementatoin controls object appearance by changing its color when focused.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class FocusedObjectColorChanger : MonoBehaviour, IFocusable
    {
        [Tooltip("Object color changes to this when focused.")]
        public Color FocusedColor = Color.red;

        private Color originalColor;
        private Material cachedMaterial;

        private void Awake()
        {
            cachedMaterial = GetComponent<Renderer>().material;
            originalColor = cachedMaterial.GetColor("_Color");
        }

        public void OnFocusEnter()
        {
            cachedMaterial.SetColor("_Color", FocusedColor);
        }

        public void OnFocusExit()
        {
            cachedMaterial.SetColor("_Color", originalColor);
        }

        private void OnDestroy()
        {
            DestroyImmediate(cachedMaterial);
        }
    }
}
