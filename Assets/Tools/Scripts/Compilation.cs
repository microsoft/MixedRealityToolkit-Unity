using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Compilation
{
    private const string SolutionTemplate = "Assets/MRTK/Tools/SolutionTemplate.sln"; //TODO this won't work, as it's for my symlinked MRTK only
    private const string SDKProjectTemplate = "Assets/MRTK/Tools/SDKProjectTemplate.csproj"; //TODO this won't work, as it's for my symlinked MRTK only
    private const string PropsFileTemplate = "Assets/MRTK/Tools/PropsFileTemplate.props"; //TODO this won't work, as it's for my symlinked MRTK only

    private const string SDKProjectTypeGuid = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";

    private struct AsmDefInfo
    {
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

    private struct CSProjInfo
    {
        public string FilePath;
        public string Name;
        public AsmDefInfo AsmDefInfo;
    }

    [MenuItem("Assets/Compile Binaries")]
    public static void ProduceCompiledBinaries()
    {
        // We create a solution file
        // We create a props file linking in all the appropriate Unity DLLs paths, and setting up all the common things for all of the proj files
        // We create a SDK style csproj for each asmdef
        // We build using dotnet

        string commonPropsFilePath = CreateCommonPropsFile();
        IEnumerable<CSProjInfo> csProjInfos = CreateCSProjFiles(commonPropsFilePath);
        CreateSolutionFile(csProjInfos);
    }

    private static string CreateCommonPropsFile()
    {
        string templateText = File.ReadAllText(AssetsRelativeToAbsolutePath(PropsFileTemplate));
        string propsFilePath = Path.Combine(Application.dataPath, "MRTK.Common.props");

        if (File.Exists(propsFilePath))
        {
            File.Delete(propsFilePath);
        }

        // Replace: <!--OUTPUT_PATH_TOKEN-->
        templateText = templateText.Replace("<!--OUTPUT_PATH_TOKEN-->", Path.Combine("..", "MRTKBuild"));

        string editorInstallFolder = Path.GetDirectoryName(EditorApplication.applicationPath);
        templateText = templateText.Replace("<!--UNITY_EDITOR_INSTALLATION_PATH-->", editorInstallFolder + Path.DirectorySeparatorChar);

        File.WriteAllText(propsFilePath, templateText);
        return propsFilePath;
    }

    private static IEnumerable<CSProjInfo> CreateCSProjFiles(string commonPropsFilePath)
    {
        IEnumerable<AsmDefInfo> asmDefAssets = AssetDatabase.FindAssets("t:asmdef")
            .Select(ProcessAsmDefAsset)
            .Where(t => t.AssetsRelativePath.Contains("/MixedRealityToolkit"));

        Debug.LogWarning(string.Join("\n", asmDefAssets));

        // Read the template
        string templateText = File.ReadAllText(AssetsRelativeToAbsolutePath(SDKProjectTemplate));

        // TODO order files based on folder hiearchy to specify which folders should be excluded
        // This is a map of an asmdef to a list of paths to exclude because of nested asmdefs
        Dictionary<string, HashSet<string>> projectExcludeDirectories = asmDefAssets.ToDictionary(asmDefInfo => Path.GetDirectoryName(asmDefInfo.AssetsRelativePath), t => new HashSet<string>());

        foreach (AsmDefInfo asmDefInfo in asmDefAssets)
        {
            string directoryToExclude = Path.GetDirectoryName(asmDefInfo.AssetsRelativePath);
            string parent = directoryToExclude;
            while (!string.IsNullOrWhiteSpace(parent = Path.GetDirectoryName(parent)))
            {
                if (projectExcludeDirectories.TryGetValue(parent, out HashSet<string> exludeSet))
                {
                    exludeSet.Add(directoryToExclude);
                }
            }
        }

        Dictionary<string, AsmDefInfo> asmDefMap = asmDefAssets.ToDictionary(t => t.name);

        // Write the files
        List<CSProjInfo> projFiles = new List<CSProjInfo>();
        foreach (AsmDefInfo asmDefInfo in asmDefAssets)
        {
            CSProjInfo projFile = CreateSDKProject(asmDefMap, commonPropsFilePath, templateText, asmDefInfo, projectExcludeDirectories[Path.GetDirectoryName(asmDefInfo.AssetsRelativePath)]);
            projFiles.Add(projFile);
        }

        return projFiles;
    }

    private static string AssetsRelativeToAbsolutePath(string path)
    {
        return path.Replace("Assets", Application.dataPath);
    }

    private static AsmDefInfo ProcessAsmDefAsset(string assetGuid)
    {
        if (!Guid.TryParse(assetGuid, out Guid guid))
        {
            throw new InvalidOperationException("Asset guid is not in fact a Guid.");
        }

        string relativePath = AssetDatabase.GUIDToAssetPath(assetGuid);
        string asmdefContents = File.ReadAllText(AssetsRelativeToAbsolutePath(relativePath));

        AsmDefInfo toReturn = JsonUtility.FromJson<AsmDefInfo>(asmdefContents);
        toReturn.Guid = guid;
        toReturn.AssetsRelativePath = relativePath;
        return toReturn;
    }

    private static CSProjInfo CreateSDKProject(Dictionary<string, AsmDefInfo> asmDefMap, string commonPropsFilePath, string templateText, AsmDefInfo asmDefInfo, HashSet<string> excludeDirectories)
    {
        string projFilePath = Path.ChangeExtension(AssetsRelativeToAbsolutePath(asmDefInfo.AssetsRelativePath), ".csproj");

        if (File.Exists(projFilePath))
        {
            File.Delete(projFilePath);
        }

        string projFolder = Path.GetDirectoryName(asmDefInfo.AssetsRelativePath) + Path.DirectorySeparatorChar;
        string excludeFolders = string.Join(";", excludeDirectories.Select(t => t.Replace(projFolder, string.Empty).Replace('/', Path.DirectorySeparatorChar) + $"{Path.DirectorySeparatorChar}**"));

        // TODO convert propsfile path to project relative for portability
        templateText = templateText.Replace("COMMON_PROPS_FILE_PATH", commonPropsFilePath.Replace('/', Path.DirectorySeparatorChar));
        templateText = templateText.Replace("<!--FOLDER_EXCLUDE_TOKEN-->", excludeFolders);

        File.WriteAllText(projFilePath, templateText);

        return new CSProjInfo() { FilePath = projFilePath, AsmDefInfo = asmDefInfo, Name = Path.GetFileNameWithoutExtension(projFilePath) };
    }

    private static void CreateSolutionFile(IEnumerable<CSProjInfo> projectInfos)
    {
        string solutionFilePath = Path.Combine(Application.dataPath, "MRTK.sln");
        // Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UnityAgnostic.Common", ".src\UnityAgnostic.Common\UnityAgnostic.Common.csproj", "{C8FDEFD7-034E-451D-BF66-04079EF4A454}"

        if (File.Exists(solutionFilePath))
        {
            File.Delete(solutionFilePath);
        }

        List<string> projEntries = new List<string>();
        foreach (CSProjInfo proj in projectInfos)
        {
            string projectEntryPath = $"Project(\"{{{SDKProjectTypeGuid}}}\") = \"{proj.Name}\", \"{proj.FilePath.Replace(Application.dataPath.Replace('/', Path.DirectorySeparatorChar), string.Empty)}\", \"{{{proj.AsmDefInfo.Guid}}}\"";
            projEntries.Add(projectEntryPath);
        }

        string solutionFileText = File.ReadAllText(AssetsRelativeToAbsolutePath(SolutionTemplate));
        solutionFileText = solutionFileText.Replace("#PROJECT_ENTRIES_TOKEN", string.Join("\r\n", projEntries));

        File.WriteAllText(solutionFilePath, solutionFileText);
    }
}
