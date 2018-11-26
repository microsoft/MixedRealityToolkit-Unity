// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to manipulation gestures.
    /// </summary>
    public interface IManipulationHandler : IEventSystemHandler
    {
        void OnManipulationStarted(ManipulationEventData eventData);
        void OnManipulationUpdated(ManipulationEventData eventData);
        void OnManipulationCompleted(ManipulationEventData eventData);
        void OnManipulationCanceled(ManipulationEventData eventData);
    }
}
