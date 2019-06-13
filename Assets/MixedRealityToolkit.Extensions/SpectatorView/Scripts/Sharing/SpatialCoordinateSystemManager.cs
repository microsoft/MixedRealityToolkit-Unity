using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class SpatialCoordinateSystemManager : Singleton<SpatialCoordinateSystemManager>
    {
        /// <summary>
        /// Check for debug logging.
        /// </summary>
        [Tooltip("Check for debug logging.")]
        [SerializeField]
        private bool debugLogging = false;

        /// <summary>
        /// Check to show debug visuals.
        /// </summary>
        [Tooltip("Check to show debug visuals.")]
        public bool showDebugVisuals = false;

        /// <summary>
        /// Game Object to render at spatial coordinate locations when showing debug visuals.
        /// </summary>
        [Tooltip("Game Object to render at spatial coordinate locations when showing debug visuals.")]
        public GameObject debugVisual = null;

        /// <summary>
        /// Debug visual scale.
        /// </summary>
        [Tooltip("Debug visual scale.")]
        public float debugVisualScale = 1.0f;

        internal const string CoordinateStateMessageHeader = "COORDSTATE";
        private const string LocalizeCommand = "LOCALIZE";
        private readonly Dictionary<Guid, ISpatialLocalizer> localizers = new Dictionary<Guid, ISpatialLocalizer>();
        private Dictionary<SocketEndpoint, SpatialCoordinateSystemParticipant> participants = new Dictionary<SocketEndpoint, SpatialCoordinateSystemParticipant>();
        private HashSet<INetworkManager> networkManagers = new HashSet<INetworkManager>();
        private ISpatialLocalizationSession currentLocalizationSession = null;

        public void RegisterSpatialLocalizer(ISpatialLocalizer localizer)
        {
            if (localizers.ContainsKey(localizer.SpatialLocalizerId))
            {
                Debug.LogError($"Cannot register multiple SpatialLocalizers with the same ID {localizer.SpatialLocalizerId}");
                return;
            }

            localizers.Add(localizer.SpatialLocalizerId, localizer);
        }

        public void UnregisterSpatialLocalizer(ISpatialLocalizer localizer)
        {
            if (!localizers.Remove(localizer.SpatialLocalizerId))
            {
                Debug.LogError($"Attempted to unregister SpatialLocalizer with ID {localizer.SpatialLocalizerId} that was not registered.");
            }
        }

        public void RegisterNetworkManager(INetworkManager networkManager)
        {
            if (!networkManagers.Add(networkManager))
            {
                Debug.LogError($"Attempted to register the same network manager multiple times");
                return;
            }

            RegisterEvents(networkManager);
        }

        public void UnregisterNetworkManager(INetworkManager networkManager)
        {
            if (!networkManagers.Remove(networkManager))
            {
                Debug.LogError($"Attempted to unregister a network manager that was not registered");
                return;
            }

            UnregisterEvents(networkManager);
        }

        public void InitiateRemoteLocalization(SocketEndpoint socketEndpoint, Guid spatialLocalizerID, ISpatialLocalizationSettings settings)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(stream))
            {
                message.Write(LocalizeCommand);
                message.Write(spatialLocalizerID);
                settings.Serialize(message);

                socketEndpoint.Send(stream.ToArray());
            }
        }

        public Task LocalizeAsync(SocketEndpoint socketEndpoint, ISpatialLocalizer localizer, ISpatialLocalizationSettings settings)
        {
            if (currentLocalizationSession != null)
            {
                Debug.LogError($"Failed to start localization session because an existing localization session is in progress");
                return Task.CompletedTask;
            }

            if (!participants.TryGetValue(socketEndpoint, out SpatialCoordinateSystemParticipant participant))
            {
                Debug.LogError($"Could not find a SpatialCoordinateSystemParticipant for SocketEndpoint {socketEndpoint.Address}");
                return Task.CompletedTask;
            }

            return RunLocalizationSessionAsync(localizer, settings, participant);
        }

        protected override void OnDestroy()
        {
            foreach (INetworkManager networkManager in networkManagers)
            {
                UnregisterEvents(networkManager);
            }

            CleanUpParticipants();
        }

        private void Update()
        {
            foreach (var participant in participants.Values)
            {
                participant.CheckForStateChanges();
            }
        }

        private void OnConnected(SocketEndpoint endpoint)
        {
            if (participants.ContainsKey(endpoint))
            {
                Debug.LogWarning("SpatialCoordinateSystemParticipant connected that already existed");
                return;
            }

            DebugLog($"Creating new SpatialCoordinateSystemParticipant, IPAddress: {endpoint.Address}, DebugLogging: {debugLogging}");

            SpatialCoordinateSystemParticipant participant = new SpatialCoordinateSystemParticipant(endpoint, debugVisual, debugVisualScale);
            participants[endpoint] = participant;
            participant.ShowDebugVisuals = showDebugVisuals;
        }

        private void OnDisconnected(SocketEndpoint endpoint)
        {
            if (participants.TryGetValue(endpoint, out var participant))
            {
                participant.Dispose();
                participants.Remove(endpoint);
            }
        }

        private void OnCoordinateStateReceived(SocketEndpoint socketEndpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            if (!participants.TryGetValue(socketEndpoint, out SpatialCoordinateSystemParticipant participant))
            {
                Debug.LogError($"Failed to find a SpatialCoordinateSystemParticipant for an attached SocketEndpoint");
                return;
            }

            participant.PeerDeviceHasTracking = reader.ReadBoolean();
            participant.PeerSpatialCoordinateIsLocated = reader.ReadBoolean();
            participant.PeerIsLocatingSpatialCoordinate = reader.ReadBoolean();
            participant.PeerSpatialCoordinateWorldPosition = reader.ReadVector3();
            participant.PeerSpatialCoordinateWorldRotation = reader.ReadQuaternion();
        }

        private async void OnLocalizeMessageReceived(SocketEndpoint socketEndpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            if (currentLocalizationSession != null)
            {
                Debug.LogError($"Failed to start localization session because an existing localization session is in progress");
                return;
            }

            Guid spatialLocalizerID = reader.ReadGuid();

            if (!localizers.TryGetValue(spatialLocalizerID, out ISpatialLocalizer localizer))
            {
                Debug.LogError($"Request to begin localization with localizer {spatialLocalizerID} but no localizer with that ID was registered");
                return;
            }

            if (!localizer.TryDeserializeSettings(reader, out ISpatialLocalizationSettings settings))
            {
                Debug.LogError($"Failed to deserialize settings for localizer {spatialLocalizerID}");
                return;
            }

            if (!participants.TryGetValue(socketEndpoint, out SpatialCoordinateSystemParticipant participant))
            {
                Debug.LogError($"Could not find a SpatialCoordinateSystemParticipant for SocketEndpoint {socketEndpoint.Address}");
                return;
            }

            await RunLocalizationSessionAsync(localizer, settings, participant);
        }

        private void OnParticipantDataReceived(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            if (!TryGetSpatialCoordinateSystemParticipant(endpoint, out SpatialCoordinateSystemParticipant participant))
            {
                Debug.LogError("Received participant localization data for a missing participant");
                return;
            }

            if (participant.CurrentLocalizationSession == null)
            {
                Debug.LogError("Received participant localization data for a participant that is not currently running a localization session");
                return;
            }

            participant.CurrentLocalizationSession.OnDataReceived(reader);
        }

        private async Task RunLocalizationSessionAsync(ISpatialLocalizer localizer, ISpatialLocalizationSettings settings, SpatialCoordinateSystemParticipant participant)
        {
            using (currentLocalizationSession = localizer.CreateLocalizationSession(participant, settings))
            {
                participant.CurrentLocalizationSession = currentLocalizationSession;

                try
                {
                    var coordinate = await currentLocalizationSession.LocalizeAsync(CancellationToken.None);

                    participant.Coordinate = coordinate;
                }
                finally
                {
                    participant.CurrentLocalizationSession = null;
                }
            }
            currentLocalizationSession = null;
        }

        private void RegisterEvents(INetworkManager networkManager)
        {
            networkManager.Connected += OnConnected;
            networkManager.Disconnected += OnDisconnected;
            networkManager.RegisterCommandHandler(LocalizeCommand, OnLocalizeMessageReceived);
            networkManager.RegisterCommandHandler(CoordinateStateMessageHeader, OnCoordinateStateReceived);
            networkManager.RegisterCommandHandler(SpatialCoordinateSystemParticipant.LocalizationDataExchangeCommand, OnParticipantDataReceived);
        }

        private void UnregisterEvents(INetworkManager networkManager)
        {
            networkManager.Connected -= OnConnected;
            networkManager.Disconnected -= OnDisconnected;
            networkManager.UnregisterCommandHandler(LocalizeCommand, OnLocalizeMessageReceived);
            networkManager.UnregisterCommandHandler(CoordinateStateMessageHeader, OnCoordinateStateReceived);
            networkManager.UnregisterCommandHandler(SpatialCoordinateSystemParticipant.LocalizationDataExchangeCommand, OnParticipantDataReceived);
        }

        private void CleanUpParticipants()
        {
            foreach(var participant in participants)
            {
                if (participant.Value != null)
                {
                    participant.Value.Dispose();
                }
            }

            participants.Clear();
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"SpatialCoordinateSystemManager: {message}");
            }
        }

        internal bool TryGetSpatialCoordinateSystemParticipant(SocketEndpoint connectedEndpoint, out SpatialCoordinateSystemParticipant participant)
        {
            return participants.TryGetValue(connectedEndpoint, out participant);
        }
    }
}
