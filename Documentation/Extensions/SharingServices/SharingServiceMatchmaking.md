# Sharing Service : Matchmaking

## Fast Connect

The simplest way to connect to other devices is with `ISharingService.FastConnect()`. This will join the lobby and attempt to join or create a room based on your service profile settings. We recommend using this while prototyping your app.

If `ConnectOnStartup` is set to **true** in your service profile the service will automatcally call this on startup.

## Lobbies and Rooms

You can find and connect with other devices via lobbies and rooms. These are synonymous with the lobby / room structure used by Photon, UNet and the like.

## Joining a lobby

To join a lobby, using `ISharingService.JoinLobby()`. The service will connect and join the `LobbyName` from your service profile.

## Selecting a room

Once you've joined a lobby `ISharingService.AvailableRooms` will automatically be populated with `RoomInfo` structs.

## Joining a room

You can join a selected room with `ISharingService.JoinRoom(RoomConfig config)`.

## Editor tools

If [service inspectors](../..\MixedRealityConfigurationGuide.md#service-inspectors) are enabled you can join lobbies, view available rooms and join rooms from the service's inspector:

![In-Editor Matchmaking Test](../../Images/SharingSystem/InEditorMatchmakingTest.gif)