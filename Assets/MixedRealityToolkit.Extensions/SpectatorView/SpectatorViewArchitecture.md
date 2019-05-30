# Overview

[SpectatorView](Scripts/SpectatorView.cs) is a multi-device experience that allows a user's HoloLens application to be viewed by additional devices at their own viewpoints. It offers functionality for unidirectional content synchronization (State Synchronization) and leverages spatial coordinates for scene alignment (Spatial Alignment). It can be used to enable a variety of filming experiences including documenting prototypes and keynote demos.

## Application Flow

### Pre-compilation
1) All of the assets in the unity project are assigned unique identifiers. This allows content in the user's application scene to be recreated/updated/destroyed dynamically in the spectator's application scene. This is done through calling [Spectator View -> Update All Asset Caches](Scripts/Editor/StateSynchronizationMenuItems.cs) in the Unity toolbar.

2) The main user's ip address as well as a network port are hardcoded in the application. This ip address allows spectator devices to connect to the user device. Hardcoding ip addresses and port numbers has limitations (The same compiled application cannot currently be used for different user devices). Long term, this matchmaking process will be replaced with a more robust solution.

### In application
1) First, the user's device starts listening for network connections on the specified network port. Spectator devices then connect to the user's device using the user ip address and the same network port. This is facilitated through the [TCPConnectionManager](../Socketer/Scripts/TCPConnectionManager.cs).

2) With each connection, the user application sets up state synchronization and spatial alignment for the spectator application. Both state synchronization and spatial alignment use the same network connection, but they aren't directly related to one another and run in parallel.

### State synchronization
1) On the user device, a [StateSynchronizationBroadcaster](Scripts/StateSynchronization/StateSynchronizationBroadcaster.cs) is enabled, while on the spectator device a 
[StateSynchronizationObserver](Scripts/StateSynchronization/StateSynchronizationObserver.cs) is enabled.
    * These classes are responsible for delegating both network messages and network changes to the [StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs), which drives scene state synchronization.
    * These classes are used to relay camera location, application time, and performance data from the user to spectator device.
    * These classes allow other components to register for custom network events and send network messages through the [CommandService](Scripts/StateSynchronization/CommandService.cs) (Note: this allows Spatial Alignment components to use the same network connection).


2) In both the user and spectator application, [ComponentBroadcasterServices](Scripts/StateSynchronization/ComponentBroadcasterService.cs) register with the [StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs).
      * [ComponentBroadcasterServices](Scripts/StateSynchronization/ComponentBroadcasterService.cs) specify [ComponentBroadcaster](Scripts/StateSynchronization/ComponentBroadcaster.cs) types for in scene class types. This allows broadcasters to be created as new components are added to the user application scene.
      * [ComponentBroadcasterServices](Scripts/StateSynchronization/ComponentBroadcasterService.cs) also register for a specific id so that they can receive network messages and create ComponentObservers in the spectator scene.


3) When the [StateSynchronizationBroadcaster](Scripts/StateSynchronization/StateSynchronizationBroadcaster.cs) observes that a [StateSynchronizationObserver](Scripts/StateSynchronization/StateSynchronizationObserver.cs)
 has connected, the user's scene is configured to be broadcasted. Configuring the user scene for broadcasting requires adding TransformBroadcasters to root game objects of content that is intended to be synchronized. This can be achieved through different manners:
      * [GameObjectHierarchyBroadcaster](Scripts/StateSynchronization/GameObjectHierarchyBroadcaster.cs) items in the Unity scene will add a [TransformBroadcaster](Scripts/StateSynchronization/TransformBroadcaster.cs) to their associated game object.
      * If [BroadcasterSettings.AutomaticallyBroadcastAllGameObjects](Scripts/StateSynchronization/BroadcasterSettings.cs) is set to true, a [TransformBroadcaster](Scripts/StateSynchronization/TransformBroadcaster.cs) will be added to the root game object of every scene (This is DISABLED by default in SpectatorView).


4) On awake and for every related hierarchy change, the [TransformBroadcaster](Scripts/StateSynchronization/TransformBroadcaster.cs)
 will ensure that all of its children also have [TransformBroadcasters](Scripts/StateSynchronization/TransformBroadcaster.cs). On creation, [TransformBroadcasters](Scripts/StateSynchronization/TransformBroadcaster.cs) also make sure that their associated game objects have [ComponentBroadcasters](Scripts/StateSynchronization/ComponentBroadcaster.cs) created for all components with registered [ComponentBroadcasterServices](Scripts/StateSynchronization/ComponentBroadcasterService.cs). This effectively sets up the classes needed for components in the user application to broadcast state information to spectator devices.


