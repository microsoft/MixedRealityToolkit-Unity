// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Base descriptor required of all Mixed Reality Toolkit subsystems.
    /// </summary>
    public interface IMRTKSubsystemDescriptor
    {
        /// <summary>
        /// Identifier of the subsystem being described.
        /// </summary>
        /// <remarks>
        /// It is recommended to use reverse dot notation (ex: com.microsoft.mrtk.test) as the formal name, to avoid collisions.
        /// </remarks>
        string Name { get; set; }

        /// <summary>
        /// The creator (ex: Microsoft) of the subsystem being described.
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// The friendly name (ex: MRTK Test Subsystem) of the subsystem being described.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Specifies the provider implementation type to use for instantiation.
        /// </summary>
        /// <value>
        /// The provider implementation type to use for instantiation.
        /// </value>
        Type ProviderType { get; set; }

        /// <summary>
        /// Specifies the <see cref="MRTKSubsystem{TSubsystem, TSubsystemDescriptor, TProvider}"/>-derived type that forwards casted calls to its provider.
        /// </summary>
        /// <value>
        /// The type of the subsystem to use for instantiation.
        /// </value>
        Type SubsystemTypeOverride { get; set; }

        /// <summary>
        /// The *least specific* type of the configuration object compatible with this subsystem.
        /// </summary>
        /// <remarks>
        /// The user will be allowed to specify any derived config, up to and including this type,
        /// but no less derived than this type.
        /// </remarks>
        Type ConfigType { get; set; }
    }
}
