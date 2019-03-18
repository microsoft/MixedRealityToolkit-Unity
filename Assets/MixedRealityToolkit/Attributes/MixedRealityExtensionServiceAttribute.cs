// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor;
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Core.Attributes
{
    /// <summary>
    /// Attribute that defines the properties of a Mixed Reality Toolkit extension service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MixedRealityExtensionServiceAttribute : Attribute
    {
        /// <summary>
        /// The runtime platform(s) to run this service.
        /// </summary>
        public virtual SupportedPlatforms RuntimePlatforms { get; }

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
                string path;
                if (EditorProjectUtilities.FindRelativeDirectory(PackageFolder, out path))
                {
                    return AssetDatabase.LoadAssetAtPath<BaseMixedRealityProfile>(System.IO.Path.Combine(path, DefaultProfilePath));
                }

                Debug.LogError("Unable to find or load the profile.");
#endif  
                return null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runtimePlatforms">The platforms on which the extension service is supported.</param>
        /// <param name="profilePath">The relative path to the default profile asset.</param>
        /// <param name="packageFolder">The package folder to which the path is relative.</param>
        public MixedRealityExtensionServiceAttribute(
            SupportedPlatforms runtimePlatforms,
            string defaultProfilePath = "",
            string packageFolder = "MixedRealityToolkit")
        {
            RuntimePlatforms = runtimePlatforms;
            DefaultProfilePath = defaultProfilePath;
            PackageFolder = packageFolder;
        }

        /// <summary>
        /// Convenience function for retrieving the attribute given a certain class type.
        /// </summary>
        public static MixedRealityExtensionServiceAttribute Find(Type type)
        {
            return type.GetCustomAttributes(typeof(MixedRealityExtensionServiceAttribute), true).FirstOrDefault() as MixedRealityExtensionServiceAttribute;
        }
    }
}
