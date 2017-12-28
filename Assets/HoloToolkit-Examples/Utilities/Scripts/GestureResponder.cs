// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Tests;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class GestureResponder : MonoBehaviour, IInputClickHandler
    {
        private void Start()
        {
            InputManager.Instance.PushFallbackInputHandler(gameObject);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            PlaneTargetGroupPicker.Instance.PickNewTarget();
        }
    }
}
