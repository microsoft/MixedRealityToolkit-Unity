// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TestOverrideFocusedObject : MonoBehaviour, IPointerHandler
    {
        private TextMesh textMesh;

        private void Start()
        {
            FocusManager.Instance.OverrideFocusedObject = gameObject;

            textMesh = FindObjectOfType<TextMesh>();
        }

        public void OnPointerUp(ClickEventData eventData) { }

        public void OnPointerDown(ClickEventData eventData) { }

        public void OnPointerClicked(ClickEventData eventData)
        {
            if (textMesh != null)
            {
                textMesh.text = "Air tap worked and OverrideFocusedObject is null.";
                FocusManager.Instance.OverrideFocusedObject = null;
            }
        }
    }
}