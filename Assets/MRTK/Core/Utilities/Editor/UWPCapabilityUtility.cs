// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utility to check and configure UWP capability request from MRTK systems
    /// </summary>
    public class UWPCapabilityUtility
    {
        /// <summary>
        /// Given capability is required by the given component. Check if capability is enabled, if not auto-enable if possible and log to console
        /// </summary>
        /// <param name="capability">Desired capability needed</param>
        /// <param name="dependentComponent">Component type that requires the associated capability to perform operations</param>
        public static void RequireCapability(PlayerSettings.WSACapability capability, Type dependentComponent)
        {
            // Any changes made in editor while playing will not save
            if (!EditorApplication.isPlaying && !PlayerSettings.WSA.GetCapability(capability))
            {
                if (MixedRealityProjectPreferences.AutoEnableUWPCapabilities)
                {
                    Debug.Log($"<b>{dependentComponent.Name}</b> requires the UWP <b>{capability}</b> capability. Auto-enabling this capability in Player Settings.\nDisable this automation tool via MRTK Preferences under <i>Project Settings</i>.");
                    PlayerSettings.WSA.SetCapability(capability, true);
                }
                else
                {
                    Debug.LogWarning($"<b>{dependentComponent.Name}</b> requires the UWP <b>{capability}</b> capability which is currently not enabled. To utilize this component on device, enable the capability in <i>Player Settings</i> > <i>Publishing Settings</i>.");
                }
            }
        }
    }
}
