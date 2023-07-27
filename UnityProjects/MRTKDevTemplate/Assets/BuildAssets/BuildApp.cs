﻿// Copyright (c) Microsoft Corporation.
// Licensed under the Apache License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Build
{
    /// <summary>
    /// A static class that provides functions for compiling the example scenes via command line.
    /// </summary>
    public static class BuildApp
    {
        private static string[] scenes = 
        {
            "Assets/Scenes/BoundsControlExamples.unity",
            "Assets/Scenes/CanvasExample.unity",
            "Assets/Scenes/CanvasUITearsheet.unity",
            "Assets/Scenes/ClippingExamples.unity",
            "Assets/Scenes/ClippingInstancedExamples.unity",
            "Assets/Scenes/DiagnosticsDemo.unity",
            "Assets/Scenes/DialogExample.unity",
            "Assets/Scenes/DictationExample.unity",
            "Assets/Scenes/DirectionalIndicatorExample.unity",
            "Assets/Scenes/DisableInteractorsExample.unity",
            "Assets/Scenes/DwellExample.unity",
            "Assets/Scenes/EyeGazeExample.unity",
            "Assets/Scenes/FontIconExample.unity",
            "Assets/Scenes/HandInteractionExamples.unity",
            "Assets/Scenes/HandMenuExamples.unity",
            "Assets/Scenes/InputFieldExamples.unity",
            "Assets/Scenes/InteractableButtonExamples.unity",
            "Assets/Scenes/LegacyConstraintsExample.unity",
            "Assets/Scenes/MagicWindowExample.unity",
            "Assets/Scenes/NearMenuExamples.unity",
            "Assets/Scenes/NonCanvasDialogExample.unity",
            "Assets/Scenes/NonCanvasObjectBarExample.unity",
            "Assets/Scenes/NonCanvasUIBackplateExample.unity",
            "Assets/Scenes/NonCanvasUITearSheet.unity",
            "Assets/Scenes/OutlineExamples.unity",
            "Assets/Scenes/PerformanceEvaluation.unity",
            "Assets/Scenes/SeeItSayIt Example.unity",
            "Assets/Scenes/SlateDrawingExample.unity",
            "Assets/Scenes/SolverExamples.unity",
            "Assets/Scenes/SpatialMappingExample.unity",
            "Assets/Scenes/SpeechInputExamples.unity",
            "Assets/Scenes/TapToPlaceExample.unity",
            "Assets/Scenes/TextPrefabExamples.unity",
            "Assets/Scenes/TextToSpeechExamples.unity",
            "Assets/Scenes/ToggleCollectionExample.unity",
            "Assets/Scenes/TopNavigationExample.unity",
            "Assets/Scenes/VanillaUGUIExample.unity",
            "Assets/Scenes/Audio/AudioLoFiExample.unity",
            "Assets/Scenes/Audio/AudioOcclusionExample.unity",
            "Assets/Scenes/Experimental/NonNativeKeyboard.unity",
            "Assets/Scenes/Experimental/ScrollingExample.unity",
            "Assets/Scenes/Experimental/SpatialMouseSample.unity",
            "Assets/Scenes/Experimental/VirtualizedScrollRectList.unity",
            "Assets/Scenes/EyeTracking/EyeTrackingBasicSetupExample.unity",
            "Assets/Scenes/EyeTracking/EyeTrackingExampleNavigationExample.unity",
            "Assets/Scenes/EyeTracking/EyeTrackingTargetPositioningExample.unity",
            "Assets/Scenes/EyeTracking/EyeTrackingTargetSelectionExample.unity",
            "Assets/Scenes/EyeTracking/EyeTrackingVisualizerExample.unity"
        };

        private static string buildPath = "build";

        /// <summary>
        /// Build the Unity project's example scenes.
        /// </summary>
        public static void StartCommandLineBuild()
        {
            ParseBuildCommandLine();

            // We don't need stack traces on all our logs. Makes things a lot easier to read.
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Debug.Log($"Starting command line build for {EditorUserBuildSettings.activeBuildTarget}...");

            BuildPlayerOptions options = new BuildPlayerOptions()
            {
                scenes = scenes,
                locationPathName = buildPath,
                targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,
                target = EditorUserBuildSettings.activeBuildTarget,
            };

            bool success;
            try
            {
                BuildReport buildReport = BuildPipeline.BuildPlayer(options);
                success = buildReport != null && buildReport.summary.result == BuildResult.Succeeded;
            }
            catch (Exception e)
            {
                Debug.LogError($"Build Failed!\n{e.Message}\n{e.StackTrace}");
                success = false;
            }

            Debug.Log($"Finished build... Build success? {success}");

            EditorApplication.Exit(success ? 0 : 1);
        }

        /// <summary>
        /// Ensure that the Text Mesh Pro assets are included in the Unity project.
        /// </summary>
        /// <remarks>
        /// This is currently not functioning correctly. When running via command line,
        /// the assets are imported, but are not available in the built application.
        /// </remarks>
        public static void EnsureTMPro()
        {
            string assetsFullPath = Path.GetFullPath("Assets/TextMesh Pro");
            if (Directory.Exists(assetsFullPath))
            {
                Debug.Log("TMPro assets folder already imported. Skipping import.");
                return;
            }

            // Import the TMP Essential Resources package
            string packageFullPath = Path.GetFullPath("Packages/com.unity.textmeshpro");
            if (Directory.Exists(packageFullPath))
            {
                Debug.Log("Importing TextMesh Pro...");
                AssetDatabase.ImportPackage(packageFullPath + "/Package Resources/TMP Essential Resources.unitypackage", false);
            }
            else
            {
                Debug.LogError("Unable to locate the Text Mesh Pro package.");
            }
        }

        private static void ParseBuildCommandLine()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-sceneList":
                        scenes = SplitSceneList(arguments[++i]);
                        break;
                    case "-buildOutput":
                        buildPath = arguments[++i];
                        break;
                }
            }
        }

        private static string[] SplitSceneList(string sceneList)
        {
            return (from scene in sceneList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    select scene.Trim()).ToArray();
        }
    }
}
#pragma warning restore CS1591