# Overview

[SpectatorView](Scripts/SpectatorView.cs) is a multi-device experience that allows a user's HoloLens application to be viewed by additional devices at their own viewpoints. It offers functionality for unidirectional content synchronization (State Synchronization) and leverages spatial coordinates for scene alignment (Spatial Alignment). It can be used to enable a variety of filming experiences including documenting prototypes and keynote demos.

## Flow of Logic

#### Pre-compilation
1) All of the assets in the unity project are assigned unique identifiers. This allows content in the user's application scene to be recreated/updated/destroyed dynamically in the spectator's application scene.

2) The main user's ip address as well as a network port are hardcoded in the application. This ip address allows spectator devices to connect to the user device. Hardcoding ip addresses and port numbers has limitations (The same compiled application cannot currently be used for different user devices). Long term, this matchmaking process will be replaced with a more robust solution.

#### In application
1) First, the user's device starts listening for network connections on the specified network port. Spectator devices then connect to the user's device using the user ip address and the same network port. This is facilitated through the [TCPConnectionManager](../Socketer/Scripts/TCPConnectionManager.cs).

2) With each connection, the user application sets up state synchronization and spatial alignment for the spectator application.

#### State synchronization
1) On the user device, a [StateSynchronizationBroadcaster](Scripts/StateSynchronization/StateSynchronizationBroadcaster.cs) is enabled, while on the spectator device a 
[StateSynchronizationObserver](Scripts/StateSynchronization/StateSynchronizationObserver.cs) is enabled.
    * These classes are responsible for delegating both network messages and network changes to the [StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs).
    * These classes are used to relay camera location, application time, and performance data from the user to spectator device.
    * These classes allow other components to register for custom network events and send network messages through the [CommandService](Scripts/StateSynchronization/CommandService.cs).

2) When the user device registers a spectator connection, the [StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs) caches said connection.

3) Every end of frame, the [StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs) assesses the state of network connections. 



# Networking

[TCPConnectionManager](../Socketer/Scripts/TCPConnectionManager.cs)

[SocketEndpoint](../Socketer/Scripts/SocketEndpoint.cs)

# State Synchronization

[StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs)

[StateSynchronizationBroadcaster](Scripts/StateSynchronization/StateSynchronizationBroadcaster.cs)

[StateSynchronizationObserver](Scripts/StateSynchronization/StateSynchronizationObserver.cs)

[CommandService](Scripts/StateSynchronization/CommandService.cs)

[GameObjectHierarchyBroadcaster](Scripts/StateSynchronization/GameObjectHierarchyBroadcaster.cs)

[ComponentBroadcaster](Scripts/StateSynchronization/ComponentBroadcaster.cs)

[ComponentObserver](Scripts/StateSynchronization/ComponentObserver.cs)

[AssetCache](Scripts/StateSynchronization/AssetCache.cs)

# Spatial Alignment

[ISpatialCoordinate](../Sharing/SpatialAlignment/Common/ISpatialCoordinate.cs)

* SpatialAnchorCoordinate

[ISpatialCoordinateService](../Sharing/SpatialAlignment/Common/ISpatialCoordinateService.cs)

* SpatialAnchorCoordinateService

[SpatialCoordinateSystemManager](Scripts/Sharing/SpatialCoordinateSystemManager.cs)

[SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs)

[SpatialLocalizer](Scripts/Sharing/SpatialLocalizer.cs)

* SpatialAnchorLocalizer
