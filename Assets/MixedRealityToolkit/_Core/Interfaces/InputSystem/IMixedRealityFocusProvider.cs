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
        /// <summary>
        /// Maximum distance at which all pointers can collide with a <see cref="GameObject"/>, unless it has an override extent.
        /// </summary>
        float GlobalPointingExtent { get; }

        /// <summary>
        /// The Camera the <see cref="UnityEngine.EventSystems.EventSystem"/> uses to raycast against.
        /// <para><remarks>Every uGUI canvas in your scene should use this camera as its event camera.</remarks></para>
        /// </summary>
        Camera UIRaycastCamera { get; }

        /// <summary>
        /// To tap on a hologram even when not focused on,
        /// set OverrideFocusedObject to desired game object.
        /// If it's null, then focused object will be used.
        /// </summary>
        GameObject OverrideFocusedObject { get; set; }

        /// <summary>
        /// Gets the currently focused object based on specified the event data.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns>Currently focused <see cref="GameObject"/> for the events input source.</returns>
        GameObject GetFocusedObject(BaseInputEventData eventData);

        /// <summary>
        /// Try to get the focus details based on the specified event data.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="focusDetails"></param>
        /// <returns>True, if event data pointer input source is registered.</returns>
        bool TryGetFocusDetails(BaseInputEventData eventData, out FocusDetails focusDetails);

        /// <summary>
        /// Try to get the registered pointer source that raised the event.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="pointer"></param>
        /// <returns>True, if event data's pointer input source is registered.</returns>
        bool TryGetPointingSource(BaseInputEventData eventData, out IMixedRealityPointer pointer);

        /// <summary>
        /// Gets the currently focused object for the pointing source.
        /// <para><remarks>If the pointing source is not registered, then the Gaze's Focused <see cref="GameObject"/> is returned.</remarks></para>
        /// </summary>
        /// <param name="pointingSource"></param>
        /// <returns>Currently Focused Object.</returns>
        GameObject GetFocusedObject(IMixedRealityPointer pointingSource);

        /// <summary>
        /// Gets the currently focused object for the pointing source.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="focusDetails"></param>
        bool TryGetFocusDetails(IMixedRealityPointer pointer, out FocusDetails focusDetails);

        /// <summary>
        /// Get the Graphic Event Data for the specified pointing source.
        /// </summary>
        /// <param name="pointer">The pointer who's graphic event data we're looking for.</param>
        /// <param name="graphicInputEventData">The graphihc event data for the specified pointer</param>
        /// <returns>True, if graphic event data exists.</returns>
        bool TryGetSpecificPointerGraphicEventData(IMixedRealityPointer pointer, out GraphicInputEventData graphicInputEventData);

        /// <summary>
        /// Generate a new unique pointer id.
        /// </summary>
        /// <returns></returns>
        uint GenerateNewPointerId();

        /// <summary>
        /// Checks if the pointer is registered with the Focus Manager.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns>True, if registered, otherwise false.</returns>
        bool IsPointerRegistered(IMixedRealityPointer pointer);

        /// <summary>
        /// Registers the pointer with the Focus Manager.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns>True, if the pointer was registered, false if the pointer was previously registered.</returns>
        bool RegisterPointer(IMixedRealityPointer pointer);

        /// <summary>
        /// Unregisters the pointer with the Focus Manager.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns>True, if the pointer was unregistered, false if the pointer was not registered.</returns>
        bool UnregisterPointer(IMixedRealityPointer pointer);
    }
}