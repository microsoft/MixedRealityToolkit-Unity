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
        [SerializeField]
        private SpectatorView spectatorView;

        /// <summary>
        /// Prefab created and placed based at each spatial coordinate
        /// </summary>
        [SerializeField]
        private GameObject spatialCoordinatePrefab;

        public const string SpatialLocalizationMessageHeader = "LOCALIZE";
        readonly string[] supportedCommands = { SpatialLocalizationMessageHeader };
        private Dictionary<SocketEndpoint, SpatialCoordinateSystemMember> members = new Dictionary<SocketEndpoint, SpatialCoordinateSystemMember>();
        internal SpatialLocalizer spatialLocalizer;

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

            var roleOfConnection = (spectatorView.Role == Role.User) ? Role.Spectator : Role.User;
            var member = new SpatialCoordinateSystemMember(Role.Spectator, endpoint, null, true);
            members[endpoint] = member;
            if (spatialLocalizer != null)
            {
                member.LocalizeAsync(spatialLocalizer).FireAndForget();
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

        public void HandleCommand(string command, SocketEndpoint endpoint, BinaryReader reader)
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
                            member.ReceiveMessage(reader);
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
            foreach(var command in supportedCommands)
            {
                StateSynchronizationBroadcaster.Instance.Register(command, this);
                StateSynchronizationObserver.Instance.Register(command, this);
            }
        }

        private void UnregisterCommands()
        {
            foreach (var command in supportedCommands)
            {
                StateSynchronizationBroadcaster.Instance.Unregister(command, this);
                StateSynchronizationObserver.Instance.Unregister(command, this);
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
    }
}
