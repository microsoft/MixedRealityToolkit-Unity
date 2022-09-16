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
        #region Describable object management

        /// <summary>
        /// Attempts to register the specified <see cref="GameObject"/> using the associated <see cref="ObjectClassification"/>.
        /// </summary>
        /// <param name="gameObj">The <see cref="GameObject"/> to be registered.</param>
        /// <param name="classification">The classification (people, places, things, etc.) for the <see cref="GameObject"/>.</param>
        /// <returns>True if successfully registered or false.</returns>
        /// <remarks>
        /// The registration process requires that a <see cref="GameObject"/> belongs to exactly one classification.
        /// </remarks>
        bool TryRegisterDescribableObject(GameObject gameObj, ObjectClassification classification);

        /// <summary>
        /// Attempts to unregister the specified <see cref="GameObject"/> using the associated <see cref="ObjectClassification"/>
        /// </summary>
        /// <param name="gameObj">The <see cref="GameObject"/> to be unregistered.</param>
        /// <param name="classification">The classification (people, places, things, etc.) for the <see cref="GameObject"/>.</param>
        /// <remarks>
        /// The registration process requires that a <see cref="GameObject"/> belongs to exactly one classification.
        /// </remarks>
        bool TryUnregisterDescribableObject(GameObject gameObj, ObjectClassification classification);

        #endregion Describable object management

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
        /// <param name="enable">True to enable inversion, or false.</param>
        /// <remarks>
        /// This method requires the material to use the Text Mesh Pro shader which is
        /// provided in the Microsoft Mixed Reality Toolkit Graphics Tools package.
        /// </remarks>
        void ApplyTextColorInversion(Material material, bool enable);

        #endregion Text color inversion
    }
}
