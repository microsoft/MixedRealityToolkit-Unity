// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public static class PlatformUtility
    {
        public static bool IsPlatformSupported(this RuntimePlatform runtimePlatform, SupportedPlatforms platforms)
        {
            SupportedPlatforms target = GetSupportedPlatformMask(runtimePlatform);
            return IsPlatformSupported(target, platforms);
        }

        private static SupportedPlatforms GetSupportedPlatformMask(RuntimePlatform runtimePlatform)
        {
            SupportedPlatforms supportedPlatforms = 0;

            switch (runtimePlatform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    supportedPlatforms |= SupportedPlatforms.WindowsStandalone;
                    break;
                case RuntimePlatform.WSAPlayerARM:
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.XboxOne:
                    supportedPlatforms |= SupportedPlatforms.WindowsUniversal;
                    break;
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    supportedPlatforms |= SupportedPlatforms.MacStandalone;
                    break;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    supportedPlatforms |= SupportedPlatforms.LinuxStandalone;
                    break;
                case RuntimePlatform.Android:
                    supportedPlatforms |= SupportedPlatforms.Android;
                    break;
            }

            return supportedPlatforms;
        }

        private static bool IsPlatformSupported(SupportedPlatforms target, SupportedPlatforms supported)
        {
            return (target & supported) > 0;
        }

#if UNITY_EDITOR
        public static bool IsPlatformSupported(this UnityEditor.BuildTarget editorBuildTarget, SupportedPlatforms platforms)
        {
            SupportedPlatforms target = GetSupportedPlatformMask(editorBuildTarget);
            return IsPlatformSupported(target, platforms);
        }

        private static SupportedPlatforms GetSupportedPlatformMask(UnityEditor.BuildTarget editorBuildTarget)
        {
            SupportedPlatforms supportedPlatforms = 0;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                supportedPlatforms |= SupportedPlatforms.WindowsEditor;
            }

            switch (editorBuildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    supportedPlatforms |= SupportedPlatforms.WindowsStandalone;
                    break;
                case UnityEditor.BuildTarget.WSAPlayer:
                case UnityEditor.BuildTarget.XboxOne:
                    supportedPlatforms |= SupportedPlatforms.WindowsUniversal;
                    break;
                case UnityEditor.BuildTarget.StandaloneOSX:
                    supportedPlatforms |= SupportedPlatforms.MacStandalone;
                    break;
                case UnityEditor.BuildTarget.StandaloneLinux:
                case UnityEditor.BuildTarget.StandaloneLinux64:
                case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
                    supportedPlatforms |= SupportedPlatforms.LinuxStandalone;
                    break;
                case UnityEditor.BuildTarget.Android:
                    supportedPlatforms |= SupportedPlatforms.Android;
                    break;
            }

            return supportedPlatforms;
        }
#endif
    }
}