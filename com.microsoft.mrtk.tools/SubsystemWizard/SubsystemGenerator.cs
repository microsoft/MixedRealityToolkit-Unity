// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tools
{
    /// <summary>
    /// 
    /// </summary>
    internal class SubsystemGenerator
    {
        // SubsystemWizard/Templates/SubsystemClassTemplate.txt
        private const string ClassTemplateGuid = "2cb3a153fb7b8b142a1dac1bcfc3b38c";
        // SubsystemWizard/Templates/SubsystemConfigTemplate.txt
        private const string ConfigTemplateGuid = "ab5924fb380e64d47bdbd9fed4c08910";
        // SubsystemWizard/Templates/SubsystemDescriptorTemplate.txt
        private const string DescriptorTemplateGuid = "8e1afb29fbaf1c2419511d266f49b976";
        // SubsystemWizard/Templates/SubsystemInterfaceTemplate.txt
        private const string InterfaceTemplateGuid = "9bac30e514266984d947e360cfd17b05";

        private const bool DefaultCreateConfiguration = false;
        private static readonly string DefaultSubsystemName = "NewSubsystem";
        private static readonly string DefaultSubsystemNamespace = "Custom.MRTK3.Subsystems";
        private static readonly string OutputFolderRoot = Path.Combine("Assets", "MRTK.Generated");

        private SubsystemWizardState state = SubsystemWizardState.Start;

        /// <summary>
        /// 
        /// </summary>
        public SubsystemWizardState State
        {
            get => state;
            set => state = value;
        }

        private bool createConfiguration = DefaultCreateConfiguration;

        /// <summary>
        /// 
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

        /// <summary>
        /// 
        /// </summary>
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

            // Validate the subsystem name.
            if (!ValidateName(SubsystemName, out string error))
            {
                errors.Add(error);
            }

            // Validate the namespace.
            if (!ValidateName(SubsystemNamespace, out error))
            {
                errors.Add(error);
            }

            // Make sure there is a folder in which to create the new files.
            DirectoryInfo outputFolder = new DirectoryInfo(
                Path.Combine(OutputFolderRoot, SubsystemName));
            if (!outputFolder.Exists)
            {
                outputFolder.Create();
            }

            // If we already have one or more errors, abort.
            if (errors.Count > 0)
            {
                errors.Add("Aborting subsystem creation");
                return false;
            }

            // Create subsystem descriptor file
            // todo: skip?
            FileInfo descriptorFile = new FileInfo(
                Path.Combine(outputFolder.FullName, $"{DescriptorName}.cs"));
            if (!CreateFile(descriptorTemplate, descriptorFile, out error))
            {
                errors.Add(error);
            }

            // If we failed creating the previous file, abort.
            if (errors.Count > 0)
            {
                errors.Add("Aborting subsystem creation");
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
                errors.Add("Aborting subsystem creation");
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
                    errors.Add("Aborting subsystem creation");
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
        private bool ValidateName(string name, out string error)
        {
            error = string.Empty;

            // Ensure a name was provided.
            if (string.IsNullOrWhiteSpace(SubsystemName))
            {
                SubsystemNamespace = DefaultSubsystemName;
            }

            // Verify that the name is valid within C#
            if (!CSharpCodeProvider.CreateProvider("C#").IsValidIdentifier(name))
            {
                error = $"{name} is not a valid C# identifier";
            }

            return true;
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
