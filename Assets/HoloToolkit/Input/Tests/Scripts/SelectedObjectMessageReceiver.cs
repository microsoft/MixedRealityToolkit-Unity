// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// SelectedObjectMessageReceiver class shows how to handle messages sent by SelectedObjectMessageSender.
    /// This particular implementatoin controls object appearance by changing its color when selected.
    /// </summary>
    public class SelectedObjectMessageReceiver : MonoBehaviour
    {
#if UNITY_WSA
        [Tooltip("Object color changes to this when selected.")] public Color SelectedColor = Color.red;

        private Material material;
        private Color originalColor;

        private void Start()
        {
            material = GetComponent<Renderer>().material;
            originalColor = material.color;
        }

        public void OnSelectObject()
        {
            material.color = SelectedColor;
        }

        public void OnClearSelection()
        {
            material.color = originalColor;
        }
#endif
    }
}

