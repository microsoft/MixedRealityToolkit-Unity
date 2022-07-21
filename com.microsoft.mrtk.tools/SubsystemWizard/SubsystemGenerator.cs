// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Codice.CM.Common.Tree.Partial;
using Codice.CM.SEIDInfo;
using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tools
{
    /// <summary>
    /// Class which generates subsystem source files from templates contained within
    /// the package.
    /// </summary>
    internal class SubsystemGenerator
    {
        // SubsystemWizard/Templates/SubsystemClassTemplate.txt
        public const string ClassTemplateGuid = "2cb3a153fb7b8b142a1dac1bcfc3b38c";
        // SubsystemWizard/Templates/SubsystemConfigTemplate.txt
        public const string ConfigTemplateGuid = "ab5924fb380e64d47bdbd9fed4c08910";
        // SubsystemWizard/Templates/SubsystemDescriptorTemplate.txt
        public const string DescriptorTemplateGuid = "8e1afb29fbaf1c2419511d266f49b976";
        // SubsystemWizard/Templates/SubsystemInterfaceTemplate.txt
        public const string InterfaceTemplateGuid = "9bac30e514266984d947e360cfd17b05";

        private const bool DefaultCreateConfiguration = false;
        private static readonly string DefaultSubsystemName = "NewSubsystem";
        private static readonly string DefaultSubsystemNamespace = "Custom.MRTK3.Subsystems";
        private static readonly string OutputFolderRoot = Path.Combine("Assets", "MRTK.Generated");

        private SubsystemWizardState state = SubsystemWizardState.Start;

        /// <summary>
        /// The current state of the wizard.
        /// </summary>
        public SubsystemWizardState State
        {
            get => state;
            set => state = value;
        }

        private bool createConfiguration = DefaultCreateConfiguration;

        /// <summary>
        /// Inidcates whether or not the wizard should generate a subsystem configuration
        /// source file.
        /// </summary>
        public bool CreateConfiguration
        {
            get => createConfiguration;
            set => createConfiguration = value;
        }

        /// <summary>
        /// Name of configuration class, if enabled by <see cref="CreateConfiguration"/>
        /// to create for new subystem.
        /// </summary>
        public string ConfigurationName
        {
            get => $"{SubsystemName}Configuration";
        }

        /// <summary>
        /// Name of descriptor class to create for new subystem.
        /// </summary>
        public string DescriptorName
        {
            get => $"{SubsystemName}Descriptor";
        }

        /// <summary>
        /// Name of interface to create for new subystem.
        /// </summary>
        public string InterfaceName
        {
            get => $"I{SubsystemName}";
        }

        private string subsystemName = DefaultSubsystemName;

        /// <summary>
        /// The name class to create for the new subsystem.
        /// </summary>
        public string SubsystemName
        {
            get => subsystemName;
            set => subsystemName = value;
        }

        private string subsystemNamespace = DefaultSubsystemNamespace;

        /// <summary>
        /// The interface in which the new subsystem code will be contained.
        /// </summary>
        public string SubsystemNamespace
        {
            get => subsystemNamespace;
            set => subsystemNamespace = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SubsystemGenerator()
        {
            Reset();
        }

        private const string abortMessage = "Aborting subsystem generation";

        /// <summary>
        /// Creates the subsystem source files based off of the templates in the
        /// package.
        /// </summary>
        /// <param name="errors">Collection of errors encountered in the generation process.</param>
        /// <returns>True if successful, or false.</returns>
        public bool Generate(List<string> errors)
        {
            errors.Clear();

            // Ensure the package is not corrupted / missing files.
            if (!GetFileFromGuid(DescriptorTemplateGuid, out FileInfo descriptorTemplate) ||
                !GetFileFromGuid(InterfaceTemplateGuid, out FileInfo interfaceTemplate) ||
                !GetFileFromGuid(ClassTemplateGuid, out FileInfo classTemplate) ||
                !GetFileFromGuid(ConfigTemplateGuid, out FileInfo configTemplate))
            {
                errors.Add($"Package error: Unable to locate one or more subsystem template files.");
                return false;
            }

            // Make sure there is a folder in which to create the new files.
            DirectoryInfo outputFolder = new DirectoryInfo(
                Path.Combine(OutputFolderRoot, SubsystemName));
            if (!outputFolder.Exists)
            {
                outputFolder.Create();
            }

            // Create subsystem descriptor file
            // todo: skip?
            FileInfo descriptorFile = new FileInfo(
                Path.Combine(outputFolder.FullName, $"{DescriptorName}.cs"));
            if (!CreateFile(descriptorTemplate, descriptorFile, out string error))
            {
                errors.Add(error);
            }

            // If we failed creating the previous file, abort.
            if (errors.Count > 0)
            {
                errors.Add(abortMessage);
                return false;
            }

            // Create subsystem interface file
            // todo: skip?
            FileInfo interfaceFile = new FileInfo(
                Path.Combine(outputFolder.FullName, $"{InterfaceName}.cs"));
            if (!CreateFile(interfaceTemplate, interfaceFile, out error))
            {
                errors.Add(error);
            }

            // If we failed creating the previous file, abort.
            if (errors.Count > 0)
            {
                errors.Add(abortMessage);
                return false;
            }

            // Create subsystem class file
            // todo: skip?
            FileInfo classFile = new FileInfo(
                Path.Combine(outputFolder.FullName, $"{SubsystemName}.cs"));
            if (!CreateFile(classTemplate, classFile, out error))
            {
                errors.Add(error);
            }

            // Create subsystem configuration file
            if (CreateConfiguration)
            {
                // If we failed creating the previous file, abort.
                if (errors.Count > 0)
                {
                    errors.Add(abortMessage);
                    return false;
                }

                FileInfo configurationFile = new FileInfo(
                    Path.Combine(outputFolder.FullName, $"{ConfigurationName}.cs"));
                if (!CreateFile(configTemplate, configurationFile, out error))
                {
                    errors.Add(error);
                }
            }

            AssetDatabase.Refresh();

            return (errors.Count == 0);
        }

        /// <summary>
        /// Reset the state of the wizard.
        /// </summary>
        public void Reset()
        {
            CreateConfiguration = DefaultCreateConfiguration;
            SubsystemName = DefaultSubsystemName;
            SubsystemNamespace = DefaultSubsystemNamespace;
            State = SubsystemWizardState.Start;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool CreateFile(
            FileInfo templateFile,
            FileInfo outputFile,
            out string error)
        {
            error = string.Empty;

            if (outputFile.Exists)
            {
                error = $"Unable to create {outputFile.FullName}, overwriting existing files is not supported.";
                return false;
            }

            try
            {
                string template = null;

                using (FileStream fs = templateFile.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        template = reader.ReadToEnd();

                        // Insert namespace and subsystem name
                        template = template.Replace("%NAMESPACE%", SubsystemNamespace);
                        template = template.Replace("%SUBSYSTEMNAME%", SubsystemName);
                    }
                }

                if (template == null)
                {
                    error = $"Failed to read the contents of {templateFile.FullName}";
                    return false;
                }

                using (FileStream fs = outputFile.OpenWrite())
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.AutoFlush = true;
                        writer.Write(template);
                    }
                }
            }
            catch (Exception e)
            {
                error = $"Failed to create {outputFile.FullName} - {e.Message} ({e.GetType()})";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ValidateSubsystemName(string name, out string error)
        {
            // Ensure a name was provided.
            if (string.IsNullOrWhiteSpace(SubsystemName))
            {
                SubsystemName = DefaultSubsystemName;
            }

            bool success = ValidateName(name);

            error = success ? string.Empty :
                $"Subsystem name: {name} is not a valid C# identifier.";

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ValidateNamespace(string name, out string error)
        {
            // Ensure a name was provided.
            if (string.IsNullOrWhiteSpace(SubsystemNamespace))
            {
                SubsystemNamespace = DefaultSubsystemNamespace;
            }

            bool success = true;
            StringBuilder sb = new StringBuilder();

            string[] namespaceComponents = SubsystemNamespace.Split('.');

            foreach (string s in namespaceComponents)
            {
                if (!ValidateName(s))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(s);
                    success = false;
                }
            }

            error = (sb.Length == 0) ? string.Empty :
                $"Namespace: {sb} is/are invalid C# identifier(s)";
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool ValidateName(string name)
        {
            // Verify that the name is valid within C#
            bool isValid = CSharpCodeProvider.CreateProvider("C#").IsValidIdentifier(name);
            return isValid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private bool GetFileFromGuid(
            string guid,
            out FileInfo fileInfo)
        {
            fileInfo = null;

            FileInfo fi = new FileInfo(AssetDatabase.GUIDToAssetPath(guid));
            if (fi.Exists)
            {
                fileInfo = fi;
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Enumeration representing the states of the subsystem wizard.
    /// </summary>
    internal enum SubsystemWizardState
    {
        /// <summary>
        /// The wizard is collecting data from the user.
        /// </summary>
        Start = 0,

        /// <summary>
        /// The wizard is ready to generate, waiting for confirmation from the user.
        /// </summary>
        PreGenerate,

        /// <summary>
        /// The wizard is complete.
        /// </summary>
        Complete
    }
}
