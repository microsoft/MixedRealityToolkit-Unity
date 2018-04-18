// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.EventData;
using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using Microsoft.MixedReality.Toolkit.InputSystem.InputHandlers;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    public interface IFocusProvider : ISourceStateHandler
    {
        float GlobalPointingExtent { get; }
        Camera UIRaycastCamera { get; }
        GameObject OverrideFocusedObject { get; }
        GameObject GetFocusedObject(BaseInputEventData eventData);
        bool TryGetFocusDetails(BaseInputEventData eventData, out FocusDetails focusDetails);
        bool TryGetPointingSource(BaseInputEventData eventData, out IPointer pointer);
        GameObject GetFocusedObject(IPointer pointingSource);
        bool TryGetFocusDetails(IPointer pointer, out FocusDetails focusDetails);
        GraphicInputEventData GetSpecificPointerGraphicEventData(IPointer pointer);
        uint GenerateNewPointerId();
        bool IsPointerRegistered(IPointer pointer);
        bool RegisterPointer(IPointer pointer);
        bool UnregisterPointer(IPointer pointer);
    }
}