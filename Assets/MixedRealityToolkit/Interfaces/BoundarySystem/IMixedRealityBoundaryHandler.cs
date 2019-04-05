// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    public interface IMixedRealityBoundaryHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when the boundary visualization has changed.
        /// </summary>
        /// <param name="eventData"></param>
        void OnBoundaryVisualizationChanged(BoundaryEventData eventData);
    }
}
