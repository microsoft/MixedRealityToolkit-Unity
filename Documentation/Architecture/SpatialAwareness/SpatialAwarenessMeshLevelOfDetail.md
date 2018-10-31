# SpatialAwarenessMeshLevelOfDetail Enumeration

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.Defintitions.SpatialAwareness |

``` C#
public enum SpatialAwarenessMeshLevelOfDetail
{
    /// <summary>
    /// The custom level of detail allows specifying a custom value for
    /// MeshTrianglesPerCubicMeter.
    /// </summary>
    Custom = -1,

    /// <summary>
    /// The coarse level of detail is well suited for identifying large
    /// environmental features, such as floors and walls.
    /// </summary>
    Coarse = 0,

    /// <summary>
    /// The fine level of detail is well suited for using as an occlusion
    /// mesh.
    /// </summary>
    Fine = 2000
}
```

## See Also

- [Mixed Reality Spatial Awareness System Architecture](./SpatialAwarenessSystemArchitecture.md)
