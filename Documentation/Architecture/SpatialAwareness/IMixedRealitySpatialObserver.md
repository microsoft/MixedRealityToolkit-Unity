#IMixedRealitySpatialObserver Interface

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem |

The IMixedRealitySpatialObserver is the interface that defines the requirements for platform specific spatial observers which provide data to the spatial awareness system.

<img src="Images/IMixedRealitySpatialObserver.png">

## IsRunning

| Type |
| --- |
| bool |

Gets a value that indicates whether or not the spatial observer is currently active observing the environment.

## void StartObserving()

Instructs the spatial observer to start actively observing the enviornment.

## void StopObserving()

Instructs the spatial observer to stop actively observing the enviornment.

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
