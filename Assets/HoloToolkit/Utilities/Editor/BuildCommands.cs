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

        struct BuildSettings
        {
            public BuildTarget BuildTarget;
            public WSASDK WSASDK;
            public WSAUWPBuildType WSAUWPBuildType;

            public static BuildSettings Current
            {
                get
                {
                    return new BuildSettings()
                    {
                        BuildTarget = EditorUserBuildSettings.activeBuildTarget,
                        WSASDK = EditorUserBuildSettings.wsaSDK,
                        WSAUWPBuildType = EditorUserBuildSettings.wsaUWPBuildType
                    };
                }
            }

            public void Apply()
            {
                EditorUserBuildSettings.wsaSDK = WSASDK;
                EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget);
            }
        }

        /// <summary>
        /// Do a build configured for the HoloLens, returns the error from BuildPipeline.BuildPlayer
        /// </summary>
        public static string BuildForHololens()
        {

            // Cache the current settings
            BuildSettings oldBuildSettings = BuildSettings.Current;

            // Define and apply the desired settings
            BuildSettings newBuildSettings = oldBuildSettings;
            newBuildSettings.BuildTarget = BuildTarget.WSAPlayer;
            newBuildSettings.WSASDK = WSASDK.UWP;
            newBuildSettings.WSAUWPBuildType = WSAUWPBuildType.D3D;
            newBuildSettings.Apply();

            // Capture the active scenes, and build
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path);
            string error = BuildPipeline.BuildPlayer(scenes.ToArray(), BuildLocation, newBuildSettings.BuildTarget, BuildOptions.None);

            // Restore old build settings
            oldBuildSettings.Apply();

            return error;
        }
    }
}
