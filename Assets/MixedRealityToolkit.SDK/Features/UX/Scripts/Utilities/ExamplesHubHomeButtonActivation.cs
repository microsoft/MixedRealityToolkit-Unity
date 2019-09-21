// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Shows the home button in the ToggleFeaturesPanel if the Scene System is enabled
    /// </summary>
    public class ExamplesHubHomeButtonActivation : MonoBehaviour
    {
        /// <summary>
        /// Home button which brings the user back to examples hub
        /// </summary>
        [SerializeField]
        [Tooltip("Home button which brings the user back to examples hub")]
        private GameObject buttonHubHome = null;

        private void OnEnable()
        {
            if (MixedRealityToolkit.IsSceneSystemEnabled == true)
            {
                if (buttonHubHome != null)
                {
                    buttonHubHome.SetActive(true);
                }
            }
        }
    }
}
