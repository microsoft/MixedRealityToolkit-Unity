// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace HoloToolkit.Sharing
{
    public static class SharingMenu
    {
        [MenuItem("HoloToolkit/Launch Sharing Service", false)]
        public static void LaunchSessionServer()
        {
            Utilities.ExternalProcess.FindAndLaunch(@"External\HoloToolkit\Sharing\Server\SharingService.exe", @"-local");
        }

        [MenuItem("HoloToolkit/Launch Session Manager", false)]
        public static void LaunchSessionUI()
        {
            Utilities.ExternalProcess.FindAndLaunch(@"External\HoloToolkit\Sharing\Tools\SessionManager\x86\SessionManager.UI.exe");
        }

        [MenuItem("HoloToolkit/Launch Profiler", false)]
        public static void LaunchProfilerX()
        {
            Utilities.ExternalProcess.FindAndLaunch(@"External\HoloToolkit\Sharing\Tools\Profiler\x86\ProfilerX.exe");
        }
    }
}