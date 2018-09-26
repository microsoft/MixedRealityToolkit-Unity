# MixedRealitySpatialAwarenessBaseEventData Class

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwareness |

The MixedRealitySpatialAwarenessBaseEventData provides the data shared by all of the spatial awareness event types.

<img src="Images/MixedRealitySpatialAwarenessBaseEventData.png">

## EventTime

| Type |
| --- |
| DateTime |

The time at which the event occurred.

## EventType

| Type |
| --- |
| [SpatialAwarenessEventType](./SpatialAwarenessEventType.md) |

The type of event that has occurred.

## Id

| Type |
| --- |
| UInt32 |

An identifier assigned to a specific object in the spatial awareness system.

## GameObject

| Type |
| --- |
| GameObject |

Unity GameObject, managed by the spatial awareness system, representing the data in this event.

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
- [SpatialAwarenessEventType Enumeration](./SpatialAwarenessEventType.md)
