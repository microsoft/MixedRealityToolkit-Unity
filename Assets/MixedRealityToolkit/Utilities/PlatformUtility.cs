// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utility class with helper methods to determine current runtime platform and environment modes
    /// </summary>
    public static class PlatformUtility
    {
        /// <summary>
        /// Determine if the current runtime build target is matched in the SupportedPlatforms flag list. 
        /// </summary>
        /// <param name="platforms">SupportedPlatforms flag mask to check against</param>
        /// <returns>true if current runtime is contained in flag mask</returns>
        /// <remarks>
        /// If running in Unity Editor, then checks against the current build target settings.
        /// If running in Unity Player (i.e on device endpoint), then checks if the current platform environment (i.e UWP, Linux etc)
        /// </remarks>
        public static bool IsPlatformSupported(SupportedPlatforms platforms)
        {
#if UNITY_EDITOR
            return EditorUserBuildSettings.activeBuildTarget.IsPlatformSupported(platforms);
#else
            return Application.platform.IsPlatformSupported(platforms);
#endif
        }

        /// <summary>
        /// Extension method for RuntimePlatform class to check if the current runtime is matched in the provided SupportedPlatforms flag mask
        /// </summary>
        /// <param name="runtimePlatform">RuntimePlatform object extending to call this method</param>
        /// <param name="platforms">SupportedPlatforms flag mask to check against</param>
        /// <returns>true if current RuntimePlatform is supported and contained in the given flag mask</returns>
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
                case RuntimePlatform.IPhonePlayer:
                    supportedPlatforms |= SupportedPlatforms.IOS;
                    break;
                case RuntimePlatform.WebGLPlayer:
                    supportedPlatforms |= SupportedPlatforms.Web;
                    break;
                case RuntimePlatform.Lumin:
                    supportedPlatforms |= SupportedPlatforms.Lumin;
                    break;
            }

            return supportedPlatforms;
        }

        private static bool IsPlatformSupported(SupportedPlatforms target, SupportedPlatforms supported)
        {
            return (target & supported) > 0;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Extension method for BuildTarget class to check if the current build target is matched in the provided SupportedPlatforms flag mask
        /// </summary>
        /// <param name="editorBuildTarget">BuildTarget object extending to call this method</param>
        /// <param name="platforms">SupportedPlatforms flag mask to check against</param>
        /// <returns>true if current build target in editor is supported and contained in the given flag mask</returns>
        public static bool IsPlatformSupported(this BuildTarget editorBuildTarget, SupportedPlatforms platforms)
        {
            SupportedPlatforms target = GetSupportedPlatformMask(editorBuildTarget);
            return IsPlatformSupported(target, platforms);
        }

        private static SupportedPlatforms GetSupportedPlatformMask(BuildTarget editorBuildTarget)
        {
            SupportedPlatforms supportedPlatforms = 0;

            // Editor platforms
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    supportedPlatforms |= SupportedPlatforms.WindowsEditor;
                    break;

                case RuntimePlatform.OSXEditor:
                    supportedPlatforms |= SupportedPlatforms.MacEditor;
                    break;

                case RuntimePlatform.LinuxEditor:
                    supportedPlatforms |= SupportedPlatforms.LinuxEditor;
                    break;
            }

            // Build target platforms
            switch (editorBuildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    supportedPlatforms |= SupportedPlatforms.WindowsStandalone;
                    break;
                case UnityEditor.BuildTarget.WSAPlayer:
                    supportedPlatforms |= SupportedPlatforms.WindowsUniversal;
                    break;
                case UnityEditor.BuildTarget.StandaloneOSX:
                    supportedPlatforms |= SupportedPlatforms.MacStandalone;
                    break;
#if !UNITY_2019_2_OR_NEWER
                case UnityEditor.BuildTarget.StandaloneLinux:
                case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
#endif
                case UnityEditor.BuildTarget.StandaloneLinux64:
                    supportedPlatforms |= SupportedPlatforms.LinuxStandalone;
                    break;
                case UnityEditor.BuildTarget.Android:
                    supportedPlatforms |= SupportedPlatforms.Android;
                    break;
                case UnityEditor.BuildTarget.iOS:
                    supportedPlatforms |= SupportedPlatforms.IOS;
                    break;
                case UnityEditor.BuildTarget.WebGL:
                    supportedPlatforms |= SupportedPlatforms.Web;
                    break;
                case UnityEditor.BuildTarget.Lumin:
                    supportedPlatforms |= SupportedPlatforms.Lumin;
                    break;
            }

            return supportedPlatforms;
        }
#endif

        /// <summary>
        /// Returns true if the modes specified by the specified SupportedApplicationModes matches
        /// the current mode that the code is running in.
        /// </summary>
        /// <remarks>
        /// For example, if the code is currently running in editor mode (for testing in-editor
        /// simulation), this would return true if modes contained the SupportedApplicationModes.Editor 
        /// bit.
        /// </remarks>
        public static bool IsSupportedApplicationMode(SupportedApplicationModes modes)
        {
#if UNITY_EDITOR
            return (modes & SupportedApplicationModes.Editor) != 0;
#else // !UNITY_EDITOR
            return (modes & SupportedApplicationModes.Player) != 0;
#endif
        }

        /// <summary>
        /// Updates the given SupportedApplicationModes by setting the bit associated with the
        /// currently active application mode.
        /// </summary>
        /// <remarks>
        /// For example, if the code is currently running in editor mode (for testing in-editor
        /// simulation), and modes is currently SupportedApplicationModes.Player | SupportedApplicationModes.Editor
        /// and enabled is 'false', this would return SupportedApplicationModes.Player.
        /// </remarks>
        public static SupportedApplicationModes UpdateSupportedApplicationMode(bool enabled, SupportedApplicationModes modes)
        {
#if UNITY_EDITOR
            var bitValue = enabled ? SupportedApplicationModes.Editor : 0;
            return (modes & ~SupportedApplicationModes.Editor) | bitValue;
#else // !UNITY_EDITOR
            var bitValue = enabled ? SupportedApplicationModes.Player : 0;
            return (modes & ~SupportedApplicationModes.Player) | bitValue;
#endif
        }
    }
}