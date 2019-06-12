// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public interface IMixedRealitySpatialAwarenessSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Gets the parent object to which all spatial awareness <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s are to be parented.
        /// </summary>
        GameObject SpatialAwarenessObjectParent { get; }

        /// <summary>
        /// Creates the a parent, that is a child if the Spatial Awareness System parent so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to which spatial awareness objects will be parented.
        /// </returns>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, not by application code.
        /// </remarks>
        GameObject CreateSpatialAwarenessObjectParent(string name);

        /// <summary>
        /// Generates a new source identifier for an <see cref="IMixedRealitySpatialAwarenessObserver"/> implementation.
        /// </summary>
        /// <returns>The source identifier to be used by the <see cref="IMixedRealitySpatialAwarenessObserver"/> implementation.</returns>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, not by application code.
        /// </remarks>
        uint GenerateNewSourceId();

        /// <summary>
        /// Typed representation of the ConfigurationProfile property.
        /// </summary>
        MixedRealitySpatialAwarenessSystemProfile SpatialAwarenessSystemProfile { get; }

        /// <summary>
        /// Gets the collection of registered <see cref="IMixedRealitySpatialAwarenessObserver"/> data providers.
        /// </summary>
        /// <returns>
        /// Read only copy of the list of registered observers.
        /// </returns>
        [Obsolete("GetObservers will be removed in a future release. Check to see if the instance implements IMixedRealityDataProviderAccess and call GetDataProviders.")]
        IReadOnlyList<IMixedRealitySpatialAwarenessObserver> GetObservers();

        /// <summary>
        /// Get the collection of registered observers of the specified type.
        /// </summary>
        /// <typeparam name="T">The desired spatial awareness observer type (ex: <see cref="IMixedRealitySpatialAwarenessMeshObserver"/>)</typeparam>
        /// <returns>
        /// Readonly copy of the list of registered observers that implement the specified type.
        /// </returns>
        [Obsolete("GetObservers<T> will be removed in a future release. Check to see if the instance implements IMixedRealityDataProviderAccess and call GetDataProviders<T>.")]
        IReadOnlyList<T> GetObservers<T>() where T : IMixedRealitySpatialAwarenessObserver;

        /// <summary>
        /// Get the <see cref="IMixedRealitySpatialAwarenessObserver"/> that is registered under the specified name.
        /// </summary>
        /// <param name="name">The friendly name of the observer.</param>
        /// <returns>
        /// The requested observer, or null if one cannot be found.
        /// </returns>
        /// <remarks>
        /// If more than one observer is registered under the specified name, the first will be returned.
        /// </remarks>
        [Obsolete("GetObserver will be removed in a future release. Check to see if the instance implements IMixedRealityDataProviderAccess and call GetDataProvider.")]
        IMixedRealitySpatialAwarenessObserver GetObserver(string name);

        /// <summary>
        /// Get the observer that is registered under the specified name matching the specified type.
        /// </summary>
        /// <typeparam name="T">The desired spatial awareness observer type (ex: <see cref="IMixedRealitySpatialAwarenessMeshObserver"/>)</typeparam>
        /// <param name="name">The friendly name of the observer.</param>
        /// <returns>
        /// The requested observer, or null if one cannot be found.
        /// </returns>
        /// <remarks>
        /// If more than one observer is registered under the specified name, the first will be returned.
        /// </remarks>
        [Obsolete("GetObserver<T> will be removed in a future release. Check to see if the instance implements IMixedRealityDataProviderAccess and call GetDataProvider<T>.")]
        T GetObserver<T>(string name = null) where T : IMixedRealitySpatialAwarenessObserver;

        /// <summary>
        /// Starts / restarts all spatial observers of the specified type.
        /// </summary>
        void ResumeObservers();

        /// <summary>
        /// Starts / restarts all spatial observers of the specified type.
        /// </summary>
        /// <typeparam name="T">The desired spatial awareness observer type (ex: <see cref="IMixedRealitySpatialAwarenessMeshObserver"/>)</typeparam>
        void ResumeObservers<T>() where T : IMixedRealitySpatialAwarenessObserver;

        /// <summary>
        /// Starts / restarts the spatial observer registered under the specified name matching the specified type.
        /// </summary>
        /// <typeparam name="T">The desired spatial awareness observer type (ex: <see cref="IMixedRealitySpatialAwarenessMeshObserver"/>)</typeparam>
        /// <param name="name">The friendly name of the observer.</param>
        void ResumeObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver;

        /// <summary>
        /// Stops / pauses all spatial observers.
        /// </summary>
        void SuspendObservers();

        /// <summary>
        /// Stops / pauses all spatial observers of the specified type.
        /// </summary>
        void SuspendObservers<T>() where T : IMixedRealitySpatialAwarenessObserver;

        /// <summary>
        /// Stops / pauses the spatial observer registered under the specified name matching the specified type.
        /// </summary>
        /// <typeparam name="T">The desired spatial awareness observer type (ex: <see cref="IMixedRealitySpatialAwarenessMeshObserver"/>)</typeparam>
        /// <param name="name">The friendly name of the observer.</param>
        void SuspendObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver;

        // TODO: make these (and future plane) events more generic

        /// <summary>
        /// <see cref="IMixedRealitySpatialAwarenessMeshObserver"/>'s should call this method to indicate a mesh has been added.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, not by application code.
        /// </remarks>
        void RaiseMeshAdded(IMixedRealitySpatialAwarenessObserver observer, int meshId, SpatialAwarenessMeshObject meshObject);

        /// <summary>
        /// <see cref="IMixedRealitySpatialAwarenessMeshObserver"/>'s should call this method to indicate an existing mesh has been updated.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, not by application code.
        /// </remarks>
        void RaiseMeshUpdated(IMixedRealitySpatialAwarenessObserver observer, int meshId, SpatialAwarenessMeshObject meshObject);

        /// <summary>
        /// <see cref="IMixedRealitySpatialAwarenessMeshObserver"/>'s should call this method to indicate an existing mesh has been removed.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, not by application code.
        /// </remarks>
        void RaiseMeshRemoved(IMixedRealitySpatialAwarenessObserver observer, int meshId);
    }
}
