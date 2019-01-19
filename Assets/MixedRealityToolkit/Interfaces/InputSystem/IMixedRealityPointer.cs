// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Physics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.TeleportSystem;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem
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
        /// The currently active teleport hotspot.
        /// </summary>
        IMixedRealityTeleportHotSpot TeleportHotSpot { get; set; }

        /// <summary>
        /// Has the conditions for the interaction been satisfied to enable the interaction?
        /// </summary>
        bool IsInteractionEnabled { get; }

        /// <summary>
        /// Is the focus for this pointer currently locked?
        /// </summary>
        bool IsFocusLocked { get; set; }

        /// <summary>
        /// The pointer's maximum extent when raycasting.
        /// </summary>
        float PointerExtent { get; set; }

        /// <summary>
        /// The raycast rays.
        /// </summary>
        RayStep[] Rays { get; }

        /// <summary>
        /// The physics layers to use when raycasting.
        /// </summary>
        /// <remarks>If set, will override the <see cref="IMixedRealityInputSystem"/>'s default raycasting layer mask array.
        /// </remarks>
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
        /// The physics raycast pointer result.
        /// </summary>
        IPointerResult Result { get; set; }

        /// <summary>
        /// Ray stabilizer used when calculating position of pointer end point.
        /// </summary>
        IBaseRayStabilizer RayStabilizer { get; set; }

        /// <summary>
        /// The physics raycast mode to use.
        /// </summary>
        RaycastMode RaycastMode { get; set; }

        /// <summary>
        /// The radius to use when <see cref="RaycastMode"/> is set to Sphere.
        /// </summary>
        float SphereCastRadius { get; set; }

        /// <summary>
        /// The Y orientation of the pointer - used for touchpad rotation and navigation
        /// </summary>
        float PointerOrientation { get; }

        /// <summary>
        /// Called before all rays have casted.
        /// </summary>
        void OnPreRaycast();

        /// <summary>
        /// Called after all rays have casted.
        /// </summary>
        void OnPostRaycast();

        /// <summary>
        /// Returns the position of the input source, if available.
        /// Not all input sources support positional information, and those that do may not always have it available.
        /// </summary>
        /// <param name="position">Out parameter filled with the position if available, otherwise <see cref="Vector3.zero"/>.</param>
        /// <returns>True if a position was retrieved, false if not.</returns>
        bool TryGetPointerPosition(out Vector3 position);

        /// <summary>
        /// Returns the pointing ray of the input source, if available.
        /// Not all input sources support pointing information, and those that do may not always have it available.
        /// </summary>
        /// <param name="pointingRay">Out parameter filled with the pointing ray if available.</param>
        /// <returns>True if a pointing ray was retrieved, false if not.</returns>
        bool TryGetPointingRay(out Ray pointingRay);

        /// <summary>
        /// Returns the rotation of the input source, if available.
        /// Not all input sources support rotation information, and those that do may not always have it available.
        /// </summary>
        /// <param name="rotation">Out parameter filled with the rotation if available, otherwise <see cref="Quaternion.identity"/>.</param>
        /// <returns>True if an rotation was retrieved, false if not.</returns>
        bool TryGetPointerRotation(out Quaternion rotation);
    }
}
