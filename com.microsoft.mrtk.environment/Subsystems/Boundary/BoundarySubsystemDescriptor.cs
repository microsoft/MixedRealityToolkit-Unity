// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Environment
{
    /// <summary>
    /// Encapsulates the parameters for creating a new <see cref="BoundarySubsystemDescriptor"/>.
    /// </summary>
    public struct BoundarySubsystemCinfo :
        IEquatable<BoundarySubsystemCinfo>, IMRTKSubsystemDescriptor
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
        /// <param name="other">The other <see cref="BoundarySubsystemCinfo"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="BoundarySubsystemCinfo"/>, otherwise false.</returns>
        public bool Equals(BoundarySubsystemCinfo other)
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
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="BoundarySubsystemCinfo"/> and
        /// <see cref="Equals(BoundarySubsystemCinfo)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(System.Object obj)
        {
            return ((obj is BoundarySubsystemCinfo) && Equals((BoundarySubsystemCinfo)obj));
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(BoundarySubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(BoundarySubsystemCinfo lhs, BoundarySubsystemCinfo rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(BoundarySubsystemCinfo)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(BoundarySubsystemCinfo lhs, BoundarySubsystemCinfo rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            throw new ApplicationException("Do not hash subsystem descriptors as keys.");
        }
    }

    /// <summary>
    /// Specifies a functionality description that may be registered for each implementation that provides the
    /// <see cref="BoundarySubsystem"/> interface.
    /// </summary>
    public class BoundarySubsystemDescriptor :
        SubsystemDescriptorWithProvider<BoundarySubsystem, BoundarySubsystem.Provider>,
        IMRTKSubsystemDescriptor
    {
        /// <summary>
        /// Constructs a <c>BoundarySubsystemDescriptor</c> based on the given parameters.
        /// </summary>
        /// <param name='BoundarySubsystemCinfo'>The parameters required to initialize the descriptor.</param>
        BoundarySubsystemDescriptor(BoundarySubsystemCinfo boundarySubsystemCinfo)
        {
            Name = boundarySubsystemCinfo.Name;
            DisplayName = boundarySubsystemCinfo.DisplayName;
            Author = boundarySubsystemCinfo.Author;
            ProviderType = boundarySubsystemCinfo.ProviderType;
            SubsystemTypeOverride = boundarySubsystemCinfo.SubsystemTypeOverride;
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
        /// Creates a <c>BoundarySubsystemDescriptor</c> based on the given parameters validating that the
        /// <c>id</c> and <c>implentationType</c> properties are specified.
        /// </summary>
        /// <param name='cinfo'>The parameters required to initialize the descriptor.</param>
        /// <returns>
        /// The created <c>BoundarySubsystemDescriptor</c>.
        /// </returns>
        internal static BoundarySubsystemDescriptor Create(BoundarySubsystemCinfo cinfo)
        {
            // Validates cinfo.
            if (!XRSubsystemHelpers.CheckTypes<BoundarySubsystem, BoundarySubsystem.Provider>(cinfo.Name,
                                                                                              cinfo.SubsystemTypeOverride,
                                                                                              cinfo.ProviderType))
            {
                throw new ArgumentException("Could not create BoundarySubsystemDescriptor.");
            }

            Debug.Log("Successfully created new descriptor");
            return new BoundarySubsystemDescriptor(cinfo);
        }
    }
}
