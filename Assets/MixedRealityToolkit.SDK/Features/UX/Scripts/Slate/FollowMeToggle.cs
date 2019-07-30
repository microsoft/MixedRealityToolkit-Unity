// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [RequireComponent(typeof(RadialView))]
    public class FollowMeToggle : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("An opional object for visualizing the 'Follow Me' mode state")]
        private GameObject visualizationObject = null;

        private RadialView radialView = null;

        private void Start()
        {
            // Get Radial Solver component
            radialView = GetComponent<RadialView>();
        }

        public void ToggleFollowMeBehavior()
        {
            if (radialView != null)
            {
                // Toggle Radial Solver component
                // You can tweak the detailed positioning behavior such as offset, lerping time, orientation type in the Inspector panel
                radialView.enabled = !radialView.enabled;

                if(visualizationObject != null)
                {
                    visualizationObject.SetActive(radialView.enabled);
                }
            }

        }
    }
}