// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.HandPhysics
{
    /// <summary>
    /// Generic interface for applying rigidbodies to hand joints
    /// </summary>
    public interface IHandPhysicsService : IMixedRealityExtensionService
    {
        /// <summary>
        /// The parent GameObject that contains all the physics joints
        /// </summary>
        GameObject HandPhysicsServiceRoot { get; }

        /// <summary>
        /// The LayerMask the physics joints will be on
        /// </summary>
        int HandPhysicsLayer { get; set; }

        /// <summary>
        /// Whether to make the palm a physics joint
        /// </summary>
        bool UsePalmKinematicBody { get; set; }

        /// <summary>
        /// The prefab to represent each physics joints
        /// </summary>
        GameObject FingerTipKinematicBodyPrefab { get; set; }

        /// <summary>
        /// The prefab to represent the Palm physics joints
        /// </summary>
        GameObject PalmKinematicBodyPrefab { get; set; }
    }
}