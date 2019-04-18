// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement to react to simple pointer input and pointer updates.
    /// </summary>
    public interface IMixedRealityPointerUpdatedHandler : IMixedRealityPointerHandler
    {
        /// <summary>
        /// Called every frame a pointer is down. Can be used to implement drag-like behaviors.
        /// </summary>
        /// <param name="eventData"></param>
        void OnPointerUpdated(MixedRealityPointerEventData eventData);
    }
}