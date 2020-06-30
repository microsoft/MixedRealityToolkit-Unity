// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The supported platforms for Mixed Reality Toolkit Components and Features.
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
}