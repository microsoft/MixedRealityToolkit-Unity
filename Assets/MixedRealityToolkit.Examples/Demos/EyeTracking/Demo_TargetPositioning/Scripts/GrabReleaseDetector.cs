// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    public class GrabReleaseDetector : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField]
        private UnityEvent OnGrab = null;

        [SerializeField]
        private UnityEvent OnRelease = null;

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            Debug.Log("OnGrab");
            OnGrab.Invoke();
        }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            Debug.Log("OnRelease");
            OnRelease.Invoke();
        }
    }
}