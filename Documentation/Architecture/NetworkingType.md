# SpatialAwarenessEventType Enumeration

| Toolkit Layer | Namespace |
| --- | --- |
| Core | Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwareness |

``` C#
public enum SpatialAwarenessEventType
{
    /// <summary>
    /// A spatial awareness subsystem is reporting that a new spatial element 
    /// has been identified.
    /// </summary>
    Added = 0,

    /// <summary>
    /// A spatial awareness subsystem is reporting that an existing spatial
    /// element has been modified.
    /// </summary>
    Updated,

    /// <summary>
    /// A spatial awareness subsystem is reporting that an existing spatial
    /// element has been discarded.
    /// </summary>
    Deleted
}
```
