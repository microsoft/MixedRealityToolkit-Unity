// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class FollowMeToggle : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("An opional object for visualizing the carry mode state")]
        private GameObject visualizationObject = null;

        private Orbital orbital = null;

        private void Start()
        {
            // Get Orbital Solver component
            orbital = GetComponent<Orbital>();
        }

        public void ToggleFollowMeBehavior()
        {
            if (orbital != null)
            {
                // Toggle Orbital Solver component
                // You can tweak the detailed positioning behavior such as offset, lerping time, orientation type in the Inspector panel
                orbital.enabled = !orbital.enabled;

                if(visualizationObject != null)
                {
                    visualizationObject.SetActive(orbital.enabled);
                }
            }

        }
    }
}