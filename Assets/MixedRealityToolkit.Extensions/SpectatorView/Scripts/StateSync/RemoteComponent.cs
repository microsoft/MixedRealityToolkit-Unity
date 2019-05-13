// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Abstract class for updating components on the spectator device.
    /// </summary>
    public abstract class RemoteComponent : MonoBehaviour, IRemoteComponent
    {
        /// <summary>
        /// The type associated with this component.
        /// </summary>
        public abstract Type ComponentType { get; }

        /// <summary>
        /// Interprets data received from another device.
        /// </summary>
        /// <param name="sendingEndpoint">sender</param>
        /// <param name="message">data received</param>
        public abstract void Read(SocketEndpoint sendingEndpoint, BinaryReader message);

        protected virtual void Awake()
        {
            if (gameObject.GetComponent(ComponentType) == null)
            {
                gameObject.AddComponent(ComponentType);
            }
        }

        protected virtual void OnDestroy()
        {
            Destroy(gameObject.GetComponent(ComponentType));
        }
    }

    /// <summary>
    /// Abstract class for updating components on the spectator device.
    /// </summary>
    public abstract class RemoteComponent<TComponent> : RemoteComponent where TComponent : Component
    {
        protected TComponent attachedComponent;

        /// <inheritdoc />
        public override Type ComponentType => typeof(TComponent);

        protected override void Awake()
        {
            base.Awake();

            attachedComponent = GetComponent<TComponent>();
        }
    }
}
