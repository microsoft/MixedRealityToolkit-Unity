// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class SlateEnabler : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField]
    private HandInteractionPan panComponent = null;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (panComponent != null)
        {
            panComponent.Enabled = false;
        }
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (panComponent != null)
        {
            panComponent.Enabled = true;
        }
    }
}
