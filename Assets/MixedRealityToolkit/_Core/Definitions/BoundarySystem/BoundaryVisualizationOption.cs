// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem
{
    [Serializable]
    public struct BoundaryVisualizationOption
    {
        [SerializeField]
        [Tooltip("The prefab to use when visualizing the boundary.")]
        private GameObject boundaryVisualizationPrefab;

        /// <summary>
        /// The prefab to usw when visualizing the boundary.
        /// </summary>
        public GameObject BoundaryVisualizationPrefab => boundaryVisualizationPrefab;

        [SerializeField]
        private string sceneName;

        public string SceneName => sceneName;

        [SerializeField]
        private int sceneId;

        public int SceneId => sceneId;
    }
}