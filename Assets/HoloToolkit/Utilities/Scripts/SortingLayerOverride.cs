// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity
{
    // Overrides the sorting layer of all renderers and child renderers
    public class SortingLayerOverride : MonoBehaviour
    {
        // Grabs the last layer in the project
        public bool UseLastLayer = false;

        // Only is used if UseLastLayer is false
        public string TargetSortingLayerName = "Default";

        private Renderer[] renderers;


        void Start()
        {
            renderers = GetComponentsInChildren<Renderer>();

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
