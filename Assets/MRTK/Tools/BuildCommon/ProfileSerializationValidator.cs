// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.Compilation;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tools.Build
{
    public class ProfileSerializationValidator
    {
        /// <summary>
        /// Enumerates the profile classes within the project and validates that
        /// all of them support all platforms.
        /// </summary>
        /// <remarks>
        /// This is intended to avoid a class of errors captured in issues:
        /// https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8067
        /// https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7669
        /// 
        /// The issue that this is intending to safeguard against is where the class of a profile
        /// is marked as only supporting a subset of the different platforms (i.e. only editor, only
        /// Windows UWP, only Standalone). When this happens and the project is built for a
        /// different platform, a Unity asset deserialization error occurs because the type of
        /// that class may be referenced by a loaded profile, but because the assembly containing
        /// that class is compiled away (it doesn't support that platform), Unity deserialization
        /// errors when it isn't able to find that particular type. This leads to unavoidable
        /// error messages in consoles/logs.
        /// </remarks>
        /// <returns>True if profile classes are properly marked for each platform</returns>
        [UnityEditor.MenuItem("Mixed Reality Toolkit/Build/ProfileSerializationValidator")]
        public static bool Validate()
        {
            // In order to convert from System.Reflection.Assembly to UnityEditor.Compilation.Assembly,
            // we generate a map of assmebly name to Unity Assembly
            Dictionary<string, Assembly> unityAssemblies = new Dictionary<string, Assembly>();
            foreach (Assembly assembly in CompilationPipeline.GetAssemblies())
            {
                unityAssemblies.Add(assembly.name, assembly);
            }

            var errors = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(type => IsProfileMissingPlatforms(type, unityAssemblies));

            foreach (Type type in errors)
            {
                Debug.LogError($"Profile {type.Name} belongs to assembly {type.Assembly.GetName().Name} which should be changed to support all platforms");
            }

            return errors.Count() == 0;
        }

        /// <summary>
        /// Returns true if the given type is both a profile that derives from BaseMixedRealityProfile
        /// and belongs to an assembly definition that is not compiled for all platforms.
        /// </summary>
        private static bool IsProfileMissingPlatforms(Type type, Dictionary<string, Assembly> unityAssemblies)
        {
            if (typeof(BaseMixedRealityProfile).IsAssignableFrom(type) &&
                unityAssemblies.ContainsKey(type.Assembly.GetName().Name))
            {
                AssemblyDefinition assemblyDefinition = AssemblyDefinitionLoader.Load(unityAssemblies[type.Assembly.GetName().Name]);

                if (assemblyDefinition.excludePlatforms.Length != 0 || assemblyDefinition.includePlatforms.Length != 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}