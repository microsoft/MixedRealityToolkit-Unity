// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// This class represents an AssemblyDefinition file of a Unity project. It can be used to parse the file contents using <see href="https://docs.unity3d.com/ScriptReference/JsonUtility.FromJson.html">JsonUtility.FromJson</see>.
    /// </summary>
    public class AssemblyDefinitionInfo
    {
        public const string EditorPlatform = "Editor";
        public const string TestAssembliesReference = "TestAssemblies";

        /// <summary>
        /// Creates an instance of <see cref="AssemblyDefinitionInfo"/> for the default projects (such as Assembly-CSharp)
        /// </summary>
        /// <param name="assembly">The Unity assembly reference.</param>
        /// <returns>A new instance.</returns>
        public static AssemblyDefinitionInfo GetDefaultAssemblyCSharpInfo(Assembly assembly)
        {
            AssemblyDefinitionInfo toReturn = new AssemblyDefinitionInfo() { IsDefaultAssembly = true, Guid = Guid.NewGuid(), Directory = new DirectoryInfo(Utilities.AssetPath) };
            toReturn.assembly = assembly;
            toReturn.name = assembly.name;
            toReturn.references = assembly.assemblyReferences.Select(t => t.name).ToArray();
            toReturn.PrecompiledAssemblyReferences = new HashSet<string>();
            return toReturn;
        }

        /// <summary>
        /// Parses an asmdef file creating a new instance of <see cref="AssemblyDefinitionInfo"/>.
        /// </summary>
        /// <param name="file">The file representing asmdef.</param>
        /// <param name="unityProjectInfo">Instance of <see cref="UnityProjectInfo"/>.</param>
        /// <param name="assembly">The Unity assembly reference.</param>
        /// <param name="isBuiltInPackage">True whether this asmdef lives in the editor installation folder.</param>
        public static AssemblyDefinitionInfo Parse(FileInfo file, UnityProjectInfo unityProjectInfo, Assembly assembly, bool isBuiltInPackage = false)
        {
            if (file.Extension != ".asmdef")
            {
                throw new ArgumentException($"Given file '{file.FullName}' is not an assembly definition file.");
            }
            else if (!file.Exists)
            {
                throw new ArgumentException($"Given file '{file.FullName}' does not exist.");
            }


            AssemblyDefinitionInfo toReturn = JsonUtility.FromJson<AssemblyDefinitionInfo>(File.ReadAllText(file.FullName));

            if (!Utilities.TryGetGuidForAsset(file, out Guid guid))
            {
                Debug.LogError($"Failed to parse AsmDef meta for asm def: '{file.FullName}', didn't find guid.");
            }

            toReturn.assembly = assembly;
            toReturn.Directory = file.Directory;
            toReturn.file = file;
            toReturn.Guid = guid;
            toReturn.BuiltInPackage = isBuiltInPackage;
            toReturn.Validate(unityProjectInfo.AvailablePlatforms);
            toReturn.PrecompiledAssemblyReferences = new HashSet<string>(toReturn.precompiledReferences?.Select(t => t.Replace(".dll", string.Empty)) ?? Array.Empty<string>());
            return toReturn;
        }

        private Assembly assembly;
        private SourceFileInfo[] sourceFiles;
        private FileInfo file;

        // JSON Values
        [SerializeField]
        private string name = null;
        [SerializeField]
        private string[] references = null;
        public string[] optionalUnityReferences;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        [SerializeField]
        private string[] precompiledReferences = null;
        public bool autoReferenced;
        public string[] defineConstraints;

        /// <summary>
        /// Gets or sets the parsed Guid of the AssemblyDefinition asset.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets the parent directory of the associated file, or returns the Assets folder if it's a DefaultAssembly.
        /// </summary>
        public DirectoryInfo Directory { get; private set; }

        /// <summary>
        /// Name of this assembly definition info
        /// </summary>
        public string Name => name;

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
        /// Gets whether this is a default assembly definition like Assembly-CSharp
        /// </summary>
        public bool IsDefaultAssembly { get; private set; } = false;

        /// <summary>
        /// Gets whether this represents a built-in package that lives in Editor installation folder.
        /// </summary>
        public bool BuiltInPackage { get; private set; } = false;

        /// <summary>
        /// Gets AsmDef references for this assembly definition.
        /// </summary>
        public string[] References => references ?? (references = Array.Empty<string>());

        /// <summary>
        /// Gets DLL references for this assembly definition.
        /// </summary>
        public HashSet<string> PrecompiledAssemblyReferences { get; private set; }

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

        /// <summary>
        /// Gets the source files for this assembly definition.
        /// </summary>
        /// <returns>The array of <see cref="SourceFileInfo"/>.</returns>
        public SourceFileInfo[] GetSources()
        {
            if (sourceFiles == null)
            {
                List<SourceFileInfo> sourceFiles = new List<SourceFileInfo>();
                if (assembly != null)
                {
                    foreach (string sourceFile in assembly.sourceFiles)
                    {
                        MonoScript asset = AssetDatabase.LoadAssetAtPath<MonoScript>(sourceFile);
                        sourceFiles.Add(SourceFileInfo.Parse(new FileInfo(Utilities.GetFullPathFromKnownRelative(sourceFile)), asset.GetClass()));
                    }
                }
                else
                {
                    SearchForSourceFiles(sourceFiles, file.Directory);
                }
                this.sourceFiles = sourceFiles.ToArray();
            }

            return sourceFiles;
        }

        private void SearchForSourceFiles(List<SourceFileInfo> sourceFiles, DirectoryInfo directory)
        {
            if (directory.GetFiles("*.asmdef", SearchOption.TopDirectoryOnly).Length > 1)
            {
                // Source file is managed by another asmdef
                return;
            }

            foreach (FileInfo fileInfo in directory.GetFiles("*.cs", SearchOption.TopDirectoryOnly))
            {
                sourceFiles.Add(SourceFileInfo.Parse(fileInfo, null));
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                SearchForSourceFiles(sourceFiles, subDirectory);
            }
        }

        /// <summary>
        /// A more readable string representation for assembly definition.
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name}: {Name}";
        }
    }
}
