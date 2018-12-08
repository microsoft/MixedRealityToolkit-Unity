# Mixed Reality Toolkit Networking System Architecture

The Mixed Reality Networking System is intended to abstract the specific implementation details of platforms, such as the Photon and WebRTC, that provide support for synchronizing applications and experiences and providing the data to Mixed Reality experiences.

Each interface defined will implement one or more Properties, Methods and/or Events (PMEs) that can be accessed by application code.

## Interfaces

- [`INetworkHandler<T>`](./INetworkHandler.md)
- [`IMixedRealityNetworkingSystem`](./IMixedRealityNetworkingSystem.md)

## Classes
- [`MixedRealityNetworkService`](./MixedRealityNetworkService.md)

## Event Data Types

- [`BaseNetworkingEventData<T>`](./BaseNetworkingEventData.md)


## Example Implementation

The Mixed Reality Toolkit's default implementation of the networking system can be found in the Assets/MixedRealityToolkit/Core/Services/NetworkingSystem folder. This implementation can be used as an example of how to build your own networking system. It is well documented and demonstrates each of the features described in this architecture specification.
