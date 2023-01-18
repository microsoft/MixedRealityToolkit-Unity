// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="MRTKSubsystemDescriptor"/>.
    /// </summary>
    public class MRTKSubsystemCinfo :
        IEquatable<MRTKSubsystemCinfo>, IMRTKSubsystemDescriptor
    {
        #region IMRTKDescriptor implementation

        ///<inheritdoc/>
        public string Name { get; set; }

        ///<inheritdoc/>
        public string DisplayName { get; set; }

        ///<inheritdoc/>
        public string Author { get; set; }

        ///<inheritdoc/>
        public Type ConfigType { get; set; }

        ///<inheritdoc/>
        public Type ProviderType { get; set; }

        ///<inheritdoc/>
        public Type SubsystemTypeOverride { get; set; }

        #endregion IMRTKDescriptor implementation

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="MRTKSubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="MRTKSubsystemCinfo"/>, otherwise false.</returns>
        public virtual bool Equals(MRTKSubsystemCinfo other)
        {
            return
                ReferenceEquals(Name, other.Name)
                && ReferenceEquals(ProviderType, other.ProviderType)
                && ReferenceEquals(SubsystemTypeOverride, other.SubsystemTypeOverride);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="MRTKSubsystemCinfo"/> and
        /// <see cref="Equals(MRTKSubsystemCinfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return (obj is MRTKSubsystemCinfo cinfo) && Equals(cinfo);
        }

        public override int GetHashCode()
        {
            throw new ApplicationException("Do not hash subsystem descriptors as keys.");
        }
    }

    /// <summary>
    /// Specifies a functionality description that implements the IMRTKSubsystemDescriptor interface.
    /// Generic, and useful for basic subsystems that don't require more advanced properties/metadata.
    /// </summary>
    public class MRTKSubsystemDescriptor<TSubsystem, TProvider> :
        SubsystemDescriptorWithProvider<TSubsystem, TProvider>,
        IMRTKSubsystemDescriptor
        where TSubsystem : SubsystemWithProvider, new()
        where TProvider : SubsystemProvider<TSubsystem>
    {
        /// <summary>
        /// Constructs a <c>MRTKSubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='MRTKSubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        public MRTKSubsystemDescriptor(MRTKSubsystemCinfo MRTKSubsystemCinfo)
        {
            Name = MRTKSubsystemCinfo.Name;
            DisplayName = MRTKSubsystemCinfo.DisplayName;
            Author = MRTKSubsystemCinfo.Author;
            ConfigType = MRTKSubsystemCinfo.ConfigType;
            ProviderType = MRTKSubsystemCinfo.ProviderType;
            SubsystemTypeOverride = MRTKSubsystemCinfo.SubsystemTypeOverride;
        }

        #region IMRTKDescriptor implementation

        ///<inheritdoc/>
        public string Name { get => id; set => id = value; }

        ///<inheritdoc/>
        public string DisplayName { get; set; }

        ///<inheritdoc/>
        public string Author { get; set; }

        ///<inheritdoc/>
        public Type ConfigType { get; set; }

        ///<inheritdoc/>
        public Type ProviderType { get => providerType; set => providerType = value; }

        ///<inheritdoc/>
        public Type SubsystemTypeOverride { get => subsystemTypeOverride; set => subsystemTypeOverride = value; }

        #endregion IMRTKDescriptor implementation

        /// <summary>
        /// Creates a <c>MRTKSubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>MRTKSubsystemDescriptor</c>.
        /// </returns>
        internal static MRTKSubsystemDescriptor<TSubsystem, TProvider> Create(MRTKSubsystemCinfo cinfo)
        {
            UnityEngine.Debug.Assert(cinfo != null, "cinfo passed to generic descriptor Create() was null!");

            // Validates cinfo.
            if (!XRSubsystemHelpers.CheckTypes<TSubsystem, TProvider>(cinfo.Name,
                                                                        cinfo.SubsystemTypeOverride,
                                                                        cinfo.ProviderType))
            {
                throw new ArgumentException("Could not create MRTKSubsystemDescriptor.");
            }

            return new MRTKSubsystemDescriptor<TSubsystem, TProvider>(cinfo);
        }
    }
}
