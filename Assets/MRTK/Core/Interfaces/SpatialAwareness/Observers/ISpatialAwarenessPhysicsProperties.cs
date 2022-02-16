// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// The interface for defining an <see cref="ISpatialAwarenessPhysicsProperties"/> which provides physical materials
    /// </summary>
    public interface ISpatialAwarenessPhysicsProperties
    {
        /// <summary>
        /// Gets or sets the <see href="https://docs.unity3d.com/ScriptReference/PhysicMaterial.html">PhysicMaterial</see> to be used when displaying <see href="https://docs.unity3d.com/ScriptReference/Mesh.html">Mesh</see>es.
        /// </summary>
        PhysicMaterial PhysicsMaterial { get; set; }
    }
}