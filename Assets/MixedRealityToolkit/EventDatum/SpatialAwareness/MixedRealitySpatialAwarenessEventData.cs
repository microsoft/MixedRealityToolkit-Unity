// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem
{
    /// <summary>
    /// Data for spatial awareness events.
    /// </summary>
    public class MixedRealitySpatialAwarenessEventData : GenericBaseEventData
    {
        /// <summary>
        /// Identifier of the object associated with this event.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MixedRealitySpatialAwarenessEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Initialize the event data.
        /// </summary>
        /// <param name="observer">The <see cref="IMixedRealitySpatialAwarenessObserver"/> that raised the event.</param>
        /// <param name="id">The identifier of the observed spatial object.</param>
        public void Initialize(IMixedRealitySpatialAwarenessObserver observer, int id)
        {
            BaseInitialize(observer);
            Id = id;
        }
    }

    /// <summary>
    /// Data for spatial awareness events.
    /// </summary>
    /// <typeparam name="T">The spatial object data type.</typeparam>
    public class MixedRealitySpatialAwarenessEventData<T> : MixedRealitySpatialAwarenessEventData
    {
        /// <summary>
        /// The spatial object to which this event pertains.
        /// </summary>
        public T SpatialObject { get; private set; }

        /// <inheritdoc />
        public MixedRealitySpatialAwarenessEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Initialize the event data.
        /// </summary>
        /// <param name="observer">The <see cref="IMixedRealitySpatialAwarenessObserver"/> that raised the event.</param>
        /// <param name="id">The identifier of the observed spatial object.</param>
        /// <param name="spatialObject">The observed spatial object.</param>
        public void Initialize(IMixedRealitySpatialAwarenessObserver observer, int id, T spatialObject)
        {
            Initialize(observer, id);
            SpatialObject = spatialObject;
        }
    }
}
