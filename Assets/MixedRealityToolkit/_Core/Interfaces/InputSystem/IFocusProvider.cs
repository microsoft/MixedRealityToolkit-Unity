// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatas.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
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

    public interface ICursorModifier : IFocusChangedHandler
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
        Vector3 GetModifiedPosition(ICursor cursor);

        /// <summary>
        /// Returns the cursor rotation after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <returns>New rotation for the cursor</returns>
        Quaternion GetModifiedRotation(ICursor cursor);

        /// <summary>
        /// Returns the cursor local scale after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <returns>New local scale for the cursor</returns>
        Vector3 GetModifiedScale(ICursor cursor);

        /// <summary>
        /// Returns the modified transform for the cursor after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <param name="position">Modified position.</param>
        /// <param name="rotation">Modified rotation.</param>
        /// <param name="scale">Modified scale.</param>
        void GetModifiedTransform(ICursor cursor, out Vector3 position, out Quaternion rotation, out Vector3 scale);
    }

    public interface ITeleportTarget : IFocusChangedHandler
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