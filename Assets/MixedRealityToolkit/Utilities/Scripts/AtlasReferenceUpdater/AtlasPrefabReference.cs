// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace MixedRealityToolkit.Utilities
{
    public class AtlasPrefabReference : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> prefabs;

        [SerializeField]
        private SpriteAtlas atlas;

        public List<GameObject> Prefabs
        {
            get { return prefabs; }
            set { prefabs = value; }
        }

        public SpriteAtlas Atlas
        {
            get { return atlas; }
            set { atlas = value; }
        }
    }
}
