// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem
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
        /// Is the cursor currently visible?
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Sets the visibility of the <see cref="IMixedRealityCursor"/> when the source is detected.
        /// </summary>
        bool SetVisibilityOnSourceDetected { get; set; }

        /// <summary>
        /// Returns the <see cref="IMixedRealityCursor"/>'s <see cref="GameObject"/> reference.
        /// </summary>
        /// <returns>The <see cref="GameObject"/> this <see cref="IMixedRealityCursor"/> component is attached to.</returns>
        GameObject GameObjectReference { get; }
    }
}
