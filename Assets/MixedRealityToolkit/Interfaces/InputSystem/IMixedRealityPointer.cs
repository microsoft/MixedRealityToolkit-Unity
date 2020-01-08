// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for handling pointers.
    /// </summary>
    public interface IMixedRealityPointer : IEqualityComparer
    {
        /// <summary>
        /// The pointer's current controller reference.
        /// </summary>
        IMixedRealityController Controller { get; set; }

        /// <summary>
        /// This pointer's id.
        /// </summary>
        uint PointerId { get; }

        /// <summary>
        /// This pointer's name.
        /// </summary>
        string PointerName { get; set; }

        /// <summary>
        /// This pointer's input source parent.
        /// </summary>
        IMixedRealityInputSource InputSourceParent { get; }

        /// <summary>
        /// The pointer's cursor.
        /// </summary>
        IMixedRealityCursor BaseCursor { get; set; }

        /// <summary>
        /// The currently active cursor modifier.
        /// </summary>
        ICursorModifier CursorModifier { get; set; }

        /// <summary>
        /// Is the pointer active and have the conditions for the interaction been satisfied to enable the interaction?
        /// </summary>
        bool IsInteractionEnabled { get; }

        /// <summary>
        /// Controls whether the pointer dispatches input..
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Is the focus for this pointer currently locked?
        /// </summary>
        bool IsFocusLocked { get; set; }

        /// <summary>
        /// Specifies whether the pointer's target position (cursor) is locked to the target object when focus is locked.
        /// </summary>
        bool IsTargetPositionLockedOnFocusLock { get; set; }

        /// <summary>
        /// The scene query rays.
        /// </summary>
        RayStep[] Rays { get; }

        /// <summary>
        /// The physics layers to use when performing scene queries.
        /// </summary>
        /// <remarks>If set, will override the <see cref="IMixedRealityInputSystem"/>'s default scene query layer mask array.</remarks>
        /// <example>
        /// Allow the pointer to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)
        /// <code language="csharp"><![CDATA[
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers &amp; ~sr;
        /// IMixedRealityPointer.PrioritizedLayerMasksOverride = new LayerMask[] { nonSR, sr };
        /// ]]></code>
        /// </example>
        LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        /// <summary>
        /// The currently focused target.
        /// </summary>
        IMixedRealityFocusHandler FocusTarget { get; set; }

        /// <summary>
        /// The scene query pointer result.
        /// </summary>
        IPointerResult Result { get; set; }

        /// <summary>
        /// The type of physics scene query to use.
        /// </summary>
        SceneQueryType SceneQueryType { get; set; }

        /// <summary>
        /// The radius to use when <see cref="SceneQueryType"/> is set to Sphere or SphereColliders.
        /// </summary>
        float SphereCastRadius { get; set; }

        /// <summary>
        /// Pointer position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Pointer rotation.
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// Called before performing the scene query.
        /// </summary>
        void OnPreSceneQuery();

        /// <summary>
        /// Called after performing the scene query.
        /// </summary>
        void OnPostSceneQuery();

        /// <summary>
        /// Called during the scene query just before the current pointer target changes.
        /// </summary>
        void OnPreCurrentPointerTargetChange();

        /// <summary>
        /// Resets pointer to initial state. After invoked pointer should be functional and ready for re-use.
        /// </summary>
        /// <remarks>
        /// Useful for caching and recycling of pointers
        /// </remarks>
        void Reset();
    }
}
