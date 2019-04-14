// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    public class ColorTap : MonoBehaviour, IMixedRealityFocusHandler, IMixedRealityPointerHandler
    {
        [SerializeField]
        private MixedRealityInputAction tapAction = MixedRealityInputAction.None;

        [SerializeField]
        private Color color_IdleState = Color.cyan;

        [SerializeField]
        private Color color_OnHover = Color.white;

        [SerializeField]
        private Color color_OnSelect = Color.blue;

        private Material material;

        private void Awake()
        {
            material = GetComponent<Renderer>().material;
        }

        void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData)
        {
            material.color = color_OnHover;
        }

        void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
        {
            material.color = color_IdleState;
        }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.MixedRealityInputAction == tapAction)
            {
                material.color = color_OnHover;
            }
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (eventData.MixedRealityInputAction == tapAction)
            {
                material.color = color_OnSelect;
            }
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }
    }
}