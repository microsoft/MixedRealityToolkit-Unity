//
// Copyright (C) Microsoft. All rights reserved.
//

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of NetworkConnection to register to receive event callbacks without
    /// having their classes inherit directly from NetworkConnectionListener
    /// </summary>
    public class NetworkConnectionAdapter : NetworkConnectionListener
    {
        public event System.Action<NetworkConnection> ConnectedCallback;
        public event System.Action<NetworkConnection> ConnectionFailedCallback;
        public event System.Action<NetworkConnection> DisconnectedCallback;
        public event System.Action<NetworkConnection, NetworkInMessage> MessageReceivedCallback;

        public NetworkConnectionAdapter() { }

        public override void OnConnected(NetworkConnection connection)
        {
            Profile.BeginRange("OnConnected");
            if (this.ConnectedCallback != null)
            {
                this.ConnectedCallback(connection);
            }
            Profile.EndRange();
        }

        public override void OnConnectFailed(NetworkConnection connection)
        {
            Profile.BeginRange("OnConnectFailed");
            if (this.ConnectionFailedCallback != null)
            {
                this.ConnectionFailedCallback(connection);
            }
            Profile.EndRange();
        }

        public override void OnDisconnected(NetworkConnection connection)
        {
            Profile.BeginRange("OnDisconnected");
            if (this.DisconnectedCallback != null)
            {
                this.DisconnectedCallback(connection);
            }
            Profile.EndRange();
        }

        public override void OnMessageReceived(NetworkConnection connection, NetworkInMessage message)
        {
            Profile.BeginRange("OnMessageReceived");
            if (this.MessageReceivedCallback != null)
            {
                this.MessageReceivedCallback(connection, message);
            }
            Profile.EndRange();
        }
    }
}