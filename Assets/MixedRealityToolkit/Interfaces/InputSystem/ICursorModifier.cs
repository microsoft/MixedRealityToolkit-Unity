// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for cursor modifiers that can modify a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>'s properties.
    /// </summary>
    public interface ICursorModifier : IMixedRealityFocusChangedHandler
    {
        /// <summary>
        /// Transform for which this <see cref="IMixedRealityCursor"/> modifies applies its various properties.
        /// </summary>
        Transform HostTransform { get; set; }

        /// <summary>
        /// How much a <see cref="IMixedRealityCursor"/>'s position should be offset from the surface of the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> when overlapping.
        /// </summary>
        Vector3 CursorPositionOffset { get; set; }

        /// <summary>
        /// Should the <see cref="IMixedRealityCursor"/> snap to the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>'s position?
        /// </summary>
        bool SnapCursorPosition { get; set; }

        /// <summary>
        /// Scale of the <see cref="IMixedRealityCursor"/> when looking at this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>.
        /// </summary>
        Vector3 CursorScaleOffset { get; set; }

        /// <summary>
        /// Direction of the <see cref="IMixedRealityCursor"/> offset.
        /// </summary>
        Vector3 CursorNormalOffset { get; set; }

        /// <summary>
        /// If true, the normal from the pointing vector will be used to orient the <see cref="IMixedRealityCursor"/> instead of the targeted <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>'s normal at point of contact.
        /// </summary>
        bool UseGazeBasedNormal { get; set; }

        /// <summary>
        /// Should the <see cref="IMixedRealityCursor"/> be hidden when this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> is focused?
        /// </summary>
        bool HideCursorOnFocus { get; set; }

        /// <summary>
        /// <see cref="IMixedRealityCursor"/> animation parameters to set when this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> is focused. Leave empty for none.
        /// </summary>
        AnimatorParameter[] CursorParameters { get; }

        /// <summary>
        /// Indicates whether the <see cref="IMixedRealityCursor"/> should be visible or not.
        /// </summary>
        /// <returns>True if <see cref="IMixedRealityCursor"/> should be visible, false if not.</returns>
        bool GetCursorVisibility();

        /// <summary>
        /// Returns the <see cref="IMixedRealityCursor"/> position after considering this modifier.
        /// </summary>
        /// <param name="cursor"><see cref="IMixedRealityCursor"/> that is being modified.</param>
        /// <returns>New position for the <see cref="IMixedRealityCursor"/></returns>
        Vector3 GetModifiedPosition(IMixedRealityCursor cursor);

        /// <summary>
        /// Returns the <see cref="IMixedRealityCursor"/> rotation after considering this modifier.
        /// </summary>
        /// <param name="cursor"><see cref="IMixedRealityCursor"/> that is being modified.</param>
        /// <returns>New rotation for the <see cref="IMixedRealityCursor"/></returns>
        Quaternion GetModifiedRotation(IMixedRealityCursor cursor);

        /// <summary>
        /// Returns the <see cref="IMixedRealityCursor"/>'s local scale after considering this modifier.
        /// </summary>
        /// <param name="cursor"><see cref="IMixedRealityCursor"/> that is being modified.</param>
        /// <returns>New local scale for the <see cref="IMixedRealityCursor"/></returns>
        Vector3 GetModifiedScale(IMixedRealityCursor cursor);

        /// <summary>
        /// Returns the modified <see href="https://docs.unity3d.com/ScriptReference/Transform.html">Transform</see> for the <see cref="IMixedRealityCursor"/> after considering this modifier.
        /// </summary>
        /// <param name="cursor">Cursor that is being modified.</param>
        /// <param name="position">Modified position.</param>
        /// <param name="rotation">Modified rotation.</param>
        /// <param name="scale">Modified scale.</param>
        void GetModifiedTransform(IMixedRealityCursor cursor, out Vector3 position, out Quaternion rotation, out Vector3 scale);
    }
}