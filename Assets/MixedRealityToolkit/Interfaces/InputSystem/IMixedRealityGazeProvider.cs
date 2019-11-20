// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Implements the Gaze Provider for an Input Source.
    /// </summary>
    public interface IMixedRealityGazeProvider
    {
        /// <summary>
        /// Enable or disable the <see href="https://docs.unity3d.com/ScriptReference/Component.html">Component</see> attached to the <see cref="GameObjectReference"/>
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// The Gaze Input Source for the provider.
        /// </summary>
        IMixedRealityInputSource GazeInputSource { get; }

        /// <summary>
        /// The Gaze Pointer for the provider.
        /// </summary>
        IMixedRealityPointer GazePointer { get; }

        /// <summary>
        /// The prefab to be instantiated as the gaze cursor.
        /// </summary>
        GameObject GazeCursorPrefab { set; }

        /// <summary>
        /// The Gaze Cursor for the provider.
        /// </summary>
        IMixedRealityCursor GazeCursor { get; }

        /// <summary>
        /// The game object that is currently being gazed at, if any.
        /// </summary>
        GameObject GazeTarget { get; }

        /// <summary>
        /// HitInfo property gives access to information at the object being gazed at, if any.
        /// </summary>
        MixedRealityRaycastHit HitInfo { get; }

        /// <summary>
        /// Position at which the gaze manager hit an object.
        /// If no object is currently being hit, this will use the last hit distance.
        /// </summary>
        Vector3 HitPosition { get; }

        /// <summary>
        /// Normal of the point at which the gaze manager hit an object.
        /// If no object is currently being hit, this will return the previous normal.
        /// </summary>
        Vector3 HitNormal { get; }

        /// <summary>
        /// Origin of the gaze.
        /// </summary>
        Vector3 GazeOrigin { get; }

        /// <summary>
        /// Normal of the gaze.
        /// </summary>
        Vector3 GazeDirection { get; }

        /// <summary>
        /// The current head velocity.
        /// </summary>
        Vector3 HeadVelocity { get; }

        /// <summary>
        /// The current head movement direction.
        /// </summary>
        Vector3 HeadMovementDirection { get; }

        /// <summary>
        /// Get the GameObject reference for this Gaze Provider.
        /// </summary>
        GameObject GameObjectReference { get; }

        /// <summary>
        /// Notifies this gaze provider of its new hit details.
        /// </summary>
        /// <remarks>
        /// For components that care where the user's looking, we need
        /// to separately update the gaze info even if gaze isn't used for focus.
        /// </remarks>
        void UpdateGazeInfoFromHit(MixedRealityRaycastHit raycastHit);
    }
}