# Sharing Service : Matchmaking

## Joining a Lobby

To join a lobby, using `ISharingService.JoinLobby()`. The service will connect and to a lobby using your service profile's `LobbyName`.

## Joining a Room

Once you've joined a lobby you can create, join or re-join a room.

### Joining a listed room

Once you've joined the lobby `ISharingService.AvailableRooms` will automatically be populated with `RoomInfo` structs, which describe the properties of the room. You can join a listed room using using `ISharingService.JoinRoom(ConnectConfig config)`.

```c#
private bool TryJoinRandomRoom()
{
    bool result = false;

    if (service.NumAvailableRooms > 0) 
    {
        List<RoomInfo> rooms = new List<RoomInfo>(service.AvailableRooms);
        RoomInfo randomRoom = rooms[Random.Range(0, rooms.Count)];
        // The ConnectConfig struct can take a room info as an argument.
        // This automatically sets ConnectConfig.JoinMode to RoomJoinMode.JoinRejoin since we know the room exists.
        // All other config settings are left as default so they will be populated by your service profile settings.
        result = await service.JoinRoom(new ConnectConfig(randomRoom));
    }

    return result;
}

```

### Joining an unlisted room

Rooms can be configured to be unlisted. However if the name of the room has been shared by other means you can still attempt to join it.

```c#
private async void TryJoinRoomByName(string roomName)
{
    // The ConnectConfig struct can take a room name as an argument.
    // This automatically sets ConnectConfig.JoinMode to RoomJoinMode.JoinRejoin since we know the room exists.
    // All other config settings are left as default so they will be populated by your service profile settings.
    bool result = await service.JoinRoom(new ConnectConfig(roomName));
}

```

### Creating a room

By default, calling `ISharingService.FastConnect()` will join or create a room based on your profile settings. Or you can manually configure room properties using `ISharingService.JoinRoom(ConnectConfig config)` and setting `ConnectConfig.RoomConfig` values.

### Using JoinMode

For more control you can set the `ConnectConfig.JoinMode` value. Some examples:

```c#
private async void CreateRoomIfItDoesntExist(RoomConfig roomConfig)
{
    // We want to create a room, not join a room. If a room with this name already exists, don't try to join it.
    bool result = await service.JoinRoom(new ConnectConfig()
    {
        RoomConfig = roomConfig,
        JoinMode = RoomJoinMode.CreateOnly,        
    });
}

private async void RejoinRoomOnlyIfPreviouslyDisconnected(string roomName)
{
    // We want to rejoin a room we were previously in, not join a new room or create a new room.
    bool result = await service.JoinRoom(new ConnectConfig()
    {
        RoomName = roomName,
        JoinMode = RoomJoinMode.JoinOnly,
    });
}

private async void JoinRoomAsANewDevice(string roomName)
{
    // We want to join a room as a new device, even if we were disconnected.
    // If we're recognized from a previous attempt, disconnect and re-join.
    bool result = await service.JoinRoom(new ConnectConfig()
    {
        RoomName = roomName,
        JoinMode = RoomJoinMode.JoinForceRejoin,
    });
}
```
See `RoomJoinMode` comments for more detail.

## Editor tools

If [service inspectors](../..\MixedRealityConfigurationGuide.md#service-inspectors) are enabled you can fast connect, join lobbies, view available rooms and join rooms from the service's inspector:

![In-Editor Connections](../../Images/SharingServices/PhotonServiceConnectInEditor.png)