// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    /// <summary>
    /// Implements the Focus Provider for handling focus of pointers.
    /// </summary>
    public interface IMixedRealityFocusProvider : IMixedRealitySourceStateHandler
    {
        float GlobalPointingExtent { get; }
        Camera UIRaycastCamera { get; }
        GameObject OverrideFocusedObject { get; set; }
        GameObject GetFocusedObject(BaseInputEventData eventData);
        bool TryGetFocusDetails(BaseInputEventData eventData, out FocusDetails focusDetails);
        bool TryGetPointingSource(BaseInputEventData eventData, out IMixedRealityPointer pointer);
        GameObject GetFocusedObject(IMixedRealityPointer pointingSource);
        bool TryGetFocusDetails(IMixedRealityPointer pointer, out FocusDetails focusDetails);
        GraphicInputEventData GetSpecificPointerGraphicEventData(IMixedRealityPointer pointer);
        uint GenerateNewPointerId();
        bool IsPointerRegistered(IMixedRealityPointer pointer);
        bool RegisterPointer(IMixedRealityPointer pointer);
        bool UnregisterPointer(IMixedRealityPointer pointer);
    }
}