5) After each frame on the user device, the [StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs) will monitor network connection changes. It also determine if any [ComponentBroadcasters](Scripts/StateSynchronization/ComponentBroadcaster.cs)
 have been destroyed. It then hands all of the known network connections to each [ComponentBroadcaster](Scripts/StateSynchronization/ComponentBroadcaster.cs)
 so that state information can be sent to the spectator devices.


6) On the spectator device, the [StateSynchronizationSceneManager](Scripts/StateSynchronization/StateSynchronizationSceneManager.cs) will receive network messages to relay to the appropriate [ComponentBroadcasterServices](Scripts/StateSynchronization/ComponentBroadcasterService.cs). These messages signal component creation, updates and destruction on the users device. This component state information also contains unique component ids that allow specific instances of [ComponentBroadcasters](Scripts/StateSynchronization/ComponentBroadcaster.cs)
 on the user device to map 1:1 with specific instances of [ComponentObservers](Scripts/StateSynchronization/ComponentObserver.cs) on the spectator device. Through this state information, the spectator device's scene is updated to reflect content on the user's device.

### Spatial alignment
1) On both the user and spectator devices, a [SpatialCoordinateSystemManager](Scripts/Sharing/SpatialCoordinateSystemManager.cs) exists and registers for network information and messages through the state synchronization [CommandService](Scripts/StateSynchronization/CommandService.cs).

2) When the [SpatialCoordinateSystemManager](Scripts/Sharing/SpatialCoordinateSystemManager.cs)
 observes a network connection, it creates a [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs). On the user device, a [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) will be created for each spectator. On the spectator device, a [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) will only be created for the user.

3) When creating the [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs), the [SpatialCoordinateSystemManager](Scripts/Sharing/SpatialCoordinateSystemManager.cs)
 tells the [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) whether or not it is hosting the spatial coordinate system (Typically the user hosts the coordinate system, but this may not always be the case). The 
[SpatialCoordinateSystemManager](Scripts/Sharing/SpatialCoordinateSystemManager.cs) then instructs the [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) to localize itself relative to the shared experience.

4) When localizing, the [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) is handed the application's [SpatialLocalizer](Scripts/Sharing/SpatialLocalizer.cs). This abstraction allows using different [SpatialLocalizers](Scripts/Sharing/SpatialLocalizer.cs) for different spatial alignment experiences (ex. Azure spatial anchors, marker detection, etc). The [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) then instructs the [SpatialLocalizer](Scripts/Sharing/SpatialLocalizer.cs) to obtain an [ISpatialCoordinate](../Sharing/SpatialAlignment/Common/ISpatialCoordinate.cs) from its [ISpatialCoordinateService](../Sharing/SpatialAlignment/Common/ISpatialCoordinateService.cs).

5) If the [SpatialLocalizer](Scripts/Sharing/SpatialLocalizer.cs) is running as a host, it creates/obtains an [ISpatialCoordinate](../Sharing/SpatialAlignment/Common/ISpatialCoordinate.cs) id. It then locates said [ISpatialCoordinate](../Sharing/SpatialAlignment/Common/ISpatialCoordinate.cs) in its own local application space.

6) After the host device locates its [ISpatialCoordinate](../Sharing/SpatialAlignment/Common/ISpatialCoordinate.cs), the [SpatialLocalizer](Scripts/Sharing/SpatialLocalizer.cs) sends the [ISpatialCoordinate](../Sharing/SpatialAlignment/Common/ISpatialCoordinate.cs) id to the [SpatialLocalizer](Scripts/Sharing/SpatialLocalizer.cs) on the non-host device. The non-host device then attempts to find the same [ISpatialCoordinate](../Sharing/SpatialAlignment/Common/ISpatialCoordinate.cs).

7) After sending the [ISpatialCoordinate](../Sharing/SpatialAlignment/Common/ISpatialCoordinate.cs) id, the SpatialLocalizer declares to the [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) that the spatial coordinate was located. The [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) then calculates the local application origin relative to this spatial coordinate. It then sends this local application origin in spatial coordinate space to its peer [SpatialCoordinateSystemParticipant](Scripts/Sharing/SpatialCoordinateSystemParticipant.cs) on the other device (Note: both the host and non-host devices tell each other where these application origins are in the spatial coordinate system).

8) Once application origins have been located for both devices in the spatial coordinate space, transforms are obtained to move content on both the host and non-host devices to the shared coordinate system (Currently, the host device's application origin is used as the real world application origin, so only content on the non-host device is updated). The [SpatialCoordinateSystemManager](Scripts/Sharing/SpatialCoordinateSystemManager.cs)
 then applies this transform to a parent game object of the main unity camera camera, which results in both devices viewing content with the same origin in world space.
