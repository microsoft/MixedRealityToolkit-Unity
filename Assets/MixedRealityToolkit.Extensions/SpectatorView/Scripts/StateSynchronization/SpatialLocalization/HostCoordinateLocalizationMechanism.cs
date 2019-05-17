// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Helper localization method here the host finds a QR code and gives it to the observer
    /// </summary>
    internal abstract class HostCoordinateLocalizationMechanism : LocalizationMechanismBase
    {
        private readonly object lockObject = new object();
        private readonly List<Guid> guids = new List<Guid>();
        private Task<ISpatialCoordinate> initializeBroadcasterCoordinateTask = null;
        private TaskCompletionSource<string> observerCoordinateIdToLookFor = null;

        /// <summary>
        /// The spatial coordinate service for sub class to instantiate and this helper base to rely on.
        /// </summary>
        protected abstract ISpatialCoordinateService SpatialCoordinateService { get; }
        
        /// <summary>
        /// The logic for the host to figure out which coordinate to use for localizing with observer.
        /// </summary>
        /// <param name="token">The token that first requested this host coordinate.</param>
        /// <returns>The spatial coordinate.</returns>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
}
