// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessSystem : IMixedRealityEventSystem
    {
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

        // TODO: make these events more generic

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshAdded"/> method to indicate a mesh has been added.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshAdded(IMixedRealitySpatialAwarenessObserver observer, int meshId, GameObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshUpdated"/> method to indicate an existing mesh has been updated.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshUpdated(IMixedRealitySpatialAwarenessObserver observer, int meshId, GameObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshUpdated"/> method to indicate an existing mesh has been removed.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshRemoved(IMixedRealitySpatialAwarenessObserver observer, int meshId);

    }
}
