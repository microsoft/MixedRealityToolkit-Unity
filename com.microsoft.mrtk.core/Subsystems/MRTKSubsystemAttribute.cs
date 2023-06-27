// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Attribute marking a concrete and creatable MRTK subsystem.
    /// Subsystem classes marked with this attribute will be discoverable
    /// to the <see cref="MRTKLifecycleManager"/> and to the MRTK profile editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MRTKSubsystemAttribute : Attribute, IMRTKSubsystemDescriptor
    {
        private string name;

        private string displayName;

        private string author;

        private SystemType providerType;

        private SystemType subsystemTypeOverride;

        private SystemType configType = typeof(BaseSubsystemConfig);

        /// <inheritdoc/>
        public string Name
        {
            get => name;
            set => name = value;
        }

        /// <inheritdoc/>
        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }

        /// <inheritdoc/>
        public string Author
        {
            get => author;
            set => author = value;
        }

        /// <inheritdoc/>
        public Type ProviderType
        {
            get => providerType;
            set => providerType = value;
        }

        /// <inheritdoc/>
        public Type SubsystemTypeOverride
        {
            get => subsystemTypeOverride;
            set => subsystemTypeOverride = value;
        }

        /// <inheritdoc/>
        public Type ConfigType
        {
            get => configType;
            set => configType = value;
        }
    }
}