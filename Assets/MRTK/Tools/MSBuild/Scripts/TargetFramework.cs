// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// Represents TargetFrameworks that Unity supports.
    /// </summary>
    public enum TargetFramework
    {
        NetStandard20,
        Net20,
        Net46
    }

    /// <summary>
    /// Helper extensions for the <see cref="TargetFramework"/> enum.
    /// </summary>
    public static class TargetFrameworkExtensions
    {
        /// <summary>
        /// Converts a <see cref="TargetFramework"/> into an MSBuild acceptable string.
        /// </summary>
        /// <param name="this">The <see cref="TargetFramework"/> to convert.</param>
        /// <returns>The MSBuild acceptable string representing the <see cref="TargetFramework"/>.</returns>
        public static string AsMSBuildString(this TargetFramework @this)
        {
            switch (@this)
            {
                case TargetFramework.NetStandard20:
                    return "netstandard2.0";
                case TargetFramework.Net20:
                    return "net20";
                case TargetFramework.Net46:
                    return "net46";
            }

            throw new ArgumentOutOfRangeException(nameof(@this));
        }

        /// <summary>
        /// Returns the configured <see cref="TargetFramework"/> for the <see href="https://docs.unity3d.com/ScriptReference/BuildTargetGroup.html">BuildTargetGroup</see>.
        /// </summary>
        /// <param name="this">The <see href="https://docs.unity3d.com/ScriptReference/BuildTargetGroup.html">BuildTargetGroup</see> to get <see cref="TargetFramework"/> for.</param>
        /// <returns>The <see cref="TargetFramework"/> configured for given <see href="https://docs.unity3d.com/ScriptReference/BuildTargetGroup.html">BuildTargetGroup</see>.</returns>
        public static TargetFramework GetTargetFramework(this BuildTargetGroup @this)
        {
            if (@this == BuildTargetGroup.Unknown)
            {
                // This may be different on older unity versions
                return TargetFramework.Net46;
            }

            switch (PlayerSettings.GetApiCompatibilityLevel(@this))
            {
                case ApiCompatibilityLevel.NET_2_0:
                case ApiCompatibilityLevel.NET_2_0_Subset:
                    return TargetFramework.Net20;
                case ApiCompatibilityLevel.NET_4_6:
                    return TargetFramework.Net46;
                case ApiCompatibilityLevel.NET_Web:
                case ApiCompatibilityLevel.NET_Micro:
                    throw new PlatformNotSupportedException("Don't currently support NET_Web and NET_Micro API compat");
                case ApiCompatibilityLevel.NET_Standard_2_0:
                    return TargetFramework.NetStandard20;
            }

            throw new PlatformNotSupportedException("ApiCompatibilityLevel platform not matched.");
        }
    }
}
