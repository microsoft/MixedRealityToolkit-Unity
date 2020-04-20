// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Utilities
{
    /// <summary>
    /// Leap Motion Utilities for determining if the Leap Motion Core Assets are in the project.
    /// </summary>
    static class LeapMotionUtilities
    {
        // The presence of the LeapXRServiceProvider.cs is used to determine if the Leap Motion Core Assets are in the project.
        private const string trackedLeapFileName = "LeapXRServiceProvider.cs";

        /// <summary>
        /// If true, the LeapXRServiceProvider.cs file is in the project.  The presence of this file is used to determine if the 
        /// Leap Motion Core Assets are in the project.
        /// </summary>
        public static bool IsLeapInProject => LeapMotionFileDetected();

        // Check if the LeapXRServiceProvider.cs file is in the project.
        private static bool LeapMotionFileDetected()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(trackedLeapFileName);

            if (files.Length > 0)
            {
                return true;
            }

            return false;
        }
    }
}
