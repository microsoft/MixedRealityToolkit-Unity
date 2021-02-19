// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// This contains parsed information using Unity configuration about a specific compilation platform.
    /// </summary>
    public class CompilationPlatformInfo
    {
        /// <summary>
        /// Given a non-editor <see href="https://docs.unity3d.com/ScriptReference/Compilation.AssemblyDefinitionPlatform.html">AssemblyDefinitionPlatform</see> platform, creates an instances of CompilationPlatform fetching defines and references.
        /// </summary>
        /// <param name="platform">The platform to use for parsing.</param>
        /// <returns>The <see cref="CompilationPlatformInfo"/> containing building information for the platform.</returns>
        public static CompilationPlatformInfo GetCompilationPlatform(AssemblyDefinitionPlatform platform)
        {
            if (platform.BuildTarget == BuildTarget.NoTarget)
            {
                throw new ArgumentException(nameof(platform), "Provided an editor platform, use GetEditorPlatform.");
            }

            AssemblyBuilder builder = new AssemblyBuilder("dummy.dll", new string[] { @"Editor\dummy.cs" })
            {
                buildTarget = platform.BuildTarget,
                buildTargetGroup = Utilities.GetBuildTargetGroup(platform.BuildTarget),
                flags = AssemblyBuilderFlags.None
            };

            HashSet<string> platformCommonDefines = new HashSet<string>(builder.defaultDefines);
            HashSet<string> playerDefines = new HashSet<string>(builder.defaultDefines);

            HashSet<string> platformCommonReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
            HashSet<string> playerReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));

            // Set the editor flag to get the updated set of references/defines.
            builder.flags = AssemblyBuilderFlags.EditorAssembly;
            HashSet<string> inEditorDefines = new HashSet<string>(builder.defaultDefines);
            platformCommonDefines.IntersectWith(inEditorDefines);

            HashSet<string> inEditorReferences = new HashSet<string>(FilterOutProjectReferences(GetEditorReferences(builder)));
            platformCommonReferences.IntersectWith(inEditorReferences);

            // Remove the common
            playerDefines.ExceptWith(platformCommonDefines);
            inEditorDefines.ExceptWith(platformCommonDefines);

            playerReferences.ExceptWith(platformCommonReferences);
            inEditorReferences.ExceptWith(platformCommonReferences);

            return new CompilationPlatformInfo(platform.Name, platform.BuildTarget,
                platformCommonDefines, playerDefines, inEditorDefines,
                platformCommonReferences.ToList(), playerReferences.ToList(), inEditorReferences.ToList());
        }

        /// <summary>
        /// Creates an editor <see cref="CompilationPlatformInfo"/> (think Assembly-CSharp-Editor).
        /// </summary>
        /// <returns>The editor <see cref="CompilationPlatformInfo"/>.</returns>
        public static CompilationPlatformInfo GetEditorPlatform()
        {
            ApiCompatibilityLevel cached = PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Unknown);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Unknown, ApiCompatibilityLevel.NET_4_6);

            try
            {
                AssemblyBuilder builder = new AssemblyBuilder("editor.dll", new string[] { @"Editor\dummy.cs" })
                {
                    buildTarget = BuildTarget.NoTarget,
                    buildTargetGroup = BuildTargetGroup.Unknown,
                    flags = AssemblyBuilderFlags.EditorAssembly
                };

                return new CompilationPlatformInfo("Editor", BuildTarget.NoTarget,
                    new HashSet<string>(), null, new HashSet<string>(builder.defaultDefines),
                    new List<string>(), null, new List<string>(FilterOutProjectReferences(GetEditorReferences(builder))));
            }
            finally
            {
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Unknown, cached);
            }
        }

        private static string[] GetEditorReferences(AssemblyBuilder builder)
        {
#if UNITY_2020_2_OR_NEWER
            // Starting from Unity 2020.2 there are two versions of UnityEditor.dll in the Editor\Data\Managed folder.
            // Here we want to reference the "full version" (i.e. version with submodules)
            string[] editorReferences = builder.defaultReferences.ToArray();
            if (editorReferences[1].EndsWith("/UnityEngine/UnityEditor.dll"))
            {
                editorReferences[1] = editorReferences[1].Substring(0, editorReferences[1].LastIndexOf("/UnityEngine/UnityEditor.dll")) + "\\UnityEditor.dll";
            }

            return editorReferences;
#else
            return builder.defaultReferences;
#endif
        }

        private static IEnumerable<string> FilterOutProjectReferences(string[] references)
        {
            string editorApplicationFolder = Path.GetDirectoryName(EditorApplication.applicationPath);
            return references
                .Select(Path.GetFullPath)
                .Where(t => t.StartsWith(editorApplicationFolder));
        }

        /// <summary>
        /// The name of this compilation platform.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The <see cref="BuildTarget"/> this compilation platform represents.
        /// </summary>
        public BuildTarget BuildTarget { get; }

        /// <summary>
        /// The <see cref="BuildTargetGroup"/> this compilation platform represents.
        /// </summary>
        public BuildTargetGroup BuildTargetGroup { get; }

        /// <summary>
        /// The <see cref="TargetFramework"/> of this compilation platform.
        /// </summary>
        public TargetFramework TargetFramework { get; }

        /// <summary>
        ///  These defines are specific for this platform and common or player/editor.
        /// </summary>
        public HashSet<string> CommonPlatformDefines { get; }

        /// <summary>
        ///  These defines are specific for this platform player build.
        /// </summary>
        public HashSet<string> AdditionalPlayerDefines { get; }

        /// <summary>
        ///  These defines are specific for this platform editor build.
        /// </summary>
        public HashSet<string> AdditionalInEditorDefines { get; }

        /// <summary>
        ///  These references are specific for this platform and common or player/editor.
        /// </summary>
        public IReadOnlyList<string> CommonPlatformReferences { get; }

        /// <summary>
        ///  These references are specific for this platform player build.
        /// </summary>
        public IReadOnlyList<string> AdditionalPlayerReferences { get; }

        /// <summary>
        ///  These references are specific for this platform editor build.
        /// </summary>
        public IReadOnlyList<string> AdditionalInEditorReferences { get; }

        private CompilationPlatformInfo(string name, BuildTarget buildTarget,
            HashSet<string> commonPlatformDefines, HashSet<string> additionalPlayerDefines, HashSet<string> additionalInEditorDefines,
            IReadOnlyList<string> commonPlatformReferences, IReadOnlyList<string> additionalPlayerReferences, IReadOnlyList<string> additionalInEditorReferences)
        {
            Name = name;
            BuildTarget = buildTarget;
            BuildTargetGroup = Utilities.GetBuildTargetGroup(BuildTarget);

            TargetFramework = BuildTargetGroup.GetTargetFramework();

            CommonPlatformDefines = commonPlatformDefines;
            AdditionalPlayerDefines = additionalPlayerDefines;
            AdditionalInEditorDefines = additionalInEditorDefines;

            CommonPlatformReferences = commonPlatformReferences;
            AdditionalPlayerReferences = additionalPlayerReferences;
            AdditionalInEditorReferences = additionalInEditorReferences;
        }
    }
}
