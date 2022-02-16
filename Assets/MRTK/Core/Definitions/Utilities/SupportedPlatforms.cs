﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The supported platforms for Mixed Reality Toolkit components and features.
    /// </summary>
    [Flags]
    public enum SupportedPlatforms
    {
        WindowsStandalone = 1 << 0,
        MacStandalone = 1 << 1,
        LinuxStandalone = 1 << 2,
        WindowsUniversal = 1 << 3,
        WindowsEditor = 1 << 4,
        Android = 1 << 5,
        MacEditor = 1 << 6,
        LinuxEditor = 1 << 7,
        IOS = 1 << 8,
        Web = 1 << 9,
        Lumin = 1 << 10
    }

    /// <summary>
    /// The supported Unity XR pipelines for Mixed Reality Toolkit components and features.
    /// </summary>
    [Flags]
    public enum SupportedUnityXRPipelines
    {
#if UNITY_2020_1_OR_NEWER
        [Obsolete("The legacy XR pipeline has been removed in Unity 2020 or newer. Please migrate to XR SDK.")]
#endif // UNITY_2020_1_OR_NEWER
        LegacyXR = 1 << 0,
        XRSDK = 1 << 1,
    }

    /// <summary>
    /// Extension methods specific to the <see cref="SupportedUnityXRPipelines"/> enum.
    /// </summary>
    public static class SupportedUnityXRPipelinesExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="SupportedUnityXRPipelines"/> value.</param>
        /// <param name="b"><see cref="SupportedUnityXRPipelines"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this SupportedUnityXRPipelines a, SupportedUnityXRPipelines b)
        {
            return (a & b) == b;
        }
    }
}