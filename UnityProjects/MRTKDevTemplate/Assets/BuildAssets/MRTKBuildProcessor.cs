// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Build
{
    internal class MRTKBuildProcessor : IPreprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            string assetsFullPath = Path.GetFullPath("Assets/TextMesh Pro");
            if (Directory.Exists(assetsFullPath))
            {
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
    }
}
