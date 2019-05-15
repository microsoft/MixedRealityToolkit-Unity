// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class DynamicGameObjectHierarchyBroadcaster<TComponentService> : ComponentBroadcaster<TComponentService, byte> where TComponentService : Singleton<TComponentService>, IComponentBroadcasterService
    {
        private GameObject dynamicObject;
        private Dictionary<SocketEndpoint, PerConnectionInstantiationState> perConnectionInstantiationState = new Dictionary<SocketEndpoint, PerConnectionInstantiationState>();

        private class PerConnectionInstantiationState
        {
            public bool remoteObjectCreated;
            public bool sendInstantiationRequest;
            public bool sendTransformHierarchyBinding;
        }

        public static class ChangeType
        {
            public const byte CreateRemoteObject = 0x0;
            public const byte RemoteObjectCreated = 0x1;
            public const byte BindTransformHierarchy = 0x2;
            public const byte RemoteHierarchyBound = 0x3;
        }

        protected GameObject DynamicObject
        {
            get { return dynamicObject; }
            set
            {
                if (dynamicObject != value)
                {
                    dynamicObject = value;

                    if (dynamicObject != null)
                    {
                        OnDynamicObjectConstructed();
                    }
                }
            }
        }

        protected override byte CalculateDeltaChanges()
        {
            return 0;
        }

        protected override bool HasChanges(byte changeFlags)
        {
            return true;
        }

        protected override bool ShouldSendChanges(SocketEndpoint endpoint)
        {
            // We always need to send changes for dynamic components to negotiate the remote instantiation
            return true;
        }

        protected override void ProcessNewConnections(IEnumerable<SocketEndpoint> connectionsRequiringFullUpdate)
        {
            foreach (SocketEndpoint newConnection in connectionsRequiringFullUpdate)
            {
                if (DynamicObject == null)
                {
                    TransformBroadcaster.BlockedConnections.Add(newConnection);
                }
                else
                {
                    DynamicObject.GetComponent<TransformBroadcaster>().BlockedConnections.Add(newConnection);
                }
            }
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            foreach (SocketEndpoint endpoint in endpoints)
            {
                PerConnectionInstantiationState state;
                if (!perConnectionInstantiationState.TryGetValue(endpoint, out state))
                {
                    perConnectionInstantiationState.Add(endpoint, new PerConnectionInstantiationState
                    {
                        remoteObjectCreated = false,
                        sendInstantiationRequest = true
                    });
                }
            }

            SendDeltaChanges(endpoints, 0);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, byte changeFlags)
        {
            foreach (SocketEndpoint endpoint in endpoints)
            {
                PerConnectionInstantiationState state;
                if (perConnectionInstantiationState.TryGetValue(endpoint, out state))
                {
                    if (state.sendInstantiationRequest)
                    {
                        state.sendInstantiationRequest = false;
                        using (MemoryStream memoryStream = new MemoryStream())
                        using (BinaryWriter message = new BinaryWriter(memoryStream))
                        {
                            ComponentBroadcasterService.WriteHeader(message, this);

                            message.Write((byte)ChangeType.CreateRemoteObject);
                            WriteInstantiationRequestParameters(message);

                            message.Flush();
                            endpoint.Send(memoryStream.ToArray());
                        }
                    }

                    if (state.sendTransformHierarchyBinding)
                    {
                        state.sendTransformHierarchyBinding = false;
                        SendTransformHierarchyBinding(endpoint);
                    }
                }
            }
        }

        protected override void RemoveDisconnectedEndpoints(IEnumerable<SocketEndpoint> endpoints)
        {
            foreach (SocketEndpoint endpoint in endpoints)
            {
                perConnectionInstantiationState.Remove(endpoint);
            }
        }

        protected abstract void WriteInstantiationRequestParameters(BinaryWriter message);
        
        private void OnDynamicObjectConstructed()
        {
            foreach (SocketEndpoint endpoint in perConnectionInstantiationState.Keys)
            {
                if (TransformBroadcaster.BlockedConnections.Remove(endpoint))
                {
                    DynamicObject.GetComponent<TransformBroadcaster>().BlockedConnections.Add(endpoint);
                }
                else
                {
                    Debug.LogError("Expected that the object was previously blocked");
                }
            }

            foreach (KeyValuePair<SocketEndpoint, PerConnectionInstantiationState> state in perConnectionInstantiationState)
            {
                if (state.Value.remoteObjectCreated)
                {
                    state.Value.sendTransformHierarchyBinding = true;
                }
            }
        }

        protected void SendInstantiationRequest()
        {
            DynamicObject = null;

            foreach (KeyValuePair<SocketEndpoint, PerConnectionInstantiationState> state in perConnectionInstantiationState)
            {
                TransformBroadcaster.BlockedConnections.Add(state.Key);
                state.Value.remoteObjectCreated = false;
                state.Value.sendInstantiationRequest = true;
            }
        }

        public void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            byte changeType = message.ReadByte();

            Read(sendingEndpoint, message, changeType);
        }

        protected virtual void Read(SocketEndpoint sendingEndpoint, BinaryReader message, byte changeType)
        {
            switch (changeType)
            {
                case ChangeType.RemoteObjectCreated:
                    {
                        PerConnectionInstantiationState state;
                        if (perConnectionInstantiationState.TryGetValue(sendingEndpoint, out state))
                        {
                            state.remoteObjectCreated = true;

                            if (DynamicObject != null)
                            {
                                state.sendTransformHierarchyBinding = true;
                            }
                        }
                    }
                    break;
                case ChangeType.RemoteHierarchyBound:
                    if (DynamicObject != null)
                    {
                        DynamicObject.GetComponent<TransformBroadcaster>().BlockedConnections.Remove(sendingEndpoint);
                    }
                    break;
            }
        }

        private void SendTransformHierarchyBinding(SocketEndpoint sendingEndpoint)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                ComponentBroadcasterService.WriteHeader(message, this);

                message.Write((byte)ChangeType.BindTransformHierarchy);
                DynamicObject.GetComponent<TransformBroadcaster>().WriteChildHierarchyTree(message);

                message.Flush();

                sendingEndpoint.Send(memoryStream.ToArray());
            }
        }
    }
}