// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
#endif // UNITY_2019_3_OR_NEWER

#if XR_MANAGEMENT_ENABLED
using System.Threading;
using UnityEngine.XR.Management;
#endif //XR_MANAGEMENT_ENABLED

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utilities that abstract XR settings functionality so that the MRTK need not know which
    /// implementation is being used.
    /// </summary>
    public static class XRSettingsUtilities
    {
        /// <summary>
        /// Gets or sets the legacy virtual reality supported property in the player settings object.
        /// </summary>
        public static bool LegacyXREnabled
        {
            get
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                return PlayerSettings.virtualRealitySupported;
#pragma warning restore 0618
            }

            set
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                PlayerSettings.virtualRealitySupported = value;
#pragma warning restore 0618
            }
        }

    }
}
