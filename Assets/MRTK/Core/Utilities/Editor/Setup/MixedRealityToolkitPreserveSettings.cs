// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Manages the Mixed Reality Toolkit code preservation settings. Please see
    /// https://docs.unity3d.com/Manual/ManagedCodeStripping.html for more information.
    /// </summary>
    internal static class MixedRealityToolkitPreserveSettings
    {
        /// <summary>
        /// The data that will be written to the link.xml file if this class creates one on
        /// behalf of the project.
        /// </summary>
        private const string defaultLinkXmlContents =
            "<linker> \n" +
            "  <!-- \n" +
            "    This link.xml file is provided to prevent MRTK code from being optimized away \n" +
            "    during IL2CPP builds.More details on when this is needed and why this is needed \n" +
            "    can be found here: https://github.com/microsoft/MixedRealityToolkit-Unity/issues/5273 \n" +
            "    If your application doesn't use some specific services (for example, if teleportation system is \n" +
            "    disabled in the profile), it is possible to remove their corresponding lines down \n" +
            "    below (in the previous example, we would remove the TeleportSystem below). \n" +
            "    It's recommended to start with this list and narrow down if you want to ensure \n" +
            "    specific bits of code get optimized away. \n" +
            "  --> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit\" preserve=\"all\"/> \n " +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.SDK\" preserve=\"all\"/> \n " +
            "  <!-- Core systems --> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Services.BoundarySystem\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Services.CameraSystem\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Services.DiagnosticsSystem\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Services.InputSystem\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Services.SceneSystem\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Services.SpatialAwarenessSystem\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Services.TeleportSystem\" preserve=\"all\"/> \n" +
            "  <!-- Data providers --> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.LeapMotion\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.OpenVR\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.OpenXR\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.UnityAR\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.WindowsMixedReality.Shared\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.WindowsMixedReality\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.XRSDK.WindowsMixedReality\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.WindowsVoiceInput\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.XRSDK\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Providers.WindowsSceneUnderstanding\" preserve=\"all\"/> \n" +
            "  <!-- Extension services --> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Extensions.HandPhysics\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Extensions.Tracking\" preserve=\"all\"/> \n" +
            "  <assembly fullname = \"Microsoft.MixedReality.Toolkit.Extensions.SceneTransitionService\" preserve=\"all\"/> \n" +
            "</linker>";

        /// <summary>
        /// Ensure that a link.xml file exists in the MixedRealityToolkit.Generated folder.
        /// This file is used to control the Unity linker's byte code stripping of MRTK assemblies.
        /// </summary>
        public static void EnsureLinkXml()
        {
            string generatedFolder = MixedRealityToolkitFiles.MapRelativeFolderPathToAbsolutePath(MixedRealityToolkitModuleType.Generated, "");
            string linkXmlPath = Path.Combine(generatedFolder, "link.xml");
            if (File.Exists(linkXmlPath))
            {
                // Do not touch the existing file.
                return;
            }

            // Create a default link.xml with an initial set of assembly preservation rules.
            using (StreamWriter writer = new StreamWriter(linkXmlPath))
            {
                writer.WriteLine(defaultLinkXmlContents);
                Debug.Log($"A link.xml file was created in {MixedRealityToolkitFiles.GetGeneratedFolder}. \n" +
                    "This file is used to control preservation of MRTK code during linking. It is recommended to add link.xml (and link.xml.meta) to source control.");
            }
        }
    }
}
