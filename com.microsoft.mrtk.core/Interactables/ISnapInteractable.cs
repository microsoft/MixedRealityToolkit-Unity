// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Interactables that represent a handle-like affordance should implement this
    /// interface, such that interactors can snap their visuals/ray/etc directly to the
    /// affordance instead of using the typical local offset.
    /// </summary>
    public interface ISnapInteractable
    {
        /// <summary>
        /// Called by interactors to query which exact transform on an interactable
        /// should be considered the snappable affordance.
        /// </summary>
        /// <remarks>
        /// For example, sliders return the sliding handle transform.
        /// </remarks>
        Transform HandleTransform { get; }
    }
}