using Assets.MRTK.Tools.Scripts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Compilation
{
    private const string CSharpVersion = "7.3";

    private const string SolutionTemplate = "Assets/MRTK/Tools/SolutionTemplate.sln"; //TODO this won't work, as it's for my symlinked MRTK only
    private const string SDKProjectTemplate = "Assets/MRTK/Tools/SDKProjectTemplate.csproj"; //TODO this won't work, as it's for my symlinked MRTK only
    private const string PropsFileTemplate = "Assets/MRTK/Tools/PropsFileTemplate.props"; //TODO this won't work, as it's for my symlinked MRTK only

    [MenuItem("Assets/Compile Binaries")]
    public static void ProduceCompiledBinaries()
    {
        // We create a solution file
        // We create a props file linking in all the appropriate Unity DLLs paths, and setting up all the common things for all of the proj files
        // We create a SDK style csproj for each asmdef
        // We build using dotnet

        MakePackagesCopy();

        string path = Application.dataPath.Replace("Assets", "MSBuild");
        try
        {
            Utilities.EnsureCleanDirectory(path);
        }
        catch (IOException ex)
        {
            if (ex.Message.Contains(@".vs\MRTK\v15\Server\sqlite3\db.lock"))
            {
                Debug.LogError("Generated project appears to be still open with Visual Studio.");
                throw new InvalidDataException("Generated project appears to be still open with Visual Studio.", ex);
            }
            else
            {
                throw;
            }
        }

        string commonPropsFilePath = CreateCommonPropsFile();
        UnityProjectInfo unityProjectInfo = UnityProjectInfo.Instance;

        //// Read the solution template
        string solutionTemplateText = File.ReadAllText(Utilities.UnityFolderRelativeToAbsolutePath(SolutionTemplate));

        //// Read the project template
        string projectTemplateText = File.ReadAllText(Utilities.UnityFolderRelativeToAbsolutePath(SDKProjectTemplate));

        unityProjectInfo.ExportSolution(solutionTemplateText, projectTemplateText, commonPropsFilePath);

        Debug.Log("Completed.");
    }

    private static void MakePackagesCopy()
    {
        string packageCache = Path.Combine(Application.dataPath, "..", "Library/PackageCache");
        string[] directories = Directory.GetDirectories(packageCache);

        string outputDirectory = Path.Combine(Application.dataPath, "..", Utilities.PackagesCopy);
        Utilities.EnsureCleanDirectory(outputDirectory);

        foreach (string directory in directories)
        {
            Utilities.CopyDirectory(directory, Path.Combine(outputDirectory, Path.GetFileName(directory).Split('@')[0]));
        }
    }

    private static string CreateCommonPropsFile()
    {
        string templateText = File.ReadAllText(Utilities.UnityFolderRelativeToAbsolutePath(PropsFileTemplate));
        string propsFilePath = Path.Combine(Application.dataPath.Replace("Assets", "MSBuild"), "MRTK.Common.props");

        if (File.Exists(propsFilePath))
        {
            File.Delete(propsFilePath);
        }

        Dictionary<string, string> tokensToReplace = new Dictionary<string, string>()
        {
            {"<!--LANGUAGE_VERSION-->", CSharpVersion },
            {"<!--DEVELOPMENT_BUILD-->", "false" }, // Default to false
            {"<!--OUTPUT_PATH_TOKEN-->", Path.Combine("..", "MRTKBuild") },
            {"<!--COMMON_DEFINE_CONSTANTS-->", string.Join(";", CompilationSettings.Instance.CommonDefines) },
            {"<!--COMMON_DEVELOPMENT_DEFINE_CONSTANTS-->", string.Join(";", CompilationSettings.Instance.DevelopmentBuildAdditionalDefines) },
            {"<!--COMMON_INEDITOR_DEFINE_CONSTANTS-->", string.Join(";", CompilationSettings.Instance.InEditorBuildAdditionalDefines) },
        };

        ProcessReferences(BuildTarget.NoTarget, CompilationSettings.Instance.CommonReferences, out HashSet<string> commonAssemblySearchPaths, out HashSet<string> commonAssemblyReferences);
        ProcessReferences(BuildTarget.NoTarget, CompilationSettings.Instance.DevelopmentBuildAdditionalReferences, out HashSet<string> developmentAssemblySearchPaths, out HashSet<string> developmentAssemblyReferences, commonAssemblySearchPaths);
        ProcessReferences(BuildTarget.NoTarget, CompilationSettings.Instance.InEditorBuildAdditionalReferences, out HashSet<string> inEditorAssemblySearchPaths, out HashSet<string> inEditorAssemblyReferences, commonAssemblySearchPaths, developmentAssemblySearchPaths);

        if (Utilities.TryGetXMLTemplate(templateText, "PLATFORM", out string platformTemplate)
            && Utilities.TryGetXMLTemplate(platformTemplate, "PLATFORM_COMMON_REFERENCE", out string platformCommonReferenceTemplate)
            && Utilities.TryGetXMLTemplate(platformTemplate, "PLATFORM_INEDITOR_REFERENCE", out string platformInEditorReferenceTemplate)
            && Utilities.TryGetXMLTemplate(platformTemplate, "PLATFORM_PLAYER_REFERENCE", out string platformPlayerReferenceTemplate))
        {
            List<string> platformConfigurations = new List<string>();

            foreach (KeyValuePair<BuildTarget, CompilationSettings.CompilationPlatform> pair in CompilationSettings.Instance.AvailablePlatforms)
            {
                ProcessReferences(pair.Key, pair.Value.CommonPlatformReferences, out HashSet<string> platformCommonAssemblySearchPaths, out HashSet<string> platformCommonAssemblyReferences, commonAssemblySearchPaths);
                ProcessReferences(pair.Key, pair.Value.AdditionalInEditorReferences, out HashSet<string> platformInEditorAssemblySearchPaths, out HashSet<string> platformInEditorAssemblyReferences, platformCommonAssemblySearchPaths, commonAssemblySearchPaths, inEditorAssemblySearchPaths);
                ProcessReferences(pair.Key, pair.Value.AdditionalPlayerReferences, out HashSet<string> platformPlayerAssemblySearchPaths, out HashSet<string> platformPlayerAssemblyReferences, platformCommonAssemblySearchPaths, commonAssemblySearchPaths);

                Dictionary<string, string> platformTokens = new Dictionary<string, string>()
                {
                    {"##PLATFORM_TOKEN##", pair.Value.BuildTarget.ToString() },
                    {"<!--TARGET_FRAMEWORK_TOKEN-->", pair.Value.TargetFramework.AsMSBuildString() },

                    {"<!--PLATFORM_COMMON_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", platformCommonAssemblySearchPaths)},
                    {"<!--PLATFORM_INEDITOR_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", platformInEditorAssemblySearchPaths)},
                    {"<!--PLATFORM_PLAYER_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", platformPlayerAssemblySearchPaths)},

                    {"<!--PLATFORM_COMMON_DEFINE_CONSTANTS-->", string.Join(";", pair.Value.CommonPlatformDefines) },
                    {"<!--PLATFORM_INEDITOR_DEFINE_CONSTANTS-->", string.Join(";", pair.Value.AdditionalInEditorDefines) },
                    {"<!--PLATFORM_PLAYER_DEFINE_CONSTANTS-->", string.Join(";", pair.Value.AdditionalPlayerDefines) },
                };

                platformTokens.Add(platformCommonReferenceTemplate, string.Join("\r\n", platformCommonAssemblyReferences.Select(t => platformCommonReferenceTemplate.Replace("##REFERENCE_TOKEN##", t))));
                platformTokens.Add(platformInEditorReferenceTemplate, string.Join("\r\n", platformInEditorAssemblyReferences.Select(t => platformInEditorReferenceTemplate.Replace("##REFERENCE_TOKEN##", t))));
                platformTokens.Add(platformPlayerReferenceTemplate, string.Join("\r\n", platformPlayerAssemblyReferences.Select(t => platformPlayerReferenceTemplate.Replace("##REFERENCE_TOKEN##", t))));

                string filledData = Utilities.ReplaceTokens(platformTemplate, platformTokens);
                platformConfigurations.Add(filledData);
            }

            tokensToReplace.Add(platformTemplate, string.Join("\r\n", platformConfigurations));
        }
        else
        {
            Debug.LogError($"Failed to get the correct platform configuration template from {PropsFileTemplate} with references");
        }

        if (Utilities.TryGetXMLTemplate(templateText, "COMMON_REFERENCE", out string commonReferenceTemplate)
            && Utilities.TryGetXMLTemplate(templateText, "DEVELOPMENT_REFERENCE", out string developmentReferenceTemplate)
            && Utilities.TryGetXMLTemplate(templateText, "INEDITOR_REFERENCE", out string inEditorReferenceTemplate))
        {
            tokensToReplace.Add("<!--COMMON_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", commonAssemblySearchPaths));
            tokensToReplace.Add("<!--DEVELOPMENT_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", developmentAssemblySearchPaths));
            tokensToReplace.Add("<!--INEDITOR_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", inEditorAssemblySearchPaths));

            tokensToReplace.Add(commonReferenceTemplate, string.Join("\r\n", commonAssemblyReferences.Select(t => commonReferenceTemplate.Replace("##REFERENCE_TOKEN##", t))));
            tokensToReplace.Add(developmentReferenceTemplate, string.Join("\r\n", developmentAssemblyReferences.Select(t => developmentReferenceTemplate.Replace("##REFERENCE_TOKEN##", t))));
            tokensToReplace.Add(inEditorReferenceTemplate, string.Join("\r\n", inEditorAssemblyReferences.Select(t => inEditorReferenceTemplate.Replace("##REFERENCE_TOKEN##", t))));
        }
        else
        {
            Debug.LogError($"Failed to get the correct default references template from {PropsFileTemplate} with references");
        }

        // Replace tokens
        templateText = Utilities.ReplaceTokens(templateText, tokensToReplace);

        File.WriteAllText(propsFilePath, templateText);
        return propsFilePath;
    }

    private static void ProcessReferences(BuildTarget buildTarget, IEnumerable<string> references, out HashSet<string> searchPaths, out HashSet<string> referenceNames, params HashSet<string>[] priorToCheck)
    {
        searchPaths = new HashSet<string>();
        referenceNames = new HashSet<string>();

        foreach (string reference in references)
        {
            string directory = Path.GetDirectoryName(reference);
            string fileName = Path.GetFileName(reference);
            if (!priorToCheck.Any(t => t.Contains(directory))) // Don't add duplicates
            {
                searchPaths.Add(directory);
            }

            if (!referenceNames.Add(fileName))
            {
                Debug.LogError($"Duplicate assembly reference found for platform '{buildTarget}' - {reference} ignoring.");
            }
        }
    }
}
