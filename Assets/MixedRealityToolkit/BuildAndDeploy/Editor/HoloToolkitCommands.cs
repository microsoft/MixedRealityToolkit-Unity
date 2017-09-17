//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Implements functionality for building HoloLens applications
    /// </summary>
    public static class HoloToolkitCommands
    {
        /// <summary>
        /// Do a build configured for the HoloLens, returns the error from BuildPipeline.BuildPlayer
        /// </summary>
        public static bool BuildSLN()
        {
            return BuildDeployTools.BuildSLN(BuildDeployPrefs.BuildDirectory, false);
        }
    }
}
