// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.U2D;
#endif // UNITY_2017_1_OR_NEWER

namespace HoloToolkit.Unity
{
    public class AtlasPrefabReference : ScriptableObject
    {
#if UNITY_2017_1_OR_NEWER
        [SerializeField] public List<GameObject> Prefabs;
        [SerializeField] public SpriteAtlas Atlas;
#endif // UNITY_2017_1_OR_NEWER
    }
}
