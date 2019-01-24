// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This class is an example of the <see cref="IMixedRealitySpatialAwarenessMeshHandler"/> interface. It keeps track
    /// of the IDs of each mesh and tracks the number of updates they have received.
    /// </summary>
    public class DemoSpatialMeshHandler : MonoBehaviour, IMixedRealitySpatialAwarenessMeshHandler
    {
        /// <summary>
        /// Collection that tracks the IDs and count of updates for each active spatial awareness mesh.
        /// </summary>
        private Dictionary<int, uint> meshUpdateData = new Dictionary<int, uint>();

        private async void OnEnable()
        {
            await new WaitUntil(() => MixedRealityToolkit.SpatialAwarenessSystem != null);
            MixedRealityToolkit.SpatialAwarenessSystem.Register(gameObject);
        }

        private void OnDisable()
        {
            MixedRealityToolkit.SpatialAwarenessSystem?.Unregister(gameObject);
        }

        /// <inheritdoc />
        public virtual void OnMeshAdded(MixedRealitySpatialAwarenessEventData eventData)
        {
            Debug.Log("DemoSpatialMeshHandler.OnMeshAdded");

            // A new mesh has been added.
            if (!meshUpdateData.ContainsKey(eventData.Id))
            {
                Debug.Log($"Tracking mesh {eventData.Id}");
                meshUpdateData.Add(eventData.Id, 0);
            }
        }

        /// <inheritdoc />
        public virtual void OnMeshUpdated(MixedRealitySpatialAwarenessEventData eventData)
        {
            uint updateCount = 0;

            Debug.Log("DemoSpatialMeshHandler.OnMeshUpdated");

            // A mesh has been updated. Find it and increment the update count.
            if (meshUpdateData.TryGetValue(eventData.Id, out updateCount))
            {
                // Set the new update count.
                meshUpdateData[eventData.Id] = ++updateCount;

                Debug.Log($"Mesh {eventData.Id} has been updated {updateCount} times.");
            }
        }

        /// <inheritdoc />
        public virtual void OnMeshRemoved(MixedRealitySpatialAwarenessEventData eventData)
        {
            Debug.Log("DemoSpatialMeshHandler.OnMeshRemoved");

            // A mesh has been removed. We no longer need to track the count of updates.
            if (meshUpdateData.ContainsKey(eventData.Id))
            {
                Debug.Log($"No longer tracking mesh {eventData.Id}.");
                meshUpdateData.Remove(eventData.Id);
            }
        }
    }
}
