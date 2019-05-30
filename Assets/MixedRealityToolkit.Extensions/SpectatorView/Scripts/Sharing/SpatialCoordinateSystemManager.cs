using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class SpatialCoordinateSystemManager : Singleton<SpatialCoordinateSystemManager>
    {
        /// <summary>
        /// SpectatorView MonoBehaviour running on the device.
        /// </summary>
        [Tooltip("SpectatorView MonoBehaviour running on the device.")]
        [SerializeField]
        private SpectatorView spectatorView = null;

        /// <summary>
        /// SpatialLocalizer used for setting up the coordinate system.
        /// </summary>
        [Tooltip("SpatialLocalizer used for setting up the coordinate system.")]
        [SerializeField]
        private SpatialLocalizer spatialLocalizer = null;

        /// <summary>
        /// GameObject that is transformed to move content into the correct position within the spatial coordinate system.
        /// </summary>
        public GameObject transformedGameObject { get; set; }

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

        public const string SpatialLocalizationMessageHeader = "LOCALIZE";
        readonly string[] supportedCommands = { SpatialLocalizationMessageHeader };

        private Dictionary<SocketEndpoint, SpatialCoordinateSystemParticipant> participants = new Dictionary<SocketEndpoint, SpatialCoordinateSystemParticipant>();

        public void OnConnected(SocketEndpoint endpoint)
        {
            if (participants.ContainsKey(endpoint))
            {
                Debug.LogWarning("SpatialCoordinateSystemParticipant connected that already existed");
                return;
            }

            if (spectatorView.Role == Role.Spectator)
            {
                if (participants.Count > 0 &&
                    !participants.ContainsKey(endpoint))
                {
                    Debug.LogWarning("A second SpatialCoordinateSystemParticipant connected while the device was running as a spectator. This is an unexpected scenario.");
                    return;
                }
            }

            DebugLog($"Creating new SpatialCoordinateSystemParticipant, Role: {spectatorView.Role}, IPAddress: {endpoint.Address}, SceneRoot: {transformedGameObject}, DebugLogging: {debugLogging}");
            var actAsHost = spectatorView.Role == Role.User;
            var participant = new SpatialCoordinateSystemParticipant(actAsHost, endpoint, WriteAndSendMessage, CreateDebugVisualParent, debugLogging, showDebugVisuals, debugVisual, debugVisualScale);
            participants[endpoint] = participant;
            if (spatialLocalizer != null)
            {
                DebugLog($"Localizing SpatialCoordinateSystemParticipant: {endpoint.Address}");
                participant.LocalizeAsync(spatialLocalizer).FireAndForget();
            }
            else
            {
                Debug.LogWarning("Spatial localizer not specified for SpatialCoordinateSystemManager");
            }
        }

        public void OnDisconnected(SocketEndpoint endpoint)
        {
            if (participants.TryGetValue(endpoint, out var participant))
            {
                participant.Dispose();
                participants.Remove(endpoint);
            }
        }

        public void HandleCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            if (command == SpatialLocalizationMessageHeader)
            {
                if (!participants.TryGetValue(endpoint, out var participant))
                {
                    Debug.LogError("Received a message for an endpoint that had no associated spatial coordinate system participant");
                }
                else
                {
                    participant.ReceiveMessage(reader);
                }
            }
        }

        protected override void Awake()
        {
            RegisterCommands();
        }

        protected override void OnDestroy()
        {
            UnregisterCommands();
            CleanUpParticipants();
        }

        private void Update()
        {
            if (spectatorView.parentOfMainCamera != null)
            {
                if (spectatorView.Role == Role.User)
                {
                    // We don't currently apply a transform to the user's camera.
                    return;
                }
                else
                {
                    // When running as the spectator, we should currently only have one potential participant (the user)
                    foreach (var participant in participants)
                    {
                        if (participant.Value.TryTransformToHostWorldSpace(Vector3.zero, out var position) &&
                            participant.Value.TryTransformToHostWorldSpace(Quaternion.identity, out var rotation))
                        {
                            DebugLog($"Parent of main camera transform set: Position: {position.ToString("G4")}, Rotation: {rotation.ToString("G4")}");
                            spectatorView.parentOfMainCamera.transform.position = position;
                            spectatorView.parentOfMainCamera.transform.rotation = rotation;
                            break;
                        }
                    }
                }
            }
        }

        private void RegisterCommands()
        {
            DebugLog($"Registering for appropriate commands: StateSynchronizationObserver.IsInitialized: {StateSynchronizationObserver.IsInitialized}, StateSynchronizationBroadcaster.IsInitialized: {StateSynchronizationBroadcaster.IsInitialized}");
            foreach (var command in supportedCommands)
            {
                if (StateSynchronizationObserver.IsInitialized)
                {
                    StateSynchronizationObserver.Instance.Connected += OnConnected;
                    StateSynchronizationObserver.Instance.Disconnected += OnDisconnected;
                    StateSynchronizationObserver.Instance.RegisterCommandHandler(command, HandleCommand);
                }
                if (StateSynchronizationBroadcaster.IsInitialized)
                {
                    StateSynchronizationBroadcaster.Instance.Connected += OnConnected;
                    StateSynchronizationBroadcaster.Instance.Disconnected += OnDisconnected;
                    StateSynchronizationBroadcaster.Instance.RegisterCommandHandler(command, HandleCommand);
                }
            }
        }

        private void UnregisterCommands()
        {
            DebugLog($"Unregistering for appropriate commands: StateSynchronizationObserver.IsInitialized: {StateSynchronizationObserver.IsInitialized}, StateSynchronizationBroadcaster.IsInitialized: {StateSynchronizationBroadcaster.IsInitialized}");
            foreach (var command in supportedCommands)
            {
                if (StateSynchronizationObserver.IsInitialized)
                {
                    StateSynchronizationObserver.Instance.Connected -= OnConnected;
                    StateSynchronizationObserver.Instance.Disconnected -= OnDisconnected;
                    StateSynchronizationObserver.Instance.UnregisterCommandHandler(command, HandleCommand);
                }
                if (StateSynchronizationBroadcaster.IsInitialized)
                {
                    StateSynchronizationBroadcaster.Instance.Connected -= OnConnected;
                    StateSynchronizationBroadcaster.Instance.Disconnected -= OnDisconnected;
                    StateSynchronizationBroadcaster.Instance.UnregisterCommandHandler(command, HandleCommand);
                }
            }
        }

        private void CleanUpParticipants()
        {
            foreach(var participant in participants)
            {
                participant.Value.Dispose();
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

        private void WriteAndSendMessage(SocketEndpoint endpoint, Action<BinaryWriter> callToWrite)
        {
            DebugLog($"Writing and sending message to endpoint: {endpoint.Address}");
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(SpatialLocalizationMessageHeader);
                callToWrite(writer);
                endpoint.Send(memoryStream.ToArray());
            }
        }

        private GameObject CreateDebugVisualParent()
        {
            var go = new GameObject();
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            if (spectatorView.parentOfMainCamera != null)
            {
                go.transform.parent = spectatorView.parentOfMainCamera.transform;
            }

            return go;
        }
    }
}
