// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="MRTKSubsystemDescriptor{T, U}"/>.
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
        /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="MRTKSubsystemCinfo"/>, otherwise <see langword="false"/>.</returns>
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
        /// <param name="obj">The object to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is of type <see cref="MRTKSubsystemCinfo"/> and
        /// <see cref="Equals(MRTKSubsystemCinfo)"/> also returns <see langword="true"/>; otherwise <see langword="false"/>.</returns>
        public override bool Equals(System.Object obj)
        {
            return (obj is MRTKSubsystemCinfo cinfo) && Equals(cinfo);
        }

        /// <summary>
        /// This <see cref="GetHashCode"/> override is meant to disable hash lookups of <see cref="MRTKSubsystemCinfo"/> objects.
        /// </summary>
        /// <remarks>
        /// This will throw a <see cref="ApplicationException"/> if called.
        /// </remarks>
        /// <exception cref="ApplicationException">
        /// Thrown if this function is called.
        /// </exception>
        public override int GetHashCode()
        {
            throw new ApplicationException("Do not hash subsystem descriptors as keys.");
        }
    }

    /// <summary>
    /// Specifies a functionality description that implements the <see cref="IMRTKSubsystemDescriptor"/> interface.
    /// </summary>
    /// <remarks>
    /// Generic, and useful for basic subsystems that don't require more advanced properties or metadata.
    /// </remarks>
    public class MRTKSubsystemDescriptor<TSubsystem, TProvider> :
        SubsystemDescriptorWithProvider<TSubsystem, TProvider>,
        IMRTKSubsystemDescriptor
        where TSubsystem : SubsystemWithProvider, new()
        where TProvider : SubsystemProvider<TSubsystem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MRTKSubsystemDescriptor{T, U}"/> class.
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
        /// Creates a <see cref="MRTKSubsystemDescriptor{T, U}"/> based on the given parameters.
        /// </summary>
        /// <remarks>
        /// This function will verify that the <see cref="MRTKSubsystemCinfo"/> properties are valid.
        /// </remarks>
        /// <param name="cinfo">The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The newly created instance of the <see cref="MRTKSubsystemDescriptor{T, U}"/> class.
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
