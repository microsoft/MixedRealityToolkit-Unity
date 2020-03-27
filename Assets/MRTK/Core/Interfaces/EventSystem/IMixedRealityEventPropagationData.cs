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
    public enum EventPropagation // Mainly used to build Paths
    {
        None = 0,
        BubblesUp = 1, // Target up to root
        TricklesDown = 2, // Root down to target
        Cancellable = 4, // ?
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
    /// Phase of event propagation through a hierarchy of elements
    /// </summary>
    [System.Flags]
    public enum LifeStatus
    {
        None = 0,
        PropagationStopped = 1,
        ImmediatePropagationStopped = 2,
        DefaultPrevented = 4,
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
        /// Stop propagation through hierarnchy elements. Does not prevent event to be handled by target.
        /// </summary>
        void StopPropagation();

        /// <summary>
        /// Stop any other CallBackss registered. 
        /// At this point, stopping propagation imeddiately does not make sense for MRTK.
        /// MRTK Input distacher is based on calling Events with Unity's ExecuteEvents.Execute, calling all callbacks on target object.
        /// </summary> 
        void StopPropagationImmediately();

        /// <summary>
        /// Checks if object should skip propagation.
        /// </summary>
        bool Skip(GameObject gameObject);
    }
}
