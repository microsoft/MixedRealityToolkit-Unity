// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        void OnSourceDetected(SourceStateEventData eventData);

        /// <summary>
        /// Raised when a source is lost.
        /// </summary>
        void OnSourceLost(SourceStateEventData eventData);
    }
}
