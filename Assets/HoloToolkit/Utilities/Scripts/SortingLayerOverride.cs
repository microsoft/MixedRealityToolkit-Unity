// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Overrides the sorting layer of all renderers and child renderers
    /// </summary>
    public class SortingLayerOverride : MonoBehaviour
    {
        // Grabs the last layer in the project
        public bool UseLastLayer = false;

        // Only is used if UseLastLayer is false
        public string TargetSortingLayerName = "Default";

        [SerializeField]
        private Renderer[] renderers;

        private void Start()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }

            if (UseLastLayer && SortingLayer.layers.Length > 0)
            {
                var lastSortingLayer = SortingLayer.layers[SortingLayer.layers.Length - 1];
                TargetSortingLayerName = lastSortingLayer.name;
            }

            // Set sorting layer name in each child renderer
            foreach (var componentRenderer in renderers)
            {
                componentRenderer.sortingLayerName = TargetSortingLayerName;
            }
        }
    }
}
