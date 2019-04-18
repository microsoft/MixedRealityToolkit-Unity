#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    internal class ScriptReferenceRetargettingSettings : ScriptableObject
    {
        public const string k_MyCustomSettingsPath = "Assets/Editor/MyCustomSettings.asset";

        [SerializeField]
        private int m_Number;

        [SerializeField]
        private string m_SomeString;

        internal static ScriptReferenceRetargettingSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ScriptReferenceRetargettingSettings>(k_MyCustomSettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<ScriptReferenceRetargettingSettings>();
                settings.m_Number = 42;
                settings.m_SomeString = "The answer to the universe";
                AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    internal static class ScriptReferenceRetargettingSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/ScriptReferenceRetargetter", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Script Reference Retargetter Settings",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = ScriptReferenceRetargettingSettings.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("m_Number"), new GUIContent("My Number"));
                    EditorGUILayout.PropertyField(settings.FindProperty("m_SomeString"), new GUIContent("My String"));
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Number", "Some String" })
            };

            return provider;
        }
    }


    public static class AssetScriptReferenceRetargeter
    {
        private struct ClassInformation
        {
            public string Name;
            public string Namespace;
            public string Guid;
            public long FileId;
        }

        private const string YamlPrefix = "%YAML 1.1";
        
        private static readonly Dictionary<string, string> sourceToOutputFolders = new Dictionary<string, string>
        {
            {"Library/PlayerDataCache/WindowsStoreApps", "UAP" },
            {"Library/PlayerDataCache/Win", "Standalone" },
        };

        private static readonly HashSet<string> ExcludedYamlAssetExtensions = new HashSet<string> { ".jpg", ".csv", ".meta", ".pfx", ".txt", ".nuspec", ".asmdef", ".yml", ".cs", ".md", ".json", ".ttf", ".png", ".shader", ".wav", ".bin", ".gltf", ".glb", ".fbx", ".FBX", ".pdf", ".cginc" };
        private static readonly HashSet<string> ExcludedSuffixFromCopy = new HashSet<string>() { ".cs", ".cs.meta" };

        private static Dictionary<string, string> nonClassDictionary = new Dictionary<string, string>(); //Guid, FileName

        private const string ScriptFileIdConstant = "11500000";

        [MenuItem("Assets/Retarget To DLL")]
        public static void RetargetAssets()
        {
            try
            {
                RunRetargetToDLL();
                Debug.Log("Complete.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed.");
                Debug.LogException(ex);
            }
        }

        private static void RunRetargetToDLL()
        {
            string[] allFilesUnderAssets = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            //ProcessYAMLAssets(allFilesUnderAssets, Application.dataPath.Replace("Assets", "NuGet/Output"), null);

            Dictionary<string, ClassInformation> scriptFilesReferences = ProcessScripts(allFilesUnderAssets);
            Debug.Log($"Found {scriptFilesReferences.Count} script file references.");

            Dictionary<string, ClassInformation> compiledClassReferences = ProcessCompiledDLLs("PackagedAssemblies", Application.dataPath.Replace("Assets", "NuGet/Plugins/Editor"));
            Debug.Log($"Found {compiledClassReferences.Count} compiled class references.");

            Dictionary<string, Tuple<string, long>> remapDictionary = new Dictionary<string, Tuple<string, long>>();

            foreach (var pair in scriptFilesReferences)
            {
                if (compiledClassReferences.TryGetValue(pair.Key, out ClassInformation compiledClassInfo))
                {
                    remapDictionary.Add(pair.Value.Guid, new Tuple<string, long>(compiledClassInfo.Guid, compiledClassInfo.FileId));
                }
                else
                {
                    // Switch to throwing exception later
                    Debug.LogWarning($"Can't find a compiled version of the script: {pair.Key}; guid: {pair.Value.Guid}");
                }
            }

            ProcessYAMLAssets(allFilesUnderAssets, Application.dataPath.Replace("Assets", "NuGet/Content"), remapDictionary);
        }

        private static void ProcessYAMLAssets(string[] allFilePaths, string outputDirectory, Dictionary<string, Tuple<string, long>> remapDictionary)
        {
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }

            HashSet<string> foundNonYamlExtensions = new HashSet<string>();
            List<Tuple<string, string>> yamlAssets = new List<Tuple<string, string>>();
            foreach (string filePath in allFilePaths)
            {
                string targetPath = filePath.Replace(Application.dataPath, outputDirectory);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

                if (IsYamlFile(filePath))
                {
                    yamlAssets.Add(new Tuple<string, string>(filePath, targetPath));
                }
                else
                {
                    string extension = Path.GetExtension(filePath);
                    if (!ExcludedYamlAssetExtensions.Contains(extension))
                    {
                        foundNonYamlExtensions.Add(extension);
                    }

                    bool copyFile = true;
                    foreach (var suffix in ExcludedSuffixFromCopy)
                    {
                        if (filePath.EndsWith(suffix))
                        {
                            copyFile = false;
                            break;
                        }
                    }

                    if (copyFile)
                    {
                        File.Copy(filePath, targetPath);
                    }
                }
            }

            foreach (var extension in foundNonYamlExtensions)
            {
                Debug.Log($"Not a YAML extension: {extension}");
            }

            var tasks = yamlAssets.Select(t => Task.Run(() => ProcessYamlFile(t.Item1, t.Item2, remapDictionary)));
            Task.WhenAll(tasks).Wait();

            PostProcess(outputDirectory);
        }

        private static async Task ProcessYamlFile(string filePath, string targetPath, Dictionary<string, Tuple<string, long>> remapDictionary)
        {
            using (StreamReader reader = new StreamReader(filePath))
            using (StreamWriter writer = new StreamWriter(targetPath))
            {
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();

                    if (line.Contains("m_Script"))
                    {
                        if (!line.Contains('}'))
                        {
                            // Read the second line as well
                            line += await reader.ReadLineAsync();

                            if (!line.Contains('}'))
                            {
                                throw new InvalidDataException($"Unexpected part of YAML line split over more than two lines, starting two lines: {line}");
                            }
                        }

                        if (line.Contains(ScriptFileIdConstant))
                        {
                            Match regexResults = Regex.Match(line, @"guid:\s*([0-9a-fA-F]*)");
                            if (!regexResults.Success || regexResults.Groups.Count != 2 || !regexResults.Groups[1].Success || regexResults.Groups[1].Captures.Count != 1)
                            {
                                throw new InvalidDataException($"Failed to find the guid in line: {line}.");
                            }

                            string guid = regexResults.Groups[1].Captures[0].Value;
                            if (remapDictionary.TryGetValue(guid, out Tuple<string, long> tuple))
                            {
                                line = $"  m_Script: {{fileID: {tuple.Item2}, guid: {tuple.Item1}, type: 3}}";
                            }
                            else if (nonClassDictionary.ContainsKey(guid))
                            {
                                Debug.LogErrorFormat("A script without a class ({0}) is being processed.", nonClassDictionary[guid]);
                            }
                            else
                            {
                                // Switch to error later
                                Debug.LogWarning($"Couldn't find a script remap for {guid}.");
                            }
                        }
                        // else this is not a script file reference
                    }
                    else if (line.Contains(ScriptFileIdConstant))
                    {
                        throw new InvalidDataException($"Line contains script type but not m_Script: {line}");
                    }
                    //{ fileID: 11500000, guid: 83d9acc7968244a8886f3af591305bcb, type: 3}

                    await writer.WriteLineAsync(line);
                }
            }
        }

        private static bool IsYamlFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            if (ExcludedYamlAssetExtensions.Contains(extension))
            {
                return false;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadLine().StartsWith(YamlPrefix);
            }
        }

        private static Dictionary<string, ClassInformation> ProcessScripts(string[] allFilePaths)
        {
            int lengthOfPrefix = Application.dataPath.IndexOf("Assets");

            Dictionary<string, ClassInformation> toReturn = new Dictionary<string, ClassInformation>();

            foreach (string filePath in allFilePaths)
            {
                if (Path.GetExtension(filePath) == ".cs")
                {
                    MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath.Substring(lengthOfPrefix));
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(monoScript, out string guid, out long fileId))
                    {
                        Type type = monoScript.GetClass();
                        if (type != null)
                        {
                            toReturn.Add(type.FullName, new ClassInformation() { Name = type.Name, Namespace = type.Namespace, FileId = fileId, Guid = guid });
                        }
                        else
                        {
                            nonClassDictionary.Add(guid, Path.GetFileName(filePath));
                            Debug.LogWarning($"Found script that we can't get type from: {monoScript.name}");
                        }
                    }
                }
            }

            return toReturn;
        }

        private static Dictionary<string, ClassInformation> ProcessCompiledDLLs(string temporaryDirectoryName, string outputDirectory)
        {
            Assembly[] dlls = CompilationPipeline.GetAssemblies();

            string tmpDirPath = Path.Combine(Application.dataPath, temporaryDirectoryName);
            if (Directory.Exists(tmpDirPath))
            {
                Directory.Delete(tmpDirPath);
            }

            Directory.CreateDirectory(tmpDirPath);

            try
            {
                foreach (Assembly dll in dlls)
                {
                    if (dll.name.Contains("MixedReality"))
                    {
                        File.Copy(dll.outputPath, Path.Combine(tmpDirPath, $"{dll.name}.dll"), true);
                    }
                }

                // Load these directories
                // TODO maybe we don't need to
                AssetDatabase.Refresh();

                Dictionary<string, ClassInformation> toReturn = new Dictionary<string, ClassInformation>();

                if (Directory.Exists(outputDirectory))
                {
                    Directory.Delete(outputDirectory);
                }

                Directory.CreateDirectory(outputDirectory);
                foreach (Assembly dll in dlls)
                {
                    if (dll.name.Contains("MixedReality"))
                    {
                        File.Copy(Path.Combine(tmpDirPath, $"{dll.name}.dll"), Path.Combine(outputDirectory, $"{dll.name}.dll"));
                        File.Copy(Path.Combine(tmpDirPath, $"{dll.name}.dll.meta"), Path.Combine(outputDirectory, $"{dll.name}.dll.meta"));

                        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(Path.Combine("Assets", temporaryDirectoryName, $"{dll.name}.dll"));

                        foreach (Object asset in assets)
                        {
                            MonoScript monoScript = asset as MonoScript;
                            if (!(monoScript is null) && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(monoScript, out string guid, out long fileId))
                            {
                                Type type = monoScript.GetClass();

                                if (type.Namespace == null || !type.Namespace.Contains("Microsoft.MixedReality.Toolkit"))
                                {
                                    throw new InvalidDataException($"Type {type.Name} is not a member of the Microsoft.MixedReality.Toolkit namespace");
                                }
                                toReturn.Add(type.FullName, new ClassInformation() { Name = type.Name, Namespace = type.Namespace, FileId = fileId, Guid = guid });
                            }
                        }
                    }
                }

                return toReturn;
            }
            finally
            {
                Directory.Delete(tmpDirPath, true);
                AssetDatabase.Refresh();
            }
        }

        private static void PostProcess(string outputPath)
        {
            DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);
            RecursiveFolderCleanup(outputDirectory);
            CopyIntermediaryAssemblies(Application.dataPath.Replace("Assets", "NuGet/Plugins"));
            UpdateMetaFiles();
        }

        private static void CopyIntermediaryAssemblies(string outputPath)
        {
            List<DirectoryInfo> outputDirectories = new List<DirectoryInfo>();
            string playerDataCachePath = Application.dataPath.Replace("Assets", "Library/PlayerDataCache");
            DirectoryInfo playerDataCahce = new DirectoryInfo(playerDataCachePath);
            DirectoryInfo[] dllStores = playerDataCahce.GetDirectories("*", SearchOption.TopDirectoryOnly);
            string subfolderName = string.Empty;
            foreach(KeyValuePair<string, string> sourceToOutputPair in sourceToOutputFolders)
            {
                DirectoryInfo directory = new DirectoryInfo(Application.dataPath.Replace("Assets", sourceToOutputPair.Key));
                if (!directory.Exists)
                {
                    throw new InvalidDataException($"The source directory {sourceToOutputPair.Key} does not exist.");
                }

                subfolderName = sourceToOutputPair.Value;

                string pluginPath = Path.Combine(outputPath, subfolderName);
                if (Directory.Exists(pluginPath))
                {
                    Directory.Delete(pluginPath);
                }
                Directory.CreateDirectory(pluginPath);

                FileInfo[] dlls = directory.GetFiles("Microsoft.MixedReality.Toolkit*.dll", SearchOption.AllDirectories);
                foreach (FileInfo dll in dlls)
                {
                    File.Copy(dll.FullName, Path.Combine(pluginPath, dll.Name), true);
                }
            }
        }

        private static void UpdateMetaFiles()
        {
            int lengthOfPrefix = Application.dataPath.IndexOf("Assets");
            Dictionary<string, string> dllGuids = new Dictionary<string, string>();
            string[] asmdefFiles = Directory.GetFiles(Application.dataPath, "*.asmdef", SearchOption.AllDirectories);

            foreach (string asmdefFile in asmdefFiles)
            {
                string asmdefText = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefFile.Substring(lengthOfPrefix)).text;
                string dllName = JsonUtility.FromJson<AssemblyDefinitionStub>(asmdefText).name;
                string guid = File.ReadAllLines($"{asmdefFile}.meta")[1].Substring(6);
                guid = CycleGuidForward(guid);
                dllGuids.Add($"{dllName}.dll", guid);
            }

            //Load the sample meta files
            DirectoryInfo assetDirectory = new DirectoryInfo(Application.dataPath);
            FileInfo editorMetaFile = assetDirectory.GetFiles("*sampleEditorDllMeta.txt", SearchOption.AllDirectories).FirstOrDefault();
            if (editorMetaFile == null)
            {
                Debug.LogError("Could not find sample editor dll.meta file");
            }
            FileInfo uapMetaFile = assetDirectory.GetFiles("*sampleUAPDllMeta.txt", SearchOption.AllDirectories).FirstOrDefault();
            if (uapMetaFile == null)
            {
                Debug.LogError("Could not find sample UAP dll.meta file");
            }
            FileInfo standaloneMetaFile = assetDirectory.GetFiles("*sampleStandaloneDllMeta.txt", SearchOption.AllDirectories).FirstOrDefault();
            if (standaloneMetaFile == null)
            {
                Debug.LogError("Could not find sample Standalone dll.meta file");
            }

            string[] metaFileContent = File.ReadAllLines(editorMetaFile.FullName);

            foreach (FileInfo dllFile in new DirectoryInfo(Application.dataPath.Replace("Assets", "NuGet/Plugins")).GetFiles("*", SearchOption.AllDirectories))
            {
                if (dllFile.Extension.Equals(".meta"))
                {
                    string dllFileName = dllFile.Name.Remove(dllFile.Name.Length - 5);
                    if (dllGuids.ContainsKey(dllFileName))
                    {
                        metaFileContent[1] = string.Format("guid: {0}", dllGuids[dllFileName]);
                        File.WriteAllLines(dllFile.FullName, metaFileContent);
                    }
                }
            }

            DirectoryInfo[] directories = new DirectoryInfo(Application.dataPath.Replace("Assets", "NuGet/Plugins")).GetDirectories("*", SearchOption.AllDirectories);
            string dllGuid = string.Empty;
            bool isUAP = false;
            foreach (DirectoryInfo directory in directories)
            {
                isUAP = false;
                if (!directory.Name.Equals("Editor"))
                {
                    if (directory.Name.Contains("Standalone") || directory.Parent.Name.Contains("Standalone"))
                    {
                        metaFileContent = File.ReadAllLines(standaloneMetaFile.FullName);
                    }
                    else if (directory.Name.Contains("UAP") || directory.Parent.Name.Contains("UAP"))
                    {
                        metaFileContent = File.ReadAllLines(uapMetaFile.FullName);
                        isUAP = true;
                    }

                    FileInfo[] files = directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                    string metaFilePath = string.Empty;
                    foreach (FileInfo file in files)
                    {
                        //Editor is guid + 1
                        //Standalone is guid + 2
                        //UAP is guid + 3
                        dllGuid = dllGuids[file.Name];
                        dllGuid = CycleGuidForward(dllGuid);
                        if (isUAP)
                        {
                            dllGuid = CycleGuidForward(dllGuid);
                        }

                        if (dllGuids.ContainsKey(file.Name))
                        {
                            metaFilePath = string.Format("{0}.meta", file.FullName);
                            metaFileContent[1] = string.Format("guid: {0}", dllGuid);
                            if (File.Exists(metaFilePath))
                            {
                                File.Delete(metaFilePath);
                            }
                            File.WriteAllLines(metaFilePath, metaFileContent);
                        }
                    }
                }                
            }
        }

        private static StringBuilder guidBuilder = new StringBuilder(32);
        private static string CycleGuidForward(string guid)
        {
            guidBuilder.Clear();
            //Add one to each hexit in the guid to make it unique, but also reproducible
            foreach (char hexit in guid)
            {
                switch (hexit)
                {
                    case 'f':
                        guidBuilder.Append('0');
                        break;
                    case '9':
                        guidBuilder.Append('a');
                        break;
                    default:
                        guidBuilder.Append((char)(hexit + 1));
                        break;
                }
            }
            return guidBuilder.ToString();
        }

        private static void RecursiveFolderCleanup(DirectoryInfo folder)
        {            
            foreach (DirectoryInfo subFolder in folder.GetDirectories())
            {
                RecursiveFolderCleanup(subFolder);               
            }

            string fileCheck;
            FileInfo[] fileList = folder.GetFiles("*");
            DirectoryInfo[] folderList = folder.GetDirectories();
            foreach (FileInfo file in fileList)
            {
                if (file.Extension.Equals(".meta"))
                {
                    fileCheck = file.FullName.Remove(file.FullName.Length - 5);
                    bool foundMatch = false;
                    foreach (FileInfo checkFile in fileList)
                    {
                        if (checkFile.FullName.Equals(fileCheck))
                        {
                            foundMatch = true;
                            break;
                        }
                    }
                    if (!foundMatch)
                    {
                        foreach (DirectoryInfo checkFolder in folderList)
                        {
                            if (checkFolder.FullName.Equals(fileCheck))
                            {
                                foundMatch = true;
                                break;
                            }
                        }
                    }
                    if (!foundMatch)
                    {
                        file.Delete();
                    }
                }
            }

            if (folder.GetDirectories().Length == 0 && folder.GetFiles().Length == 0)
            {
                folder.Delete();
            }
        }
    }

    struct AssemblyDefinitionStub
    {
        public string name;
    }
}
#endif