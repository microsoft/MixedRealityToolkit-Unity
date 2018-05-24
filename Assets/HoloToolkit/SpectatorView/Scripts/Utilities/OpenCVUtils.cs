// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System.Runtime.InteropServices;
using UnityEngine;

namespace HoloToolkit.Unity.SpectatorView
{
    /// <summary>
    /// Utility function to ensure the OpenCVWrapper has been successfully loaded
    /// </summary>
    public static class OpenCVUtils
    {
        /// <summary>
        /// Utility function to ensure the OpenCVWrapper has been successfully loaded
        /// </summary>
        [DllImport("SpectatorViewPlugin", EntryPoint="CheckLibraryHasLoaded", ExactSpelling=true)]
        public static extern void CheckOpenCVWrapperHasLoaded();
    }
}
