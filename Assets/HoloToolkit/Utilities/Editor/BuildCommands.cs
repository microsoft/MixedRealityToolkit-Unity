using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Implements functionality for building HoloLens applications
    /// </summary>
    public static class BuildCommands
    {
        public static readonly string BuildLocation = "Builds/HoloLens";

        /// <summary>
        /// Do a build configured for the HoloLens, returns the error from BuildPipeline.BuildPlayer
        /// </summary>
        public static string BuildForHololens()
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path);

            // Cache the current settings
            BuildTarget oldBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            WSASDK oldSDK = EditorUserBuildSettings.wsaSDK;
            WSAUWPBuildType oldBuildType = EditorUserBuildSettings.wsaUWPBuildType;

            // Set the sdk and build type
            EditorUserBuildSettings.wsaSDK = WSASDK.UWP;
            EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;

            string error = BuildPipeline.BuildPlayer(scenes.ToArray(), BuildLocation, BuildTarget.WSAPlayer, BuildOptions.None);

            // Restore the sdk and build type
            EditorUserBuildSettings.wsaSDK = oldSDK;
            EditorUserBuildSettings.wsaUWPBuildType = oldBuildType;
            EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildTarget);

            return error;
        }
    }
}
