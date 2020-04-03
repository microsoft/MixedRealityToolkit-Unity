// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Direction of event propagation through a hierarchy of elements
    /// </summary>
    [System.Flags]
    public enum EventPropagation // Mainly used to build propagation paths in the event dispatcher
    {
        None = 0 << 0,
        BubblesUp = 1 << 0, // Target up to root
        TricklesDown = 1 << 1 // Root down to target
    }

    /// <summary>
    /// Current phase of event propagation through a hierarchy of elements
    /// </summary>
    public enum PropagationPhase
    {
        None,
        Target, // Event being handled by target object
        TrickleDown, // Propagating from root object down to target object
        BubbleUp // Propagating from target object up to target root object
    }

    /// <summary>
    /// Phase of event propagation through a hierarchy of elements
    /// </summary>
    [System.Flags]
    public enum LifeStatus
    {
        None = 0 << 0,
        PropagationStopped = 1 << 0,
        PropagationStoppedImmediately = 1 << 1,
        DefaultPrevented = 1 << 2,
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

        /// <summary>
        /// Current life cycle status of event
        /// </summary>
        LifeStatus Status { get; set; }

        /// <summary>
        /// Prevent event to be handled by the target
        /// </summary>
        void PreventDefault();

        /// <summary>
        /// Stop propagation through hierarchy elements. Does not prevent event to be handled by target.
        /// </summary>
        void StopPropagation();

        /// <summary>
        /// Stop propagation through hierarchy elements and to any other handlers on the current target object.
        /// Does not prevent event to be handled by target.
        /// </summary> 
        void StopPropagationImmediately();

        /// <summary>
        /// Checks if object should skip propagation.
        /// </summary>
        bool Skip(GameObject gameObject);
    }
}
