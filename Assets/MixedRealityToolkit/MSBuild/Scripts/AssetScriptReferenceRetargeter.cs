// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    public static class AssetScriptReferenceRetargeter
    {
        private struct ClassInformation
        {
            public string Name;
            public string Namespace;
            public string Guid;
            public long FileId;
            public int ExecutionOrder;
        }

        private class AssemblyInformation
        {
            public string Name { get; }

            public Dictionary<string, ClassInformation> CompiledClasses { get; }

            public AssemblyInformation(string name)
            {
                Name = name;
                CompiledClasses = new Dictionary<string, ClassInformation>();
            }
        }

        private const string YamlPrefix = "%YAML 1.1";

        private static readonly Dictionary<string, string> sourceToOutputFolders = new Dictionary<string, string>
        {
            { "MSBuild/Publish/Player/WSA", "UAPPlayer" },
            { "MSBuild/Publish/Player/WindowsStandalone32", "StandalonePlayer" },
        };

        private static readonly HashSet<string> ExcludedYamlAssetExtensions = new HashSet<string> { ".jpg", ".csv", ".meta", ".pfx", ".txt", ".nuspec", ".asmdef", ".yml", ".cs", ".md", ".json", ".ttf", ".png", ".shader", ".wav", ".bin", ".gltf", ".glb", ".fbx", ".pdf", ".cginc", ".rsp", ".xml", ".targets", ".props", ".template", ".csproj", ".sln", ".psd", ".room" };
        private static readonly HashSet<string> ExcludedSuffixFromCopy = new HashSet<string>() { ".cs", ".cs.meta", ".asmdef", ".asmdef.meta" };

        private static Dictionary<string, string> nonClassDictionary = new Dictionary<string, string>(); //Guid, FileName

        // This is the known Unity-defined script fileId
        private const string ScriptFileIdConstant = "11500000";

        [MenuItem("Assets/Retarget To DLL")]
        public static void RetargetAssets()
        {
            try
            {
                Debug.Log("Starting to retarget assets.");
                RunRetargetToDLL();
                Debug.Log("Completed asset retargeting.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to retarget assets.");
                Debug.LogException(ex);

                throw ex;
            }
        }

        private static void RunRetargetToDLL()
        {
            string[] allFilesUnderAssets = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);

            Dictionary<string, ClassInformation> scriptFilesReferences = ProcessScripts(allFilesUnderAssets);
            Debug.Log($"Found {scriptFilesReferences.Count} script file references.");

            // DLL name to Guid
            Dictionary<string, string> asmDefMappings = RetrieveAsmDefGuids(allFilesUnderAssets);

            Dictionary<string, AssemblyInformation> compiledClassReferences = ProcessCompiledDLLs("PackagedAssemblies", Application.dataPath.Replace("Assets", "NuGet/Plugins/EditorPlayer"), asmDefMappings);
            Debug.Log($"Found {compiledClassReferences.Select(t => t.Value.CompiledClasses.Count).Sum()} compiled class references.");

            Dictionary<string, Tuple<string, long>> remapDictionary = new Dictionary<string, Tuple<string, long>>();

            foreach (KeyValuePair<string, AssemblyInformation> pair in compiledClassReferences)
            {
                foreach (KeyValuePair<string, ClassInformation> compiledClass in pair.Value.CompiledClasses)
                {
                    ClassInformation compiledClassInfo = compiledClass.Value;
                    if (scriptFilesReferences.TryGetValue(compiledClass.Key, out ClassInformation scriptClassInfo))
                    {
                        remapDictionary.Add(scriptClassInfo.Guid, new Tuple<string, long>(compiledClassInfo.Guid, compiledClassInfo.FileId));
                        scriptFilesReferences.Remove(compiledClass.Key);
                    }
                    else
                    {
                        Debug.LogWarning($"Can't find a script version of the compiled class: {compiledClass.Key}; {pair.Key}.dll. This generally means the compiled class is second or later in a script file, and Unity doesn't parse it as two different assets.");
                    }
                }
            }

            ProcessYAMLAssets(allFilesUnderAssets, Application.dataPath.Replace("Assets", "NuGet/Content"), remapDictionary, asmDefMappings);
        }

        private static Dictionary<string, string> RetrieveAsmDefGuids(string[] allFiles)
        {
            int lengthOfPrefix = Application.dataPath.IndexOf("Assets");
            Dictionary<string, string> dllGuids = new Dictionary<string, string>();
            foreach (string asmdefFile in allFiles.Where(t => t.EndsWith(".asmdef")))
            {
                string asmdefText = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefFile.Substring(lengthOfPrefix)).text;
                string dllName = JsonUtility.FromJson<AssemblyDefinitionStub>(asmdefText).name;
                string guid = File.ReadAllLines($"{asmdefFile}.meta")[1].Substring(6);
                if (!Guid.TryParse(guid, out Guid _))
                {
                    throw new InvalidDataException("AsmDef meta file must have changed, as we can no longer parse a guid out of it.");
                }
                guid = CycleGuidForward(guid);
                dllGuids.Add($"{dllName}.dll", guid);
            }

            return dllGuids;
        }

        /// <param name="remapDictionary">Script file guid references to final editor DLL guid and fileID.</param>
        /// <param name="dllGuids">DLL name to DLL file guid mapping.</param>
        private static void ProcessYAMLAssets(string[] allFilePaths, string outputDirectory, Dictionary<string, Tuple<string, long>> remapDictionary, Dictionary<string, string> dllGuids)
        {
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }

            Debug.Log($"Output Directory: {outputDirectory}");

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
                    if (!ExcludedYamlAssetExtensions.Contains(extension.ToLower()))
                    {
                        foundNonYamlExtensions.Add(extension);
                    }

                    bool copyFile = true;
                    foreach (string suffix in ExcludedSuffixFromCopy)
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

            foreach (string extension in foundNonYamlExtensions)
            {
                Debug.LogWarning($"Not a YAML extension: {extension}");
            }

            IEnumerable<Task> tasks = yamlAssets.Select(t => Task.Run(() => ProcessYamlFile(t.Item1, t.Item2, remapDictionary)));
            Task.WhenAll(tasks).Wait();

            PostProcess(outputDirectory, dllGuids);
        }

        private static async Task ProcessYamlFile(string filePath, string targetPath, Dictionary<string, Tuple<string, long>> remapDictionary)
        {
            int lineNum = 0;
            using (StreamReader reader = new StreamReader(filePath))
            using (StreamWriter writer = new StreamWriter(targetPath))
            {
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    lineNum++;
                    if (line.Contains("m_Script"))
                    {
                        if (!line.Contains('}'))
                        {
                            // Read the second line as well
                            line += await reader.ReadLineAsync();
                            lineNum++;

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
                                throw new InvalidDataException($"A script without a class ({nonClassDictionary[guid]}) is being processed.");
                            }
                            else
                            {
                                // Switch to error later
                                Debug.LogWarning($"Couldn't find a script remap for {guid} in file: '{filePath}' at line '{lineNum}'.");
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
                string line = reader.ReadLine();
                return line?.StartsWith(YamlPrefix) ?? false;
            }
        }

        /// <returns>Returns a dictionary of type name at the script location mapped to additional data.</returns>
        private static Dictionary<string, ClassInformation> ProcessScripts(string[] allFilePaths)
        {
            int lengthOfPrefix = Application.dataPath.IndexOf("Assets");

            Dictionary<string, ClassInformation> toReturn = new Dictionary<string, ClassInformation>();

            foreach (string filePath in allFilePaths)
            {
                if (Path.GetExtension(filePath) == ".cs")
                {
                    Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(filePath.Substring(lengthOfPrefix));
                    IEnumerable<MonoScript> allScripts = allAssets.Cast<MonoScript>();
                    if (allAssets.Length > 1)
                    {
                        Debug.Log("Test");
                    }

                    foreach (MonoScript monoScript in allScripts)
                    {
                        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(monoScript, out string guid, out long fileId))
                        {
                            Type type = monoScript.GetClass();
                            if (type != null)
                            {
                                toReturn.Add(type.FullName, new ClassInformation() { Name = type.Name, Namespace = type.Namespace, FileId = fileId, Guid = guid, ExecutionOrder = MonoImporter.GetExecutionOrder(monoScript) });
                            }
                            else
                            {
                                nonClassDictionary.Add(guid, Path.GetFileName(filePath));
                                // anborod: This warning is very noisy, and often is correct due to "interface", "abstract", "enum" classes that won't return type with call to GetClass above.
                                // To turn it on, we should do extra checking, but removing for now.
                                //Debug.LogWarning($"Found script that we can't get type from: {monoScript.name}");
                            }
                        }
                    }
                }
            }

            return toReturn;
        }

        /// <returns>Returns a dictionary of type name inside MRTK DLLs mapped to additional data.</returns>
        private static Dictionary<string, AssemblyInformation> ProcessCompiledDLLs(string temporaryDirectoryName, string outputDirectory, Dictionary<string, string> asmDefMappings)
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
                        string dllPath = Utilities.GetFullPathFromAssetsRelative($"Assets/../MSBuild/Publish/InEditor/WindowsStandalone32/{dll.name}.dll");
                        File.Copy(dllPath, Path.Combine(tmpDirPath, $"{dll.name}.dll"), true);
                    }
                }

                // Load these directories
                AssetDatabase.Refresh();

                Dictionary<string, AssemblyInformation> toReturn = new Dictionary<string, AssemblyInformation>();

                Utilities.EnsureCleanDirectory(outputDirectory);
                foreach (Assembly dll in dlls)
                {
                    if (dll.name.Contains("MixedReality"))
                    {
                        AssemblyInformation assemblyInformation = new AssemblyInformation(dll.name);
                        if (!asmDefMappings.TryGetValue($"{dll.name}.dll", out string newDllGuid))
                        {
                            throw new InvalidOperationException($"No guid based on .asmdef was generated for DLL '{dll.name}'.");
                        }

                        File.Copy(Path.Combine(tmpDirPath, $"{dll.name}.dll"), Path.Combine(outputDirectory, $"{dll.name}.dll"));
                        File.Copy(Path.Combine(tmpDirPath, $"{dll.name}.dll.meta"), Path.Combine(outputDirectory, $"{dll.name}.dll.meta"));

                        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(Path.Combine("Assets", temporaryDirectoryName, $"{dll.name}.dll"));

                        foreach (Object asset in assets)
                        {
                            MonoScript monoScript = asset as MonoScript;
                            if (!(monoScript is null) && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(monoScript, out string guid, out long fileId))
                            {
                                Type type = monoScript.GetClass();

                                if (type == null)
                                {
                                    Debug.LogError($"Encountered a MonoScript we get a null Type from: '{monoScript.name}'");
                                }
                                else if (type.Namespace == null || !type.Namespace.Contains("Microsoft.MixedReality.Toolkit"))
                                {
                                    throw new InvalidDataException($"Type {type.Name} is not a member of the Microsoft.MixedReality.Toolkit namespace");
                                }
                                else
                                {
                                    assemblyInformation.CompiledClasses.Add(type.FullName, new ClassInformation() { Name = type.Name, Namespace = type.Namespace, FileId = fileId, Guid = newDllGuid });
                                }
                            }
                        }

                        toReturn.Add(dll.name, assemblyInformation);
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

        private static void PostProcess(string outputPath, Dictionary<string, string> dllGuids)
        {
            DirectoryInfo outputDirectory = new DirectoryInfo(outputPath);
            RecursiveFolderCleanup(outputDirectory);
            CopyIntermediaryAssemblies(Application.dataPath.Replace("Assets", "NuGet/Plugins"));
            UpdateMetaFiles(dllGuids);
        }

        private static void CopyIntermediaryAssemblies(string outputPath)
        {
            foreach (KeyValuePair<string, string> sourceToOutputPair in sourceToOutputFolders)
            {
                DirectoryInfo directory = new DirectoryInfo(Application.dataPath.Replace("Assets", sourceToOutputPair.Key));
                if (!directory.Exists)
                {
                    throw new InvalidDataException($"The required platform intermediary build directory {sourceToOutputPair.Key} does not exist. Was the build successful?");
                }

                string pluginPath = Path.Combine(outputPath, sourceToOutputPair.Value);
                if (Directory.Exists(pluginPath))
                {
                    Directory.Delete(pluginPath, true);
                }
                Directory.CreateDirectory(pluginPath);

                FileInfo[] dlls = directory.GetFiles("Microsoft.MixedReality.Toolkit*.dll", SearchOption.AllDirectories);
                foreach (FileInfo dll in dlls)
                {
                    File.Copy(dll.FullName, Path.Combine(pluginPath, dll.Name), true);
                }
            }
        }

        private static string ProcessMetaTemplate(string templateText, string guid, Dictionary<string, int> executionOrderEntries = null)
        {
            if (!Utilities.TryGetTextTemplate(templateText, "PROJECT_GUID", out string projectGuidTemplate))
            {
                throw new FormatException("Incorrect format for the meta template, doesn't contain a place for project_guid.");
            }

            Dictionary<string, string> tokenReplacements = new Dictionary<string, string>()
            {
                { projectGuidTemplate, projectGuidTemplate.Replace("<PROJECT_GUID_TOKEN>", guid) }
            };

            if (Utilities.TryGetTextTemplate(templateText, "EXECUTION_ORDER", out string executionOrderTemplate)
                && Utilities.TryGetTextTemplate(templateText, "EXECUTION_ORDER_ENTRY", out string executionOrderEntryTemplate))
            {
                if ((executionOrderEntries?.Count ?? 0) == 0)
                {
                    tokenReplacements.Add(executionOrderTemplate, executionOrderTemplate.Replace("<EMPTY_TOKEN>", "{}"));
                    tokenReplacements.Add(executionOrderEntryTemplate, string.Empty);
                }
                else
                {
                    List<string> entries = new List<string>();
                    foreach (KeyValuePair<string, int> pair in executionOrderEntries)
                    {
                        entries.Add(Utilities.ReplaceTokens(executionOrderEntryTemplate, new Dictionary<string, string>()
                        {
                            { "<SCRIPT_FULL_NAME_TOKEN>", pair.Key },
                            { "<SCRIPT_EXECUTION_VALUE_TOKEN>", pair.Value.ToString() }
                        }));
                    }

                    tokenReplacements.Add(executionOrderTemplate, executionOrderTemplate.Replace("<EMPTY_TOKEN>", string.Empty));
                    tokenReplacements.Add(executionOrderEntryTemplate, string.Join("\r\n", entries));
                }
            }

            return Utilities.ReplaceTokens(templateText, tokenReplacements);
        }

        private static void UpdateMetaFiles(Dictionary<string, string> dllGuids)
        {
            if (!TemplateFiles.Instance.PluginMetaTemplatePaths.TryGetValue(BuildTargetGroup.Unknown, out FileInfo editorMetaFile))
            {
                throw new FileNotFoundException("Could not find sample editor dll.meta template.");
            }

            if (!TemplateFiles.Instance.PluginMetaTemplatePaths.TryGetValue(BuildTargetGroup.WSA, out FileInfo uapMetaFile))
            {
                throw new FileNotFoundException("Could not find sample editor dll.meta template.");
            }

            if (!TemplateFiles.Instance.PluginMetaTemplatePaths.TryGetValue(BuildTargetGroup.Standalone, out FileInfo standaloneMetaFile))
            {
                throw new FileNotFoundException("Could not find sample editor dll.meta template.");
            }

            string metaFileTemplate = File.ReadAllText(editorMetaFile.FullName);

            foreach (FileInfo dllFile in new DirectoryInfo(Application.dataPath.Replace("Assets", "NuGet/Plugins")).GetFiles("*", SearchOption.AllDirectories))
            {
                if (dllFile.Extension.Equals(".meta"))
                {
                    string dllFileName = dllFile.Name.Remove(dllFile.Name.Length - 5);
                    if (dllGuids.TryGetValue(dllFileName, out string guid))
                    {
                        File.WriteAllText(dllFile.FullName, ProcessMetaTemplate(metaFileTemplate, guid, null));
                    }
                }
            }

            DirectoryInfo[] directories = new DirectoryInfo(Application.dataPath.Replace("Assets", "NuGet/Plugins")).GetDirectories("*", SearchOption.AllDirectories);
            string dllGuid = string.Empty;
            bool isUAP = false;
            foreach (DirectoryInfo directory in directories)
            {
                isUAP = false;
                if (!directory.Name.Equals("EditorPlayer"))
                {
                    if (directory.Name.Contains("Standalone") || directory.Parent.Name.Contains("Standalone"))
                    {
                        metaFileTemplate = File.ReadAllText(standaloneMetaFile.FullName);
                    }
                    else if (directory.Name.Contains("UAP") || directory.Parent.Name.Contains("UAP"))
                    {
                        metaFileTemplate = File.ReadAllText(uapMetaFile.FullName);
                        isUAP = true;
                    }

                    FileInfo[] files = directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                    string metaFilePath = string.Empty;
                    foreach (FileInfo file in files)
                    {
                        //Editor is guid + 1; which has done when we processed Editor DLLs
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
                            metaFilePath = $"{file.FullName}.meta";
                            File.WriteAllText(metaFilePath, ProcessMetaTemplate(metaFileTemplate, dllGuid, null));
                        }
                    }
                }
            }
        }

        private static string CycleGuidForward(string guid)
        {
            StringBuilder guidBuilder = new StringBuilder();
            guid = guid.ToLower();

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

            FileInfo[] fileList = folder.GetFiles("*");
            DirectoryInfo[] folderList = folder.GetDirectories();
            foreach (FileInfo file in fileList)
            {
                if (file.Extension.Equals(".meta"))
                {
                    string nameCheck = file.FullName.Remove(file.FullName.Length - 5);

                    // If we don't have any files or folders match the nameCheck we will delete the file
                    if (!fileList.Concat<FileSystemInfo>(folderList).Any(t => nameCheck.Equals(t.FullName)))
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
        private struct AssemblyDefinitionStub
        {
#pragma warning disable CS0649
            public string name;
#pragma warning restore CS0649
        }
    }
}
#endif