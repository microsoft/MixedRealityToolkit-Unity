// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement to react to focus changed events.
    /// </summary>
    public interface IMixedRealityFocusChangedHandler : IEventSystemHandler
    {
        /// <summary>
        /// Focus event that is raised before the focus is actually changed.
        /// <remarks>Useful for logic that needs to take place before focus changes.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnBeforeFocusChange(FocusEventData eventData);

        /// <summary>
        /// Focus event that is raised when the focused object is changed.
        /// </summary>
        /// <param name="eventData"></param>
        void OnFocusChanged(FocusEventData eventData);
    }
}
