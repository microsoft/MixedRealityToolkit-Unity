// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// This file contains a build post processor that adds a specific compiler flag to avoid
// the ARM64 compiler issue described in this issue:
// https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7624
// It works by updating the specific .vxcproj file and only modifying the ARM64 compiler
// option to turn off the specific optimization that leads to the ARM64 issue

// This build post processor should only run after a UWP player build.
// This ifdef exists so that we don't add extra overhead to other platform builds.
#if UNITY_WSA && UNITY_2019_4_OR_NEWER
using System;
using System.IO;
using System.Xml.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    public class Arm64Workaround : IPostprocessBuildWithReport
    {
        private const string VcxProjectRelativeFilePath = "Il2CppOutputProject/Il2CppOutputProject.vcxproj";
        private const string Arm64Condition = "'$(Platform)'=='ARM64'";
        private const string WorkaroundCompilerFlags = " --compiler-flags=\"-d2ssa-cfg-jt-\"";

        // Arbitrary callback order, chosen to be larger so that it runs after other things that
        // a developer may have already.
        public int callbackOrder => 100;

        public void OnPostprocessBuild(BuildReport report)
        {
            EnsureWorkaround(Path.Combine(report.summary.outputPath, VcxProjectRelativeFilePath));
        }

        private static void EnsureWorkaround(string path)
        {
            // Silently eat exceptions - this workaround could fail if files aren't found, if there are
            // errors writing to the path (i.e. the csproj is open by another process and locked)
            // This is a best-effort workaround.
            try
            {
                XElement root = XElement.Load(path);

                // Adding the workaround is basically a two-step process:
                // 1) Finding the actual build command line (the thing we will append --compiler-flags="-d2ssa-cfg-jt-"
                // to.
                // 2) Appending a new PropertyGroup that is conditional on Arm64Condition, which appends those
                // compiler flags.
                // If this code discovers that the work was already done (i.e. there's already some conditional there
                // with a build command line override, or there's a command line with compiler flags, then
                // we abort.
                string buildCommandLineText = "";
                string rebuildCommandLineText = "";
                foreach (XElement propertyGroup in root.Elements(root.GetDefaultNamespace() + "PropertyGroup"))
                {
                    XAttribute condition = propertyGroup.Attribute("Condition");

                    // We look for both, even though we actually only use one (i.e. this is to provide another layer
                    // of assurance that we actually got the right PropertyGroup element).
                    XElement buildCommandLine = propertyGroup.Element(root.GetDefaultNamespace() + "NMakeBuildCommandLine");
                    XElement rebuildCommandLine = propertyGroup.Element(root.GetDefaultNamespace() + "NMakeReBuildCommandLine");
                    if (buildCommandLine != null && rebuildCommandLine != null)
                    {
                        // It's possible that we have already had the workaround in this file, in which case there will be
                        // an ARM64 condition with the buildCommandLine value present. In that case, we just return and
                        // don't do anything. It's also possible there are already compiler flags set which is not the default
                        // and if this happens, avoid doing anything (so as to not cause a problem in untested scenarios)
                        if (condition != null && condition.Value == Arm64Condition || buildCommandLine.Value.Contains("--compiler-flags"))
                        {
                            return;
                        }

                        buildCommandLineText = buildCommandLine.Value;
                        rebuildCommandLineText = rebuildCommandLine.Value;
                    }
                }

                // This is unexpected for the workaround but if this happens, we should abort.
                if (buildCommandLineText.Length == 0)
                {
                    return;
                }

                buildCommandLineText += WorkaroundCompilerFlags;

                // The newly added node has to be appended to the end of the Project node, otherwise
                // its values will not take effect (i.e. the last setter of the given values will win)
                XElement newPropertyGroup = new XElement(root.GetDefaultNamespace() + "PropertyGroup");
                newPropertyGroup.SetAttributeValue("Condition", Arm64Condition);

                XElement newBuildCommandLine = new XElement(root.GetDefaultNamespace() + "NMakeBuildCommandLine");
                XElement newRebuildCommandLine = new XElement(root.GetDefaultNamespace() + "NMakeReBuildCommandLine");
                newBuildCommandLine.SetValue(buildCommandLineText);
                newRebuildCommandLine.SetValue(rebuildCommandLineText);

                newPropertyGroup.Add(newBuildCommandLine);
                newPropertyGroup.Add(newRebuildCommandLine);

                root.Add(newPropertyGroup);
                root.Save(path);
            }
            catch (Exception)
            {
                Debug.Log("Encountered an error when applying an ARM64 compiler workaround. See https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7624 for more information");
            }
        }
    }
}
#endif // UNITY_WSA && UNITY_2019_4_OR_NEWER