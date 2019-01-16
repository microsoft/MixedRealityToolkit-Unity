// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
