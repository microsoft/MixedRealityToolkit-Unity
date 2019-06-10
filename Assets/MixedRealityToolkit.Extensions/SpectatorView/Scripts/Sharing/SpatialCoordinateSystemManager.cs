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
        private const string RestorePersistentSharedCoordinateCommand = "INITSHAREDCOORD";
        private readonly Dictionary<Guid, ISpatialLocalizer> localizers = new Dictionary<Guid, ISpatialLocalizer>();
        private Dictionary<SocketEndpoint, SpatialCoordinateSystemParticipant> participants = new Dictionary<SocketEndpoint, SpatialCoordinateSystemParticipant>();
        private HashSet<INetworkManager> networkManagers = new HashSet<INetworkManager>();
        private ISpatialLocalizationSession currentLocalizationSession = null;

        public void RegisterSpatialLocalizer(ISpatialLocalizer localizer)
        {
            if (localizers.ContainsKey(localizer.SpatialLocalizerID))
            {
                Debug.LogError($"Cannot register multiple SpatialLocalizers with the same ID {localizer.SpatialLocalizerID}");
                return;
            }

            localizers.Add(localizer.SpatialLocalizerID, localizer);
        }

        public void UnregisterSpatialLocalizer(ISpatialLocalizer localizer)
        {
            if (!localizers.Remove(localizer.SpatialLocalizerID))
            {
                Debug.LogError($"Attempted to unregister SpatialLocalizer with ID {localizer.SpatialLocalizerID} that was not registered.");
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

        public void RestorePersistentSharedCoordinate(SocketEndpoint socketEndpoint, string sharedCoordinateName)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(stream))
            {
                message.Write(RestorePersistentSharedCoordinateCommand);
                message.Write(sharedCoordinateName);

                socketEndpoint.Send(stream.ToArray());
            }
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

        private void OnConnected(SocketEndpoint endpoint)
        {
            if (participants.ContainsKey(endpoint))
            {
                Debug.LogWarning("SpatialCoordinateSystemParticipant connected that already existed");
                return;
            }

            DebugLog($"Creating new SpatialCoordinateSystemParticipant, IPAddress: {endpoint.Address}, DebugLogging: {debugLogging}");

            GameObject participantGameObject = new GameObject($"Spatial Coordinate - {endpoint.Address}");
            SpatialCoordinateSystemParticipant participant = participantGameObject.AddComponent<SpatialCoordinateSystemParticipant>();
            participant.SocketEndpoint = endpoint;
            participants[endpoint] = participant;

            if (showDebugVisuals)
            {
                var debugVisualInstance = Instantiate(debugVisual, participant.transform);
                debugVisualInstance.transform.localPosition = Vector3.zero;
                debugVisualInstance.transform.localRotation = Quaternion.identity;
                debugVisualInstance.transform.localScale = Vector3.one * debugVisualScale;
            }
        }

        private void OnDisconnected(SocketEndpoint endpoint)
        {
            if (participants.TryGetValue(endpoint, out var participant))
            {
                Destroy(participant.gameObject);
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

        private void OnRestorePersistentSharedCoordinateReceived(SocketEndpoint socketEndpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            if (!participants.TryGetValue(socketEndpoint, out SpatialCoordinateSystemParticipant participant))
            {
                Debug.LogError($"Failed to find a SpatialCoordinateSystemParticipant for an attached SocketEndpoint");
                return;
            }

            string coordinateId = reader.ReadString();
            //participant.PersistentCoordinateId = coordinateId;
            //participant.Coordinate = new WorldAnchorSpatialCoordinate(coordinateId);
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

        private async Task RunLocalizationSessionAsync(ISpatialLocalizer localizer, ISpatialLocalizationSettings settings, SpatialCoordinateSystemParticipant participant)
        {
            using (currentLocalizationSession = localizer.CreateLocalizationSession(settings))
            {
                //if (participant.Coordinate is WorldAnchorSpatialCoordinate worldAnchorSpatialCoordinate)
                //{
                //    worldAnchorSpatialCoordinate.RemoveAnchor();
                //}

                participant.IsLocatingSpatialCoordinate = true;
                var coordinate = await currentLocalizationSession.LocalizeAsync(CancellationToken.None);
                //if (participant.PersistentCoordinateId != null)
                //{
                //    participant.Coordinate = new WorldAnchorSpatialCoordinate(participant.PersistentCoordinateId, coordinate.CoordinateToWorldSpace(Vector3.zero), coordinate.CoordinateToWorldSpace(Quaternion.identity));
                //}
                //else
                //{
                    participant.Coordinate = coordinate;
                //}
                participant.IsLocatingSpatialCoordinate = false;
            }
            currentLocalizationSession = null;
        }

        private void RegisterEvents(INetworkManager networkManager)
        {
            networkManager.Connected += OnConnected;
            networkManager.Disconnected += OnDisconnected;
            networkManager.RegisterCommandHandler(LocalizeCommand, OnLocalizeMessageReceived);
            networkManager.RegisterCommandHandler(RestorePersistentSharedCoordinateCommand, OnRestorePersistentSharedCoordinateReceived);
            networkManager.RegisterCommandHandler(CoordinateStateMessageHeader, OnCoordinateStateReceived);
        }

        private void UnregisterEvents(INetworkManager networkManager)
        {
            networkManager.Connected -= OnConnected;
            networkManager.Disconnected -= OnDisconnected;
            networkManager.UnregisterCommandHandler(LocalizeCommand, OnLocalizeMessageReceived);
            networkManager.UnregisterCommandHandler(RestorePersistentSharedCoordinateCommand, OnRestorePersistentSharedCoordinateReceived);
            networkManager.UnregisterCommandHandler(CoordinateStateMessageHeader, OnCoordinateStateReceived);
        }

        private void CleanUpParticipants()
        {
            foreach(var participant in participants)
            {
                if (participant.Value != null)
                {
                    Destroy(participant.Value.gameObject);
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
