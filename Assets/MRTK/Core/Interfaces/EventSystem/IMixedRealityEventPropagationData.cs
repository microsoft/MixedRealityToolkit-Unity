// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Direction of event propagation through a hierarchy of elements
    /// </summary>
    [System.Flags]
    public enum EventPropagation
    {
        None = 0,
        BubblesUp = 1, // Target up to root
        TricklesDown = 2, // Root down to target
    }

    /// <summary>
    /// Phase of event propagation through a hierarchy of elements
    /// </summary>
    public enum PropagationPhase
    {
        None,
        Target,
        TrickeDown,
        BubbleUp,
    }

    /// <summary>
    /// Interface that allows event data types to support propagation configuration
    /// </summary>
    public interface IMixedRealityEventPropagationData
    {
        /// <summary>
        /// Propagation phases supported by the event
        /// </summary>
        EventPropagation Propagation { get; set; }

        /// <summary>
        /// Current phase of the event propagation
        /// </summary>
        PropagationPhase Phase { get; set; }
    }
}
