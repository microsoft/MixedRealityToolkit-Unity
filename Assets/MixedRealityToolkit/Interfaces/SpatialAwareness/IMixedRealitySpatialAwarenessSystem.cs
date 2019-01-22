// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Indicates the fallback behavior for observers when not explicitly set.
        /// </summary>
        AutoStartBehavior DefaultObserverStartupBehavior { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        GameObject SpatialAwarenessParent { get; }

        // TODO

        ///// <summary>
        ///// //todo Starts / restarts the spatial observer.
        ///// </summary>
        ///// <remarks>This will cause spatial awareness events to resume.</remarks>
        //void ResumeObservers<T>();

        ///// <summary>
        ///// //todo Starts / restarts the spatial observer.
        ///// </summary>
        ///// <remarks>This will cause spatial awareness events to resume.</remarks>
        //void ResumeObserver<T>();

        ///// <summary>
        ///// //todo Starts / restarts the spatial observer.
        ///// </summary>
        ///// <remarks>This will cause spatial awareness events to resume.</remarks>
        //void ResumeObserver<T>(int id);

        ///// <summary>
        ///// //todo Stops / pauses the spatial observer.
        ///// </summary>
        ///// <remarks>This will cause spatial awareness events to be suspended until ResumeObserver is called.</remarks>
        //void SuspendObservers<T>();

        ///// <summary>
        ///// //todo Stops / pauses the spatial observer.
        ///// </summary>
        ///// <remarks>This will cause spatial awareness events to be suspended until ResumeObserver is called.</remarks>
        //void SuspendObserver<T>();

        ///// <summary>
        ///// //todo Stops / pauses the spatial observer.
        ///// </summary>
        ///// <remarks>This will cause spatial awareness events to be suspended until ResumeObserver is called.</remarks>
        //void SuspendObserver<T>(int id);


        ///// <summary>
        ///// //todo returns all given IMixedRealitySpatialAwarenessObservers
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        //T GetObservers<T>();

        ///// <summary>
        ///// //todo returns first IMixedRealitySpatialAwarenessObserver
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        //T GetObserver<T>();

        ///// <summary>
        ///// //todo returns IMixedRealitySpatialAwarenessObserver matching given id
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        //T GetObserver<T>(int id);

        /// <summary>
        /// Generates a new source identifier for an <see cref="IMixedRealitySpatialAwarenessObserver"/> implementation.
        /// </summary>
        /// <returns>The source identifier to be used by the <see cref="IMixedRealitySpatialAwarenessObserver"/> implementation.</returns>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, and not by application code.
        /// </remarks>
        uint GenerateNewSourceId();
    }
}
