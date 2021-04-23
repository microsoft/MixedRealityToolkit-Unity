// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Creates menu items to show users how to get help
    /// </summary>
    public class MixedRealityToolkitHelpLinks : MonoBehaviour
    {
        internal const string MRTKIssuePageUrl = "https://github.com/microsoft/MixedRealityToolkit-Unity/issues";
        internal const string MRTKDocsUrl = "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/";
        internal const string MRTKAPIRefUrl = "https://docs.microsoft.com/dotnet/api/microsoft.mixedreality.toolkit";

        [MenuItem("Mixed Reality/Toolkit/Help/Show Documentation", false)]
        private static void ShowDocumentation()
        {
            Application.OpenURL(MRTKDocsUrl);
        }
        [MenuItem("Mixed Reality/Toolkit/Help/Show API Reference", false)]
        private static void ShowAPIReference()
        {
            Application.OpenURL(MRTKAPIRefUrl);
        }
        [MenuItem("Mixed Reality/Toolkit/Help/File a bug report", false)]
        private static void FileBugReport()
        {
            Application.OpenURL(MRTKIssuePageUrl);
        }
    }
}
