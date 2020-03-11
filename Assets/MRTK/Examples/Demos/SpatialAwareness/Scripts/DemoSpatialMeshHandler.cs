// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    using SpatialAwarenessHandler = IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>;

    /// <summary>
    /// This class is an example of the <see cref="SpatialAwareness.IMixedRealitySpatialAwarenessObservationHandler{T}"/> interface. It keeps track
    /// of the IDs of each mesh and tracks the number of updates they have received.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/DemoSpatialMeshHandler")]
    public class DemoSpatialMeshHandler : MonoBehaviour, SpatialAwarenessHandler
    {
        /// <summary>
        /// Collection that tracks the IDs and count of updates for each active spatial awareness mesh.
        /// </summary>
        private Dictionary<int, uint> meshUpdateData = new Dictionary<int, uint>();

        /// <summary>
        /// Value indicating whether or not this script has registered for spatial awareness events.
        /// </summary>
        private bool isRegistered = false;

        private void Start()
        {
            RegisterEventHandlers();
        }

        private void OnEnable()
        {
            RegisterEventHandlers();
        }

        private void OnDisable()
        {
            UnregisterEventHandlers();
        }

        private void OnDestroy()
        {
            UnregisterEventHandlers();
        }

        /// <summary>
        /// Registers for the spatial awareness system events.
        /// </summary>
        private void RegisterEventHandlers()
        {
            if (!isRegistered && (CoreServices.SpatialAwarenessSystem != null))
            {
                CoreServices.SpatialAwarenessSystem.RegisterHandler<SpatialAwarenessHandler>(this);
                isRegistered = true;
            }
        }

        /// <summary>
        /// Unregisters from the spatial awareness system events.
        /// </summary>
        private void UnregisterEventHandlers()
        {
            if (isRegistered && (CoreServices.SpatialAwarenessSystem != null))
            {
                CoreServices.SpatialAwarenessSystem.UnregisterHandler<SpatialAwarenessHandler>(this);
                isRegistered = false;
            }
        }

        /// <inheritdoc />
        public virtual void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
        {
            // A new mesh has been added.
            if (!meshUpdateData.ContainsKey(eventData.Id))
            {
                Debug.Log($"Tracking mesh {eventData.Id}");
                meshUpdateData.Add(eventData.Id, 0);
            }
        }

        /// <inheritdoc />
        public virtual void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
        {
            uint updateCount = 0;

            // A mesh has been updated. Find it and increment the update count.
            if (meshUpdateData.TryGetValue(eventData.Id, out updateCount))
            {
                // Set the new update count.
                meshUpdateData[eventData.Id] = ++updateCount;

                Debug.Log($"Mesh {eventData.Id} has been updated {updateCount} times.");
            }
        }

        /// <inheritdoc />
        public virtual void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> eventData)
        {
            // A mesh has been removed. We no longer need to track the count of updates.
            if (meshUpdateData.ContainsKey(eventData.Id))
            {
                Debug.Log($"No longer tracking mesh {eventData.Id}.");
                meshUpdateData.Remove(eventData.Id);
            }
        }
    }
}
