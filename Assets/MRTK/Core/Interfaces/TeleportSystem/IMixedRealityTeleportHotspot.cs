// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Teleport
{
    public interface IMixedRealityTeleportHotspot
    {
        /// <summary>
        /// The position the teleport will end at.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// The normal of the teleport raycast.
        /// </summary>
        Vector3 Normal { get; }

        /// <summary>
        /// Determines whether the teleport target is active
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Whether to override the user's rotation on the y-axis with the hotspots TargetRotation
        /// </summary>
        bool OverrideOrientation { get; }

        /// <summary>
        /// The rotation in angles around the y axis to set the user after teleport
        /// </summary>
        float TargetRotation { get; }

        /// <summary>
        /// Returns the <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> reference for this teleport target.
        /// </summary>
        GameObject GameObjectReference { get; }
    }
}