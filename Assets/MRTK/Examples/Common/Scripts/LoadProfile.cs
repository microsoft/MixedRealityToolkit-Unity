// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Loads a given Mixed Reality Toolkit configuration profile. 
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/LoadProfile")]
    public class LoadProfile : MonoBehaviour
    {
        [Tooltip("Mixed Reality Toolkit profile to load.")]
        [SerializeField]
        private MixedRealityToolkitConfigurationProfile configProfile = null;

        /// <summary>
        /// Loads a given Mixed Reality Toolkit configuration profile. 
        /// </summary>
        public void LoadConfigProfile()
        {
            if ((configProfile != null) && (MixedRealityToolkit.Instance != null))
            {
                MixedRealityToolkit.Instance.ActiveProfile = configProfile;
                Debug.Log($"Loading new MRTK configuration profile: {configProfile.name}");
                configProfile = null;
            }
        }
    }
}
