// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utility to check and configure UWP capability request from MRTK systems
    /// </summary>
    public class UWPCapabilityEditorUtils
    {
        /// <summary>
        /// Given capability is required by the given component. Check if capability is enabled, if not auto-enable if possible and log to console
        /// </summary>
        /// <param name="capability">Desired capability needed</param>
        /// <param name="dependentComponent">Component type that requires the associated capability to perform operations</param>
        public static void RequireCapability(PlayerSettings.WSACapability capability, Type dependentComponent)
        {
            if (!PlayerSettings.WSA.GetCapability(capability))
            {
                if (MixedRealityPreferences.AutoEnableUWPCapabilities)
                {
                    Debug.Log($"{dependentComponent.Name} requires the UWP {capability.ToString()} capability. Auto-enabling the capability in Player Settings.\nDisable this auto-checker via MRTK Preferences under Project Settings.");
                    PlayerSettings.WSA.SetCapability(capability, true);
                }
                else
                {
                    Debug.LogWarning($"{dependentComponent.Name} requires the UWP {capability.ToString()} capability which is currently not enabled. To utilize this component on device, enable the capability in Player Settings > Publishing Settings.");
                }
            }
        }
    }
}