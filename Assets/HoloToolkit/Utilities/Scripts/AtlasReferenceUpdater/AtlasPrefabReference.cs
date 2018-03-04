// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace HoloToolkit.Unity
{
    public class AtlasPrefabReference : ScriptableObject
    {
        [SerializeField] public List<GameObject> Prefabs;
        [SerializeField] public SpriteAtlas Atlas;
    }
}
