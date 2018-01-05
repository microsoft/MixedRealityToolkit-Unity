// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Tests;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class GestureResponder : MonoBehaviour, IPointerHandler
    {
        private void Start()
        {
            InputManager.Instance.PushFallbackInputHandler(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData) { }

        public void OnPointerDown(PointerEventData eventData) { }

        public void OnPointerClicked(PointerEventData eventData)
        {
            PlaneTargetGroupPicker.Instance.PickNewTarget();
        }
    }
}
