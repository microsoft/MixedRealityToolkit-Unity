// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// This class represents an AssemblyDefinition file of a Unity project. It can be used to parse the file contents using <see cref="JsonUtility.FromJson{T}(string)"/>.
    /// </summary>
    public class AssemblyDefinitionInfo
    {
        public const string EditorPlatform = "Editor";
        public const string TestAssembliesReference = "TestAssemblies";

        // JSON Values
        public string name;
        public string[] references;
        public string[] optionalUnityReferences;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;

        /// <summary>
        /// Gets or sets the parsed Guid of the AssemblyDefinition asset.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the path of the AssemblyDefinition asset relative to the Assets folder.
        /// </summary>
        public string AssetsRelativePath { get; set; }

        /// <summary>
        /// Gets whether the Editor platform is specified to be supported.
        /// </summary>
        public bool EditorPlatformSupported { get; private set; }

        /// <summary>
        /// Gets whether any non-Editor platforms are specified to be supported.
        /// </summary>
        public bool NonEditorPlatformSupported { get; private set; }

        /// <summary>
        /// Gets whether this is marked as a TestAssembly.
        /// </summary>
        public bool TestAssembly { get; private set; }

        /// <summary>
        /// After it's parsed from JSON, this method should be invoked to validate some of the values and set additional properties.
        /// </summary>
        public void Validate()
        {
            if (excludePlatforms.Length > 0 && includePlatforms.Length > 0)
            {
                Debug.LogError($"Assembly definition file '{name}' contains both excluded and included platform list, will refer to only included.");
                excludePlatforms = Array.Empty<string>();
            }

            EditorPlatformSupported =
                (includePlatforms.Length > 0 && includePlatforms.Contains(EditorPlatform))
                || (excludePlatforms.Length > 0 && !excludePlatforms.Contains(EditorPlatform));

            NonEditorPlatformSupported =
                (includePlatforms.Length > 0 && !includePlatforms.Contains(EditorPlatform))
                || (excludePlatforms.Length > 0 && excludePlatforms.Contains(EditorPlatform));

            TestAssembly = optionalUnityReferences?.Contains(TestAssembliesReference) ?? false;
        }
    }
}
#endif