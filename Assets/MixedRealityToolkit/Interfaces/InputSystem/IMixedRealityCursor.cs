// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Cursor Interface for handling input events and setting visibility.
    /// </summary>
    public interface IMixedRealityCursor : IMixedRealityFocusChangedHandler, IMixedRealitySourceStateHandler, IMixedRealityPointerHandler
    {
        /// <summary>
        /// The <see cref="IMixedRealityPointer"/> this <see cref="IMixedRealityCursor"/> is associated with.
        /// </summary>
        IMixedRealityPointer Pointer { get; set; }

        /// <summary>
        /// Surface distance to place the cursor off of the surface at
        /// </summary>
        float SurfaceCursorDistance { get; }

        /// <summary>
        /// The maximum distance the cursor can be with nothing hit
        /// </summary>
        float DefaultCursorDistance { get; set; }

        /// <summary>
        /// Position of the <see cref="IMixedRealityCursor"/>.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Rotation of the <see cref="IMixedRealityCursor"/>.
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// Local scale of the <see cref="IMixedRealityCursor"/>.
        /// </summary>
        Vector3 LocalScale { get; }

        /// <summary>
        /// Sets the visibility of the <see cref="IMixedRealityCursor"/>.
        /// </summary>
        /// <param name="visible">True if cursor should be visible, false if not.</param>
        void SetVisibility(bool visible);

        /// <summary>
        /// Utility method to destroy cursor dependencies (e.g. event subscriptions) in the system
        /// explicitly in the middle update loop. This is a "replacement" of Unity OnDestroy.
        /// Relying on Unity OnDestroy may cause event handler subscription to 
        /// become invalid at the point of destroying.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Is the cursor currently visible?
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Sets the visibility of the <see cref="IMixedRealityCursor"/> when the source is detected.
        /// </summary>
        bool SetVisibilityOnSourceDetected { get; set; }

        /// <summary>
        /// Returns the <see cref="IMixedRealityCursor"/>'s <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> reference.
        /// </summary>
        /// <returns>The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> this <see cref="IMixedRealityCursor"/> component is attached to.</returns>
        GameObject GameObjectReference { get; }
    }
}
