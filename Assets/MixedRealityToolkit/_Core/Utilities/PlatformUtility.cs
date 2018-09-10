// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    public static class PlatformUtility
    {
        public static bool IsPlatformSupported(this RuntimePlatform runtimePlatform, SupportedPlatforms platform)
        {
            if (platform < 0)
            {
                return true;
            }

            if (platform == 0)
            {
                return false;
            }

            switch (runtimePlatform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return (platform & SupportedPlatforms.WindowsStandalone) != 0;
                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.XboxOne:
                    return (platform & SupportedPlatforms.WindowsUniversal) != 0;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return (platform & SupportedPlatforms.MacStandalone) != 0;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    return (platform & SupportedPlatforms.LinuxStandalone) != 0;
                default:
                    return false;
            }
        }

#if UNITY_EDITOR
        public static bool IsPlatformSupported(this UnityEditor.BuildTarget editorBuildTarget, SupportedPlatforms platform)
        {
            if (platform < 0)
            {
                return true;
            }

            if (platform == 0)
            {
                return false;
            }

            switch (editorBuildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return (platform & SupportedPlatforms.WindowsStandalone) != 0;
                case UnityEditor.BuildTarget.WSAPlayer:
                case UnityEditor.BuildTarget.XboxOne:
                    return (platform & SupportedPlatforms.WindowsUniversal) != 0;
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return (platform & SupportedPlatforms.MacStandalone) != 0;
                case UnityEditor.BuildTarget.StandaloneLinux:
                case UnityEditor.BuildTarget.StandaloneLinux64:
                case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
                    return (platform & SupportedPlatforms.LinuxStandalone) != 0;
                default:
                    return false;
            }
        }
#endif
    }
}