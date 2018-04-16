// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.EventData;
using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using Microsoft.MixedReality.Toolkit.InputSystem.InputHandlers;
using Microsoft.MixedReality.Toolkit.InputSystem.InputSources;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    public interface IGazeProvider : IInputSource { }

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

    /// <summary>
    /// Generic interface for all Mixed Reality Managers
    /// </summary>
    public interface IMixedRealityManager
    {
        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Optional Reset function to perform that will Reset the manager, for example, whenever there is a profile change.
        /// </summary>
        void Reset();

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager.
        /// </summary>
        void Update();

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed.
        /// </summary>
        void Destroy();
    }
}
