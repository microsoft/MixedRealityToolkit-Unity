#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public class AssemblyDefinitionInfo
    {
        public const string EditorPlatform = "Editor";
        public const string TestAssembliesReference = "TestAssemblies";

        public Guid Guid;
        public string AssetsRelativePath;

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

        public bool EditorPlatformSupported { get; private set; }

        public bool NonEditorPlatformSupported { get; private set; }

        public bool TestAssembly { get; private set; }

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