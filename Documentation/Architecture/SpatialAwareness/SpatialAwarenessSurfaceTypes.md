# SpatialAwarenessSurfaceTypes Enumeration

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.Defintitions.SpatialAwareness |

The SpatialAwarenessSurfaceTypes enumeration is defined as a set of flags that enable applications to request only specific types of surfaces (ex: Floors and Walls) as well as providing the ability to create generalized requests (ex: all horizontal surfaces)

``` C#
    [System.Flags]
    public enum SpatialAwarenessSurfaceTypes
    {
        /// <summary>
        /// An unknown / unsupported type of surface.
        /// </summary>
        Unknown = 1 << 0,

        /// <summary>
        /// The environment’s floor.
        /// </summary>
        Floor = 1 << 1,

        /// <summary>
        /// The environment’s ceiling.
        /// </summary>
        Ceiling = 1 << 2,

        /// <summary>
        /// A wall within the user’s space.
        /// </summary>
        Wall = 1 << 3,

        /// <summary>
        /// A raised, horizontal surface such as a shelf.
        /// </summary>
        /// <remarks>
        /// Platforms, like floors, that can be used for placing objects
        /// requiring a horizontal surface.
        /// </remarks>
        Platform = 1 << 4
    }
```

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
