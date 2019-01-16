using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class BaseSpatialAwarenessObject
{
    public int Id { get; set; }
    public GameObject GameObject { get; set; }
    public MeshRenderer Renderer { get; set; }
    public MeshFilter Filter { get; set; }

    //public abstract Collider Collider { get; protected set; }
}
