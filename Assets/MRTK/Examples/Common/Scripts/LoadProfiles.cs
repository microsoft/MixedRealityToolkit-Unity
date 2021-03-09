// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Automatically loads a given Mixed Reality Toolkit configuration profile when loading up the scene. 
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/LoadProfiles")]
    public class LoadProfiles : MonoBehaviour
    {
        [Tooltip("Mixed Reality Toolkit profile to load.")]
        [SerializeField]
        private MixedRealityToolkitConfigurationProfile configProfile = null;

        public void LoadProfile()
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
