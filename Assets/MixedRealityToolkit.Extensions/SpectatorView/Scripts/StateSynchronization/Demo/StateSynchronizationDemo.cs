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
        private readonly bool debugLogging;

        private Action<BinaryReader> processIncomingMessages = null;
        private GameObject observerGO = null;

        public ConnectedObserver(Role role, SocketEndpoint socketEndpoint, Func<GameObject> createAnchor, bool debugLogging)
        {
            cancellationToken = cancellationTokenSource.Token;

            this.role = role;
            this.socketEndpoint = socketEndpoint;
            this.createAnchor = createAnchor;
            this.debugLogging = debugLogging;
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"ConnectedObserver [{role} - Connection: {socketEndpoint.Address}]: {message}");
            }
        }

        public async Task LocalizeAsync(LocalizationMechanismBase localizationMechanism)
        {
            DebugLog("Started LocalizeAsync");

            DebugLog("Initializing with LocalizationMechanism.");
            // Tell the localization mechanism to initialize, this could create anchor if need be
            Guid token = await localizationMechanism.InitializeAsync(role, cancellationToken);
            DebugLog("Initialized with LocalizationMechanism");

            try
            {
                lock (cancellationTokenSource)
                {
                    processIncomingMessages = r =>
                    {
                        DebugLog("Passing on incoming message");
                        localizationMechanism.ProcessIncomingMessage(role, token, r);
                    };
                }

                DebugLog("Telling LocalizationMechanims to begin localizng");
                ISpatialCoordinate coordinate = await localizationMechanism.LocalizeAsync(role, token, (callback) => // Allows localization mechanism to tell spectator what to do
                {
                    DebugLog("Sending message to connected client");
                    using (MemoryStream memoryStream = new MemoryStream())
                    using (BinaryWriter writer = new BinaryWriter(memoryStream))
                    {
                        writer.Write(LocalizationMessageHeader);

                        callback(writer);
                        socketEndpoint.Send(memoryStream.ToArray());
                        DebugLog("Sent Message");
                    }
                }, cancellationToken);

                if (coordinate == null)
                {
                    Debug.LogError($"Failed to localize for spectator: {socketEndpoint.Address}");
                }
                else
                {
                    DebugLog("Creating Visual Anchor");
                    lock (cancellationTokenSource)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            observerGO = createAnchor();
                            observerGO.AddComponent<SpatialCoordinateLocalizer>().Coordinate = coordinate;
                            DebugLog("Anchor created, coordinate set");
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

                DebugLog("Deinitializing");
                localizationMechanism.Deinitialize(role, token);
                DebugLog("Deinitialized");
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

            DebugLog("Disposed");

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
        private string typeName;

        [SerializeField]
        protected bool debugLogging = false;

        private string TypeName => typeName ?? (typeName = GetType().Name);

        protected void DebugLog(string message, Guid token)
        {
            if (debugLogging)
            {
                Debug.Log($"{TypeName} - {token}: {message}");
            }
        }

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

        protected virtual async Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token)
        {
            DebugLog("Getting host coordinate", token);

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                TaskCompletionSource<ISpatialCoordinate> coordinateTCS = new TaskCompletionSource<ISpatialCoordinate>();
                void coordianteDiscovered(ISpatialCoordinate coord)
                {
                    DebugLog("Coordinate found", token);
                    coordinateTCS.SetResult(coord);
                    cts.Cancel();
                }

                SpatialCoordinateService.CoordinatedDiscovered += coordianteDiscovered;
                try
                {
                    DebugLog("Starting to look for coordinates", token);
                    await SpatialCoordinateService.TryDiscoverCoordinatesAsync(cts.Token);
                    DebugLog("Stopped looking for coordinates", token);


                    DebugLog("Awaiting found coordiante", token);
                    // Don't necessarily need to await here
                    return await coordinateTCS.Task;
                }
                finally
                {
                    DebugLog("Unsubscribing from coordinate discovered", token);
                    SpatialCoordinateService.CoordinatedDiscovered -= coordianteDiscovered;
                }
            }
        }

        internal async override Task<Guid> InitializeAsync(Role role, CancellationToken cancellationToken)
        {
            Guid token = Guid.NewGuid();
            DebugLog("Begining initialization", token);
            if (role == Role.Broadcaster)
            {
                DebugLog("Broadcaster", token);
                lock (lockObject)
                {
                    DebugLog("Checking for host init task", token);
                    if (initializeBroadcasterCoordinateTask == null)
                    {
                        DebugLog("Creating new host init task", token);
                        initializeBroadcasterCoordinateTask = GetHostCoordinateAsync(token);
                        DebugLog("Host init task created", token);
                    }
                }

                DebugLog("Waiting for init or cancellation.", token);
                // Wait for broadcaster to initialize (which happens once and won't be cancelled), or until this request was cancelled.
                await Task.WhenAny(Task.Delay(-1, cancellationToken), initializeBroadcasterCoordinateTask);
                DebugLog("Got Init task finished", token);
                //We have the coordinate after this step has finished
            }
            else if (role == Role.Observer)
            {
                DebugLog("Observer reset task completion source", token);
                observerCoordinateIdToLookFor?.SetCanceled();
                observerCoordinateIdToLookFor = new TaskCompletionSource<string>();
            }

            DebugLog($"Added guid and returning.", token);
            guids.Add(token);
            return token;
        }

        internal override void ProcessIncomingMessage(Role role, Guid token, BinaryReader r)
        {
            DebugLog("Processing incoming message", token);
            switch (role)
            {
                case Role.Broadcaster:
                    break;
                case Role.Observer:
                    string result = r.ReadString();
                    DebugLog($"Incoming message string: {result}, setting as coordinate id.", token);
                    observerCoordinateIdToLookFor.TrySetResult(result);
                    DebugLog("Set coordinate id.", token);
                    break;
            }
        }

        internal override async Task<ISpatialCoordinate> LocalizeAsync(Role role, Guid token, Action<Action<BinaryWriter>> sendMessage, CancellationToken cancellationToken)
        {
            DebugLog("Beginning localization", token);
            ISpatialCoordinate coordinateToReturn = null;

            switch (role)
            {
                case Role.Broadcaster:
                    DebugLog("Broadcaster getting initialized coordinate", token);
                    coordinateToReturn = initializeBroadcasterCoordinateTask.Result;
                    DebugLog($"Sending coordinate id: {coordinateToReturn.Id}", token);
                    sendMessage(writer => writer.Write(coordinateToReturn.Id));
                    DebugLog("Message sent.", token);
                    break;
                case Role.Observer:
                    DebugLog("Spectator waiting for coord id to be sent over", token);
                    await Task.WhenAny(observerCoordinateIdToLookFor.Task, Task.Delay(-1, cancellationToken)); //If we get cancelled, or get a token

                    DebugLog("Coordinate id received, reading.", token);
                    // Now we have coordinateId in TaskCompletionSource
                    string id = observerCoordinateIdToLookFor.Task.Result;
                    DebugLog($"Coordinate id: {id}, starting discovery.", token);

                    if (await SpatialCoordinateService.TryDiscoverCoordinatesAsync(cancellationToken, id))
                    {
                        DebugLog("Discovery complete, retrieving reference to ISpatialCoordinate", token);
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

            DebugLog("Returning coordinate.", token);
            return coordinateToReturn;
        }

        internal override void Deinitialize(Role role, Guid token)
        {
            DebugLog($"Deinitializing: {role}", token);
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
