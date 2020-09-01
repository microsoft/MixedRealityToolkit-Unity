// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Base class for event receivers
    /// </summary>
    public abstract class BaseEventReceiver
    {
        public BaseEventReceiver(BaseInteractionEventConfiguration eventConfiguration, string name)
        {
            EventConfiguration = eventConfiguration;
            Name = name;
        }

        public string Name { get; protected set; } = null;

        public BaseInteractionEventConfiguration EventConfiguration { get; protected set; } = null;

        /// <summary>
        /// Update an event receiver 
        /// </summary>
        public abstract void OnUpdate(StateManager state, BaseEventData eventData);

    }
}
