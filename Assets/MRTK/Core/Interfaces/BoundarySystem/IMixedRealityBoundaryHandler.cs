// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    public interface IMixedRealityBoundaryHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when the boundary visualization has changed.
        /// </summary>
        void OnBoundaryVisualizationChanged(BoundaryEventData eventData);
    }
}
