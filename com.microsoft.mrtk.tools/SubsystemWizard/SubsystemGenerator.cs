// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CSharp;
using System.Collections.Generic;
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
        // SubsystemWizard/Templates/ApplyConfigTemplate.txt
        private const string ApplyConfigTemplateGuid = "c4bce3779fa330840bc15f96d11fcec0";
        // SubsystemWizard/Templates/SubsystemBaseClassTemplate.txt
        private const string BaseClassTemplateGuid = "2cb3a153fb7b8b142a1dac1bcfc3b38c";
        // SubsystemWizard/Templates/SubsystemConfigTemplate.txt
        private const string ConfigTemplateGuid = "ab5924fb380e64d47bdbd9fed4c08910";
        // SubsystemWizard/Templates/SubsystemDerivedClassTemplate.txt
        private const string DerivedClassTemplateGuid = "02b67503c64cf224ca2c04a04077c0d7";
        // SubsystemWizard/Templates/SubsystemDescriptorTemplate.txt
        private const string DescriptorTemplateGuid = "8e1afb29fbaf1c2419511d266f49b976";
        // SubsystemWizard/Templates/SubsystemInterfaceTemplate.txt
        private const string InterfaceTemplateGuid = "9bac30e514266984d947e360cfd17b05";

        private const bool DefaultCreateConfiguration = false;
        private static readonly string DefaultBaseSubsystemName = $"NewSubsystem";
        private static readonly string DefaultCompanyName = "Contoso";
        private static readonly string DefaultDisplayName = $"{DefaultCompanyName} {DefaultBaseSubsystemName}"; // todo
        private static readonly string DefaultSubsystemName = $"{DefaultCompanyName}{DefaultBaseSubsystemName}";
        private static readonly string DefaultSubsystemNamespace = $"{DefaultCompanyName}.MRTK3.Subsystems";
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

        private string baseClassName = DefaultBaseSubsystemName;

        /// <summary>
        /// The name of the base class from which the subsysten will derive.
        /// </summary>
        public string BaseClassName
        {
            get => baseClassName;
            set => baseClassName = value;
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

        private string companyName = DefaultCompanyName;

        /// <summary>
        /// The name of the entity which is creating / releasing the subsystem.
        /// </summary>
        public string CompanyName
        {
            get => companyName;
            set => companyName = value;
        }

        /// <summary>
        /// Name of configuration class, if enabled by <see cref="CreateConfiguration"/>
        /// to create for new subystem.
        /// </summary>
        public string ConfigurationName
        {
            get => CreateConfiguration ? $"{BaseClassName}Config" : "BaseSubsystemConfig"; 
        }

        /// <summary>
        /// Name of descriptor class to create for new subystem.
        /// </summary>
        public string DescriptorName
        {
            get => $"{BaseClassName}Descriptor";
        }

        private string displayName = DefaultDisplayName;

        /// <summary>
        /// The name that will be displayed in project settings.
        /// </summary>
        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }

        /// <summary>
        /// Name of interface to create for new subystem.
        /// </summary>
        public string InterfaceName
        {
            get => $"I{BaseClassName}";
        }

        private bool dontCreateDescriptor = false;

        /// <summary>
        /// Indicates if the user wishes to skip the creation of the subsystem
        /// descriptor source code file.
        /// </summary>
        public bool DontCreateDescriptor
        {
            get => dontCreateDescriptor;
            set => dontCreateDescriptor = value;
        }

        private bool dontCreateInterface = false;

        /// <summary>
        /// Indicates if the user wishes to skip the creation of the subsystem
        /// interface source code file.
        /// </summary>
        public bool DontCreateInterface
        {
            get => dontCreateInterface;
            set => dontCreateInterface = value;
        }

        private bool dontCreateClass = false;

        /// <summary>
        /// Indicates if the user wishes to skip the creation of the subsystem
        /// class source code file.
        /// </summary>
        public bool DontCreateBaseClass
        {
            get => dontCreateClass;
            set => dontCreateClass = value;
        }

        private bool dontCreateDerivedClass = false;

        /// <summary>
        /// Indicates if the user wishes to skip the creation of the subsystem
        /// descriptor source code file.
        /// </summary>
        public bool DontCreateDerivedClass
        {
            get => dontCreateDerivedClass;
            set => dontCreateDerivedClass = value;
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
        /// Creates the subsystem source files based off of the provided templates.
        /// </summary>
        public void Generate(
            FileInfo descriptorTemplate,
            FileInfo interfaceTemplate,
            FileInfo baseClassTemplate,
            FileInfo derivedClassTemplate,
            FileInfo configTemplate,
            FileInfo applyConfigTemplate)
        {
            // Make sure there is a folder in which to create the new files.
            DirectoryInfo outputFolder = new DirectoryInfo(
                Path.Combine(OutputFolderRoot, SubsystemName));
            if (!outputFolder.Exists)
            {
                outputFolder.Create();
            }

            // Prepare the drop-in code for applying configuration to the
            // templates.
            string configCode;
            using (FileStream fs = applyConfigTemplate.OpenRead())
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    configCode = reader.ReadToEnd();
                    configCode = ReplaceTokens(configCode);
                }
            }

            // Create the source files.
            if (!DontCreateDescriptor)
            {
                CreateFile(descriptorTemplate,
                    Path.Combine(outputFolder.FullName, $"{DescriptorName}.cs"));
            }
            if (!DontCreateInterface)
            {
                CreateFile(interfaceTemplate,
                    Path.Combine(outputFolder.FullName, $"{InterfaceName}.cs"));
            }
            if (!DontCreateBaseClass)
            {
                CreateFile(baseClassTemplate,
                    Path.Combine(outputFolder.FullName, $"{BaseClassName}.cs"));
            }
            if (!DontCreateDerivedClass)
            {
                string filePath = Path.Combine(outputFolder.FullName, $"{SubsystemName}.cs");

                CreateFile(derivedClassTemplate, filePath);
                ApplyConfigCode(CreateConfiguration ? configCode : string.Empty,
                    new FileInfo(filePath));
            }
            if (CreateConfiguration)
            {
                CreateFile(configTemplate,
                    Path.Combine(outputFolder.FullName, $"{ConfigurationName}.cs"));
            }

            // Update Unity's asset database to ensure the new files appear.
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Reset the state of the wizard.
        /// </summary>
        public void Reset()
        {
            CreateConfiguration = DefaultCreateConfiguration;
            BaseClassName = DefaultBaseSubsystemName;
            CompanyName = DefaultCompanyName;
            DisplayName = DefaultDisplayName;
            SubsystemName = DefaultSubsystemName;
            SubsystemNamespace = DefaultSubsystemNamespace;

            DontCreateBaseClass = false;
            DontCreateDerivedClass = false;
            DontCreateDescriptor = false;
            DontCreateInterface = false;

            State = SubsystemWizardState.Start;
        }

        /// <summary>
        /// Ensures that the value of <see cref="BaseClassName"/> is valid for the
        /// C# language.
        /// </summary>
        /// <param name="error">String describing the encountered error.</param>
        /// <returns>True of successful, or false.</returns>
        public bool ValidateBaseClassName(out string error)
        {
            // Ensure a name was provided.
            if (string.IsNullOrWhiteSpace(BaseClassName))
            {
                error = $"Subsystem base class name: Must specify a name.";
                return false;
            }

            bool success = ValidateName(BaseClassName);

            error = success ? string.Empty :
                $"Subsystem base class name: {BaseClassName} is not a valid C# identifier.";

            return success;
        }

        /// <summary>
        /// Ensures that the value of <see cref="SubsystemNamespace"/> is valid for the
        /// C# language.
        /// </summary>
        /// <param name="error">String describing the encountered error.</param>
        /// <returns>True of successful, or false.</returns>
        public bool ValidateNamespace(out string error)
        {
            // Ensure a name was provided.
            if (string.IsNullOrWhiteSpace(SubsystemNamespace))
            {
                error = $"Subsystem namespace: Must specify a name.";
                return false;
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
        /// Ensures that the value of <see cref="SubsystemName"/> is valid for the
        /// C# language.
        /// </summary>
        /// <param name="error">String describing the encountered error.</param>
        /// <returns>True of successful, or false.</returns>
        public bool ValidateSubsystemName(out string error)
        {
            // Ensure a name was provided.
            if (string.IsNullOrWhiteSpace(SubsystemName))
            {
                error = $"Subsystem name: Must specify a name.";
                return false;
            }

            bool success = ValidateName(SubsystemName);

            error = success ? string.Empty :
                $"Subsystem name: {SubsystemName} is not a valid C# identifier.";

            return success;
        }

        /// <summary>
        /// Ensures that the necessary templates can be located and successfully loaded.
        /// </summary>
        /// <param name="errors">List to which any encountered errors will be added.</param>
        /// <param name="descriptorTemplate">
        /// <see cref="FileInfo"/> object representing the subsystem descriptor template file.
        /// </param>
        /// <param name="interfaceTemplate">
        /// <see cref="FileInfo"/> object representing the subsystem interface template file.
        /// </param>
        /// <param name="classTemplate">
        /// <see cref="FileInfo"/> object representing the subsystem class template file.
        /// </param>
        /// <param name="configTemplate">
        /// <see cref="FileInfo"/> object representing the subsystem configuration template file.
        /// </param>
        /// <param name="applyConfigTemplate">
        /// <see cref="FileInfo"/> object representing the template file that contains information on
        /// the code to apply when configuraton is desired.
        /// </param>
        /// <returns>True if the collection of templates can all be validated, or false.</returns>
        public bool ValidateTemplates(
            List<string> errors,
            out FileInfo descriptorTemplate,
            out FileInfo interfaceTemplate,
            out FileInfo baseClassTemplate,
            out FileInfo derivedClassTemplate,
            out FileInfo configTemplate,
            out FileInfo applyConfigTemplate)
        {
            bool success = true;

            if (!GetAsset(
                DescriptorTemplateGuid,
                out descriptorTemplate,
                out string error))
            {
                errors.Add($"Descriptor template - {error}");
                success = false;
            }

            if (!GetAsset(
                InterfaceTemplateGuid,
                out interfaceTemplate,
                out error))
            {
                errors.Add($"Interface template - {error}");
                success = false;
            }

            if (!GetAsset(
                BaseClassTemplateGuid,
                out baseClassTemplate,
                out error))
            {
                errors.Add($"Class template - {error}");
                success = false;
            }

            if (!GetAsset(
                DerivedClassTemplateGuid,
                out derivedClassTemplate,
                out error))
            {
                errors.Add($"Class template - {error}");
                success = false;
            }

            if (!GetAsset(
                ConfigTemplateGuid,
                out configTemplate,
                out error))
            {
                errors.Add($"Configuration template - {error}");
                success = false;
            }

            if (!GetAsset(
                ApplyConfigTemplateGuid,
                out applyConfigTemplate,
                out error))
            {
                errors.Add($"Apply configuration template - {error}");
                success = false;
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="sourceFile"></param>
        private void ApplyConfigCode(
            string code,
            FileInfo sourceFile)
        {
            if (!sourceFile.Exists)
            {
                throw new IOException($"{sourceFile.FullName} cannot be found.");
            }

            string source;
            using (FileStream fs = sourceFile.OpenRead())
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    source = reader.ReadToEnd();
                    source = source.Replace("%APPLYCONFIG%", code);
                }
            }

            using (FileStream fs = new FileStream(sourceFile.FullName, FileMode.Truncate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.AutoFlush = true;
                    writer.Write(source);
                }
            }
        }

        /// <summary>
        /// Creates the specified subsystem source code file from the provided template.
        /// </summary>
        /// <param name="templateFile">The template to use for the specified file.</param>
        /// <param name="outputFilePath">The fully qualified path for the resulting source code file.</param>
        private void CreateFile(
            FileInfo templateFile,
            string outputFilePath)
        {
            FileInfo outputFile = new FileInfo(outputFilePath);
            if (outputFile.Exists)
            {
                throw new IOException($"Unable to create {outputFile.FullName}, overwriting existing files is not supported.");
            }

            // Read the template file.
            string template = null;
            using (FileStream fs = templateFile.OpenRead())
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    template = reader.ReadToEnd();
                    template = ReplaceTokens(template);
                }
            }

            if (template == null)
            {
                throw new IOException($"Failed to read the contents of {templateFile.FullName}");
            }

            // Write the new source file.
            using (FileStream fs = outputFile.OpenWrite())
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.AutoFlush = true;
                    writer.Write(template);
                }
            }
        }

        /// <summary>
        /// Attempts to acquire a <see cref="FileInfo"/> object representing the requested asset.
        /// </summary>
        /// <param name="guid">The guid that represents the asset.</param>
        /// <param name="fileInfo"><see cref="FileInfo"/> object representing the asset.</param>
        /// <param name="error">Error message.</param>
        /// <returns></returns>
        private bool GetAsset(
            string guid,
            out FileInfo fileInfo,
            out string error)
        {
            fileInfo = null;
            error = string.Empty;

            string filePath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                error = "Unable to resolve template asset, please file an MRTK3 bug.";
                return false;
            }

            FileInfo fi = new FileInfo(filePath);
            if (!fi.Exists)
            {
                error = $"Template file {fi.Name} could not be located, please file an MRTK3 bug.";
                return false;
            }

            fileInfo = fi;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private string ReplaceTokens(string template)
        {
            template = template.Replace("%NAMESPACE%", SubsystemNamespace);
            template = template.Replace("%SUBSYSTEMBASECLASSNAME%", BaseClassName);
            template = template.Replace("%SUBSYSTEMNAME%", SubsystemName);
            template = template.Replace("%CONFIGNAME%", ConfigurationName);
            template = template.Replace("%COMPANYNAME%", CompanyName);
            template = template.Replace("%DISPLAYNAME%", DisplayName);
            template = template.Replace("%RUNTIMENAME%", SubsystemNamespace.ToLower());

            return template;
        }

        /// <summary>
        /// Validates that the specified name is a valid indentifier for C#.
        /// </summary>
        /// <param name="name">The name to validate/</param>
        /// <returns>True if the name is a valid C# identifier, or false.</returns>
        private bool ValidateName(string name)
        {
            // Verify that the name is valid within C#
            return CSharpCodeProvider.CreateProvider("C#").IsValidIdentifier(name);
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
