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
            public BuildTarget BuildTarget { get; }
            public BuildTargetGroup BuildTargetGroup { get; }

            public TargetFramework TargetFramework { get; }

            /// <summary>
            ///  These defines are specific for this platform and common or player/editor.
            /// </summary>
            public IReadOnlyList<string> CommonPlatformDefines { get; }

            /// <summary>
            ///  These defines are specific for this platform player build.
            /// </summary>
            public IReadOnlyList<string> AdditionalPlayerDefines { get; }

            /// <summary>
            ///  These defines are specific for this platform editor build.
            /// </summary>
            public IReadOnlyList<string> AdditionalEditorDefines { get; }

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
            public IReadOnlyList<string> AdditionalEditorReferences { get; }

            public CompilationPlatform(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup, TargetFramework targetFramework,
                IReadOnlyList<string> commonPlatformDefines, IReadOnlyList<string> additionalPlayerDefines, IReadOnlyList<string> additionalEditorDefines,
                IReadOnlyList<string> commonPlatformReferences, IReadOnlyList<string> additionalPlayerReferences, IReadOnlyList<string> additionalEditorReferences)
            {
                BuildTarget = buildTarget;
                BuildTargetGroup = buildTargetGroup;

                TargetFramework = targetFramework;

                CommonPlatformDefines = commonPlatformDefines;
                AdditionalPlayerDefines = additionalPlayerDefines;
                AdditionalEditorDefines = additionalEditorDefines;

                CommonPlatformReferences = commonPlatformReferences;
                AdditionalPlayerReferences = additionalPlayerReferences;
                AdditionalEditorReferences = additionalEditorReferences;
            }
        }

        //TODO consider removing
        //private class FileNameEqualityComparer : IEqualityComparer<string>
        //{
        //    public static FileNameEqualityComparer Instance { get; } = new FileNameEqualityComparer();

        //    private FileNameEqualityComparer() { }

        //    public bool Equals(string x, string y)
        //    {
        //        if (x == null || y == null)
        //        {
        //            return Equals(x, y);
        //        }

        //        return Equals(Path.GetFileName(y), Path.GetFileName(y));
        //    }

        //    public int GetHashCode(string obj)
        //    {
        //        return obj == null ? 0 : Path.GetFileName(obj).GetHashCode();
        //    }
        //}

        private static readonly Dictionary<BuildTarget, BuildTargetGroup> SupportedPlatforms = new Dictionary<BuildTarget, BuildTargetGroup>
        {
            // The first one is special, it's the Editor platform which is sperate from Editor vs Player configuration
            //TODO { BuildTarget.NoTarget, BuildTargetGroup.Unknown },
            { BuildTarget.StandaloneWindows, BuildTargetGroup.Standalone },
            { BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone },
            { BuildTarget.iOS, BuildTargetGroup.iOS },
            { BuildTarget.Android, BuildTargetGroup.Android },
            { BuildTarget.WSAPlayer, BuildTargetGroup.WSA },
        };

        public static CompilationSettings Instance { get; } = new CompilationSettings();

        private readonly Dictionary<BuildTarget, CompilationPlatform> compilationPlatforms = new Dictionary<BuildTarget, CompilationPlatform>();

        private HashSet<string> commonDefines;
        private HashSet<string> developmentBuildAdditionalDefines;
        private HashSet<string> editorAdditionalDefines;

        private HashSet<string> nonPlayerDefines;

        private HashSet<string> commonReferences;
        private HashSet<string> developmentBuildAdditionalReferences;
        private HashSet<string> editorAdditionalReferences;

        private HashSet<string> nonPlayerReferences;

        /// <summary>
        /// These defines are common accross all platforms, and exclude development and editor builds
        /// </summary>
        public IReadOnlyList<string> CommonDefines { get; }

        /// <summary>
        /// These defines are common accross all platforms for the development builds.
        /// </summary>
        public IReadOnlyList<string> DevelopmentBuildAdditionalDefines { get; }

        /// <summary>
        /// These defines are common accross all platforms for the editor builds
        /// </summary>
        public IReadOnlyList<string> EditorBuildAdditionalDefines { get; }

        /// <summary>
        /// These defines are common accross all platforms, and exclude development and editor builds
        /// </summary>
        public IReadOnlyList<string> CommonReferences { get; }

        /// <summary>
        /// These defines are common accross all platforms for the development builds.
        /// </summary>
        public IReadOnlyList<string> DevelopmentBuildAdditionalReferences { get; }

        /// <summary>
        /// These defines are common accross all platforms for the editor builds
        /// </summary>
        public IReadOnlyList<string> EditorBuildAdditionalReferences { get; }

        public IReadOnlyDictionary<BuildTarget, CompilationPlatform> AvailablePlatforms { get; }

        private CompilationSettings()
        {
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

            CommonDefines = new ReadOnlyCollection<string>(commonDefines.ToList());
            DevelopmentBuildAdditionalDefines = new ReadOnlyCollection<string>(developmentBuildAdditionalDefines.ToList());
            EditorBuildAdditionalDefines = new ReadOnlyCollection<string>(editorAdditionalDefines.ToList());

            ProcessCommonReferences(builder);

            CommonReferences = new ReadOnlyCollection<string>(commonReferences.ToList());
            DevelopmentBuildAdditionalReferences = new ReadOnlyCollection<string>(developmentBuildAdditionalReferences.ToList());
            EditorBuildAdditionalReferences = new ReadOnlyCollection<string>(editorAdditionalReferences.ToList());

            // Parse data for compilation platforms
            CreateCompilationPlatforms(builder);
        }

        private void ProcessCommonReferences(AssemblyBuilder builder)
        {
            // Assume we have common
            commonReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));

            // Set development build flag and try to get the dev references, filter out common to common,
            builder.flags = AssemblyBuilderFlags.DevelopmentBuild;
            developmentBuildAdditionalReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
            commonReferences.RemoveWhere(t => !developmentBuildAdditionalReferences.Contains(t));

            // Set editor flag, and get editor references. Filter out commont to common
            builder.flags = AssemblyBuilderFlags.EditorAssembly;
            editorAdditionalReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
            commonReferences.RemoveWhere(t => !editorAdditionalReferences.Contains(t));

            // Reset
            builder.flags = AssemblyBuilderFlags.None;

            // Go through each platform weeding out common for each of the three
            foreach (KeyValuePair<BuildTarget, BuildTargetGroup> platformPair in SupportedPlatforms)
            {
                builder.buildTarget = platformPair.Key;
                builder.buildTargetGroup = platformPair.Value;

                builder.flags = AssemblyBuilderFlags.DevelopmentBuild;
                HashSet<string> other = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
                commonReferences.RemoveWhere(t => !other.Contains(t));
                developmentBuildAdditionalReferences.RemoveWhere(t => !other.Contains(t)); // get only the common

                builder.flags = AssemblyBuilderFlags.EditorAssembly;
                other = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
                commonReferences.RemoveWhere(t => !other.Contains(t));
                editorAdditionalReferences.RemoveWhere(t => !other.Contains(t)); // get only the common

                // Reset
                builder.flags = AssemblyBuilderFlags.None;
            }

            // Remove common from dev/editor
            developmentBuildAdditionalReferences.RemoveWhere(commonReferences.Contains);
            editorAdditionalReferences.RemoveWhere(commonReferences.Contains);

            // Reset
            builder.buildTarget = BuildTarget.NoTarget;
            builder.buildTargetGroup = BuildTargetGroup.Unknown;

            nonPlayerReferences = new HashSet<string>(commonReferences.Concat(editorAdditionalReferences).Concat(developmentBuildAdditionalReferences));
        }

        private void ProcessCommonDefines(AssemblyBuilder builder)
        {
            // Assume we have common
            commonDefines = new HashSet<string>(builder.defaultDefines);

            // Set development build flag and try to get the dev defines, filter out common to common,
            builder.flags = AssemblyBuilderFlags.DevelopmentBuild;
            developmentBuildAdditionalDefines = new HashSet<string>(builder.defaultDefines);
            commonDefines.RemoveWhere(t => !developmentBuildAdditionalDefines.Contains(t));

            // Set editor flag, and get editor defines. Filter out commont to common
            builder.flags = AssemblyBuilderFlags.EditorAssembly;
            editorAdditionalDefines = new HashSet<string>(builder.defaultDefines);
            commonDefines.RemoveWhere(t => !editorAdditionalDefines.Contains(t));

            // Reset
            builder.flags = AssemblyBuilderFlags.None;

            // Go through each platform weeding out common for each of the three
            foreach (KeyValuePair<BuildTarget, BuildTargetGroup> platformPair in SupportedPlatforms)
            {
                if (!Utilities.IsPlatformInstalled(platformPair.Key))
                {
                    Debug.LogError($"The platform '{platformPair.Key}' is not installed, it will not be supported in the MSBuild project.");
                    continue;
                }

                builder.buildTarget = platformPair.Key;
                builder.buildTargetGroup = platformPair.Value;

                builder.flags = AssemblyBuilderFlags.DevelopmentBuild;
                HashSet<string> other = new HashSet<string>(builder.defaultDefines);
                commonDefines.RemoveWhere(t => !other.Contains(t));
                developmentBuildAdditionalDefines.RemoveWhere(t => !other.Contains(t)); // get only the common

                builder.flags = AssemblyBuilderFlags.EditorAssembly;
                other = new HashSet<string>(builder.defaultDefines);
                commonDefines.RemoveWhere(t => !other.Contains(t));
                editorAdditionalDefines.RemoveWhere(t => !other.Contains(t)); // get only the common

                // Reset
                builder.flags = AssemblyBuilderFlags.None;
            }

            // Remove common from dev/editor
            developmentBuildAdditionalDefines.RemoveWhere(commonDefines.Contains);
            editorAdditionalDefines.RemoveWhere(commonDefines.Contains);

            // Reset
            builder.buildTarget = BuildTarget.NoTarget;
            builder.buildTargetGroup = BuildTargetGroup.Unknown;

            nonPlayerDefines = new HashSet<string>(commonDefines.Concat(editorAdditionalDefines).Concat(developmentBuildAdditionalDefines));
        }

        private void CreateCompilationPlatforms(AssemblyBuilder builder)
        {
            // Now go through and get defines for each platform
            foreach (KeyValuePair<BuildTarget, BuildTargetGroup> platformPair in SupportedPlatforms)
            {
                builder.buildTarget = platformPair.Key;
                builder.buildTargetGroup = platformPair.Value;
                HashSet<string> platformCommonDefines = new HashSet<string>(builder.defaultDefines);
                HashSet<string> playerDefines = new HashSet<string>(builder.defaultDefines);

                HashSet<string> platformCommonReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));
                HashSet<string> playerReferences = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));


                builder.flags = AssemblyBuilderFlags.EditorAssembly;
                HashSet<string> editorDefines = new HashSet<string>(builder.defaultDefines);
                HashSet<string> editorRefernces = new HashSet<string>(FilterOutProjectReferences(builder.defaultReferences));

                // Remove the non player ones
                platformCommonDefines.RemoveWhere(t => nonPlayerDefines.Contains(t));
                playerDefines.RemoveWhere(t => nonPlayerDefines.Contains(t));
                editorDefines.RemoveWhere(t => nonPlayerDefines.Contains(t));

                platformCommonReferences.RemoveWhere(t => nonPlayerReferences.Contains(t));
                playerReferences.RemoveWhere(t => nonPlayerReferences.Contains(t));
                editorRefernces.RemoveWhere(t => nonPlayerReferences.Contains(t));

                // Get common
                platformCommonDefines.RemoveWhere(t => !editorDefines.Contains(t));

                platformCommonReferences.RemoveWhere(t => !editorRefernces.Contains(t));

                // Get specialized
                playerDefines.RemoveWhere(t => platformCommonDefines.Contains(t));
                editorDefines.RemoveWhere(t => platformCommonDefines.Contains(t));

                playerReferences.RemoveWhere(t => platformCommonReferences.Contains(t));
                editorRefernces.RemoveWhere(t => platformCommonReferences.Contains(t));

                CompilationPlatform compilationPlatform = new CompilationPlatform(platformPair.Key, platformPair.Value, GetTargetFramework(platformPair),
                    platformCommonDefines.ToList(), playerDefines.ToList(), editorDefines.ToList(),
                    platformCommonReferences.ToList(), playerReferences.ToList(), editorRefernces.ToList());

                compilationPlatforms.Add(platformPair.Key, compilationPlatform);
            }
        }

        private TargetFramework GetTargetFramework(KeyValuePair<BuildTarget, BuildTargetGroup> platformPair)
        {
            switch (PlayerSettings.GetApiCompatibilityLevel(platformPair.Value))
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
