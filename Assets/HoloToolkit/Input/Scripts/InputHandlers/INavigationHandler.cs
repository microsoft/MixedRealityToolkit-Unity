// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to navigation gestures.
    /// </summary>
    public interface INavigationHandler : IEventSystemHandler
    {
        void OnNavigationStarted(NavigationEventData eventData);
        void OnNavigationUpdated(NavigationEventData eventData);
        void OnNavigationCompleted(NavigationEventData eventData);
        void OnNavigationCanceled(NavigationEventData eventData);
    }
}
