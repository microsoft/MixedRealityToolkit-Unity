using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public interface ISharingService : IMixedRealityExtensionService
	{
        /// <summary>
        /// The status of the connection.
        /// </summary>
        ConnectStatus Status { get; }

        /// <summary>
        /// The sharing app's role as defined by the service.
        /// </summary>
        AppRole AppRole { get; }

        /// <summary>
        /// Name of joined lobby. Will be null until a lobby has been joined;
        /// </summary>
        string LobbyName { get; }

        /// <summary>
        /// Name of joined room. Will be null until a room has been joined.
        /// </summary>
        string RoomName { get; }

        /// <summary>
        /// True if our local device has connected to the service.
        /// </summary>
        bool LocalDeviceConnected { get; }

        /// <summary>
        /// The ID of our local device. Set once connected.
        /// </summary>
        short LocalDeviceID { get; }

        /// <summary>
        /// IDs of all connected devices, including local device.
        /// </summary>
        IEnumerable<short> ConnectedDevices { get; }

        /// <summary>
        /// Number of connected devices, including local device.
        /// </summary>
        int NumConnectedDevices { get; }

        /// <summary>
        /// Invoked when the service has connected.
        /// </summary>
        event ConnectionEvent OnConnect;

        /// <summary>
        /// Invoked when the sharing service's AppRole changes.
        /// </summary>
        event ConnectionEvent OnAppRoleSelected;

        /// <summary>
        /// Invoked when the service disconnects.
        /// </summary>
        event ConnectionEvent OnDisconnect;

        /// <summary>
        /// Invoked when subscription mode changes.
        /// </summary>
        event SubscriptionEvent OnLocalSubscriptionModeChange;

        /// <summary>
        /// Invoked when data is received from the service.
        /// </summary>
        event DataEvent OnReceiveData;

        /// <summary>
        /// Invoked when a device has connected, including our own.
        /// </summary>
        event DeviceEvent OnDeviceConnected;

        /// <summary>
        /// Invoked when a device has disconnected, including our own.
        /// </summary>
        event DeviceEvent OnDeviceDisconnected;

        /// <summary>
        /// Invoked when another device calls PingDevice for this device.
        /// </summary>
        event PingEvent OnLocalDevicePinged;

        /// <summary>
        /// Connect to the service using default configuration from profile.
        /// </summary>
        void Connect();

        /// <summary>
        /// Connect to the service using custom configuration.
        /// </summary>
        void ConnectCustom(ConnectConfig config);

        /// <summary>
        /// Disconnects from the service.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends data to anyone connected to the service.
        /// </summary>
        void SendData(SendDataArgs args);

        /// <summary>
        /// Sets the subscription mode as well as manual subscription types.
        /// If subscription mode is ALL then subscription types are ignored.
        /// </summary>
        void SetLocalSubscriptionMode(SubscriptionModeEnum mode, IEnumerable<short> types = null);

        /// <summary>
        /// Returns true if local device is subscribed to this state type, or if subscription type is set to ALL
        /// </summary>
        bool IsLocalDeviceSubscribedToType(short type);

        /// <summary>
        /// Returns true if device is subscribed to this state type, or if subscription type is set to ALL
        /// </summary>
        bool IsDeviceSubscribedToType(short deviceID, short type);

        /// <summary>
        /// Used to change subscriptions after mode has been set to manual.
        /// If subscription is not manual, an error will be logged and no action will be taken.
        /// </summary>
        void SetLocalSubscription(short dataType, bool subscribed);

        /// <summary>
        /// Sends a message to target deviceID which results in an OnDevicePinged event.
        /// </summary>
        void PingDevice(short deviceID);
    }

    public delegate void ConnectionEvent(ConnectEventArgs e);
    public delegate void DataEvent(DataEventArgs e);
    public delegate void SubscriptionEvent(SubscriptionEventArgs e);
    public delegate void DeviceEvent(DeviceEventArgs e);
    public delegate void PingEvent();
}