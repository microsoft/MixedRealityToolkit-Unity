// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// All event systems providing event propagation phase subscription functionality should derive from this interface
    /// </summary>
    public interface IMixedRealityEventPropagationSystem
    {
        /// <summary>
        /// Interface for subscribing handlers to a specific phase of the event propagation
        /// </summary>
        void RegisterPropagationHandler<T>(IEventSystemHandler handler, PropagationPhase phase = PropagationPhase.BubbleUp) where T : IEventSystemHandler;

        /// <summary>
        /// Interface for unsubscribing handlers to a specific phase of the event propagation 
        /// </summary>
        void UnregisterPropagationHandler<T>(IEventSystemHandler handler, PropagationPhase phase = PropagationPhase.BubbleUp) where T : IEventSystemHandler;
    }
}
