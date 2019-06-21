#if UNITY_EDITOR
using Assets.MRTK.Tools.Scripts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Compilation
{
    private const string CSharpVersion = "7.3";
    private const string UWPMinPlatformVersion = "10.0.14393.0";
    private const string UWPTargetPlatformVersion = "10.0.18362.0";

    private const string TemplateFolderPath = "Assets/MRTK/Tools";
    private const string SolutionTemplate = "Assets/MRTK/Tools/SolutionTemplate.sln"; //TODO this won't work, as it's for my symlinked MRTK only
    private const string SDKProjectTemplate = "Assets/MRTK/Tools/SDKProjectTemplate.csproj"; //TODO this won't work, as it's for my symlinked MRTK only

    private const string PropTemplateFilePath = "PropsFileTemplate.props"; //TODO this won't work, as it's for my symlinked MRTK only
    private const string PlatformCommonTemplateFileName = "Platform.Configuration.Template.props"; //TODO this won't work, as it's for my symlinked MRTK only

    public const string CommonPropsFileName = "MRTK.Common.props";

    public static string GetPlatformCommonPropsFileName(CompilationSettings.CompilationPlatform platform, string configuration)
    {
        return $"{platform.Name}.{configuration}.props";
    }

    [MenuItem("Assets/Compile Binaries")]
    public static void ProduceCompiledBinaries()
    {
        // We create a solution file
        // We create a props file linking in all the appropriate Unity DLLs paths, and setting up all the common things for all of the proj files
        // We create a SDK style csproj for each asmdef
        // We build using dotnet

        MakePackagesCopy();

        Utilities.EnsureCleanDirectory(Application.dataPath.Replace("Assets", "MRTKBuild"));

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

        string propsOutputFolder = Application.dataPath.Replace("Assets", "MSBuild");
        CreateCommonPropsFile(propsOutputFolder);
        UnityProjectInfo unityProjectInfo = UnityProjectInfo.CreateProjectInfo();

        //// Read the solution template
        string solutionTemplateText = File.ReadAllText(Utilities.UnityFolderRelativeToAbsolutePath(SolutionTemplate));

        //// Read the project template
        string projectTemplateText = File.ReadAllText(Utilities.UnityFolderRelativeToAbsolutePath(SDKProjectTemplate));

        unityProjectInfo.ExportSolution(solutionTemplateText, projectTemplateText, propsOutputFolder);

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

    private static string CreateCommonPropsFile(string propsOutputFolder)
    {
        string templateText = File.ReadAllText(Utilities.UnityFolderRelativeToAbsolutePath(Path.Combine(TemplateFolderPath, PropTemplateFilePath)));
        string propsFilePath = Path.Combine(propsOutputFolder, CommonPropsFileName);

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
            {"<!--SUPPORTED_PLATFORMS_TOKEN-->", string.Join(";", CompilationSettings.Instance.AvailablePlatforms.Select(t=>t.Value.Name)) },
            {"<!--DEFAULT_PLATFORM_TOKEN-->", CompilationSettings.Instance.AvailablePlatforms[BuildTarget.StandaloneWindows].Name }
        };

        ProcessReferences(BuildTarget.NoTarget, CompilationSettings.Instance.CommonReferences, out HashSet<string> commonAssemblySearchPaths, out HashSet<string> commonAssemblyReferences);
        ProcessReferences(BuildTarget.NoTarget, CompilationSettings.Instance.DevelopmentBuildAdditionalReferences, out HashSet<string> developmentAssemblySearchPaths, out HashSet<string> developmentAssemblyReferences, commonAssemblySearchPaths);
        ProcessReferences(BuildTarget.NoTarget, CompilationSettings.Instance.InEditorBuildAdditionalReferences, out HashSet<string> inEditorAssemblySearchPaths, out HashSet<string> inEditorAssemblyReferences, commonAssemblySearchPaths, developmentAssemblySearchPaths);

        foreach (CompilationSettings.CompilationPlatform platform in CompilationSettings.Instance.AvailablePlatforms.Values)
        {
            // Check for specialized template, otherwise get the common one
            ProcessPlatformTemplateForConfiguration(platform, propsOutputFolder, true, commonAssemblySearchPaths, inEditorAssemblySearchPaths);
            ProcessPlatformTemplateForConfiguration(platform, propsOutputFolder, false, commonAssemblySearchPaths, inEditorAssemblySearchPaths);
        }

        if (Utilities.TryGetXMLTemplate(templateText, "COMMON_REFERENCE", out string commonReferenceTemplate)
            && Utilities.TryGetXMLTemplate(templateText, "DEVELOPMENT_REFERENCE", out string developmentReferenceTemplate)
            && Utilities.TryGetXMLTemplate(templateText, "INEDITOR_REFERENCE", out string inEditorReferenceTemplate))
        {
            tokensToReplace.Add("<!--COMMON_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", commonAssemblySearchPaths));
            tokensToReplace.Add("<!--DEVELOPMENT_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", developmentAssemblySearchPaths));
            tokensToReplace.Add("<!--INEDITOR_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", inEditorAssemblySearchPaths));

            tokensToReplace.Add(commonReferenceTemplate, string.Join("\r\n", GetReferenceEntries(commonReferenceTemplate, commonAssemblyReferences)));
            tokensToReplace.Add(developmentReferenceTemplate, string.Join("\r\n", GetReferenceEntries(developmentReferenceTemplate, developmentAssemblyReferences)));
            tokensToReplace.Add(inEditorReferenceTemplate, string.Join("\r\n", GetReferenceEntries(inEditorReferenceTemplate, inEditorAssemblyReferences)));
        }
        else
        {
            Debug.LogError($"Failed to get the correct default references template from {PropTemplateFilePath} with references");
        }

        // Replace tokens
        templateText = Utilities.ReplaceTokens(templateText, tokensToReplace);

        File.WriteAllText(propsFilePath, templateText);
        return propsFilePath;
    }

    private static void ProcessPlatformTemplateForConfiguration(CompilationSettings.CompilationPlatform platform, string propsOutputFolder, bool inEditorConfiguration, HashSet<string> commonAssemblySearchPaths, HashSet<string> inEditorAssemblySearchPaths)
    {
        string configuration = inEditorConfiguration ? "InEditor" : "Player";

        string platformCommonTemplateFilePath = Path.Combine(TemplateFolderPath, PlatformCommonTemplateFileName);
        string platformSpecificTemplateFilePath = Path.Combine(TemplateFolderPath, PlatformCommonTemplateFileName.Replace("Platform", platform.Name));
        string platformConfigSpecificTemplateFilePath = Path.Combine(TemplateFolderPath, PlatformCommonTemplateFileName.Replace("Platform", platform.Name).Replace("Configuration", configuration));

        string platformTemplate;
        if (File.Exists(platformConfigSpecificTemplateFilePath))
        {
            platformTemplate = File.ReadAllText(platformConfigSpecificTemplateFilePath);
        }
        else if (File.Exists(platformSpecificTemplateFilePath))
        {
            platformTemplate = File.ReadAllText(platformSpecificTemplateFilePath);
        }
        else
        {
            platformTemplate = File.ReadAllText(platformCommonTemplateFilePath); ;
        }

        string platformPropsText;
        if (inEditorConfiguration)
        {
            platformPropsText = ProcessPlatformTemplate(platformTemplate, platform.Name, configuration, platform.AssemblyDefinitionPlatform.BuildTarget, platform.TargetFramework,
                platform.CommonPlatformReferences.Concat(platform.AdditionalInEditorReferences), platform.CommonPlatformDefines.Concat(platform.AdditionalInEditorDefines),
                commonAssemblySearchPaths, inEditorAssemblySearchPaths);
        }
        else
        {
            platformPropsText = ProcessPlatformTemplate(platformTemplate, platform.Name, configuration, platform.AssemblyDefinitionPlatform.BuildTarget, platform.TargetFramework,
                platform.CommonPlatformReferences.Concat(platform.AdditionalPlayerReferences), platform.CommonPlatformDefines.Concat(platform.AdditionalPlayerDefines),
                commonAssemblySearchPaths);
        }

        File.WriteAllText(Path.Combine(propsOutputFolder, GetPlatformCommonPropsFileName(platform, configuration)), platformPropsText);
    }

    private static string ProcessPlatformTemplate(string platformTemplate, string platformName, string configuration, BuildTarget buildTarget, TargetFramework targetFramework, IEnumerable<string> references, IEnumerable<string> defines, params HashSet<string>[] priorToCheck)
    {
        if (Utilities.TryGetXMLTemplate(platformTemplate, "PLATFORM_COMMON_REFERENCE", out string platformCommonReferenceTemplate))
        {
            ProcessReferences(buildTarget, references, out HashSet<string> platformAssemblySearchPaths, out HashSet<string> platformAssemblyReferencePaths, priorToCheck);

            Dictionary<string, string> platformTokens = new Dictionary<string, string>()
            {
                {"<!--TARGET_FRAMEWORK_TOKEN-->", targetFramework.AsMSBuildString() },
                {"<!--PLATFORM_COMMON_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", platformAssemblySearchPaths)},
                {"<!--PLATFORM_COMMON_DEFINE_CONSTANTS-->", string.Join(";",   defines) },

                // These are UWP specific, but they will be no-op if not needed
                { "<!--UWP_TARGET_PLATFORM_VERSION_TOKEN-->", UWPTargetPlatformVersion },
                { "<!--UWP_MIN_PLATFORM_VERSION_TOKEN-->", UWPMinPlatformVersion }
            };

            platformTokens.Add(platformCommonReferenceTemplate, string.Join("\r\n", GetReferenceEntries(platformCommonReferenceTemplate, platformAssemblyReferencePaths)));

            return Utilities.ReplaceTokens(platformTemplate, platformTokens);
        }
        else
        {
            Debug.LogError($"Invalid platform template format for '{platformName}' with configuration '{configuration}'");
            return platformTemplate;
        }
    }

    private static IEnumerable<string> GetReferenceEntries(string template, IEnumerable<string> references)
    {
        return references.Select(t => Utilities.ReplaceTokens(template, new Dictionary<string, string>()
        {
            { "##REFERENCE_TOKEN##", Path.GetFileNameWithoutExtension(t) },
            { "<!--REFERENCE_HINT_PATH_TOKEN-->", t }
        }));
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

            if (!referenceNames.Add(reference))
            //if (!referenceNames.Add(fileName))
            {
                Debug.LogError($"Duplicate assembly reference found for platform '{buildTarget}' - {reference} ignoring.");
            }
        }
    }
}
#endif