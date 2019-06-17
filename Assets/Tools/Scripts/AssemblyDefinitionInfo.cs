using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public class AssemblyDefinitionInfo
    {
        public static AssemblyDefinitionInfo Parse(string assetGuid)
        {
            Guid guid = Guid.Parse(assetGuid);

            string relativePath = AssetDatabase.GUIDToAssetPath(assetGuid);
            string asmdefContents = File.ReadAllText(Utilities.UnityFolderRelativeToAbsolutePath(relativePath));

            AssemblyDefinitionInfo assemblyDefinitionInfo = JsonUtility.FromJson<AssemblyDefinitionInfo>(asmdefContents);
            assemblyDefinitionInfo.Guid = guid;
            assemblyDefinitionInfo.AssetsRelativePath = relativePath;

            return assemblyDefinitionInfo;
        }

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
    }
}
