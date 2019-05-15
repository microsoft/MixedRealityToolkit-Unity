// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Types of state changes that may occur for an <see cref="IComponentBroadcaster"/>
    /// </summary>
    public enum ComponentBroadcasterChangeType : byte
    {
        Created = 0,
        Updated,
        Destroyed,
    }

    /// <summary>
    /// Component that syncs local content based on changes to a <see cref=IComponentBroadcaster"/>
    /// </summary>
    public interface IComponentObserver
    {
        void Read(SocketEndpoint sendingEndpoint, BinaryReader message);
    }

    /// <summary>
    /// Manages component synchronization across different devices
    /// </summary>
    public interface IComponentBroadcasterService
    {
        void Create(GameObject mirror);
        void Destroy(GameObject mirror);
        void Read(SocketEndpoint sendingEndpoint, BinaryReader message, GameObject mirror);
        void LerpRead(BinaryReader message, GameObject mirror, float lerpVal);
        ShortID GetID();
        void WriteHeader(BinaryWriter message, IComponentBroadcaster component, ComponentBroadcasterChangeType changeType = ComponentBroadcasterChangeType.Updated);
        void WriteHeader(BinaryWriter message, TransformObserver TransformObserver, ComponentBroadcasterChangeType changeType = ComponentBroadcasterChangeType.Updated);
    }

    /// <summary>
    /// Used to persist asset information
    /// </summary>
    public interface IAssetCache
    {
        void UpdateAssetCache();
        void ClearAssetCache();
    }

    /// <summary>
    /// Abstract class for services that manage <see cref="IComponentBroadcaster"/>
    /// </summary>
    /// <typeparam name="ServiceType">The service type used for defining a singleton of said service</typeparam>
    /// <typeparam name="RemoteType">The <see cref="IComponentObserver"/> type that the service manages</typeparam>
    public abstract class ComponentBroadcasterService<ServiceType, RemoteType> : Singleton<ServiceType>, IComponentBroadcasterService where ServiceType : Singleton<ServiceType> where RemoteType : UnityEngine.Component, IComponentObserver
    {
        /// <summary>
        /// Ensures that the <see cref="IComponentObserver"/> type defined for the service exists for the provided game object
        /// </summary>
        /// <param name="mirror"></param>
        public virtual void Create(GameObject mirror)
        {
            ComponentExtensions.EnsureComponent<RemoteType>(mirror);
        }

        /// Ensures that the <see cref="IComponentObserver"/> type defined for the service is removed from the provided game object
        public virtual void Destroy(GameObject mirror)
        {
            RemoteType comp = mirror.GetComponent<RemoteType>();
            if (comp != null)
            {
                Destroy(comp);
            }
        }

        /// <summary>
        /// Ensures an <see cref="IComponentObserver"/> exists on the provided game object and relays it a network message
        /// </summary>
        /// <param name="sendingEndpoint">Endpoint that sent the message</param>
        /// <param name="message">network message</param>
        /// <param name="mirror">game object to synchronize</param>
        public virtual void Read(SocketEndpoint sendingEndpoint, BinaryReader message, GameObject mirror)
        {
            RemoteType comp = ComponentExtensions.EnsureComponent<RemoteType>(mirror);
            comp.Read(sendingEndpoint, message);
        }

        /// <summary>
        /// Relays the <see cref="IComponentObserver"/> defined for the game object a network message for interpolating
        /// </summary>
        /// <param name="message">network message</param>
        /// <param name="mirror">game object to synchronize</param>
        /// <param name="lerpVal">interpolation value</param>
        public virtual void LerpRead(BinaryReader message, GameObject mirror, float lerpVal)
        {
            return;
        }

        /// <summary>
        /// Writes <see cref="IComponentBroadcaster"/> information to a network message for sending to other devices
        /// </summary>
        /// <param name="message">network message to send</param>
        /// <param name="component">component that has changed</param>
        /// <param name="changeType">type of changed that occurred for the specified component</param>
        public void WriteHeader(BinaryWriter message, IComponentBroadcaster component, ComponentBroadcasterChangeType changeType = ComponentBroadcasterChangeType.Updated)
        {
            StateSynchronizationSceneManager.Instance.WriteHeader(message);
            message.Write(GetID().Value);
            message.Write(component.TransformBroadcaster.Id);
            message.Write((byte)changeType);
        }

        /// <summary>
        /// Writes <see cref="TransformObserver"/> information to a network message for sending to other devices
        /// </summary>
        /// <param name="message">network message to send</param>
        /// <param name="TransformObserver">transform that has changed</param>
        /// <param name="changeType">type of changed that occurred for the specified component</param>
        public void WriteHeader(BinaryWriter message, TransformObserver TransformObserver, ComponentBroadcasterChangeType changeType = ComponentBroadcasterChangeType.Updated)
        {
            StateSynchronizationSceneManager.Instance.WriteHeader(message);
            message.Write(GetID().Value);
            message.Write(TransformObserver.Id);
            message.Write((byte)changeType);
        }

        /// <summary>
        /// Returns a unique id associated with the service
        /// </summary>
        /// <returns>unique id</returns>
        public abstract ShortID GetID();
    }
}
