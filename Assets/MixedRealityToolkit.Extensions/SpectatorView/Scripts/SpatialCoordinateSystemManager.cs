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

        public const string SpatialLocalizationMessageHeader = "LOCALIZE";
        readonly string[] supportedCommands = { SpatialLocalizationMessageHeader };
        private Dictionary<SocketEndpoint, SpatialCoordinateSystemMember> members = new Dictionary<SocketEndpoint, SpatialCoordinateSystemMember>();

        public void OnConnected(SocketEndpoint endpoint)
        {
            if (members.ContainsKey(endpoint))
            {
                Debug.LogWarning("SpatialCoordinateSystemMember connected that already existed");
                return;
            }

            if (spectatorView.Role == Role.Spectator)
            {
                if (members.Count > 0 &&
                    !members.ContainsKey(endpoint))
                {
                    Debug.LogWarning("A second SpatialCoordinateSystemMember connected while the device was running as a spectator. This is an unexpected scenario.");
                    return;
                }
            }

            DebugLog($"Creating new SpatialCoordinateSystemMember, Role: {spectatorView.Role}, IPAddress: {endpoint.Address}, SceneRoot: {transformedGameObject}, DebugLogging: {debugLogging}");
            var localizerRole = (spectatorView.Role == Role.User) ? LocalizerRole.Creator : LocalizerRole.Consumer;
            var member = new SpatialCoordinateSystemMember(localizerRole, endpoint, () => transformedGameObject, debugLogging, showDebugVisuals, debugVisual, debugVisualScale);
            members[endpoint] = member;
            if (spatialLocalizer != null)
            {
                DebugLog($"Localizing SpatialCoordinateSystemMember: {endpoint.Address}");
                member.LocalizeAsync(spatialLocalizer, OnCoordinateLocalized).FireAndForget();
            }
            else
            {
                Debug.LogWarning("Spatial localizer not specified for SpatialCoordinateSystemManager");
            }
        }

        public void OnDisconnected(SocketEndpoint endpoint)
        {
            if (members.TryGetValue(endpoint, out var member))
            {
                member.Dispose();
                members.Remove(endpoint);
            }
        }

        public void HandleCommand(SocketEndpoint endpoint, string command, BinaryReader reader)
        {
            switch (command)
            {
                case SpatialLocalizationMessageHeader:
                    {
                        if (!members.TryGetValue(endpoint, out var member))
                        {
                            Debug.LogError("Received a message for an endpoint that had no associated spatial coordinate system member");
                        }
                        else
                        {
                            ProcessMessage(endpoint, reader, member);
                        }
                    }
                    break;
            }
        }

        protected override void Awake()
        {
            RegisterCommands();
        }

        protected override void OnDestroy()
        {
            UnregisterCommands();
            CleanUpMembers();
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

        private void CleanUpMembers()
        {
            foreach(var member in members)
            {
                member.Value.Dispose();
            }

            members.Clear();
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"SpatialCoordinateSystemManager: {message}");
            }
        }

        private void OnCoordinateLocalized(SpatialCoordinateSystemMember member, ISpatialCoordinate coordinate)
        {
            switch (member.Role)
            {
                case LocalizerRole.Creator:
                    OnCreatorCoordinateLocalized(member.SocketEndpoint, coordinate);
                    break;
                case LocalizerRole.Consumer:
                    OnConsumerCoordinateLocalized(coordinate);
                    break;
            }
        }

        private void OnCreatorCoordinateLocalized(SocketEndpoint socketEndpoint, ISpatialCoordinate coordinate)
        {
            DebugLog("Sending message to connected client");
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                // Prepare the writer for sending a message
                writer.Write(SpatialLocalizationMessageHeader);

                // Tell the consumer device what anchor to look for
                writer.Write($"{coordinate.Id}");

                socketEndpoint.Send(memoryStream.ToArray());
                DebugLog("Sent Message");
            }
        }

        private void OnConsumerCoordinateLocalized(ISpatialCoordinate coordinate)
        {
            DebugLog($"Coordinate localized for consumer: {coordinate.Id}");
        }

        private void ProcessMessage(SocketEndpoint endpoint, BinaryReader reader, SpatialCoordinateSystemMember member)
        {
            string id = reader.ReadString();
            if(!member.TrySetCoordinateIdForLocalizer(spatialLocalizer, id))
            {
                DebugLog($"Could not set obtained coordinate id: {id} for SpatialCoordinateSystemMember");
            }
        }
    }
}
