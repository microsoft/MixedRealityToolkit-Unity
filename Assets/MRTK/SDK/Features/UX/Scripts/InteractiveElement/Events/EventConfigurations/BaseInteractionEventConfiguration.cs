// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Base class for interaction event configuration.  An event configuration maps to a single Interaction State.
    /// </summary>
    [CreateAssetMenu(fileName = "BaseInteractionEventConfiguration", menuName = "Mixed Reality Toolkit/Interactive Element/Event Configurations/Base Event Configuration")]

    public abstract class BaseInteractionEventConfiguration : ScriptableObject
    {
        /// <summary>
        /// The name of the state associated with this event configuration.
        /// </summary>
        public virtual string StateName { get; } = null;

        /// <summary>
        /// The associated runtime event receiver for this event configuration.
        /// </summary>
        public BaseEventReceiver EventReceiver { get; protected set; }

        /// <summary>
        /// Initializes the runtime event receiver.
        /// </summary>
        /// <returns>Event Receiver</returns>
        public abstract BaseEventReceiver InitializeRuntimeEventReceiver();
    }
}
