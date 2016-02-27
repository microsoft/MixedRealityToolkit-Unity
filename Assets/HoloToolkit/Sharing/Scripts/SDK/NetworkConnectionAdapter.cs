namespace HoloToolkit.Sharing
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
            if (this.ConnectedCallback != null)
            {
                this.ConnectedCallback(connection);
            }            
        }

        public override void OnConnectFailed(NetworkConnection connection)
        {
            if (this.ConnectionFailedCallback != null)
            {
                this.ConnectionFailedCallback(connection);
            }            
        }

        public override void OnDisconnected(NetworkConnection connection)
        {
            if (this.DisconnectedCallback != null)
            {
                this.DisconnectedCallback(connection);
            }            
        }

        public override void OnMessageReceived(NetworkConnection connection, NetworkInMessage message)
        {
            if (this.MessageReceivedCallback != null)
            {
                this.MessageReceivedCallback(connection, message);
            }            
        }
    }
}