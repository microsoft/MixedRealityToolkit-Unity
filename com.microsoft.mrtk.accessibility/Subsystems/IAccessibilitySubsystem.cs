// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Cross-platform, portable set of specifications for what
    /// an Accessibility Subsystem is capable of. Both the Accessibility
    /// subsystem and the associated provider must implement this interface,
    /// preferably with a direct mapping or wrapping between the provider
    /// surface and the subsystem surface.
    /// </summary>
    public interface IAccessibilitySubsystem
    {
        #region Describable object management

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <param name="classification"></param>
        /// <param name="readerView"></param>
        /// <returns></returns>
        IReadOnlyList<GameObject> GetDescribableObjects(
            float maxDistance,
            ObjectClassification classification,
            ReaderView readerView);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObj"></param>
        /// <param name="classification"></param>
        /// <returns></returns>
        bool RegisterDescribableObject(
            GameObject gameObj,
            ObjectClassification classification);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObj"></param>
        void UnregisterDescribableObject(GameObject gameObj);

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
