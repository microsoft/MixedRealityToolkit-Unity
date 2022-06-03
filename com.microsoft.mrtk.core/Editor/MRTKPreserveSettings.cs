// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Manages the Mixed Reality Toolkit code preservation settings. Please see
    /// https://docs.unity3d.com/Manual/ManagedCodeStripping.html for more information.
    /// </summary>
    internal class MRTKPreserveSettings : IUnityLinkerProcessor
    {
        int IOrderedCallback.callbackOrder => 0;

        /// <summary>
        /// Ensure that a link.xml file exists with the relevant MRTK types.
        /// This file is used to control the Unity linker's byte code stripping of MRTK assemblies.
        /// </summary>
        string IUnityLinkerProcessor.GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            Dictionary<Assembly, List<Type>> typesByAssemblies = new Dictionary<Assembly, List<Type>>();

            IEnumerable<Type> mrtkTypesToPreserve = TypeCache.GetTypesWithAttribute<MRTKSubsystemAttribute>();

            foreach (Type type in mrtkTypesToPreserve)
            {
                if (type.IsGenericType || type.IsAbstract)
                {
                    continue;
                }

                if (typesByAssemblies.ContainsKey(type.Assembly))
                {
                    typesByAssemblies[type.Assembly].Add(type);
                }
                else
                {
                    typesByAssemblies[type.Assembly] = new List<Type>() { type };
                }
            }

            StringBuilder sb = new StringBuilder("<linker>\n");

            foreach (Assembly assembly in typesByAssemblies.Keys.OrderBy(a => a.GetName().Name))
            {
                sb.AppendLine($"  <assembly fullname=\"{assembly.GetName().Name}\">");

                List<Type> types = typesByAssemblies[assembly];
                foreach (Type type in types.OrderBy(t => t.FullName))
                {
                    sb.AppendLine($"    <type fullname=\"{FormatForXml(type.FullName)}\" preserve=\"all\"/>");
                }

                sb.AppendLine("  </assembly>");
            }

            sb.AppendLine("</linker>");

            string linkXmlPath = Path.Combine(Application.dataPath, "..", "Temp", "MRTKLink.xml");
            File.WriteAllText(linkXmlPath, sb.ToString());
            return linkXmlPath;
        }

        private static string FormatForXml(string value) => value.Replace('+', '/').Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

#if !UNITY_2021_2_OR_NEWER
        void IUnityLinkerProcessor.OnAfterRun(BuildReport report, UnityLinkerBuildPipelineData data) { }
        void IUnityLinkerProcessor.OnBeforeRun(BuildReport report, UnityLinkerBuildPipelineData data) { }
#endif // !UNITY_2021_2_OR_NEWER
    }
}
