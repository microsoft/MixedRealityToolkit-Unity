// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Compilation;

namespace Microsoft.MixedReality.Toolkit.Tools.Build
{
    /// <summary>
    /// A helper for loading assembly definition information from disk.
    /// </summary>
    public class AssemblyDefinitionLoader
    {
        /// <summary>
        /// Loads the AssemblyDefinition given the Unity Assembly.
        /// </summary>
        /// <remarks>
        /// Calls to load the same assembly will read from cache by default. If loadFromCache
        /// is false the disk will be hit and the cache entry will be replaced by the latest
        /// disk read.
        /// </remarks>
        public static AssemblyDefinition Load(Assembly assembly, bool loadFromCache = true)
        {
            if (loadFromCache && cache.ContainsKey(assembly.name))
            {
                return cache[assembly.name];
            }

            string relativePath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assembly.name);
            string fullPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(Application.dataPath)), relativePath);

            AssemblyDefinition assemblyDefinition = JsonUtility.FromJson<AssemblyDefinition>(File.ReadAllText(fullPath));
            Validate(assemblyDefinition);
            cache[assembly.name] = assemblyDefinition;
            return assemblyDefinition;
        }

        /// <summary>
        /// Performs basic validation on the given AssemblyDefinition object to ensure that it conforms
        /// to the asmdef spec: https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html
        /// </summary>
        private static void Validate(AssemblyDefinition assemblyDefinition)
        {   
            if (assemblyDefinition.excludePlatforms.Length > 0 && assemblyDefinition.includePlatforms.Length > 0)
            {
                // This isn't support according to the Unity asmdef spec:
                // https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html
                throw new InvalidOperationException($"Assembly definition '{assemblyDefinition.name}' erroneously contains both excluded and included platform list");
            }
        }

        /// <summary>
        /// Caches the results of Load so that future calls do not need to hit disk.
        /// </summary>
        private static Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition>();
    }
}