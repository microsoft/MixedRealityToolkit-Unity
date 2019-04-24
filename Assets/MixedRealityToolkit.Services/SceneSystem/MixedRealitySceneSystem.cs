// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// The default implementation of the <see cref="Microsoft.MixedReality.Toolkit.SceneSystem.IMixedRealitySceneSystem"/>
    /// </summary>
    public class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem
    {
        public MixedRealitySceneSystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealitySceneSystemProfile profile) : base(registrar, profile)
        {

        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Boundary Managers to compare to.
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Scene System";
    }
}