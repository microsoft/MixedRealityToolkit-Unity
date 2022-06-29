// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;

namespace Microsoft.MixedReality.Toolkit.Tools
{
    /// <summary>
    /// 
    /// </summary>
    internal class SubsystemGenerator
    {
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
            // todo
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
