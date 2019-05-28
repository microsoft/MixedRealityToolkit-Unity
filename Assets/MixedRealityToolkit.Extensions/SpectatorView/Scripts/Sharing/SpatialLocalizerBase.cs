// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public abstract class SpatialLocalizerBase : SpatialLocalizer
    {
        protected ISpatialCoordinateService spatialCoordinateService = null;
        private Task<ISpatialCoordinate> initializeUserCoordinateTask = null;
        private TaskCompletionSource<string> observerCoordinateIdToLookFor = null;

        /// <inheritdoc/>
        protected override ISpatialCoordinateService SpatialCoordinateService => spatialCoordinateService;

        /// <summary>
        /// The logic for the host to figure out which coordinate to use for localizing with observer.
        /// </summary>
        /// <param name="token">The token that first requested this host coordinate.</param>
        /// <returns>The spatial coordinate.</returns>
        protected abstract Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token);

        /// <inheritdoc/>
        internal async override Task<Guid> InitializeAsync(bool actAsHost, CancellationToken cancellationToken)
        {
            Guid token = Guid.NewGuid();
            DebugLog("Begining initialization", token);
            if (actAsHost)
            {
                DebugLog("Host", token);
                lock (lockObject)
                {
                    DebugLog("Checking for host init task", token);
                    if (initializeUserCoordinateTask == null)
                    {
                        DebugLog("Creating new host init task", token);
                        initializeUserCoordinateTask = GetHostCoordinateAsync(token);
                        DebugLog("Host init task created", token);
                    }
                }

                DebugLog("Waiting for init or cancellation.", token);
                // Wait for broadcaster to initialize (which happens once and won't be cancelled), or until this request was cancelled.
                await Task.WhenAny(Task.Delay(-1, cancellationToken), initializeUserCoordinateTask);
                DebugLog("Got Init task finished", token);
                //We have the coordinate after this step has finished
            }
            else
            {
                DebugLog("Non-host reset task completion source", token);
                observerCoordinateIdToLookFor?.SetCanceled();
                observerCoordinateIdToLookFor = new TaskCompletionSource<string>();
            }

            DebugLog($"Added guid and returning.", token);
            return token;
        }

        /// <inheritdoc/>
        internal override void ProcessIncomingMessage(bool actAsHost, Guid token, string command, BinaryReader r)
        {
            DebugLog("Processing incoming message", token);
            if (command == SpatialLocalizationMessageHeader &&
                !actAsHost)
            {
                string result = r.ReadString();
                DebugLog($"Incoming message string: {result}, setting as coordinate id.", token);
                observerCoordinateIdToLookFor.TrySetResult(result);
                DebugLog("Set coordinate id.", token);
            }
        }

        /// <inheritdoc/>
        internal override async Task<ISpatialCoordinate> LocalizeAsync(bool actAsHost, Guid token, Action<Action<BinaryWriter>> writeAndSendMessage, CancellationToken cancellationToken)
        {
            DebugLog("Beginning spatial localization", token);
            ISpatialCoordinate coordinateToReturn = null;

            if (actAsHost)
            {
                DebugLog("User getting initialized coordinate", token);
                coordinateToReturn = initializeUserCoordinateTask.Result;
                DebugLog($"Sending coordinate id: {coordinateToReturn.Id}", token);
                writeAndSendMessage(writer =>
                {
                    writer.Write(SpatialLocalizationMessageHeader);
                    writer.Write(coordinateToReturn.Id);
                });

                DebugLog("Message sent.", token);
            }
            else
            {
                DebugLog("Non-host waiting for coord id to be sent over", token);
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
            }

            DebugLog("Returning coordinate.", token);
            return coordinateToReturn;
        }

        /// <inheritdoc/>
        internal override void Uninitialize(bool actAsHost, Guid token)
        {
            string title = actAsHost ? "Host" : "Non-host";
            DebugLog($"Deinitializing: {title}", token);
        }
    }
}
