// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
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
        /// Identifier of the object associated with this event.
        /// </summary>
        public System.Guid GuidId { get; private set; }

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

        /// <summary>
        /// Initialize the event data.
        /// </summary>
        /// <param name="observer">The <see cref="IMixedRealitySpatialAwarenessObserver"/> that raised the event.</param>
        /// <param name="id">The identifier of the observed spatial object.</param>
        public void Initialize(IMixedRealitySpatialAwarenessObserver observer, System.Guid id)
        {
            BaseInitialize(observer);
            GuidId = id;
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

        /// <summary>
        /// Initialize the event data.
        /// </summary>
        /// <param name="observer">The <see cref="IMixedRealitySpatialAwarenessObserver"/> that raised the event.</param>
        /// <param name="id">The identifier of the observed spatial object.</param>
        /// <param name="spatialObject">The observed spatial object.</param>
        public void Initialize(IMixedRealitySpatialAwarenessObserver observer, System.Guid id, T spatialObject)
        {
            Initialize(observer, id);
            SpatialObject = spatialObject;
        }
    }
}
