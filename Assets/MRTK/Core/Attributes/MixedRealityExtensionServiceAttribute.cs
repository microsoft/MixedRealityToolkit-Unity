// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
#endif

#if WINDOWS_UWP && !ENABLE_IL2CPP
using Microsoft.MixedReality.Toolkit;
#endif // WINDOWS_UWP && !ENABLE_IL2CPP

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute that defines the properties of a Mixed Reality Toolkit extension service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MixedRealityExtensionServiceAttribute : Attribute
    {
        /// <summary>
        /// The friendly name for this service.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// The runtime platform(s) to run this service.
        /// </summary>
        public virtual SupportedPlatforms RuntimePlatforms { get; }

        /// <summary>
        /// Is a profile explicitly required?
        /// </summary>
        public virtual bool RequiresProfile { get; }

        /// <summary>
        /// The file path to the default profile asset relative to the package folder.
        /// </summary>
        public virtual string DefaultProfilePath { get; }

        /// <summary>
        /// The package where the default profile asset resides.
        /// </summary>
        public virtual string PackageFolder { get; }

        /// <summary>
        /// The default profile.
        /// </summary>
        public virtual BaseMixedRealityProfile DefaultProfile
        {
            get
            {
#if UNITY_EDITOR
                MixedRealityToolkitModuleType moduleType = MixedRealityToolkitFiles.GetModuleFromPackageFolder(PackageFolder);

                if (moduleType != MixedRealityToolkitModuleType.None)
                {
                    string folder = MixedRealityToolkitFiles.MapModulePath(moduleType);
                    if (!string.IsNullOrWhiteSpace(folder))
                    {
                        return AssetDatabase.LoadAssetAtPath<BaseMixedRealityProfile>(System.IO.Path.Combine(folder, DefaultProfilePath));
                    }
                }
                else
                {
                    string folder;
                    if (EditorProjectUtilities.FindRelativeDirectory(PackageFolder, out folder))
                    {
                        return AssetDatabase.LoadAssetAtPath<BaseMixedRealityProfile>(System.IO.Path.Combine(folder, DefaultProfilePath));
                    }
                }

                // If we get here, there was an issue finding the profile.
                Debug.LogError("Unable to find or load the profile.");
#endif  
                return null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runtimePlatforms">The platforms on which the extension service is supported.</param>
        /// <param name="defaultProfilePath">The relative path to the default profile asset.</param>
        /// <param name="packageFolder">The package folder to which the path is relative.</param>
        public MixedRealityExtensionServiceAttribute(
            SupportedPlatforms runtimePlatforms,
            string name = "",
            string defaultProfilePath = "",
            string packageFolder = "MixedRealityToolkit",
            bool requiresProfile = false)
        {
            RuntimePlatforms = runtimePlatforms;
            Name = name;
            DefaultProfilePath = defaultProfilePath;
            PackageFolder = packageFolder;
            RequiresProfile = requiresProfile;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Convenience function for retrieving the attribute given a certain class type.
        /// </summary>
        /// <remarks>
        /// This function is only available in a UnityEditor context.
        /// </remarks>
        public static MixedRealityExtensionServiceAttribute Find(Type type)
        {
            return type.GetCustomAttributes(typeof(MixedRealityExtensionServiceAttribute), true).FirstOrDefault() as MixedRealityExtensionServiceAttribute;
        }
#endif // UNITY_EDITOR
    }
}
