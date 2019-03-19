// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Interface to implement reacting to placement of objects.
    /// </summary>
    public interface IMixedRealityPlacementHandler : IMixedRealityEventHandler
    {
        void OnPlacingStarted(PlacementEventData eventData);

        void OnPlacingCompleted(PlacementEventData eventData);
    }
}