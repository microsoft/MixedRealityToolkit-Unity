// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

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
