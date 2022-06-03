// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Environment
{
    [Preserve]
    /// <summary>
    /// A subsystem that exposes information about the current playspace boundaries.
    /// </summary>
    public class BoundarySubsystem :
        MRTKSubsystem<BoundarySubsystem, BoundarySubsystemDescriptor, BoundarySubsystem.Provider>,
        IBoundarySystem
    {
        /// <summary>
        /// Construct the <c>BoundarySubsystem</c>.
        /// </summary>
        public BoundarySubsystem()
        { }

        /// <summary>
        /// Interface for providing boundary functionality for the implementation.
        /// </summary>
        [Preserve]
        public abstract class Provider : MRTKSubsystemProvider<BoundarySubsystem>, IBoundarySystem
        {
            #region IBoundarySystem implementation

            /// <inheritdoc/>
            public abstract ExperienceScale Scale { get; set; }

            ///<inheritdoc/>
            public abstract List<Vector3> GetBoundaryGeometry();

            ///<inheritdoc/>
            public abstract void SetTrackingSpace();

            #endregion IBoundarySystem implementation
        }

        #region IBoundarySystem implementation

        /// <inheritdoc/>
        public ExperienceScale Scale
        {
            get => provider.Scale;
            set => provider.Scale = value;
        }

        /// <inheritdoc/>
        public List<Vector3> GetBoundaryGeometry() => provider.GetBoundaryGeometry();

        /// <inheritdoc/>
        public void SetTrackingSpace() => provider.SetTrackingSpace();

        #endregion IBoundarySystem implementation

        /// <summary>
        /// Registers a boundary subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="boundarySubsystemParams">The parameters defining the boundary subsystem functionality implemented
        /// by the subsystem provider.</param>
        /// <returns>
        /// <c>true</c> if the subsystem implementation is registered. Otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">Thrown when the values specified in the
        /// <see cref="BoundarySubsystemCinfo"/> parameter are invalid. Typically, this will occur
        /// <list type="bullet">
        /// <item>
        /// <description>if <see cref="BoundarySubsystemCinfo.id"/> is <c>null</c> or empty</description>
        /// </item>
        /// <item>
        /// <description>if <see cref="BoundarySubsystemCinfo.implementationType"/> is <c>null</c></description>
        /// </item>
        /// <item>
        /// <description>if <see cref="BoundarySubsystemCinfo.implementationType"/> does not derive from the
        /// <see cref="BoundarySubsystem"/> class
        /// </description>
        /// </item>
        /// </list>
        /// </exception>
        public static bool Register(BoundarySubsystemCinfo boundarySubsystemParams)
        {
            BoundarySubsystemDescriptor boundarySubsystemDescriptor = BoundarySubsystemDescriptor.Create(boundarySubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(boundarySubsystemDescriptor);
            return true;
        }
    }
}
