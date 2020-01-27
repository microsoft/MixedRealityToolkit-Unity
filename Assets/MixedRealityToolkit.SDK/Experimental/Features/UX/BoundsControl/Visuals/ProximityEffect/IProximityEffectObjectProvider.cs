// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Interface for defining a proximity object provider used in <see cref="ProximityEffect" /> of <see cref="BoundsControl" />
    /// ProximityEffectObjectProviders are responsible for providing gameobjects that are scaled / visually altered in ProximityEffect.
    /// </summary>
    public interface IProximityEffectObjectProvider
    {
        /// <summary>
        /// Returns true if the provider has any visible objects
        /// </summary>
        bool IsActive();

        /// <summary>
        /// Base Material is applied to any proximity scaled object whenever in medium or far/no proximity mode
        /// </summary>
        Material GetBaseMaterial();

        /// <summary>
        /// Provides the highlighted material that gets applied to any proximity scaled object in close proximity mode
        /// </summary>
        Material GetHighlightedMaterial();

        /// <summary>
        /// Returns the original object size of the provided proximity scaled objects
        /// </summary>
        float GetObjectSize();

        /// <summary>
        /// This method allows iterating over the proximity scaled visuals. It should return the gameobject the scaling should be applied to.
        /// </summary>
        void ForEachProximityObject(Action<Transform> action);
    }
}
