// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement to react to simple pointer input.
    /// </summary>
    public interface IMixedRealityPointerHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a pointer down event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        void OnPointerDown(MixedRealityPointerEventData eventData);

        /// <summary>
        /// Called every frame a pointer is down. Can be used to implement drag-like behaviors.
        /// </summary>
        void OnPointerDragged(MixedRealityPointerEventData eventData);

        /// <summary>
        /// When a pointer up event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        void OnPointerUp(MixedRealityPointerEventData eventData);

        /// <summary>
        /// When a pointer clicked event is raised, this method is used to pass along the event data to the input handler.
        /// </summary>
        void OnPointerClicked(MixedRealityPointerEventData eventData);
    }
}