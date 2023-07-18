// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Cross-platform, portable set of specifications for which an Accessibility Subsystem is capable. Both the Accessibility
    /// subsystem and the associated provider must implement this interface, preferably with a direct mapping or wrapping between
    /// the provider surface and the subsystem surface.
    /// </summary>
    public interface IAccessibilitySubsystem
    {
        #region Accessible object management

        /// <summary>
        /// Attempts to retrieve the collection of <see cref="AccessibleObjectClassification"/> instances for the objects
        /// that have been registered.
        /// </summary>
        /// <param name="classifications">List to receive the collection of registered <see cref="AccessibleObjectClassification"/> instances.</param>
        /// <returns><see langword="true"/> if classifications have been successfully retrieved, or <see langword="false"/>.</returns>
        /// <remarks>
        /// The passed in list will be cleared then filled with all <see cref="AccessibleObjectClassification"/> instances that match
        /// a previously registered object. Classifications are not removed after all matching objects have been
        /// unregistered.
        /// </remarks>
        bool TryGetAccessibleObjectClassifications(List<AccessibleObjectClassification> classifications);

        /// <summary>
        /// Attempts to register the specified <see cref="GameObject"/> using the associated <see cref="AccessibleObjectClassification"/>.
        /// </summary>
        /// <param name="accessibleObject">The <see cref="GameObject"/> to be registered.</param>
        /// <param name="classification">The classification (people, places, things, etc.) for the <see cref="GameObject"/>.</param>
        /// <returns><see langword="true"/> if successfully registered or <see langword="false"/>.</returns>
        /// <remarks>
        /// The registration process requires that a <see cref="GameObject"/> belongs to exactly one classification.
        /// </remarks>
        bool TryRegisterAccessibleObject(GameObject accessibleObject, AccessibleObjectClassification classification);

        /// <summary>
        /// Attempts to unregister the specified <see cref="GameObject"/> using the associated <see cref="AccessibleObjectClassification"/>
        /// </summary>
        /// <param name="accessibleObject">The <see cref="GameObject"/> to be unregistered.</param>
        /// <param name="classification">The classification (people, places, things, etc.) for the <see cref="GameObject"/>.</param>
        /// <remarks>
        /// The registration process requires that a <see cref="GameObject"/> belongs to exactly one classification.
        /// </remarks>
        bool TryUnregisterAccessibleObject(GameObject accessibleObject, AccessibleObjectClassification classification);

        #endregion Accessible object management

        #region Text color inversion

        /// <summary>
        /// Should text color inversion be enabled?
        /// </summary>
        bool InvertTextColor { get; set; }

        /// <summary>
        /// Indicates that the value of <see cref="InvertTextColor"/> has been changed.
        /// </summary>
        event Action<bool> InvertTextColorChanged;

        /// <summary>
        /// Provides a material which should have it's text color modified.
        /// </summary>
        /// <param name="material">The material to which to apply text color inversion.</param>
        /// <param name="enable"><see langword="true"/> to enable inversion, or <see langword="false"/>.</param>
        /// <remarks>
        /// This method requires the material to use the Text Mesh Pro shader which is
        /// provided in the Microsoft Mixed Reality Toolkit Graphics Tools package.
        /// </remarks>
        void ApplyTextColorInversion(Material material, bool enable);

        #endregion Text color inversion
    }
}
