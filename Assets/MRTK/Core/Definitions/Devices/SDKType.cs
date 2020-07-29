// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The SDKType lists the XR SDKs that are supported by the Mixed Reality Toolkit.
    /// Initially, this lists proposed SDKs, not all may be implemented at this time (please see ReleaseNotes for more details)
    /// </summary>
    public enum SDKType
    {
        /// <summary>
        /// No specified type or Standalone / non-XR type
        /// </summary>
        None = 0,
        /// <summary>
        /// Undefined SDK.
        /// </summary>
        Other,
        /// <summary>
        /// The Windows 10 Mixed reality SDK provided by the Universal Windows Platform (UWP), for Immersive MR headsets and HoloLens.
        /// </summary>
        WindowsMR,
        /// <summary>
        /// The OpenVR platform provided by Unity (does not support the downloadable SteamVR SDK).
        /// </summary>
        OpenVR,
        /// <summary>
        /// The OpenXR platform. SDK to be determined once released.
        /// </summary>
        OpenXR
    }
}