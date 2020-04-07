// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// Info for joined room. Will be empty until a room has been joined.
        /// </summary>
        RoomInfo CurrentRoom { get; }

        /// <summary>
        /// True if our local device has connected to the service.
        /// </summary>
        bool LocalDeviceConnected { get; }

        /// <summary>
        /// The info for our local device. Set once connected to room.
        /// </summary>
        DeviceInfo LocalDevice { get; }

        /// <summary>
        /// Info for all devices present in the room, including local device.
        /// Some devices may not be connected - check ConnectStatus.
        /// </summary>
        IEnumerable<DeviceInfo> AvailableDevices { get; }

        /// <summary>
        /// Number of devices present in the room, including local device.
        /// </summary>
        int NumAvailableDevices { get; }

        /// <summary>
        /// Names of all available rooms. Populated once service has connected to a lobby and have called FindRooms once.
        /// </summary>
        IEnumerable<RoomInfo> AvailableRooms { get; }

        /// <summary>
        /// Number of available rooms in lobby.
        /// </summary>
        int NumAvailableRooms { get; }

        /// <summary>
        /// Number of pings received. Reset on exit room.
        /// </summary>
        int NumTimesPinged { get; }

        /// <summary>
        /// Real-time since startup of the last ping received.
        /// </summary>
        float TimeLastPinged { get; }

        /// <summary>
        /// Invoked when the service's connection status has changed.
        /// This happens when the service:
        /// - connects/disconnects to/from server
        /// - connects/disconnects to/from lobby
        /// - connects/disconnects to/from room
        /// </summary>
        event StatusEvent OnStatusChange;

        /// <summary>
        /// Invoked when the sharing service's AppRole changes.
        /// This is guaranteed to be invoked once when the device connects to a room.
        /// </summary>
        event StatusEvent OnAppRoleSelected;

        /// <summary>
        /// Invoked when subscription mode changes.
        /// This is guaranteed to be invoked once when the device connects to a room.
        /// </summary>
        event SubscriptionEvent OnLocalSubscriptionModeChange;

        /// <summary>
        /// Invoked when data is received from the service.
        /// </summary>
        event DataEvent OnReceiveData;

        /// <summary>
        /// Invoked when a device has connected to our room, including our own.
        /// </summary>
        event DeviceEvent OnDeviceConnected;

        /// <summary>
        /// Invoked when a device has disconnected from our room, including our own.
        /// </summary>
        event DeviceEvent OnDeviceDisconnected;

        /// <summary>
        /// Invoked when the properties of a device changes, including our own.
        /// </summary>
        event DeviceEvent OnDeviceUpdated;

        /// <summary>
        /// Invoked when another device in our room calls PingDevice for this device.
        /// </summary>
        event PingEvent OnLocalDevicePinged;

        /// <summary>
        /// Quick way to connect directly to a room and circumvent matchmaking.
        /// The serivice will attempt to connect to the lobby and room specified in the config file.
        /// </summary>
        Task<bool> FastConnect();

        /// <summary>
        /// Quick way to connect directly to a room and circumvent matchmaking.
        /// The service will attempt to connect to the lobby and room specified in the config argument.
        /// </summary>
        Task<bool> FastConnect(ConnectConfig config);

        /// <summary>
        /// Connects to server and joins the lobby specified in the service profile.
        /// </summary>
        /// <returns></returns>
        Task<bool> JoinLobby();
        
        /// <summary>
        /// Joins a room based on supplied configuration.
        /// </summary>
        Task<bool> JoinRoom(ConnectConfig config);

        /// <summary>
        /// Disconnects from the service.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends data to any devices connected to the same room.
        /// </summary>
        void SendData(SendDataArgs args);

        /// <summary>
        /// Sets the subscription mode as well as manual subscription types.
        /// If subscription mode is ALL then subscription types are ignored.
        /// </summary>
        void SetLocalSubscriptionMode(SubscriptionMode mode, IEnumerable<short> types = null);

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

    public delegate void StatusEvent(StatusEventArgs e);
    public delegate void DataEvent(DataEventArgs e);
    public delegate void SubscriptionEvent(SubscriptionEventArgs e);
    public delegate void DeviceEvent(DeviceInfo e);
    public delegate void PingEvent();
}