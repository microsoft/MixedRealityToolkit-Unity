// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

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

        // todo - abstract provider (default to true)?
        // todo - provider name
        // todo - generate platform implementation?

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
        public void Generate()
        {
            DirectoryInfo outputFolder = new DirectoryInfo(Path.Combine(OutputFolderRoot, SubsystemName));
            if (!outputFolder.Exists)
            {
                outputFolder.Create();
            }

            // Descriptor
            FileInfo descriptorFile = new FileInfo(
                Path.Combine(outputFolder.FullName, $"{DescriptorName}.cs"));
            if (!CreateFile(DescriptorTemplateGuid, descriptorFile))
            {
                // todo: fail
            }

            // Interface
            FileInfo interfaceFile = new FileInfo(
                Path.Combine(outputFolder.FullName, $"{InterfaceName}.cs"));
            if (!CreateFile(InterfaceTemplateGuid, interfaceFile))
            {
                // todo: fail
            }

            // Class
            FileInfo classFile = new FileInfo(
                Path.Combine(outputFolder.FullName, $"{SubsystemName}.cs"));
            if (!CreateFile(ClassTemplateGuid, classFile))
            {
                // todo: fail
            }

            // Configuration
            if (CreateConfiguration)
            {
                FileInfo configurationFile = new FileInfo(
                    Path.Combine(outputFolder.FullName, $"{ConfigurationName}.cs"));
                if (!CreateFile(ConfigTemplateGuid, configurationFile))
                {
                    // todo: fail
                }
            }

            AssetDatabase.Refresh();
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
        /// <param name="templateGuid"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        private bool CreateFile(string templateGuid, FileInfo outputFile)
        {
            // The generator does not support overwriting existing files.
            // todo: consider checking existence of all files up-front before any are created
            if (outputFile.Exists)
            {
                // todo: error
                return false;
            }

            try
            {
                FileInfo templateFile = new FileInfo(AssetDatabase.GUIDToAssetPath(templateGuid));
                if (!templateFile.Exists)
                {
                    // todo error
                    return false;
                }

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
                    // todo: error
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
                Debug.LogError("[SubsystemGenerator] Failed to create file.");
                return false;
            }

            return true;
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
        /// The wizard is in the process of creating the subsystem.
        /// </summary>
        Generating,

        /// <summary>
        /// The wizard is complete.
        /// </summary>
        Complete
    }
}
