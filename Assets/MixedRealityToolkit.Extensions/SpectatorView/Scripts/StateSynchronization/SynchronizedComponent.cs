// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Abstract class for sending component updates from the user device.
    /// </summary>
    public interface ISynchronizedComponent
    {
        /// <summary>
        /// The associated component service.
        /// </summary>
        ISynchronizedComponentService SynchronizedComponentService { get; }

        /// <summary>
        /// The components synchronized transform
        /// </summary>
        SynchronizedTransform SynchronizedTransform { get; }

        /// <summary>
        /// Call to reset the frame.
        /// </summary>
        void ResetFrame();

        /// <summary>
        /// Call to report the current state of network connections.
        /// </summary>
        void ProcessNewConnections(SocketEndpointConnectionDelta connectionDelta);

        /// <summary>
        /// Call to signal the end of a frame.
        /// </summary>
        void OnFrameCompleted(SocketEndpointConnectionDelta connectionDelta);
    }

    /// <summary>
    /// Abstract class for sending component updates from the user device.
    /// </summary>
    public abstract class SynchronizedComponent<TSynchronizedComponentService, TChangeFlags> : MonoBehaviour, ISynchronizedComponent where TSynchronizedComponentService : Singleton<TSynchronizedComponentService>, ISynchronizedComponentService
    {
        protected SynchronizedTransform synchronizedTransform;
        private readonly HashSet<SocketEndpoint> fullyInitializedEndpoints = new HashSet<SocketEndpoint>();
        private List<SocketEndpoint> endpointsNeedingComponentCreation;
        private bool isUpdatedThisFrame;
        private bool isInitialized;
        private readonly List<SocketEndpoint> endpointsNeedingCompleteChanges = new List<SocketEndpoint>();
        private readonly List<SocketEndpoint> endpointsNeedingDeltaChanges = new List<SocketEndpoint>();
        private readonly List<SocketEndpoint> filteredEndpointsNeedingCompleteChanges = new List<SocketEndpoint>();
        private readonly List<SocketEndpoint> filteredEndpointsNeedingDeltaChanges = new List<SocketEndpoint>();
        private readonly List<SocketEndpoint> filteredEndpointsNotNeedingDeltaChanges = new List<SocketEndpoint>();

        /// <inheritdoc />
        public SynchronizedTransform SynchronizedTransform
        {
            get { return synchronizedTransform; }
        }

        /// <inheritdoc />
        public TSynchronizedComponentService SynchronizedComponentService
        {
            get { return Singleton<TSynchronizedComponentService>.Instance; }
        }

        /// <inheritdoc />
        ISynchronizedComponentService ISynchronizedComponent.SynchronizedComponentService => SynchronizedComponentService;

        /// <inheritdoc />
        protected virtual void Awake()
        {
            synchronizedTransform = GetComponent<SynchronizedTransform>();

            SynchronizedSceneManager.Instance.AddSynchronizedComponent(this);
        }

        protected virtual void OnDestroy(){}

        /// <inheritdoc />
        protected virtual bool UpdateWhenDisabled => false;

        /// <inheritdoc />
        public void ResetFrame()
        {
            isUpdatedThisFrame = false;
        }

        /// <inheritdoc />
        protected virtual bool ShouldSendChanges(SocketEndpoint endpoint)
        {
            return SynchronizedTransform.ShouldSendTransformInHierarchy(endpoint);
        }

        /// <inheritdoc />
        public void ProcessNewConnections(SocketEndpointConnectionDelta connectionDelta)
        {
            if (!isInitialized)
            {
                ProcessNewConnections(connectionDelta.AddedConnections.Concat(connectionDelta.ContinuedConnections));
            }
            else
            {
                ProcessNewConnections(connectionDelta.AddedConnections);
            }
        }

        protected virtual void ProcessNewConnections(IEnumerable<SocketEndpoint> connectionsRequiringFullUpdate) { }

        /// <inheritdoc />
        public void OnFrameCompleted(SocketEndpointConnectionDelta connectionDelta)
        {
            if (!isUpdatedThisFrame)
            {
                isUpdatedThisFrame = true;

                if (SynchronizedTransform != null && (isActiveAndEnabled || UpdateWhenDisabled))
                {
                    // Make sure the transform syncs before any other components sync
                    if (SynchronizedTransform != this)
                    {
                        SynchronizedTransform.OnFrameCompleted(connectionDelta);
                    }

                    BeginUpdatingFrame(connectionDelta);

                    // The SynchronizedTransform might detect that this component is destroyed.
                    // If so, don't continue updating.
                    if (this.enabled)
                    {
                        EnsureComponentInitialized();

                        endpointsNeedingCompleteChanges.Clear();
                        endpointsNeedingDeltaChanges.Clear();

                        filteredEndpointsNeedingCompleteChanges.Clear();
                        filteredEndpointsNeedingDeltaChanges.Clear();
                        filteredEndpointsNotNeedingDeltaChanges.Clear();

                        foreach (SocketEndpoint endpoint in connectionDelta.AddedConnections)
                        {
                            endpointsNeedingCompleteChanges.Add(endpoint);
                            if (ShouldSendChanges(endpoint))
                            {
                                filteredEndpointsNeedingCompleteChanges.Add(endpoint);
                            }
                        }

                        foreach (SocketEndpoint endpoint in connectionDelta.ContinuedConnections)
                        {
                            if (fullyInitializedEndpoints.Contains(endpoint))
                            {
                                endpointsNeedingDeltaChanges.Add(endpoint);
                                if (ShouldSendChanges(endpoint))
                                {
                                    filteredEndpointsNeedingDeltaChanges.Add(endpoint);
                                }
                                else
                                {
                                    filteredEndpointsNotNeedingDeltaChanges.Add(endpoint);
                                }
                            }
                            else
                            {
                                endpointsNeedingCompleteChanges.Add(endpoint);

                                if (ShouldSendChanges(endpoint))
                                {
                                    filteredEndpointsNeedingCompleteChanges.Add(endpoint);
                                }
                            }
                        }

                        SendComponentCreation(endpointsNeedingCompleteChanges);

                        TChangeFlags changeFlags = CalculateDeltaChanges();
                        if (HasChanges(changeFlags) && filteredEndpointsNeedingDeltaChanges.Any())
                        {
                            SendDeltaChanges(filteredEndpointsNeedingDeltaChanges, changeFlags);
                        }

                        if (filteredEndpointsNeedingCompleteChanges.Count > 0)
                        {
                            SendCompleteChanges(filteredEndpointsNeedingCompleteChanges);

                            foreach (SocketEndpoint endpoint in filteredEndpointsNeedingCompleteChanges)
                            {
                                fullyInitializedEndpoints.Add(endpoint);
                            }
                        }

                        foreach (SocketEndpoint endpoint in endpointsNeedingDeltaChanges)
                        {
                            if (!ShouldSendChanges(endpoint))
                            {
                                fullyInitializedEndpoints.Remove(endpoint);
                            }
                        }
                        UpdateRemovedConnections(connectionDelta);

                        EndUpdatingFrame();
                    }
                }
            }
        }

        private void UpdateRemovedConnections(SocketEndpointConnectionDelta connectionDelta)
        {
            if (connectionDelta.RemovedConnections.Count > 0)
            {
                RemoveDisconnectedEndpoints(connectionDelta.RemovedConnections);

                foreach (SocketEndpoint endpoint in connectionDelta.RemovedConnections)
                {
                    fullyInitializedEndpoints.Remove(endpoint);
                }
            }
        }

        private void EnsureComponentInitialized()
        {
            if (!isInitialized)
            {
                OnInitialized();
                isInitialized = true;
            }
        }

        protected virtual void SendComponentCreation(IEnumerable<SocketEndpoint> newConnections)
        {
            if (endpointsNeedingComponentCreation != null)
            {
                for (int i = endpointsNeedingComponentCreation.Count - 1; i >= 0; i--)
                {
                    if (ShouldSendChanges(endpointsNeedingComponentCreation[i]))
                    {
                        SendComponentCreation(endpointsNeedingComponentCreation[i]);
                        endpointsNeedingComponentCreation.RemoveAt(i);
                    }
                }
            }

            foreach (SocketEndpoint newConnection in newConnections)
            {
                if (ShouldSendChanges(newConnection))
                {
                    SendComponentCreation(newConnection);
                }
                else
                {
                    if (endpointsNeedingComponentCreation == null)
                    {
                        endpointsNeedingComponentCreation = new List<SocketEndpoint>();
                    }
                    endpointsNeedingComponentCreation.Add(newConnection);
                }
            }
        }

        private void SendComponentCreation(SocketEndpoint endpoint)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                this.SynchronizedComponentService.WriteHeader(message, this, SynchronizedComponentChangeType.Created);
                message.Flush();
                endpoint.Send(memoryStream.ToArray());
            }
        }

        protected virtual void OnInitialized() {}

        protected virtual bool ShouldUpdateFrame(SocketEndpoint endpoint)
        {
            return this.enabled;
        }

        protected virtual void BeginUpdatingFrame(SocketEndpointConnectionDelta connectionDelta) {}

        protected virtual void EndUpdatingFrame() {}

        protected abstract void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints);

        protected abstract TChangeFlags CalculateDeltaChanges();

        protected abstract bool HasChanges(TChangeFlags changeFlags);

        protected abstract void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, TChangeFlags changeFlags);

        protected virtual void RemoveDisconnectedEndpoints(IEnumerable<SocketEndpoint> endpoints) {}
    }
}
