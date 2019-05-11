// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Implements the Focus Provider for handling focus of pointers.
    /// </summary>
    public interface IMixedRealityFocusProvider : IMixedRealitySourceStateHandler, IMixedRealityDataProvider, IMixedRealitySpeechHandler
    {
        /// <summary>
        /// Maximum distance at which all pointers can collide with a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>, unless it has an override extent.
        /// </summary>
        float GlobalPointingExtent { get; }

        /// <summary>
        /// The layer masks for the focus pointers to raycast against.
        /// </summary>
        LayerMask[] FocusLayerMasks { get; }

        /// <summary>
        /// The Camera the <see href="https://docs.unity3d.com/ScriptReference/EventSystems.EventSystem.html">EventSystem</see> uses to raycast against.
        /// </summary>
        /// <remarks>Every uGUI canvas in your scene should use this camera as its event camera.</remarks>
        Camera UIRaycastCamera { get; }

        /// <summary>
        /// Gets the currently focused object for the pointing source.
        /// </summary>
        /// <remarks>If the pointing source is not registered, then the Gaze's Focused <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> is returned.</remarks>
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

        /// <summary>
        /// Provides access to all registered pointers of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of pointers to request. Use IMixedRealityPointer to access all pointers.</typeparam>
        /// <returns></returns>
        IEnumerable<T> GetPointers<T>() where T : class, IMixedRealityPointer;
    }
}