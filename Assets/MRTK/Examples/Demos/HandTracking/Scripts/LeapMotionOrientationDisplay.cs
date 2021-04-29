// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.LeapMotion.Input;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Returns the orientation of Leap Motion Controller
    /// </summary>
    public class LeapMotionOrientationDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI orientationText;
        LeapMotionDeviceManagerProfile managerProfile;

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

    }
}

