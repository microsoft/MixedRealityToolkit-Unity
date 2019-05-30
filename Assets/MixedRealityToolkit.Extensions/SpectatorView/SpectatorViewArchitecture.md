# Overview

[SpectatorView](Scripts/SpectatorView.cs) is a multi-device experience that allows a user's HoloLens application to be viewed by additional devices at their own viewpoints. It offers functionality for unidirectional content synchronization (State Synchronization) and leverages spatial coordinates for scene alignment (Spatial Alignment). It can be used to enable a variety of filming experiences including documenting prototypes and keynote demos.

## Application Flow

#### Pre-compilation
1) All of the assets in the unity project are assigned unique identifiers. This allows content in the user's application scene to be recreated/updated/destroyed dynamically in the spectator's application scene. This is done through calling [Spectator View -> Update All Asset Caches](Scripts/Editor/StateSynchronizationMenuItems.cs) in the Unity toolbar.

2) The main user's ip address as well as a network port are hardcoded in the application. This ip address allows spectator devices to connect to the user device. Hardcoding ip addresses and port numbers has limitations (The same compiled application cannot currently be used for different user devices). Long term, this matchmaking process will be replaced with a more robust solution.

#### In application
1) First, the user's device starts listening for network connections on the specified network port. Spectator devices then connect to the user's device using the user ip address and the same network port. This is facilitated through the [TCPConnectionManager](../Socketer/Scripts/TCPConnectionManager.cs).

2) With each connection, the user application sets up state synchronization and spatial alignment for the spectator application. Both state synchronization and spatial alignment use the same network connection, but they aren't directly related to one another and run in parallel.

#### State synchronization
1) On the user device, a [StateSynchronizationBroadcaster](Scripts/StateSynchronization/StateSynchronizationBroadcaster.cs) is enabled, while on the spectator device a 
[StateSynchronizationObserver](Scripts/StateSynchronization/StateSynchronizationObserver.cs) is enabled.
    * These classes are responsible for delegating both network messages and network changes to the [StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs), which drives scene state synchronization.
    * These classes are used to relay camera location, application time, and performance data from the user to spectator device.
    * These classes allow other components to register for custom network events and send network messages through the [CommandService](Scripts/StateSynchronization/CommandService.cs) (Note: this allows Spatial Alignment components to use the same network connection).

2) In both the user and spectator application, ComponentBroadcasterServices register with the StateSynchronizationSceneManager.
      * ComponentBroadcasterServices specify ComponentBroadcaster types for in scene class types. This allows broadcasters to be created as new components are added to the user application scene.
      * ComponentBroadcasterServices also register for a specific id so that they can receive network messages and create ComponentObservers in the spectator scene.

3) When the StateSynchronizationBroadcaster observes that a StateSynchronizationObserver has connected, the user's scene is configured to be broadcasted. Configuring the user scene for broadcasting requires adding TransformBroadcasters to root game objects of content that is intended to be synchronized. This can be achieved through different manners:
      * GameObjectHierarchyBroadcaster items in the Unity scene will add a TransformBroadcaster to their associated game object.
      * If BroadcasterSettings.AutomaticallyBroadcastAllGameObjects is set to true, a TransformBroadcaster will be added to the root game object of every scene (This is DISABLED by default in SpectatorView).
      
4) On awake and for every related hierarchy change, the TransformBroadcaster will ensure that all of its children also have TransformBroadcasters. On creation, TransformBroadcasters also make sure that their associated game objects have ComponentBroadcasters created for all components with registered ComponentBroadcasterServices. This effectively sets up the classes needed for components in the user application to broadcast state information to spectator devices.

5) After each frame on the user device, the StateSynchronizationSceneManager will observer network connection changes. It also observes if any ComponentBroadcasters have been destroyed. It then hands all of the known network connections to each ComponentBroadcaster so that state information can be sent to the spectator devices.

#### Spatial alignment

# Networking

[TCPConnectionManager](../Socketer/Scripts/TCPConnectionManager.cs)

[SocketEndpoint](../Socketer/Scripts/SocketEndpoint.cs)

# State Synchronization

[StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs)

[StateSynchronizationBroadcaster](Scripts/StateSynchronization/StateSynchronizationBroadcaster.cs)

[StateSynchronizationObserver](Scripts/StateSynchronization/StateSynchronizationObserver.cs)

[CommandService](Scripts/StateSynchronization/CommandService.cs)

[GameObjectHierarchyBroadcaster](Scripts/StateSynchronization/GameObjectHierarchyBroadcaster.cs)

[ComponentBroadcasterService](Scripts/StateSynchronization/ComponentBroadcasterService.cs)

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
