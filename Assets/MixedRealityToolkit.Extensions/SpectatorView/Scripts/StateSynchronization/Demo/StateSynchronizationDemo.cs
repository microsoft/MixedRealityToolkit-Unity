// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal enum Role
    {
        Broadcaster,
        Observer
    }

    internal class ConnectedObserver : DisposableBase
    {
        public const string LocalizationMessageHeader = "LOCALIZE";

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;

        private readonly Role role;
        private readonly SocketEndpoint socketEndpoint;
        private readonly Func<GameObject> createAnchor;

        private Action<BinaryReader> processIncomingMessages = null;
        private GameObject observerGO = null;

        public ConnectedObserver(Role role, SocketEndpoint socketEndpoint, Func<GameObject> createAnchor)
        {
            cancellationToken = cancellationTokenSource.Token;

            this.role = role;
            this.socketEndpoint = socketEndpoint;
            this.createAnchor = createAnchor;
        }

        public async Task LocalizeAsync(LocalizationMechanismBase localizationMechanism)
        {
            // Tell the localization mechanism to initialize, this could create anchor if need be
            Guid token = await localizationMechanism.InitializeAsync(role, cancellationToken);

            try
            {
                lock (cancellationTokenSource)
                {
                    processIncomingMessages = r => localizationMechanism.ProcessIncomingMessage(role, token, r);
                }

                ISpatialCoordinate coordinate = await localizationMechanism.LocalizeAsync(role, token, (callback) => // Allows localization mechanism to tell spectator what to do
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    using (BinaryWriter writer = new BinaryWriter(memoryStream))
                    {
                        writer.Write(LocalizationMessageHeader);

                        callback(writer);
                        socketEndpoint.Send(memoryStream.ToArray());
                    }
                }, cancellationToken);

                if (coordinate == null)
                {
                    Debug.LogError($"Failed to localize for spectator: {socketEndpoint.Address}");
                }
                else
                {
                    lock (cancellationTokenSource)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            observerGO = createAnchor();
                            observerGO.AddComponent<SpatialCoordinateLocalizer>().Coordinate = coordinate;
                        }
                    }
                }
            }
            finally
            {
                lock (cancellationTokenSource)
                {
                    processIncomingMessages = null;
                }

                localizationMechanism.Deinitialize(role, token);
            }
        }

        public void ReceiveMessage(BinaryReader reader)
        {
            lock (cancellationTokenSource)
            {
                processIncomingMessages?.Invoke(reader);
            }
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();


            lock (cancellationTokenSource)
            {
                UnityEngine.Object.Destroy(observerGO);
            }
        }
    }

    internal abstract class LocalizationMechanismBase : MonoBehaviour
    {
        internal abstract Task<Guid> InitializeAsync(Role role, CancellationToken cancellationToken);

        internal abstract void Deinitialize(Role role, Guid token);

        internal abstract void ProcessIncomingMessage(Role role, Guid token, BinaryReader r);

        internal abstract Task<ISpatialCoordinate> LocalizeAsync(Role broadcaster, Guid token, Action<Action<BinaryWriter>> sendMessage, CancellationToken cancellationToken);
    }

    internal abstract class HostCoordinateLocalizationMechanism : LocalizationMechanismBase
    {
        private readonly object lockObject = new object();
        private readonly List<Guid> guids = new List<Guid>();
        private Task<ISpatialCoordinate> initializeBroadcasterCoordinateTask = null;
        private TaskCompletionSource<string> observerCoordinateIdToLookFor = null;

        protected abstract ISpatialCoordinateService SpatialCoordinateService { get; }

        protected virtual async Task<ISpatialCoordinate> GetHostCoordinateAsync()
        {
            TaskCompletionSource<ISpatialCoordinate> coordinateTCS = new TaskCompletionSource<ISpatialCoordinate>();
            void coordianteDiscovered(ISpatialCoordinate coord) { coordinateTCS.SetResult(coord); }

            SpatialCoordinateService.CoordinatedDiscovered += coordianteDiscovered;
            try
            {
                return await coordinateTCS.Task;
            }
            finally
            {
                SpatialCoordinateService.CoordinatedDiscovered -= coordianteDiscovered;
            }
        }

        internal async override Task<Guid> InitializeAsync(Role role, CancellationToken cancellationToken)
        {
            Guid toReturn = new Guid();
            if (role == Role.Broadcaster)
            {
                lock (lockObject)
                {
                    if (initializeBroadcasterCoordinateTask == null)
                    {
                        initializeBroadcasterCoordinateTask = GetHostCoordinateAsync();
                    }
                }

                // Wait for broadcaster to initialize (which happens once and won't be cancelled), or until this request was cancelled.
                await Task.WhenAny(Task.Delay(-1, cancellationToken), initializeBroadcasterCoordinateTask);

                //We have the coordinate after this step has finished
            }
            else if (role == Role.Observer)
            {
                observerCoordinateIdToLookFor = new TaskCompletionSource<string>();
            }

            guids.Add(toReturn);
            return toReturn;
        }

        internal override void ProcessIncomingMessage(Role role, Guid token, BinaryReader r)
        {
            switch (role)
            {
                case Role.Broadcaster:
                    break;
                case Role.Observer:
                    observerCoordinateIdToLookFor.TrySetResult(r.ReadString());
                    break;
            }
        }

        internal override async Task<ISpatialCoordinate> LocalizeAsync(Role role, Guid token, Action<Action<BinaryWriter>> sendMessage, CancellationToken cancellationToken)
        {
            ISpatialCoordinate coordinateToReturn = null;

            switch (role)
            {
                case Role.Broadcaster:
                    coordinateToReturn = initializeBroadcasterCoordinateTask.Result;
                    sendMessage(writer => writer.Write(coordinateToReturn.Id));
                    break;
                case Role.Observer:
                    await Task.WhenAny(observerCoordinateIdToLookFor.Task, Task.Delay(-1, cancellationToken)); //If we get cancelled, or get a token

                    // Now we have coordinateId in TaskCompletionSource
                    string id = observerCoordinateIdToLookFor.Task.Result;

                    if (await SpatialCoordinateService.TryDiscoverCoordinatesAsync(cancellationToken, id))
                    {
                        if (!SpatialCoordinateService.TryGetKnownCoordinate(id, out coordinateToReturn))
                        {
                            Debug.LogError("We discovered, but for some reason failed to get coordinate from service.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to discover spatial coordinate.");
                    }
                    break;
            }

            return coordinateToReturn;
        }

        internal override void Deinitialize(Role role, Guid token)
        {
            switch (role)
            {
                case Role.Broadcaster:
                    break;
                case Role.Observer:
                    break;
            }

            guids.Remove(token);
        }
    }

    internal class ArUcoLocalizationMechanism : HostCoordinateLocalizationMechanism
    {
        
        protected override ISpatialCoordinateService SpatialCoordinateService => throw new NotImplementedException();


    }

    /// <summary>
    /// Class for demoing state synchronization
    /// </summary>
    internal class StateSynchronizationDemo : Singleton<StateSynchronizationDemo>
    {
        /// <summary>
        /// Current role of the application
        /// </summary>
        [Tooltip("Current role of the application")]
        [SerializeField]
        public Role Role = Role.Broadcaster;

        [SerializeField]
        private LocalizationMechanismBase localizationMechanism = null;

        /// <summary>
        /// Broadcaster ip address
        /// </summary>
        [Tooltip("Broadcaster ip address")]
        [SerializeField]
        private string broadcasterIpAddress = "127.0.0.1";

        /// <summary>
        /// StateSynchronizationSceneManager MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationSceneManager")]
        [SerializeField]
        private StateSynchronizationSceneManager stateSynchronizationSceneManager = null;

        /// <summary>
        /// StateSynchronizationBroadcaster MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationBroadcaster MonoBehaviour")]
        [SerializeField]
        private StateSynchronizationBroadcaster stateSynchronizationBroadcaster = null;

        /// <summary>
        /// StateSynchronizationObserver MonoBehaviour
        /// </summary>
        [Tooltip("StateSynchronizationObserver MonoBehaviour")]
        [SerializeField]
        private StateSynchronizationObserver stateSynchronizationObserver = null;

        /// <summary>
        /// Content to enable in the broadcaster application
        /// </summary>
        [Tooltip("Content to enable in the broadcaster application")]
        [SerializeField]
        private GameObject broadcastedContent = null;

        private void Start()
        {
            if (stateSynchronizationSceneManager == null ||
                stateSynchronizationBroadcaster == null ||
                stateSynchronizationObserver == null)
            {
                Debug.LogError("StateSynchronization scene isn't configured correctly");
                return;
            }

            stateSynchronizationBroadcaster.LocalizationMechanism = localizationMechanism;
            stateSynchronizationObserver.LocalizationMechanism = localizationMechanism;

            switch (Role)
            {
                case Role.Broadcaster:
                    RunAsBroadcaster();
                    break;
                case Role.Observer:
                    RunAsObserver();
                    break;
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Role == Role.Observer)
            {
                Camera.main.transform.localPosition = StateSynchronizationObserver.Instance.transform.position;
                Camera.main.transform.localRotation = StateSynchronizationObserver.Instance.transform.rotation;
            }
        }
#endif

        private void RunAsBroadcaster()
        {
            broadcastedContent.SetActive(true);
            stateSynchronizationBroadcaster.gameObject.SetActive(true);
            stateSynchronizationObserver.gameObject.SetActive(false);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            stateSynchronizationSceneManager.gameObject.SetActive(true);
        }

        private void RunAsObserver()
        {
            // All content in the observer scene should be dynamically setup/created, so we hide scene content here
            broadcastedContent.SetActive(false);

            stateSynchronizationBroadcaster.gameObject.SetActive(false);
            stateSynchronizationObserver.gameObject.SetActive(true);

            // The StateSynchronizationSceneManager needs to be enabled after the broadcaster/observer
            stateSynchronizationSceneManager.gameObject.SetActive(true);

            // Make sure the StateSynchronizationSceneManager is enabled prior to connecting the observer
            stateSynchronizationObserver.ConnectTo(broadcasterIpAddress);
        }
    }
}
