using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class SpatialCoordinateSystemManager : Singleton<SpatialCoordinateSystemManager>,
        ICommandHandler
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

        readonly string[] supportedCommands = { SpatialLocalizer.SpatialLocalizationMessageHeader };
        private Dictionary<SocketEndpoint, SpatialCoordinateSystemMember> participants = new Dictionary<SocketEndpoint, SpatialCoordinateSystemMember>();

        public void OnConnected(SocketEndpoint endpoint)
        {
            if (participants.ContainsKey(endpoint))
            {
                Debug.LogWarning("SpatialCoordinateSystemMember connected that already existed");
                return;
            }

            if (spectatorView.Role == Role.Spectator)
            {
                if (participants.Count > 0 &&
                    !participants.ContainsKey(endpoint))
                {
                    Debug.LogWarning("A second SpatialCoordinateSystemMember connected while the device was running as a spectator. This is an unexpected scenario.");
                    return;
                }
            }

            DebugLog($"Creating new SpatialCoordinateSystemMember, Role: {spectatorView.Role}, IPAddress: {endpoint.Address}, SceneRoot: {transformedGameObject}, DebugLogging: {debugLogging}");
            var actAsHost = spectatorView.Role == Role.User;
            var member = new SpatialCoordinateSystemMember(actAsHost, endpoint, () => transformedGameObject, debugLogging, showDebugVisuals, debugVisual, debugVisualScale);
            participants[endpoint] = member;
            if (spatialLocalizer != null)
            {
                DebugLog($"Localizing SpatialCoordinateSystemMember: {endpoint.Address}");
                member.LocalizeAsync(spatialLocalizer).FireAndForget();
            }
            else
            {
                Debug.LogWarning("Spatial localizer not specified for SpatialCoordinateSystemManager");
            }
        }

        public void OnDisconnected(SocketEndpoint endpoint)
        {
            if (participants.TryGetValue(endpoint, out var member))
            {
                member.Dispose();
                participants.Remove(endpoint);
            }
        }

        public void HandleCommand(SocketEndpoint endpoint, string command, BinaryReader reader)
        {
            if (!participants.TryGetValue(endpoint, out var member))
            {
                Debug.LogError("Received a message for an endpoint that had no associated spatial coordinate system member");
            }
            else
            {
                member.ReceiveMessage(command, reader);
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

        private void RegisterCommands()
        {
            DebugLog($"Registering for appropriate commands: CommandService.IsInitialized: {CommandService.IsInitialized}");
            foreach (var command in supportedCommands)
            {
                CommandService.Instance.RegisterCommandHandler(command, this);
            }
        }

        private void UnregisterCommands()
        {
            DebugLog($"Unregistering for appropriate commands: CommandService.IsInitialized: {CommandService.IsInitialized}");
            foreach (var command in supportedCommands)
            {
                CommandService.Instance.UnregisterCommandHandler(command, this);
            }
        }

        private void CleanUpParticipants()
        {
            foreach(var member in participants)
            {
                member.Value.Dispose();
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
    }
}
