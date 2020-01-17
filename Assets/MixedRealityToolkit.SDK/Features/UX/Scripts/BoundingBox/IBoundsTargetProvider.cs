using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal interface IBoundsTargetProvider
{

    /// <summary>
    /// Indicates if the provider is currently active
    /// </summary>
    bool Active
    {
        get;
        set;
    }

    /// <summary>
    /// The object that this component is targeting
    /// </summary>
    GameObject Target
    {
        get;
        set;
    }

    /// <summary>
    /// The collider reference tracking the bounds utilized by this component during runtime
    /// </summary>
    BoxCollider TargetBounds
    {
        get;
    }
}
