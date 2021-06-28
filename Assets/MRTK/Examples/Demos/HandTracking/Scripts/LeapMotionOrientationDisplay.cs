// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
#if (LEAPMOTIONCORE_PRESENT && UNITY_STANDALONE) || (LEAPMOTIONCORE_PRESENT && UNITY_WSA && UNITY_EDITOR)
using Microsoft.MixedReality.Toolkit.LeapMotion.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Returns the orientation of Leap Motion Controller
    /// </summary>
    public class LeapMotionOrientationDisplay : MonoBehaviour
    {
#if (LEAPMOTIONCORE_PRESENT && UNITY_STANDALONE) || (LEAPMOTIONCORE_PRESENT && UNITY_WSA && UNITY_EDITOR)
        [SerializeField]
        private TextMeshProUGUI orientationText;
        private LeapMotionDeviceManagerProfile managerProfile;

        private void Start()
        {
            if (GetLeapManager())
            {
                orientationText.text = "Orientation: " + GetLeapManager().LeapControllerOrientation.ToString();
            }
            else
            {
                orientationText.text = "Orientation: Unavailable";
            }
        }

        private LeapMotionDeviceManagerProfile GetLeapManager()
        {
            if (!MixedRealityToolkit.Instance.ActiveProfile)
            {
                return null;
            }
            foreach (MixedRealityInputDataProviderConfiguration config in MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations)
            {
                if (config.ComponentType == typeof(LeapMotionDeviceManager))
                {
                    managerProfile = (LeapMotionDeviceManagerProfile)config.Profile;
                    return managerProfile;
                }
            }
            return null;
        }
#endif
    }
}

