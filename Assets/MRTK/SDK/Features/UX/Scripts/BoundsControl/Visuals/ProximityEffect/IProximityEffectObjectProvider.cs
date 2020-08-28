// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Event to inform subscribers that the proximity objects have changed
    /// </summary>
    public class ProximityObjectsChangedEvent : UnityEvent<IProximityEffectObjectProvider> { }

    /// <summary>
    /// Interface for defining a proximity object provider used in <see cref="ProximityEffect" /> of <see cref="BoundsControl" />
    /// ProximityEffectObjectProviders are responsible for providing gameobjects that are scaled / visually altered in ProximityEffect.
    /// </summary>
    public interface IProximityEffectObjectProvider
    {
        /// <summary>
        /// Returns true if the provider has any visible objects
        /// </summary>
        bool IsActive { get; set; }

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

        /// <summary>
        /// Allow for accessing / subscribing to the changed event for objects that show a proximity effect
        /// </summary>
        ProximityObjectsChangedEvent ProximityObjectsChanged { get; }
    }
}
