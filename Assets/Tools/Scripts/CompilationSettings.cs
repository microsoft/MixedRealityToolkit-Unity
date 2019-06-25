#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public enum TargetFramework
    {
        NetStandard20,
        Net20,
        Net46
    }

    public static class TargetFrameworkExtensions
    {
        public static string AsMSBuildString(this TargetFramework targetFramework)
        {
            switch (targetFramework)
            {
                case TargetFramework.NetStandard20:
                    return "netstandard2.0";
                case TargetFramework.Net20:
                    return "net20";
                case TargetFramework.Net46:
                    return "net46";
            }

            throw new ArgumentOutOfRangeException(nameof(targetFramework));
        }
    }

    public class CompilationSettings
    {
        public class CompilationPlatform
        {
            public AssemblyDefinitionPlatform AssemblyDefinitionPlatform { get; }
            public BuildTargetGroup BuildTargetGroup { get; }

            public TargetFramework TargetFramework { get; }

            public string Name => AssemblyDefinitionPlatform.Name;

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

            public CompilationPlatform(AssemblyDefinitionPlatform assemblyDefinitionPlatform, BuildTargetGroup buildTargetGroup, TargetFramework targetFramework,
                HashSet<string> commonPlatformDefines, HashSet<string> additionalPlayerDefines, HashSet<string> additionalInEditorDefines,
                IReadOnlyList<string> commonPlatformReferences, IReadOnlyList<string> additionalPlayerReferences, IReadOnlyList<string> additionalInEditorReferences)
            {
                AssemblyDefinitionPlatform = assemblyDefinitionPlatform;
                BuildTargetGroup = buildTargetGroup;

                TargetFramework = targetFramework;

                CommonPlatformDefines = commonPlatformDefines;
                AdditionalPlayerDefines = additionalPlayerDefines;
                AdditionalInEditorDefines = additionalInEditorDefines;

                CommonPlatformReferences = commonPlatformReferences;
                AdditionalPlayerReferences = additionalPlayerReferences;
                AdditionalInEditorReferences = additionalInEditorReferences;
            }
        }

        private class AssemblyDefinitionPlatformEqualityComparer : IEqualityComparer<AssemblyDefinitionPlatform>
        {
            public static AssemblyDefinitionPlatformEqualityComparer Instance { get; } = new AssemblyDefinitionPlatformEqualityComparer();

            private AssemblyDefinitionPlatformEqualityComparer() { }

            public bool Equals(AssemblyDefinitionPlatform x, AssemblyDefinitionPlatform y)
            {
                return x.BuildTarget == y.BuildTarget;
            }

            public int GetHashCode(AssemblyDefinitionPlatform obj)
            {
                return obj.BuildTarget.GetHashCode();
            }
        }

        private static readonly HashSet<BuildTarget> supportedBuildTargets = new HashSet<BuildTarget>()
        {
            BuildTarget.NoTarget, // This is the Unity Editor build target
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneWindows64,
            BuildTarget.iOS,
            BuildTarget.Android,
            BuildTarget.WSAPlayer
        };

        private readonly List<AssemblyDefinitionPlatform> supportedPlatforms;

        public static CompilationSettings Instance { get; } = new CompilationSettings();

        private readonly Dictionary<BuildTarget, CompilationPlatform> compilationPlatforms = new Dictionary<BuildTarget, CompilationPlatform>();

        private HashSet<string> commonDefines;
        private HashSet<string> commonDevelopmentDefines;

        private HashSet<string> commonReferences;
        private HashSet<string> commonDevelopmentReferences;

        /// <summary>
        /// These defines are common accross all platforms, and exclude development and in-editor builds
        /// </summary>
        public HashSet<string> CommonDefines { get; }

        /// <summary>
        /// These defines are common accross all platforms for the development builds.
        /// </summary>
        public HashSet<string> DevelopmentBuildAdditionalDefines { get; }

        /// <summary>
        /// These defines are common accross all platforms, and exclude development and in-editor builds
        /// </summary>
        public IReadOnlyList<string> CommonReferences { get; }

        /// <summary>
        /// These defines are common accross all platforms for the development builds.
        /// </summary>
        public IReadOnlyList<string> DevelopmentBuildAdditionalReferences { get; }

        public IReadOnlyDictionary<BuildTarget, CompilationPlatform> AvailablePlatforms { get; }

        /// <summary>
        /// This is for the .Editor assemblies, or the asmdef's that only contain "Editor" selected
        /// </summary>
        public CompilationPlatform EditorPlatform { get; private set; }

        private CompilationSettings()
        {
            supportedPlatforms = CompilationPipeline.GetAssemblyDefinitionPlatforms().Where(t => supportedBuildTargets.Contains(t.BuildTarget)).ToList();

            AvailablePlatforms = new ReadOnlyDictionary<BuildTarget, CompilationPlatform>(compilationPlatforms);

            // NOTE: builder.defaultDefines and builder.defaultReferences fetches the data each request based on settings of the builder
            // We will use that to understand all of configuration we need.
            AssemblyBuilder builder = new AssemblyBuilder("dummy.dll", new string[] { @"dummy.cs" })
            {
                buildTarget = BuildTarget.NoTarget,
                buildTargetGroup = BuildTargetGroup.Unknown,
                flags = AssemblyBuilderFlags.None
            };

            ProcessCommonDefines(builder);

            CommonDefines = commonDefines;
            DevelopmentBuildAdditionalDefines = commonDevelopmentDefines;

            ProcessCommonReferences(builder);

            CommonReferences = new ReadOnlyCollection<string>(commonReferences.ToList());
            DevelopmentBuildAdditionalReferences = new ReadOnlyCollection<string>(commonDevelopmentReferences.ToList());

            // Parse data for compilation platforms
            CreateCompilationPlatforms(builder);
        }

        private void ProcessCommonReferences(AssemblyBuilder builder)
        {
            // Assume we have common
            commonReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));

            // Set development build flag and try to get the dev references, filter out common to common,
            builder.flags = AssemblyBuilderFlags.DevelopmentBuild;
            commonDevelopmentReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
            commonReferences.RemoveWhere(t => !commonDevelopmentReferences.Contains(t));

            // Set editor flag, and get in-editor references. Filter out commont to common
            builder.flags = AssemblyBuilderFlags.EditorAssembly;
            HashSet<string> commonInEditorReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
            commonReferences.RemoveWhere(t => !commonInEditorReferences.Contains(t));

            // Go through each platform weeding out common for each of the three
            foreach (AssemblyDefinitionPlatform supportedPlatform in supportedPlatforms)
            {
                if (!IsPlatformInstalled(supportedPlatform))
                {
                    continue;
                }

                // Reset
                builder.flags = AssemblyBuilderFlags.None;

                builder.buildTarget = supportedPlatform.BuildTarget;
                builder.buildTargetGroup = GetBuildTargetGroup(supportedPlatform);

                builder.flags = AssemblyBuilderFlags.DevelopmentBuild;
                HashSet<string> other = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
                commonReferences.RemoveWhere(t => !other.Contains(t));
                commonDevelopmentReferences.RemoveWhere(t => !other.Contains(t)); // get only the common

                builder.flags = AssemblyBuilderFlags.EditorAssembly;
                other = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
                commonReferences.RemoveWhere(t => !other.Contains(t));
            }

            // Remove common from dev/editor
            commonDevelopmentReferences.RemoveWhere(commonReferences.Contains);

            // Reset
            builder.buildTarget = BuildTarget.NoTarget;
            builder.buildTargetGroup = BuildTargetGroup.Unknown;
        }

        private bool IsPlatformInstalled(AssemblyDefinitionPlatform platform)
        {
            return platform.Name.Equals("Editor") || Utilities.IsPlatformInstalled(platform.BuildTarget);
        }

        private void ProcessCommonDefines(AssemblyBuilder builder)
        {
            // Assume we have common
            commonDefines = new HashSet<string>(builder.defaultDefines);

            // Set development build flag and try to get the dev defines, filter out common to common,
            builder.flags = AssemblyBuilderFlags.DevelopmentBuild;
            commonDevelopmentDefines = new HashSet<string>(builder.defaultDefines);
            commonDefines.RemoveWhere(t => !commonDevelopmentDefines.Contains(t));

            // Set editor flag, and get in-editor defines. Filter out commont to common
            builder.flags = AssemblyBuilderFlags.EditorAssembly;
            HashSet<string> commonInEditorDefines = new HashSet<string>(builder.defaultDefines);
            commonDefines.RemoveWhere(t => !commonInEditorDefines.Contains(t));

            // Go through each platform weeding out common for each of the three
            foreach (AssemblyDefinitionPlatform supportedPlatform in supportedPlatforms)
            {
                if (!IsPlatformInstalled(supportedPlatform))
                {
                    Debug.LogError($"The platform '{supportedPlatform.DisplayName}' is not installed, it will not be supported in the MSBuild project.");
                    continue;
                }

                // Reset
                builder.flags = AssemblyBuilderFlags.None;

                builder.buildTarget = supportedPlatform.BuildTarget;
                builder.buildTargetGroup = GetBuildTargetGroup(supportedPlatform);

                builder.flags = AssemblyBuilderFlags.DevelopmentBuild;
                HashSet<string> other = new HashSet<string>(builder.defaultDefines);
                commonDefines.RemoveWhere(t => !other.Contains(t));
                commonDevelopmentDefines.RemoveWhere(t => !other.Contains(t)); // get only the common

                builder.flags = AssemblyBuilderFlags.EditorAssembly;
                other = new HashSet<string>(builder.defaultDefines);
                commonDefines.RemoveWhere(t => !other.Contains(t));
            }

            // Remove common from dev/editor
            commonDevelopmentDefines.RemoveWhere(commonDefines.Contains);

            // Reset
            builder.buildTarget = BuildTarget.NoTarget;
            builder.buildTargetGroup = BuildTargetGroup.Unknown;
        }

        private void CreateCompilationPlatforms(AssemblyBuilder builder)
        {
            // Now go through and get defines for each platform
            AssemblyDefinitionPlatform? editorPlatform = null;
            foreach (AssemblyDefinitionPlatform supportedPlatform in supportedPlatforms)
            {
                if (supportedPlatform.BuildTarget == BuildTarget.NoTarget)
                {
                    // Don't add this as a platform, we will custom handle the editor one
                    editorPlatform = supportedPlatform;
                    continue;
                }

                BuildTargetGroup buildTargetGroup = GetBuildTargetGroup(supportedPlatform);

                builder.buildTarget = supportedPlatform.BuildTarget;
                builder.buildTargetGroup = buildTargetGroup;
                builder.flags = AssemblyBuilderFlags.None;

                HashSet<string> platformCommonDefines = new HashSet<string>(builder.defaultDefines);
                HashSet<string> playerDefines = new HashSet<string>(builder.defaultDefines);

                HashSet<string> platformCommonReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
                HashSet<string> playerReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));

                builder.flags = AssemblyBuilderFlags.EditorAssembly;
                HashSet<string> inEditorDefines = new HashSet<string>(builder.defaultDefines);
                platformCommonDefines.RemoveWhere(t => !inEditorDefines.Contains(t));

                HashSet<string> inEditorReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
                platformCommonReferences.RemoveWhere(t => !inEditorReferences.Contains(t));

                // Remove the common
                platformCommonDefines.RemoveWhere(t => commonDefines.Contains(t));
                playerDefines.RemoveWhere(t => commonDefines.Contains(t) || platformCommonDefines.Contains(t));
                inEditorDefines.RemoveWhere(t => commonDefines.Contains(t) || commonDevelopmentDefines.Contains(t) || platformCommonDefines.Contains(t));

                platformCommonReferences.RemoveWhere(t => commonReferences.Contains(t));
                playerReferences.RemoveWhere(t => commonReferences.Contains(t) || platformCommonReferences.Contains(t));
                inEditorReferences.RemoveWhere(t => commonReferences.Contains(t) || commonDevelopmentReferences.Contains(t) || platformCommonReferences.Contains(t));

                // Get specialized
                playerDefines.RemoveWhere(t => platformCommonDefines.Contains(t));
                inEditorDefines.RemoveWhere(t => platformCommonDefines.Contains(t));

                playerReferences.RemoveWhere(t => platformCommonReferences.Contains(t));
                inEditorReferences.RemoveWhere(t => platformCommonReferences.Contains(t));

                CompilationPlatform compilationPlatform = new CompilationPlatform(supportedPlatform, GetBuildTargetGroup(supportedPlatform), GetTargetFramework(buildTargetGroup),
                    platformCommonDefines, playerDefines, inEditorDefines,
                    platformCommonReferences.ToList(), playerReferences.ToList(), inEditorReferences.ToList());

                compilationPlatforms.Add(supportedPlatform.BuildTarget, compilationPlatform);
            }

            if (editorPlatform == null)
            {
                Debug.LogError("Editor platform not supported? Is this a configuration error.");
            }
            else
            {
                CreateEditorPlatform(editorPlatform.Value);
            }
        }

        private void CreateEditorPlatform(AssemblyDefinitionPlatform editorPlatform)
        {
            ApiCompatibilityLevel cached = PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Unknown);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Unknown, ApiCompatibilityLevel.NET_4_6);
            AssemblyBuilder builder = new AssemblyBuilder("editor.dll", new string[] { @"Editor\dummy.cs" })
            {
                buildTarget = BuildTarget.NoTarget,
                buildTargetGroup = BuildTargetGroup.Unknown,
                flags = AssemblyBuilderFlags.EditorAssembly
            };

            HashSet<string> inEditorDefines = new HashSet<string>(builder.defaultDefines);
            HashSet<string> inEditorRefernces = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));

            inEditorDefines.RemoveWhere(t => commonDefines.Contains(t) || commonDevelopmentDefines.Contains(t));
            inEditorRefernces.RemoveWhere(t => commonReferences.Contains(t) || commonDevelopmentReferences.Contains(t));

            EditorPlatform = new CompilationPlatform(editorPlatform, BuildTargetGroup.Unknown, TargetFramework.Net46,
                new HashSet<string>(), null, inEditorDefines,
                new List<string>(), null, inEditorRefernces.ToList());

            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Unknown, cached);
        }

        private BuildTargetGroup GetBuildTargetGroup(AssemblyDefinitionPlatform assemblyDefinitionPlatform)
        {
            switch (assemblyDefinitionPlatform.BuildTarget)
            {
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.NoTarget:
                    return BuildTargetGroup.Unknown;
                default:
                    throw new PlatformNotSupportedException($"Don't currently support {assemblyDefinitionPlatform.DisplayName}");
            }
        }

        private TargetFramework GetTargetFramework(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup == BuildTargetGroup.Unknown)
            {
                //TODO this may be different on older unity versions
                return TargetFramework.Net46;
            }

            switch (PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup))
            {
                case ApiCompatibilityLevel.NET_2_0:
                case ApiCompatibilityLevel.NET_2_0_Subset:
                    return TargetFramework.Net20;
                case ApiCompatibilityLevel.NET_4_6:
                    return TargetFramework.Net46;
                case ApiCompatibilityLevel.NET_Web:
                case ApiCompatibilityLevel.NET_Micro:
                    throw new PlatformNotSupportedException("Don't currently support NET_Web and NET_Micro API compat");
                case ApiCompatibilityLevel.NET_Standard_2_0:
                    return TargetFramework.NetStandard20;
            }

            throw new PlatformNotSupportedException("ApiCompatibilityLevel platform not matched.");
        }

        private IEnumerable<string> FilterOutProjectReferences(string[] references)
        {
            string editorApplicationFolder = Path.GetDirectoryName(EditorApplication.applicationPath);
            return references
                .Select(Utilities.NormalizePath)
                .Where(t => t.StartsWith(editorApplicationFolder));
        }
    }
}
#endif