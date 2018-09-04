# MixedRealitySpatialAwarenessSurfaceTypes Enumeration

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.Defintitions.SpatialAwareness |

``` C#
public enum MixedRealitySpatialAwarenessSurfaceTypes
{
    /// <summary>
    /// An unknown / unsupported type of surface.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The environment’s floor.
    /// </summary>
    Floor,

    /// <summary>
    /// The environment’s ceiling.
    /// </summary>
    Ceiling,

    /// <summary>
    /// A wall within the user’s space.
    /// </summary>
    Wall,

    /// <summary>
    /// A raised, horizontal surface such as a shelf.
    /// </summary>
    /// <remarks>
    /// Platforms, like floors, can be used for placing objects 
    /// requiring a horizontal surface.
    /// </remarks>
    Platform
}
```

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
