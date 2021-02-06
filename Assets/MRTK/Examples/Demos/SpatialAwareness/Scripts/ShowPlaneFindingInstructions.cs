// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This class demonstrates clearing spatial observations.
    /// </summary>
    public class ShowPlaneFindingInstructions : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The panel to display when the plane finding package is imported.")]
        private GameObject planeFindingPanel = null;

        void Start()
        {
            planeFindingPanel.SetActive(SurfaceMeshesToPlanes.CanCreatePlanes);
        }
    }
}
