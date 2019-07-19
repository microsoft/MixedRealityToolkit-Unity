// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
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
        public void Validate(IEnumerable<CompilationPlatformInfo> availablePlatforms)
        {
            excludePlatforms = excludePlatforms ?? Array.Empty<string>();
            includePlatforms = includePlatforms ?? Array.Empty<string>();

            if (excludePlatforms.Length > 0 && includePlatforms.Length > 0)
            {
                // Throw, because this means something is broken. This isn't supported according to unity: https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html
                throw new InvalidOperationException($"Assembly definition file '{name}' contains both excluded and included platform list, will refer to only included.");
            }

            // Is the EditorPlatform included?
            if (includePlatforms.Contains(EditorPlatform))
            {
                EditorPlatformSupported = true;
                NonEditorPlatformSupported = availablePlatforms.Any(t => includePlatforms.Any(i => Equals(i.ToLower(), t.Name.ToLower())));
            }
            // If it's not included, that means if we have at least one other platform, then non-editor is supported
            else if (includePlatforms.Length > 0)
            {
                NonEditorPlatformSupported = true;
            }
            else if (excludePlatforms.Length == 0) // So included platforms is length 0, as first if will be true if not 0. Means rely on excluded.
            {
                EditorPlatformSupported = true;
                NonEditorPlatformSupported = true;
            }
            else
            {
                EditorPlatformSupported = !excludePlatforms.Contains(EditorPlatform);
                // Any available platform that is not found in excluded platforms
                NonEditorPlatformSupported = availablePlatforms.Any(t => !excludePlatforms.Any(i => Equals(i.ToLower(), t.Name.ToLower())));
            }

            TestAssembly = optionalUnityReferences?.Contains(TestAssembliesReference) ?? false;
        }
    }
}
#endif