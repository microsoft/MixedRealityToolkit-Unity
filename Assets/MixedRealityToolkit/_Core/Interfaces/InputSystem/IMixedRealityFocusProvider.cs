// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
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
        GameObject OverrideFocusedObject { get; }
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

    public interface ICursorModifier : IMixedRealityFocusChangedHandler
    {
        Transform HostTransform { get; set; }
        Vector3 CursorOffset { get; set; }

        /// <summary>
        /// Cursor animation parameters to set when this object is focused. Leave empty for none.
        /// </summary>
        AnimatorParameter[] CursorParameters { get; }

        /// <summary>
        /// Indicates whether the cursor should be visible or not.
        /// </summary>
        /// <returns>True if cursor should be visible, false if not.</returns>
        bool GetCursorVisibility();

        /// <summary>
        /// Returns the cursor position after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <returns>New position for the cursor</returns>
        Vector3 GetModifiedPosition(IMixedRealityCursor cursor);

        /// <summary>
        /// Returns the cursor rotation after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <returns>New rotation for the cursor</returns>
        Quaternion GetModifiedRotation(IMixedRealityCursor cursor);

        /// <summary>
        /// Returns the cursor local scale after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <returns>New local scale for the cursor</returns>
        Vector3 GetModifiedScale(IMixedRealityCursor cursor);

        /// <summary>
        /// Returns the modified transform for the cursor after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <param name="position">Modified position.</param>
        /// <param name="rotation">Modified rotation.</param>
        /// <param name="scale">Modified scale.</param>
        void GetModifiedTransform(IMixedRealityCursor cursor, out Vector3 position, out Quaternion rotation, out Vector3 scale);
    }

    public interface ITeleportTarget : IMixedRealityFocusChangedHandler
    {
        Vector3 Position { get; }
        Vector3 Normal { get; }
        bool IsActive { get; }
        bool OverrideTargetOrientation { get; }
        float TargetOrientation { get; }
    }

    public interface IPointerResult { }

    public interface IBaseRayStabilizer
    {
        Vector3 StablePosition { get; }
        Quaternion StableRotation { get; }
        Ray StableRay { get; }
        void UpdateStability(Vector3 position, Quaternion rotation);
        void UpdateStability(Vector3 position, Vector3 direction);
    }
}