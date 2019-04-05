// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement to react to source state changes, such as when an input source is detected or lost.
    /// </summary>
    public interface IMixedRealitySourceStateHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when a source is detected.
        /// </summary>
        /// <param name="eventData"></param>
        void OnSourceDetected(SourceStateEventData eventData);

        /// <summary>
        /// Raised when a source is lost.
        /// </summary>
        /// <param name="eventData"></param>
        void OnSourceLost(SourceStateEventData eventData);
    }
}
