namespace HoloToolkit.XTools
{
    /// <summary>
    /// Allows users of NetworkConnection to register to receive event callbacks without
    /// having their classes inherit directly from NetworkConnectionListener
    /// </summary>
    public class NetworkConnectionAdapter : NetworkConnectionListener
    {
        public delegate void ConnectedDelegate(NetworkConnection connection);
        public delegate void ConnectionFailedDelegate(NetworkConnection connection);
        public delegate void DisconnectedDelegate(NetworkConnection connection);
        public delegate void MessageReceivedDelegate(NetworkConnection connection, NetworkInMessage message);

        public ConnectedDelegate ConnectedCallback;
        public ConnectionFailedDelegate ConnectionFailedCallback;
        public DisconnectedDelegate DisconnectedCallback;
        public MessageReceivedDelegate MessageReceivedCallback;

        public NetworkConnectionAdapter()
        {

        }

        public override void OnConnected(NetworkConnection connection)
        {
            XTools.Profile.BeginRange("OnConnected");
            if (this.ConnectedCallback != null)
            {
                this.ConnectedCallback(connection);
            }
            XTools.Profile.EndRange();
        }

        public override void OnConnectFailed(NetworkConnection connection)
        {
            XTools.Profile.BeginRange("OnConnectFailed");
            if (this.ConnectionFailedCallback != null)
            {
                this.ConnectionFailedCallback(connection);
            }
            XTools.Profile.EndRange();
        }

        public override void OnDisconnected(NetworkConnection connection)
        {
            XTools.Profile.BeginRange("OnDisconnected");
            if (this.DisconnectedCallback != null)
            {
                this.DisconnectedCallback(connection);
            }
            XTools.Profile.EndRange();
        }

        public override void OnMessageReceived(NetworkConnection connection, NetworkInMessage message)
        {
            XTools.Profile.BeginRange("OnMessageReceived");
            if (this.MessageReceivedCallback != null)
            {
                this.MessageReceivedCallback(connection, message);
            }
            XTools.Profile.EndRange();
        }
    }
}