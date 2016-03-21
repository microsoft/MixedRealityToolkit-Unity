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
            Utilities.ExternalProcess.FindAndLaunch(@"HoloToolkit\Sharing\SharingService\SharingService.exe", @"-local");
        }        
    }
}