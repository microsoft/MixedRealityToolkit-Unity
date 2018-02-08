// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;

namespace MixedRealityToolkit.Examples.InputModule
{
    /// <summary>
    /// This particular implementation controls object appearance by changing its color when selected.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class SelectedObjectMessageReceiver : MonoBehaviour, IPointerHandler
    {
        [Tooltip("Object color changes to this when selected.")]
        public Color SelectedColor = Color.red;

        private Color originalColor;
        private Material cachedMaterial;

        private void Awake()
        {
            cachedMaterial = GetComponent<Renderer>().material;
            originalColor = cachedMaterial.GetColor("_Color");
        }

        public void OnSelectObject()
        {
            cachedMaterial.SetColor("_Color", SelectedColor);
        }

        public void OnClearSelection()
        {
            cachedMaterial.SetColor("_Color", originalColor);
        }

        private void OnDestroy()
        {
            DestroyImmediate(cachedMaterial);
        }

        public void OnPointerUp(ClickEventData eventData) { }

        public void OnPointerDown(ClickEventData eventData) { }

        public void OnPointerClicked(ClickEventData eventData)
        {
            OnClearSelection();

            if (FocusManager.Instance.GetFocusedObject(eventData) == gameObject)
            {
                OnSelectObject();
            }
        }
    }
}