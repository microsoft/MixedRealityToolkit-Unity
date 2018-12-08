// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.Networking
{
    /// <summary>
    /// The base networking event data for networked messages and events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseNetworkingEventData<T> : GenericBaseEventData
    {
        /// <summary>
        /// The data of the network event.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public BaseNetworkingEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        public void Initialize(T data)
        {
            BaseInitialize(null);
            Data = data;
        }
    }
}
